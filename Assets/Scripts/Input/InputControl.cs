using UnityEngine;

namespace Input
{
    // now is ActionControl, should be removed soon.
    [RequireComponent(typeof(CharacterController), typeof(InputHandle), typeof(AudioSource))]
    public sealed class InputControl : MonoBehaviour
    {
        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Header("General")] [Tooltip("Force applied downward when in the air")]
        public float GravityDownForce = 20f;

        [Tooltip("Physic layers checked to consider the player grounded")]
        public LayerMask GroundCheckLayers = -1;

        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        public float GroundCheckDistance = 0.05f;

        [Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float MaxSpeedOnGround = 10f;

        [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 15;

        [Tooltip("Max movement speed when crouching")] [Range(0, 1)]
        public float MaxSpeedCrouchedRatio = 0.5f;

        [Tooltip("Max movement speed when not grounded")]
        public float MaxSpeedInAir = 10f;

        [Tooltip("Acceleration speed when in the air")]
        public float AccelerationSpeedInAir = 25f;

        [Tooltip("Multiplication for the sprint speed (based on grounded speed)")]
        public float SprintSpeedModifier = 2f;

        [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;
        
        [Header("Jump")] [Tooltip("Force applied upward when jumping")]
        public float JumpForce = 9f;

        [Header("Stance")] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        public float CameraHeightRatio = 0.9f;

        [Tooltip("Height of character when standing")]
        public float CapsuleHeightStanding = 1.8f;

        [Tooltip("Height of character when crouching")]
        public float CapsuleHeightCrouching = 0.9f;

        [Tooltip("Speed of crouching transitions")]
        public float CrouchingSharpness = 10f;
        
        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public bool IsCrouching { get; private set; }
        
        //Health m_Health;
        InputHandle _inputHandler;
        CharacterController _characterController;
        Vector3 _groundNormal;
        Vector3 _characterVelocity;
        float _lastTimeJumped = 0f;
        float _cameraVerticalAngle = 0f;
        float _targetCharacterHeight;
        
        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;
        
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            
            _inputHandler = GetComponent<InputHandle>();
            
            _characterController.enableOverlapRecovery = true;
            
            // force the crouch state to false when starting
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);
        }

        private void Update()
        {
            CheckGround();
            
            // crouching
            if (_inputHandler.GetCrouchInputDown())
            {
                SetCrouchingState(!IsCrouching, false);
            }

            UpdateCharacterHeight(false);

            HandleCharacterMovement();
        }

        private void CheckGround()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            var chosenGroundCheckDistance =
                IsGrounded ? (_characterController.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            IsGrounded = false;
            _groundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= _lastTimeJumped + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(_characterController.height),
                    _characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    _groundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    var isNormalUnderSlopeLimit = Vector3.Angle(transform.up, _groundNormal) <= _characterController.slopeLimit;
                    if (Vector3.Dot(hit.normal, transform.up) > 0f && isNormalUnderSlopeLimit)
                    {
                        IsGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > _characterController.skinWidth)
                        {
                            _characterController.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        private void HandleCharacterMovement()
        {
            // horizontal character rotation
            {
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(
                    new Vector3(0f, (_inputHandler.GetLookInputsHorizontal() * RotationSpeed),
                        0f), Space.Self);
            }

            // vertical camera rotation
            {
                // add vertical inputs to the camera's vertical angle
                _cameraVerticalAngle += _inputHandler.GetLookInputsVertical() * RotationSpeed;

                // limit the camera's vertical angle to min/max
                _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                PlayerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
            }

            // character movement handling
            var isSprinting = _inputHandler.GetSprintInputHeld();
            {
                if (isSprinting)
                {
                    isSprinting = SetCrouchingState(false, false);
                }

                var speedModifier = isSprinting ? SprintSpeedModifier : 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                var worldSpaceMoveInput = transform.TransformVector(_inputHandler.GetMoveInput());

                // handle grounded movement
                if (IsGrounded)
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetVelocity = worldSpaceMoveInput * (MaxSpeedOnGround * speedModifier);
                    
                    // reduce speed if crouching by crouch speed ratio
                    if (IsCrouching)
                        targetVelocity *= MaxSpeedCrouchedRatio;
                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, _groundNormal) *
                                     targetVelocity.magnitude;

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                        MovementSharpnessOnGround * Time.deltaTime);

                    // jumping
                    if (IsGrounded && _inputHandler.GetJumpInputDown())
                    {
                        // force the crouch state to false
                        if (SetCrouchingState(false, false))
                        {
                            // start by canceling out the vertical component of our velocity
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                            // then, add the jumpSpeed value upwards
                            CharacterVelocity += Vector3.up * JumpForce;

                            // play sound
                            //AudioSource.PlayOneShot(JumpSfx);

                            // remember last time we jumped because we need to prevent snapping to ground for a short time
                            _lastTimeJumped = Time.time;

                            // Force grounding to false
                            IsGrounded = false;
                            _groundNormal = Vector3.up;
                        }
                    }
                }
                // handle air movement
                else
                {
                    // add air acceleration
                    CharacterVelocity += worldSpaceMoveInput * (AccelerationSpeedInAir * Time.deltaTime);

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity = CharacterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
                    CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    // apply the gravity to the velocity
                    CharacterVelocity += Vector3.down * (GravityDownForce * Time.deltaTime);
                }
            }

            // apply the final calculated velocity value as a character movement
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(_characterController.height);
            _characterController.Move(CharacterVelocity * Time.deltaTime);

            // detect obstructions to adjust velocity accordingly
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, _characterController.radius,
                CharacterVelocity.normalized, out var hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore))
            {
                CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
            }
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        private Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * _characterController.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - _characterController.radius));
        }

        // Gets a reoriented direction that is tangent to a given slope
        private Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            var directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        private void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                _characterController.height = _targetCharacterHeight;
                _characterController.center = Vector3.up * (_characterController.height * 0.5f);
                PlayerCamera.transform.localPosition = Vector3.up * (_targetCharacterHeight * CameraHeightRatio);
            }
            // Update smooth height
            else if (!Mathf.Approximately(_characterController.height, _targetCharacterHeight))
            {
                // resize the capsule and adjust camera position
                _characterController.height = Mathf.Lerp(_characterController.height, _targetCharacterHeight,
                    CrouchingSharpness * Time.deltaTime);
                _characterController.center = Vector3.up * (_characterController.height * 0.5f);
                PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                    Vector3.up * (_targetCharacterHeight * CameraHeightRatio), CrouchingSharpness * Time.deltaTime);
            }
        }

        // returns false if there was an obstruction
        private bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                _targetCharacterHeight = CapsuleHeightCrouching;
            }
            else
            {
                // Detect obstructions
                if (!ignoreObstructions)
                {
                    var standingOverlaps = Physics.OverlapCapsule(
                        GetCapsuleBottomHemisphere(),
                        GetCapsuleTopHemisphere(CapsuleHeightStanding),
                        _characterController.radius,
                        -1,
                        QueryTriggerInteraction.Ignore);
                    
                    foreach (Collider c in standingOverlaps)
                    {
                        if (c != _characterController)
                        {
                            return false;
                        }
                    }
                }

                _targetCharacterHeight = CapsuleHeightStanding;
            }
            
            IsCrouching = crouched;
            return true;
        }
    }
}