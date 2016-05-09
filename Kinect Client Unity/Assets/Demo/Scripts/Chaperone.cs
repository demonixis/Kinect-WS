using System.Collections;
using UnityEngine;

namespace Demonixis.VR
{
    public class Chaperone : MonoBehaviour
    {
        private Transform _player = null;
        private bool _canPlayDangerSound = true;

        [SerializeField]
        private GameObject _gridMesh = null;
        [SerializeField]
        private Rect _gameArea = new Rect(-3, -3, 6, 6);
        [SerializeField]
        private float _areaLimit = 0.5f;
        [SerializeField]
        private AudioClip _dangerSound = null;

        void Start()
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
                throw new UnityException("[Chaperone] No player was found.");

            _gameArea.x += _areaLimit;
            _gameArea.y += _areaLimit;
            _gameArea.width -= _areaLimit;
            _gameArea.height -= _areaLimit;

            _player = player.GetComponent<Transform>();
        }

        void LateUpdate()
        {
            if (Time.deltaTime <= 0)
                return;

            SetGridMeshActive(!_gameArea.Contains(_player.position));
        }

        public Vector3 GetUserScale()
        {
            return Vector3.one;
        }

        private void SetGridMeshActive(bool isActive)
        {
            if (_gridMesh.activeSelf == isActive)
                return;

            _gridMesh.SetActive(isActive);

            if (isActive && _canPlayDangerSound)
                StartCoroutine(PlayDangerSound());
        }

        private IEnumerator PlayDangerSound()
        {
            _canPlayDangerSound = false;

            AudioSource.PlayClipAtPoint(_dangerSound, _player.position);

            yield return new WaitForSeconds(1.0f);

            _canPlayDangerSound = true;
        }
    }
}
