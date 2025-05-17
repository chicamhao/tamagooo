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

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _crouchAction;
        private InputAction _useAction;
        private InputAction _interactAction;

        private InputAction m_FireAction;
        private InputAction m_AimAction;
        private InputAction m_ReloadAction;
        private InputAction m_NextWeaponAction;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            BindAction(ref _moveAction, "Move");
            BindAction(ref _lookAction, "Look");
            BindAction(ref _jumpAction, "Jump");
            BindAction(ref _sprintAction, "Sprint");
            BindAction(ref _crouchAction, "Crouch");
            BindAction(ref _useAction, "Use");
            BindAction(ref _interactAction, "Interact");

            m_FireAction = InputSystem.actions.FindAction("Player/Fire");
            m_AimAction = InputSystem.actions.FindAction("Player/Aim");
            m_ReloadAction = InputSystem.actions.FindAction("Player/Reload");
            m_NextWeaponAction = InputSystem.actions.FindAction("Player/NextWeapon");

            _moveAction.Enable();
            _lookAction.Enable();
            _jumpAction.Enable();
            _sprintAction.Enable();
            _crouchAction.Enable();
            _useAction.Enable();

            m_FireAction.Disable();
            m_AimAction.Disable();
            m_ReloadAction.Disable();
            m_NextWeaponAction.Disable();
        }

        void BindAction(ref InputAction action, string name)
        {
            action = InputSystem.actions.FindAction("Player/" + name);
        }

        private void LateUpdate()
        {
            _fireInputWasHeld = GetFireInputHeld();
        }

        public Vector3 GetMoveInput()
        {
            if (!CanProcessInput()) return Vector3.zero;
            
            var input = _moveAction.ReadValue<Vector2>();
            var move = new Vector3(input.x, 0f, input.y);

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;

        }

        public float GetLookInputsHorizontal()
        {
            if (!CanProcessInput())
                return 0.0f;
            
            var input = _lookAction.ReadValue<Vector2>().x;

            if (InvertXAxis)
                input *= -1;

            input *= LookSensitivity;
            return input;
        }

        public float GetLookInputsVertical()
        {
            if (!CanProcessInput())
                return 0.0f;
            
            var input = _lookAction.ReadValue<Vector2>().y;

            if (InvertYAxis)
                input *= -1;

            input *= LookSensitivity;
            
            return input;
        }

        public bool GetJumpInputDown()
        {
            return CanProcessInput() && _jumpAction.WasPressedThisFrame();
        }

        public bool GetJumpInputHeld()
        {
            return CanProcessInput() && _jumpAction.IsPressed();
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
            return CanProcessInput() && _sprintAction.IsPressed();
        }

        public bool GetCrouchInputDown()
        {
            return CanProcessInput() && _crouchAction.WasPressedThisFrame();
        }

        public bool GetCrouchInputReleased()
        {
            return CanProcessInput() && _crouchAction.WasReleasedThisFrame();
        }

        public bool GetInteractInputDown()
        {
            return CanProcessInput() && _interactAction.WasPressedThisFrame();
        } 

        public bool GetReloadButtonDown()
        {
            return CanProcessInput() && m_ReloadAction.WasPressedThisFrame();
        }

        public bool GetUseInputDown()
        {
            return CanProcessInput() && _useAction.WasPressedThisFrame();
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