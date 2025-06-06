using Action;
using UnityEngine;

namespace Input
{
    public sealed class ActionControl : MonoBehaviour
    {
        [SerializeField] ActionSettings _settings;
        [SerializeField] GameObject _lightObject;

        public ActionContext Context => _context;
        private ActionContext _context;

        private CrouchAction _crouchAction;
        private JumpAction _jumpAction;
        private MoveAction _moveAction;
        private UseAction _useAction;
        private InteractAction _interactAction;

        private void Start()
        {
            _context = new ActionContext(_settings, GetComponent<CharacterController>(), GetComponent<InputHandle>());
            _crouchAction = new CrouchAction(_context, _settings.Crouch);

            _jumpAction = new JumpAction(_context, _settings.Jump);
            _moveAction = new MoveAction(_context, _settings.Move, _settings.Jump, _settings.Crouch);
            _useAction = new UseAction(_context, _lightObject);
            _interactAction = new InteractAction(_context);
        }
 
        private void Update()
        {
            _context.Validate(Time.time);

            _crouchAction.Crouch();           
            _moveAction.Rotate();       
            _moveAction.Move();
            _jumpAction.Jump();

            _useAction.Use();
            _interactAction.Interact();
        }
    }
}