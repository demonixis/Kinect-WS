using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;

namespace Demonixis.Kinect
{
    public struct UserPosition
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }

    public sealed class KinectManager : MonoBehaviour
    {
        #region Constants and Static Fields

        public const int KinectXbox360JointCount = 20;
        public const int KinectXboxOneJointCount = 25;
        private static KinectManager _instance = null;

        #endregion

        #region Private Variables

        private WebSocket _webSocket = null;
        private Body[] _bodies;
        private int _bodyCount = 0;
        private UserPosition[] _usersTransform;
        private Joint _dummyJoint = new Joint();
        private Joint _spinBaseJoint = new Joint();

        #endregion

        #region Editor Fields

        [Header("Server Settings")]
        [SerializeField]
        private string _ip = "127.0.0.1";
        [SerializeField]
        private string _port = "8181";
        [SerializeField]
        private bool _showLog = true;
        [SerializeField]
        private bool _autoConnect = true;

        #endregion

        #region Properties

        public static KinectManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<KinectManager>();

                return _instance;
            }
        }

        public bool IsInitialized
        {
            get; private set;
        }

        public Body[] Bodies
        {
            get { return _bodies; }
        }

        public int KinectVersion
        {
            get { return _bodyCount > 0 ? _bodies[0].Version : 0; }
        }

        public bool IsKinectV1
        {
            get { return _bodyCount > 0 ? _bodies[0].Version == 1 : false; }
        }

        public bool IsKinectV2
        {
            get { return _bodyCount > 0 ? _bodies[0].Version == 2 : false; }
        }

        public bool IsConnected
        {
            get { return _bodyCount > 0; }
        }

        #endregion

        #region Lifecycle

        void Awake()
        {
            if (_instance != null)
            {
                Debug.Log("[KinectManager] Multiple instances is not supported.");
                Debug.Log(string.Format("[KinectManager] The object attached to {0} will be destroyed", gameObject.name));
                Destroy(this);
            }

            _instance = this;

            if (_autoConnect)
                Connect(_ip, _port);
        }

        void OnDestroy()
        {
            if (_webSocket != null)
            {
                _webSocket.OnMessage -= OnMessage;
                _webSocket.Close();
            }
        }

        public void Connect()
        {
            Connect(_ip, _port);
        }

        public void Connect(string ip, string port)
        {
            if (_webSocket != null)
            {
                _webSocket.OnMessage -= OnMessage;
                _webSocket.Close();
                _webSocket = null;
            }

            _webSocket = new WebSocket(string.Format("ws://{0}:{1}", _ip, _port));

            if (_showLog)
                _webSocket.OnOpen += (s, e) => Debug.Log("[KinectManager] Connected to the Kinect Server.");

            if (_showLog)
                _webSocket.OnError += (s, e) => Debug.LogFormat("[KinectManager] An Error was detected.\n{0}", e.Message);

            _webSocket.Connect();
            _webSocket.OnMessage += OnMessage;
        }

        #endregion

        #region Methods to get data on a body

        public Joint GetJoint(JointType type, int userIndex)
        {
            if (_bodyCount > userIndex)
            {
                var index = (int)type;
                if (IsKinectV2 || index < KinectXbox360JointCount)
                    return _bodies[userIndex].Joints[index];
            }

            return _dummyJoint;
        }

        public Body GetBody(int userIndex)
        {
            if (_bodyCount > userIndex)
                return _bodies[userIndex];

            return null;
        }

        public Vector3 GetJointPosition(JointType type, int userIndex)
        {
            return GetJoint(type, userIndex).Position;
        }

        public Quaternion GetJointOrientation(JointType type, int userIndex)
        {
            return GetJoint(type, userIndex).Rotation;
        }

        public Vector3 GetUserPosition(int userIndex)
        {
            if (_bodyCount > userIndex)
                return _usersTransform[userIndex].Position;

            return Vector3.zero;
        }

        public Quaternion GetUserRotation(int userIndex)
        {
            if (_bodyCount > userIndex)
                return _usersTransform[userIndex].Rotation;

            return Quaternion.identity;
        }

        public HandState GetHandState(bool handLeft, int userIndex)
        {
            if (_bodyCount > userIndex)
                return handLeft ? _bodies[userIndex].HandLeftState : _bodies[userIndex].HandRightState;
            else
                return HandState.Unknown;
        }

        public TrackingConfidence GetHandConfidence(bool handLeft, int userIndex)
        {
            if (_bodyCount > userIndex)
                return handLeft ? _bodies[userIndex].HandLeftConfidence : _bodies[userIndex].HandRightConfidence;
            else
                return TrackingConfidence.Low;
        }

        public Joint GetParentJoint(JointType type, int userIndex)
        {
            Joint parent = null;

            switch (type)
            {
                case JointType.SpineMid:
                case JointType.HipLeft:
                case JointType.HipRight:
                case JointType.KneeLeft:
                case JointType.KneeRight:
                case JointType.AnkleLeft:
                case JointType.AnkleRight:
                case JointType.FootLeft:
                case JointType.FootRight:
                    parent = GetJoint(JointType.SpineBase, userIndex);
                    break;

                case JointType.SpineShoulder:
                    parent = GetJoint(JointType.SpineMid, userIndex);
                    break;

                case JointType.Head:
                case JointType.Neck:
                case JointType.ShoulderLeft:
                case JointType.ShoulderRight:
                case JointType.ElbowLeft:
                case JointType.ElbowRight:
                case JointType.WristLeft:
                case JointType.WristRight:
                    parent = GetJoint(JointType.SpineShoulder, userIndex);
                    break;

                case JointType.HandRight:
                case JointType.ThumbRight:
                    parent = GetJoint(JointType.WristRight, userIndex);
                    break;

                case JointType.HandTipRight:
                    parent = GetJoint(JointType.HandRight, userIndex);
                    break;

                case JointType.HandLeft:
                case JointType.ThumbLeft:
                    parent = GetJoint(JointType.WristLeft, userIndex);
                    break;

                case JointType.HandTipLeft:
                    parent = GetJoint(JointType.HandLeft, userIndex);
                    break;
            }

            return parent;
        }

        #endregion

        #region WebSocket Handler

        private void OnMessage(object sender, MessageEventArgs e)
        {
            _bodies = JsonConvert.DeserializeObject<Body[]>(e.Data);

            if (_bodies != null)
            {
                _bodyCount = _bodies.Length;

                if (_usersTransform == null || _usersTransform.Length != _bodyCount)
                    _usersTransform = new UserPosition[_bodyCount];

                for (int i = 0; i < _bodyCount; i++)
                {
                    _spinBaseJoint = GetJoint(JointType.SpineBase, i);
                    _usersTransform[i].Position = _spinBaseJoint.Position;
                    _usersTransform[i].Rotation = _bodies[i].RootOrientation;
                }

                if (!IsInitialized)
                    IsInitialized = true;
            }
            else
                _bodyCount = 0;
        }

        #endregion
    }
}