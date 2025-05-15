using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Action
{
    public sealed class MoveAction
    {
        readonly MoveSettings _moveSettings;
        readonly JumpSettings _jumpSetting;
        readonly CrouchSettings _crouchSettings;
        readonly ActionContext _context;

        private float _cameraVerticalAngle = 0f;
        
        public MoveAction(ActionContext context,
            MoveSettings moveSettings, JumpSettings jumpSettings, CrouchSettings crouchSettings)
        {
            _context = context;
            _moveSettings = moveSettings;
            _jumpSetting = jumpSettings;
            _crouchSettings = crouchSettings;
        }

        public void Rotate()
        {
            // horizontal character rotation
            {
                // rotate the transform with the input speed around its local Y axis
                _context.Controller.transform.Rotate(new Vector3(0f, (_context.Input.GetLookInputsHorizontal() * _moveSettings.RotationSpeed), 0f), Space.Self);
            }

            // vertical camera rotation
            {
                // add vertical inputs to the camera's vertical angle
                _cameraVerticalAngle += _context.Input.GetLookInputsVertical() * _moveSettings.RotationSpeed;

                // limit the camera's vertical angle to min/max
                _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                Camera.main.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
            }
        }

        public void Move()
        {
            var speedModifier = 1.0f;
            if (_context.Input.GetSprintInputHeld())
            {
                speedModifier = _moveSettings.SprintSpeedModifier;
            }
            
            var velocity = _context.IsGrounded 
                ? GetGroundVelocity(speedModifier, _context.IsCrouching, _context) 
                : GetAirVelocity(speedModifier, _context);

            // apply the final calculated velocity value as a character movement.
            var capsuleBottomBeforeMove = Calculator.GetCapsuleBottomHemisphere(_context.Controller);
            var capsuleTopBeforeMove = Calculator.GetCapsuleTopHemisphere(_context.Controller);

            // detect obstructions to adjust velocity accordingly.
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, _context.Controller.radius,
                velocity.normalized, out var hit, velocity.magnitude * Time.deltaTime))
            {
                velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
            }

            _context.Controller.Move(velocity * Time.deltaTime);
            _context.Velocity = velocity;
        }
        
        private Vector3 GetGroundVelocity(float speedModifier, bool IsCrouching, ActionContext context)
        {
            // converts move input to a world space vector based on our character's transform orientation
            var worldSpaceMoveInput = _context.Controller.transform.TransformVector(_context.Input.GetMoveInput());
            
            // calculate the desired velocity from inputs, max speed, and current slope
            
            var targetVelocity = worldSpaceMoveInput * (_moveSettings.MaxSpeedOnGround * speedModifier);
                
            // reduce speed if crouching by crouch speed ratio
            if (IsCrouching)
            {
                targetVelocity *= _crouchSettings.MaxSpeedCrouchedRatio;
            }

            targetVelocity = Calculator.GetDirectionReorientedOnSlope(
                targetVelocity.normalized, context.GroundNormal, _context.Controller.transform.up) * targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            return Vector3.Lerp(context.Velocity, targetVelocity, _moveSettings.MovementSharpnessOnGround * Time.deltaTime);
        }

        private Vector3 GetAirVelocity(float speedModifier, ActionContext context)
        {
            var velocity = context.Velocity;
            
            // converts move input to a worldspace vector based on our character's transform orientation
            var worldSpaceMoveInput = _context.Controller.transform.TransformVector(_context.Input.GetMoveInput());
                
            // add air acceleration
            velocity += worldSpaceMoveInput * (_jumpSetting.AccelerationSpeedInAir * Time.deltaTime);

            // limit air speed to a maximum, but only horizontally
            var verticalVelocity = velocity.y;
            var horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _jumpSetting.MaxSpeedInAir * speedModifier);
            velocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            // apply the gravity to the velocity
            velocity += Vector3.down * (_jumpSetting.GravityDownForce * Time.deltaTime);
            return velocity;
        }
    }
}