using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Keyboard Inputs")]
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public bool isSprinting;
        public bool iscrouching;
        public bool testKeyPress;

        [Header("Camera Inputs")]
        public float mouseX = 0;
        public float mouseY = 0;

        [Header("Camera control")]
        public float camXSpeed = 1;
        public float camYSpeed = 1;


        [HideInInspector] public Camera mainCamera;
        public float turnSpeed = 15;

        public PlayerControls inputActions;

        Vector2 movementInput;
        Vector2 cameraInput;



        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                // Movement input
                inputActions.Movement.Movement.performed += inputAction => movementInput =
              inputAction.ReadValue<Vector2>();
                inputActions.Movement.Movement.canceled += inputAction => movementInput = Vector2.zero;

                inputActions.Movement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.Movement.Camera.canceled += i => cameraInput = Vector2.zero;

                // Sprinting 
                inputActions.Movement.Sprinting.performed += sprint => isSprinting = true;
                inputActions.Movement.Sprinting.canceled += sprint => isSprinting = false;

                // crouching
                inputActions.Movement.crouching.performed += crouching => iscrouching = true;
                inputActions.Movement.crouching.canceled += crouching => iscrouching = false;

                inputActions.Movement.TestKey.performed += TestKey => testKeyPress = true;
                inputActions.Movement.TestKey.canceled += TestKey => testKeyPress = false;
                
            }

            inputActions.Enable();
        }
        private void OnDisable()
        {
            inputActions.Disable();
        }
        public void TickInput(float delta)
        {
            MovementInput();
            //   CamLook(delta);
        }
        public void MovementInput()
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;

            // Camera Inputs 
            mouseX = cameraInput.x * camXSpeed;
            mouseY = cameraInput.y * camYSpeed;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        }

        public void PlayerAniming()
        {
            float yamCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yamCamera, 0), turnSpeed * Time.fixedDeltaTime);
        }
    }
}