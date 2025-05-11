using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public sealed class InputHandle : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;
        
        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        //GameFlowManager m_GameFlowManager;
        private bool _fireInputWasHeld;

        private InputAction m_MoveAction;
        private InputAction m_LookAction;
        private InputAction m_JumpAction;
        private InputAction m_FireAction;
        private InputAction m_AimAction;
        private InputAction m_SprintAction;
        private InputAction m_CrouchAction;
        private InputAction m_ReloadAction;
        private InputAction m_NextWeaponAction;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            m_MoveAction = InputSystem.actions.FindAction("Player/Move");
            m_LookAction = InputSystem.actions.FindAction("Player/Look");
            m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
            m_FireAction = InputSystem.actions.FindAction("Player/Fire");
            m_AimAction = InputSystem.actions.FindAction("Player/Aim");
            m_SprintAction = InputSystem.actions.FindAction("Player/Sprint");
            m_CrouchAction = InputSystem.actions.FindAction("Player/Crouch");
            m_ReloadAction = InputSystem.actions.FindAction("Player/Reload");
            m_NextWeaponAction = InputSystem.actions.FindAction("Player/NextWeapon");
            
            m_MoveAction.Enable();
            m_LookAction.Enable();
            m_JumpAction.Enable();
            m_FireAction.Enable();
            m_AimAction.Enable();
            m_SprintAction.Enable();
            m_CrouchAction.Enable();
            m_ReloadAction.Enable();
            m_NextWeaponAction.Enable();
        }

        private void LateUpdate()
        {
            _fireInputWasHeld = GetFireInputHeld();
        }

        public Vector3 GetMoveInput()
        {
            if (!CanProcessInput()) return Vector3.zero;
            
            var input = m_MoveAction.ReadValue<Vector2>();
            var move = new Vector3(input.x, 0f, input.y);

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;

        }

        public float GetLookInputsHorizontal()
        {
            if (!CanProcessInput())
                return 0.0f;
            
            var input = m_LookAction.ReadValue<Vector2>().x;

            if (InvertXAxis)
                input *= -1;

            input *= LookSensitivity;
            return input;
        }

        public float GetLookInputsVertical()
        {
            if (!CanProcessInput())
                return 0.0f;
            
            var input = m_LookAction.ReadValue<Vector2>().y;

            if (InvertYAxis)
                input *= -1;

            input *= LookSensitivity;
            
            return input;
        }

        public bool GetJumpInputDown()
        {
            return CanProcessInput() && m_JumpAction.WasPressedThisFrame();
        }

        public bool GetJumpInputHeld()
        {
            return CanProcessInput() && m_JumpAction.IsPressed();
        }

        public bool GetFireInputDown()
        {
            return GetFireInputHeld() && !_fireInputWasHeld;
        }

        public bool GetFireInputReleased()
        {
            return !GetFireInputHeld() && _fireInputWasHeld;
        }

        public bool GetFireInputHeld()
        {
            return CanProcessInput() && m_FireAction.IsPressed();
        }

        public bool GetAimInputHeld()
        {
            return CanProcessInput() && m_AimAction.IsPressed();
        }

        public bool GetSprintInputHeld()
        {
            return CanProcessInput() && m_SprintAction.IsPressed();
        }

        public bool GetCrouchInputDown()
        {
            return CanProcessInput() && m_CrouchAction.WasPressedThisFrame();
        }

        public bool GetCrouchInputReleased()
        {
            return CanProcessInput() && m_CrouchAction.WasReleasedThisFrame();
        }

        public bool GetReloadButtonDown()
        {
            return CanProcessInput() && m_ReloadAction.WasPressedThisFrame();
        }

        public int GetSwitchWeaponInput()
        {
            if (!CanProcessInput()) return 0;
            var input = m_NextWeaponAction.ReadValue<float>();

            return input switch
            {
                > 0f => -1,
                < 0f => 1,
                _ => 0
            };
        }

        public int GetSelectWeaponInput()
        {
            if (!CanProcessInput()) return 0;
            
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                return 1;
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                return 2;
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
                return 3;
            if (Keyboard.current.digit4Key.wasPressedThisFrame)
                return 4;
            if (Keyboard.current.digit5Key.wasPressedThisFrame)
                return 5;
            if (Keyboard.current.digit6Key.wasPressedThisFrame)
                return 6;
            if (Keyboard.current.digit7Key.wasPressedThisFrame)
                return 7;
            if (Keyboard.current.digit8Key.wasPressedThisFrame)
                return 8;
            if (Keyboard.current.digit9Key.wasPressedThisFrame)
                return 9;

            return 0;
        }
        
        private static bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked; //&& !m_GameFlowManager.GameIsEnding;
        }
    }
}