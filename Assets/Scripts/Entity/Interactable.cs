using Input;
using UnityEngine;

namespace Entity
{
	public class Interactable : MonoBehaviour
	{
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<ActionControl>(out var control))
            {
                control.Context.InteractObject = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<ActionControl>(out var control))
            {
                control.Context.InteractObject = null;
            }
        }

        virtual public void Interact() { }
    }
}
