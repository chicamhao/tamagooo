using Action;
using UnityEngine;

namespace Input
{
    public sealed class ActionControl : MonoBehaviour
    {
        private GroundHandle _groundHandle;
        private CrouchAction _crouchAction;
        private JumpAction _jumpAction;
        private MoveAction _moveAction;
        private FireAction _fireAction;

        private void Start()
        {
            _groundHandle = new GroundHandle(GetComponent<CharacterController>());
            _crouchAction = GetComponent<CrouchAction>();
            _jumpAction = GetComponent<JumpAction>();
            _moveAction = GetComponent<MoveAction>();
            _fireAction = GetComponent<FireAction>();
        }
 
        private void Update()
        {
            _groundHandle.Validate(Time.time);

            _crouchAction.Crouch();
            
            _moveAction.Rotate();
            
            _moveAction.Move(_crouchAction, _groundHandle);
            _jumpAction.Jump(_groundHandle);
            _fireAction.Fire();
        }
    }
}