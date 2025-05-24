using UnityEngine;

namespace Demon
{
    public class DemonSensor : MonoBehaviour
    {
        [SerializeField] Transform _eyesLocation;
        public CharacterController _player;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<CharacterController>(out var controller))
            {
                _player = controller;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_player != null)
            {
                var playerPosition = _player.transform.position + new Vector3(0f, _player.height * 0.8f, 0f);
                var distance = Vector3.Distance(playerPosition, _eyesLocation.position);

                var fromPosition = _eyesLocation.position;
                var toPosition = (playerPosition - _eyesLocation.position).normalized * distance;
                Debug.DrawRay(fromPosition, toPosition, Color.yellow);

                if (!TryDetectObstacles(playerPosition, distance))
                {
                    Debug.DrawRay(fromPosition, toPosition, Color.red);
                    Debug.Log("Demon sees you!");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<CharacterController>(out var _))
            {
                _player = null;
            }
        }

        private bool TryDetectObstacles(Vector3 playerPosition, float distance)
        {
            var hits = Physics.RaycastAll(_eyesLocation.position, (playerPosition - _eyesLocation.position).normalized, distance);
            foreach (var hit in hits)
            {
                if (!hit.transform.CompareTag("Player") && hit.collider != null && !hit.collider.isTrigger)
                    return true;

            }
            return false;
        }
    }
}
