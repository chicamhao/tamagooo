using Input;
using UnityEngine;

namespace Demon
{
    public class DemonSensor : MonoBehaviour
    {
        enum State
        {
            None,
            Insight,
            InsightCrounch,
            InsightObstacle,
        }

        [SerializeField] Transform _eyesLocation;
        public CharacterController _player;

        private float _distance;
        private Vector3 _playerPosition;
        private State _state;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<CharacterController>(out var controller))
            {
                _player = controller;

                _state = State.Insight;
                if (other.gameObject.GetComponent<ActionControl>().Context.IsCrouching)
                {
                    _state = State.InsightCrounch;
                }
                else if (TryDetectObstacles())
                {
                    _state = State.InsightObstacle;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_player != null)
            {
                if (_state == State.InsightCrounch)
                {
                    if (!other.gameObject.GetComponent<ActionControl>().Context.IsCrouching)
                    {
                        _state = State.Insight;
                    }
                }
                else if (_state == State.InsightObstacle && !TryDetectObstacles())
                {
                    _state = State.Insight;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<CharacterController>(out var _))
            {
                _player = null;
                _state = State.None;
            }
        }

        private bool TryDetectObstacles()
        {
            CalculatePosition();
            var hits = Physics.RaycastAll(_eyesLocation.position, (_playerPosition - _eyesLocation.position).normalized, _distance);
            foreach (var hit in hits)
            {
                if (!hit.transform.CompareTag("Player") && hit.collider != null && !hit.collider.isTrigger)
                    return true;

            }
            return false;
        }

        private void CalculatePosition()
        {
            if (_player == null) return;

            _playerPosition = _player.transform.position + new Vector3(0f, _player.height * 0.8f, 0f);
            _distance = Vector3.Distance(_playerPosition, _eyesLocation.position);
        }

        private void OnDrawGizmos()
        {
            if (_state == State.None) return;

            var color = _state switch
            {
                State.Insight => Color.red,
                State.InsightCrounch => Color.orange,
                State.InsightObstacle => Color.yellow,
                _ => Color.black
            };

            CalculatePosition();
            Gizmos.color = color;
            Gizmos.DrawLine(_eyesLocation.position, _playerPosition);
        }
    }
}
