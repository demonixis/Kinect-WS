using UnityEngine;

namespace Demonixis.Kinect
{
    public class KinectObject : MonoBehaviour
    {
        protected KinectManager m_kinectManager = null;

        [Header("User Settings")]
        [SerializeField]
        protected int m_userIndex = 0;

        public int UserIndex
        {
            get { return m_userIndex; }
            set { m_userIndex = value; }
        }

        protected virtual void Start()
        {
            m_kinectManager = KinectManager.Instance;

            if (m_kinectManager == null)
                throw new UnityException("KinectManager was not found.");
        }
    }
}
