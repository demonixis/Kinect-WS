using System;
using UnityEngine;

namespace Demonixis.Kinect
{
    public class KinectDebugCharacter : KinectObject
    {
        #region Private Variables
        protected Transform m_transform = null;
        protected Transform m_bones = null;
        protected bool m_ready = false;
        protected Vector3 m_userPosition = Vector3.zero;
        protected KinectJoint[] m_joints = null;
        protected Vector3 m_lastPosition = Vector3.zero;
        #endregion

        #region Editor Fields
        [SerializeField]
        protected Material m_material = null;
        [SerializeField]
        protected Vector3 m_moveScale = new Vector3(1.0f, 1.0f, 1.0f);
        [SerializeField]
        protected bool m_updatePosition = true;
        [SerializeField]
        protected bool m_updateRotation = true;
        [SerializeField]
        protected float m_dampingPosition = 10.0f;
        [SerializeField]
        protected float m_dampingRotation = 10.0f;
        [SerializeField]
        protected bool m_mirrorMovement = false;
        [SerializeField]
        protected bool m_lockYPosition = false;
        [SerializeField]
        protected Transform m_cameraNode = null;
        [SerializeField]
        protected bool m_autoCreate = true;

        [SerializeField]
        protected JointType[] m_excludedJoints = null;

        #endregion

        protected override void Start()
        {
            base.Start();
            m_transform = GetComponent<Transform>();

            var bones = new GameObject("KinectBones");
            m_bones = bones.GetComponent<Transform>();
            m_bones.parent = m_transform;
        }

        protected virtual void Update()
        {
            if (m_autoCreate && !m_ready && m_kinectManager.IsInitialized)
            {
                CreateDebugCharacter(m_kinectManager.KinectVersion);
                return;
            }

            if (m_ready)
                UpdateDebugCharacter();
        }

        protected virtual void CreateDebugCharacter(int version)
        {
            GameObject jointObject = null;
            KinectJoint kinectJoint = null;
            Rigidbody rigidbody = null;
            var jointTypeNames = Enum.GetNames(typeof(JointType));
            var size = jointTypeNames.Length;

            if (version == 1)
                size = KinectManager.KinectXbox360JointCount;

            m_joints = new KinectJoint[size];

            for (int i = 0; i < size; i++)
            {
                if (IsJointExcluded((JointType)i))
                    continue;

                jointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                jointObject.name = jointTypeNames[i];
                jointObject.transform.parent = m_bones;
                jointObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                jointObject.GetComponent<Renderer>().material = m_material;

                kinectJoint = jointObject.AddComponent<KinectJoint>();
                kinectJoint.UserIndex = m_userIndex;
                kinectJoint.JointType = (JointType)i;
                kinectJoint.DampingPosition = m_dampingPosition;
                kinectJoint.DampingRotation = m_dampingRotation;
                kinectJoint.MirroredMovement = m_mirrorMovement;
                kinectJoint.UpdateRotation = false;

                rigidbody = jointObject.AddComponent<Rigidbody>();
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                rigidbody.useGravity = false;

                switch (kinectJoint.JointType)
                {
                    case JointType.Head:
                        if (m_cameraNode != null)
                        {
                            m_cameraNode.parent = kinectJoint.transform;
                            m_cameraNode.localPosition = Vector3.zero;
                            m_cameraNode.localRotation = Quaternion.identity;
                            m_cameraNode.localScale = Vector3.one;

                            // Because the camera manages this itself
                            kinectJoint.UpdateRotation = false;
                            kinectJoint.GetComponent<MeshRenderer>().enabled = false;
                        }
                        break;

                    case JointType.HandLeft:
                    case JointType.HandRight:
                        jointObject.transform.localScale = new Vector3(0.065f, 0.065f, 0.065f);
                        break;

                    case JointType.HandTipLeft:
                    case JointType.HandTipRight:
                    case JointType.ThumbLeft:
                    case JointType.ThumbRight:
                        jointObject.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
                        break;

                    case JointType.WristLeft:
                    case JointType.WristRight:
                        jointObject.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);
                        break;
                }

                m_joints[i] = kinectJoint;
            }

            m_lastPosition = m_kinectManager.GetUserPosition(m_userIndex);
            m_updateRotation = version == 2;
            m_ready = true;
        }

        public void DestroyBones()
        {
            var count = m_bones.childCount;
            if (count > 0)
                for (var i = 0; i < count; i++)
                    Destroy(m_bones.GetChild(i));

            m_autoCreate = false;
            m_ready = false;
        }

        protected virtual void UpdateDebugCharacter()
        {
            if (m_updatePosition)
            {
                m_userPosition = m_kinectManager.GetUserPosition(m_userIndex) - m_lastPosition;
                m_lastPosition = m_kinectManager.GetUserPosition(m_userIndex);

                if (!m_mirrorMovement)
                    m_userPosition.z *= -1.0f;

                m_userPosition.Scale(m_moveScale);
                m_userPosition.y /= m_moveScale.y;

                m_transform.position += m_userPosition;
            }

            if (m_updateRotation)
            {
                var targetRotation = (m_kinectManager.GetUserRotation(m_userIndex));
                targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0, targetRotation.eulerAngles.z);
                m_transform.rotation = Quaternion.Slerp(m_transform.rotation, targetRotation, Time.deltaTime * m_dampingRotation);
            }
        }

        protected bool IsJointExcluded(JointType joint)
        {
            if (m_excludedJoints == null)
                return false;

            return Array.IndexOf<JointType>(m_excludedJoints, joint) > -1;
        }

        public KinectJoint GetJoint(JointType type)
        {
            return m_joints[(int)type];
        }

        public void ForEachJoints(Action<KinectJoint> callback)
        {
            for (int i = 0, l = m_joints.Length; i < l; i++)
                callback(m_joints[i]);
        }
    }
}