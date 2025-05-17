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
                if (_context.InteractObject != null)
                {
                    _context.InteractObject.Interact();
                }
            }
        }
    }
}