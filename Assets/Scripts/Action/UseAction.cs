using UnityEngine;

namespace Action
{
    public sealed class UseAction
    {
        readonly ActionContext _context;
        readonly GameObject _lightObject;

        public UseAction(ActionContext context, GameObject lightObject)
        {
            _context = context;
            _lightObject = lightObject;
            _lightObject.SetActive(false);
        }

        public void Use()
        {
            if (_context.Input.GetUseInputDown())
            {
                _lightObject.SetActive(!_lightObject.activeSelf);
            }
        }
    }
}