using UnityEngine;

namespace Demonixis.Kinect
{
    [RequireComponent(typeof(Collider))]
    public class KinectHand : KinectObject
    {
        private Transform _grabbed;
        private Transform _originalTransform;

        [SerializeField]
        private bool leftHand = true;

        public HandState HandState
        {
            get
            {
                var body = m_kinectManager.GetBody(m_userIndex);
                if (body == null)
                    return HandState.Unknown;

                return leftHand ? body.HandLeftState : body.HandRightState;
            }
        }

        void Update()
        {
            if (_grabbed != null && HandState != HandState.Closed)
            {
                _grabbed.parent = _originalTransform;
                _grabbed = null;
            }
        }

        void OnCollisionStay(Collision collision)
        {
            var target = collision.collider.transform;
            if (target != transform && _grabbed == null && HandState == HandState.Closed)
            {
                _originalTransform = target.parent;
                _originalTransform.parent = transform;
                _grabbed = target;
            }
        }
    }
}
