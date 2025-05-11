using System.Numerics;
using Input;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Action
{
    public sealed class MoveAction : MonoBehaviour
    {
        [Header("Rotation")] 
        [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;
        
        [Header("Movement")] 
        [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float MaxSpeedOnGround = 13f;

        [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 15f;

        [Tooltip("Max movement speed when crouching")] [Range(0, 1)]
        public float MaxSpeedCrouchedRatio = 0.5f;

        [Tooltip("Max movement speed when not grounded")]
        public float MaxSpeedInAir = 25f;
        
        [Tooltip("Acceleration speed when in the air")]
        public float AccelerationSpeedInAir = 25f;

        [Tooltip("Multiplication for the sprint speed (based on grounded speed)")]
        public float SprintSpeedModifier = 1.5f;
        
        [Header("General")] [Tooltip("Force applied downward when in the air")]
        public float GravityDownForce = 12f;
        
        
        private CharacterController _controller;
        private InputHandle _inputHandler;

        private float _cameraVerticalAngle = 0f;
        
        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _inputHandler = GetComponent<InputHandle>();
        }

        public void Rotate()
        {
            // horizontal character rotation
            {
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(new Vector3(0f, (_inputHandler.GetLookInputsHorizontal() * RotationSpeed), 0f), Space.Self);
            }

            // vertical camera rotation
            {
                // add vertical inputs to the camera's vertical angle
                _cameraVerticalAngle += _inputHandler.GetLookInputsVertical() * RotationSpeed;

                // limit the camera's vertical angle to min/max
                _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                Camera.main.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
            }
        }

        public void Move(CrouchAction crouchAction, GroundHandle groundHandle)
        {
            var speedModifier = 1.0f;
            if (_inputHandler.GetSprintInputHeld() && crouchAction.TryCrouch(false, false))
            {
                speedModifier = SprintSpeedModifier;
            }
            
            var velocity = groundHandle.IsGrounded 
                ? MoveGround(speedModifier, crouchAction.IsCrouching, groundHandle) 
                : MoveAir(speedModifier, groundHandle);

            // apply the final calculated velocity value as a character movement.
            var capsuleBottomBeforeMove = Calculator.GetCapsuleBottomHemisphere(_controller);
            var capsuleTopBeforeMove = Calculator.GetCapsuleTopHemisphere(_controller);
            _controller.Move(velocity * Time.deltaTime);

            // detect obstructions to adjust velocity accordingly.
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, _controller.radius,
                velocity.normalized, out var hit, velocity.magnitude * Time.deltaTime))
            {
                velocity = Vector3.ProjectOnPlane(velocity, hit.normal);
            }
            
            groundHandle.Velocity = velocity;
        }
        
        private Vector3 MoveGround(float speedModifier, bool IsCrouching, GroundHandle groundHandle)
        {
            // converts move input to a world space vector based on our character's transform orientation
            var worldSpaceMoveInput = transform.TransformVector(_inputHandler.GetMoveInput());
            
            // calculate the desired velocity from inputs, max speed, and current slope
            
            var targetVelocity = worldSpaceMoveInput * (MaxSpeedOnGround * speedModifier);
                
            // reduce speed if crouching by crouch speed ratio
            if (IsCrouching)
            {
                targetVelocity *= MaxSpeedCrouchedRatio;
            }

            targetVelocity = Calculator.GetDirectionReorientedOnSlope(
                targetVelocity.normalized, groundHandle.GroundNormal, transform.up) * targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            return Vector3.Lerp(groundHandle.Velocity, targetVelocity, MovementSharpnessOnGround * Time.deltaTime);
        }

        private Vector3 MoveAir(float speedModifier, GroundHandle groundHandle)
        {
            var velocity = groundHandle.Velocity;
            
            // converts move input to a worldspace vector based on our character's transform orientation
            var worldSpaceMoveInput = transform.TransformVector(_inputHandler.GetMoveInput());
                
            // add air acceleration
            velocity += worldSpaceMoveInput * (AccelerationSpeedInAir * Time.deltaTime);

            // limit air speed to a maximum, but only horizontally
            var verticalVelocity = velocity.y;
            var horizontalVelocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
            velocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            // apply the gravity to the velocity
            velocity += Vector3.down * (GravityDownForce * Time.deltaTime);
            return velocity;
        }
    }
}