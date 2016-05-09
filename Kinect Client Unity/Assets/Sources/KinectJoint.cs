using UnityEngine;

namespace Demonixis.Kinect
{
    public class KinectJoint : KinectObject
    {
        #region Private Variables

        private Transform _transform;
        private Joint _joint;
        private Vector3 _centerPosition = Vector3.zero;
        private Vector3 _targetPosition = Vector3.zero;
        private Quaternion _targetRotation = Quaternion.identity;
        private float _offsetX = 0;
        private float _offsetY = 0;
        private float _offsetZ = 0;

        #endregion

        #region Editor Fields
        [SerializeField]
        private JointType _jointType = JointType.HandLeft;
        [SerializeField]
        private float _dampingPosition = 10.0f;
        [SerializeField]
        private float _dampingRotation = 25.0f;
        [SerializeField]
        private float _jointScale = 1.0f;
        [SerializeField]
        private bool _updatePosition = true;
        [SerializeField]
        private bool _updateRotation = true;
        [SerializeField]
        private bool m_mirroredMovement = false;

        #endregion

        #region Public Fields

        public Joint Joint
        {
            get { return _joint; }
        }

        public JointType JointType
        {
            get { return _jointType; }
            set { _jointType = value; }
        }

        public float JointScale
        {
            get { return _jointScale; }
            set { _jointScale = value; }
        }

        public float DampingPosition
        {
            get { return _dampingPosition; }
            set { _dampingPosition = value; }
        }

        public float DampingRotation
        {
            get { return _dampingRotation; }
            set { _dampingRotation = value; }
        }

        public bool MirroredMovement
        {
            get { return m_mirroredMovement; }
            set { m_mirroredMovement = value; }
        }

        public bool UpdatePosition
        {
            get { return _updatePosition; }
            set { _updatePosition = value; }
        }

        public bool UpdateRotation
        {
            get { return _updateRotation; }
            set { _updateRotation = value; }
        }

        #endregion

        protected override void Start()
        {
            base.Start();
            _transform = GetComponent<Transform>();
        }

        void Update()
        {
            _joint = m_kinectManager.GetJoint(_jointType, m_userIndex);
            _centerPosition = m_kinectManager.GetUserPosition(m_userIndex);

            _offsetX = _centerPosition.x * _jointScale;
            _offsetY = _centerPosition.y * _jointScale;
            _offsetZ = (!m_mirroredMovement ? -_centerPosition.z : _centerPosition.z) * _jointScale;

            if (_updatePosition)
            {
                _targetPosition.x = (_joint.Position.x * _jointScale - _offsetX);
                _targetPosition.y = (_joint.Position.y * _jointScale - _offsetY);
                _targetPosition.z = (!m_mirroredMovement ? (-_joint.Position.z * _jointScale - _offsetZ) : (_joint.Position.z * _jointScale - _offsetZ));
                _transform.localPosition = Vector3.Lerp(_transform.localPosition, _targetPosition, Time.deltaTime * _dampingPosition);
            }

            if (_updateRotation)
            {
                _targetRotation = _joint.Rotation;

                var parent = m_kinectManager.GetParentJoint(_jointType, m_userIndex);
                if (parent != null)
                    _targetRotation = parent.Position != _joint.Position ? Quaternion.LookRotation(parent.Position - _joint.Position) : _transform.localRotation;

                _transform.localRotation = Quaternion.Slerp(_transform.localRotation, Quaternion.Inverse(_targetRotation), Time.deltaTime * _dampingRotation);
            }
        }
    }
}