using System;
using Input;
using UnityEngine;

namespace Action
{
    public sealed class JumpAction : MonoBehaviour
    {
        [SerializeField] float _jumpForce = 9f;
        
        private bool _isGrounded;
        private bool _hasJumpedThisFrame;
        private float _lastTimeJumped; 
        private Vector3 _groundNormal;
        
        private InputHandle _inputHandle;
        private CrouchAction _crouchAction;
        
        private void Start()
        {
            _inputHandle = GetComponent<InputHandle>();
            _crouchAction = GetComponent<CrouchAction>();
        }
        
        public void Jump(GroundHandle groundHandle)
        {
            if (!groundHandle.IsGrounded) return; 
            
            if (!_inputHandle.GetJumpInputDown()) return;
            
            if (!_crouchAction.TryCrouch(false, false)) return;
            
            var velocity = groundHandle.Velocity;

            // cancel out vertical velocity.
            velocity = new Vector3(velocity.x, 0f, velocity.z);

            // add jumpSpeed values upwards.
            velocity += Vector3.up * _jumpForce;

            // to prevent snapping to ground for a short time.
            groundHandle.LastTimeJumped = Time.time;
            
            // force grounding.
            groundHandle.IsGrounded = false;
            groundHandle.GroundNormal = Vector3.up;

            groundHandle.Velocity = velocity;
        }
    }
}