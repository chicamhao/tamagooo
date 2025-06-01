using UnityEngine;

namespace Action
{
    public sealed class InteractAction
    {
        readonly ActionContext _context;

        public InteractAction(ActionContext context)
        {
            _context = context;
        }

        public void Interact()
        {
            if (_context.Input.GetInteractInputDown())
            {
                if (_context.InteractObject == null) return;             
                
                if (IsPlayerFaceToObject())
                {
                    _context.InteractObject.Interact();
                }
            }
        }

        private bool IsPlayerFaceToObject()
        {
            //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red, 10f);
            var hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.isTrigger)
                    continue;

                if (hit.transform.root == _context.InteractObject.transform.root)
                    return true;
            }
            
            return false;
        }
    }
}