using UnityEngine;

namespace Demonixis.Kinect.Demo
{
    public class KinectShootWithHand : KinectObject
    {
        private bool _canShoot = true;
        private AudioSource _audioSource = null;
        private HandState _lastHandState = HandState.Unknown;
        private Transform _shootPoint;

        public bool isLeftHand = true;
        public AudioClip shootSound = null;
        public GameObject bullet = null;

        protected override void Start()
        {
            base.Start();

            _audioSource = Camera.main.GetComponent<AudioSource>();

            if (_audioSource == null)
                _audioSource = Camera.main.gameObject.AddComponent<AudioSource>();

            var shoot = new GameObject("ShootPoint");

            _shootPoint = shoot.transform;
            _shootPoint.parent = transform;
            _shootPoint.localPosition = new Vector3(0, 0, 5);
        }

        void Update()
        {
            var handState = m_kinectManager.GetHandState(isLeftHand, m_userIndex);
            var mouseClicked = isLeftHand ? Input.GetMouseButtonDown(0) : Input.GetMouseButtonDown(1);
            var handConfidence = m_kinectManager.GetHandConfidence(isLeftHand, m_userIndex);
            var handJustClosed = false;// handConfidence == TrackingConfidence.High && _lastHandState != HandState.Closed && handState == HandState.Closed;

            if ((mouseClicked || handJustClosed) && _canShoot)
            {
                _canShoot = false;
                _audioSource.PlayOneShot(shootSound);
                Invoke("AllowShoot", 0.25f);

                var sphere = (GameObject)Instantiate(bullet, _shootPoint.position, _shootPoint.rotation);
                var rb = sphere.GetComponent<Rigidbody>();
                rb.AddForce(_shootPoint.forward * 2000);
            }

            _lastHandState = handState;
        }

        private void AllowShoot()
        {
            _canShoot = true;
        }
    }
}
