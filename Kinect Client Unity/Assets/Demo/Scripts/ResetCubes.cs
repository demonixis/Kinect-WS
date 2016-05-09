using UnityEngine;

namespace Demonixis.Kinect.Demo
{
    public class ResetCubes : MonoBehaviour
    {
        private Transform _transform = null;
        private Vector3[] _positions = null;
        private Quaternion[] _rotations = null;
        private int _countElements = 0;

        void Start()
        {
            _transform = GetComponent<Transform>();
            _countElements = _transform.childCount;
            _positions = new Vector3[_countElements];
            _rotations = new Quaternion[_countElements];

            Transform child = null;
            for (int i = 0; i < _countElements; i++)
            {
                child = _transform.GetChild(i);
                _positions[i] = child.position;
                _rotations[i] = child.rotation;
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Transform child = null;
                for (int i = 0; i < _countElements; i++)
                {
                    child = _transform.GetChild(i);
                    child.position = _positions[i];
                    child.rotation = _rotations[i];
                }
            }
        }
    }
}
