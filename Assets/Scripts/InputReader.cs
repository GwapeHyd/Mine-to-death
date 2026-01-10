using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
    public class InputReader : MonoBehaviour
    {
        PlayerInput playerInput;
        InputAction moveAction;
        InputAction jumpAction;
        InputAction fireAction;
        InputAction fireSpecialAction;

        public Vector2 Move => moveAction.ReadValue<Vector2>();
        public bool Fire => fireAction.ReadValue<float>() > 0f;
        public bool FireSpecialPressedThisFrame {get; private set; }
        private bool jumpPressedThisFrame;
        private bool jumpHeld;
        

        public bool JumpHeld => jumpHeld;

        void OnEnable()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null) return;
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            fireAction = playerInput.actions["Fire"];
            fireSpecialAction = playerInput.actions["FireSpecial"];

            if (jumpAction != null)
            {
                jumpAction.performed += OnJumpPerformed;
                jumpAction.canceled += OnJumpCanceled;
            }

            if (fireSpecialAction != null)
            {
                fireSpecialAction.performed += OnFireSpecialPerformed;
            }
        }

        void OnDisable()
        {
            if (jumpAction != null)
            {
                jumpAction.performed -= OnJumpPerformed;
                jumpAction.canceled -= OnJumpCanceled;
            }

            if (fireSpecialAction != null)
            {
                fireSpecialAction.performed -= OnFireSpecialPerformed;
            }   
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            jumpPressedThisFrame = true;
            jumpHeld = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            jumpHeld = false;
        }

        private void OnFireSpecialPerformed(InputAction.CallbackContext context)
        {
            FireSpecialPressedThisFrame = true;
        }

        public bool ConsumeJumpPressed()
        {
            if (jumpPressedThisFrame)
            {
                jumpPressedThisFrame = false;
                return true;
            }
            return false;
        }

        public bool ConsumeFireSpecialPressed()
        {
            if (FireSpecialPressedThisFrame)
            {
                FireSpecialPressedThisFrame = false;
                return true;
            }
            return false;
        }

        void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            fireAction = playerInput.actions["Fire"];
            fireSpecialAction = playerInput.actions["FireSpecial"];
        }

    void LateUpdate()
    {
        FireSpecialPressedThisFrame = false;
    }
}
