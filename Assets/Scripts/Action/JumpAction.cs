using Input;
using UnityEngine;

namespace Action
{
    public sealed class JumpAction
    {      
        readonly ActionContext _context;
        readonly JumpSettings _settings;

        public JumpAction(ActionContext context, JumpSettings settings)
        {
            _context = context;
            _settings = settings;
        }

        public void Jump()
        {
            if (!_context.IsGrounded) return; 
            
            if (!_context.Input.GetJumpInputDown()) return;
            
            if (!Calculator.Standable(_context, _settings.CapsuleHeightStanding)) return;

            var velocity = _context.Velocity;

            // cancel out vertical velocity.
            velocity = new Vector3(velocity.x, 0f, velocity.z);

            // add jump speed values upwards.
            velocity += Vector3.up * _settings.JumpForce;

            // to prevent snapping to ground for a short time.
            _context.LastTimeJumped = Time.time;

            // force grounding.
            _context.IsGrounded = false;
            _context.IsCrouching = false;
            _context.GroundNormal = Vector3.up;

            _context.Velocity = velocity;
        }
    }
}