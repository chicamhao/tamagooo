using UnityEngine;
using System.Collections;
using Input;

namespace Demon
{
    public class DemonSensor : MonoBehaviour
    {
        [SerializeField] Transform _eyesLocation;

        public SensorContext Context = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<ActionControl>(out var control))
            {
                Context.Player = other.gameObject;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (Context.Player != null)
            {
                var playerPosition = Context.Player.transform.position + new Vector3(0f, 1.5f, 0f);
                var distance = Vector3.Distance(playerPosition, _eyesLocation.position);

                Debug.DrawRay(_eyesLocation.position,
                    (playerPosition - _eyesLocation.position).normalized * distance, Color.yellow);

                var hits = Physics.RaycastAll(_eyesLocation.position, (playerPosition - _eyesLocation.position).normalized, distance);

                foreach (var hit in hits) 
                {
                    if (!hit.transform.CompareTag("Player") && hit.collider != null && !hit.collider.isTrigger)
                        break;
                    
                    Debug.DrawRay(_eyesLocation.position,
                        (playerPosition - _eyesLocation.position).normalized * distance, Color.red);
                    Debug.Log("Demon sees you!");                                       
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<ActionControl>(out var control))
            {
                Context.Player = null;
            }
        }

        virtual public void Interact() { }
    }

    public class SensorContext
    {
        public GameObject Player;
    }
}
