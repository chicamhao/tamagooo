using System;
using Input;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Action
{
    public sealed class CrouchAction : MonoBehaviour
    {
        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Header("Stance")] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        public float CameraHeightRatio = 0.9f;

        [Tooltip("Speed of crouching transitions")]
        public float CrouchingSharpness = 10f;
        
        [Tooltip("Height of character when standing")]
        public float CapsuleHeightStanding = 1.8f;
        
        [Tooltip("Height of character when crouching")]
        public float CapsuleHeightCrouching = 0.9f;
        
        float _targetCharacterHeight;
        public bool IsCrouching { get; private set; }
        CharacterController _controller;
        InputHandle _inputHandle;
        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _inputHandle = GetComponent<InputHandle>();
            
            TryCrouch(false, true);
            ForceUpdateHeight();
        }

        public void Crouch()
        {
            if (_inputHandle.GetCrouchInputDown())
            {
                TryCrouch(!IsCrouching, false);
            }
            UpdateHeight();
        }

        // returns false if there was an obstruction
        public bool TryCrouch(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                _targetCharacterHeight = CapsuleHeightCrouching;
            }
            else
            {
                // detect obstructions
                if (!ignoreObstructions)
                {
                    var standingOverlaps = Physics.OverlapCapsule(
                        Calculator.GetCapsuleBottomHemisphere(_controller),
                        Calculator.GetCapsuleTopHemisphere(_controller, CapsuleHeightStanding),
                        _controller.radius,
                        -1,
                        QueryTriggerInteraction.Ignore);
                    
                    foreach (Collider c in standingOverlaps)
                    {
                        if (c != _controller)
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
        
        private void UpdateHeight()
        {
            if (Mathf.Approximately(_controller.height, _targetCharacterHeight)) return;
            
            // resize the capsule and adjust camera position
            _controller.height = Mathf.Lerp(
                _controller.height, _targetCharacterHeight, CrouchingSharpness * Time.deltaTime);
           
            _controller.center = Vector3.up * (_controller.height * 0.5f);
            
            PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                Vector3.up * (_targetCharacterHeight * CameraHeightRatio), CrouchingSharpness * Time.deltaTime);
        }
        
        public void ForceUpdateHeight()
        {
            _controller.height = _targetCharacterHeight;
            _controller.center = Vector3.up * (_controller.height * 0.5f);
            PlayerCamera.transform.localPosition = Vector3.up * (_targetCharacterHeight * CameraHeightRatio);
        }
    }
}