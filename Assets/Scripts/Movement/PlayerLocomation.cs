using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class PlayerLocomation : MonoBehaviour
    {
        PlayerInputHandler playerInputHandler;
        AnimationHandler animatorHandler;

        // Movement
        public Vector3 MoveDirection;
        Vector3 velocity;
        Vector3 noralVector;
        Vector3 previousFramePosition;

        Transform myTransform;
        Transform cameraObject;

        // Rigidbody
        private Rigidbody rb;
        public CapsuleCollider playerCol;
        float originalHeight;
        public float reducedHeight;

        // Player Stats
        PlayerStats playerStats;


        public GameObject playerModle;
        public Transform headT;


        float currentSpeed = 0f;

        // Controller
        float controllerHeight;

        [Header("Player Stats")]
        [SerializeField] float walkSpeed = 3f;
        [SerializeField] float sprintSpeed = 7f;
        [SerializeField] float RotationSpeed = 20f;
        [SerializeField] float gravity = -9.81f;
        [SerializeField] float crouchingHeight;
        [SerializeField] float crouchSpeed = 3.5f;
        [SerializeField] float sanityDecay = 0.25f;

        // public int playerSpeed;
        public float CurrentV;


        // Checks 
        public bool canSprint = true;


        Transform respawnT;

        void Start()
        {

            // Rigidbody
            rb = GetComponent<Rigidbody>();

            playerInputHandler = GetComponent<PlayerInputHandler>();
            animatorHandler = GetComponentInChildren<AnimationHandler>();

            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            // Movement sprint
            currentSpeed = walkSpeed;

            // CapsuleCollider
            playerCol = GetComponent<CapsuleCollider>();
            originalHeight = playerCol.height;

            // Player Stats
            playerStats = GetComponent<PlayerStats>();

            previousFramePosition = this.transform.position;
            SetRespawn();
        }

        public void HandleMovement(float delta)
        {
            if (playerStats.isInteracting)
                return;

            // Movement Calculations
            MoveDirection = cameraObject.forward * playerInputHandler.vertical;
            MoveDirection += cameraObject.right * playerInputHandler.horizontal;

            MoveDirection.Normalize();
            MoveDirection.y = 0;

            // Sprinting 

            // If there is no Sanity Player can't sprint.
            if (playerStats.currentSanity <= 0)
            {
                canSprint = false;
            } else
            {
                canSprint = true;
            }

            float targetSpeed = walkSpeed;
            if (playerInputHandler.isSprinting == true && !playerInputHandler.iscrouching && canSprint == true)
            {
                targetSpeed = sprintSpeed;


                // Sanity Bar
                if (playerInputHandler.moveAmount != 0)
                {
                    playerStats.LostOfSanity(sanityDecay);
                }

                if (playerStats.currentSanity > 0 && playerInputHandler.moveAmount != 0)
                {
                    playerModle.transform.localRotation = Quaternion.Euler(new Vector3(0, -30, 0));
                } else if (playerStats.currentSanity <= 0 && playerInputHandler.moveAmount != 0)
                {
                    playerModle.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }

            }
            if (playerInputHandler.isSprinting == true && !playerInputHandler.iscrouching && canSprint == false)
            {
                targetSpeed = walkSpeed;

                // Sanity Bar
                if (playerInputHandler.moveAmount != 0)
                    playerStats.LostOfSanity(sanityDecay);
            }

            #region Crouching
            if (playerInputHandler.iscrouching)
            {
                playerInputHandler.isSprinting = false;
                targetSpeed = crouchSpeed;

                playerModle.transform.localRotation = Quaternion.Euler(new Vector3(0, 45, 0));


                Crouch();
            }
            if (playerInputHandler.iscrouching == false && playerInputHandler.isSprinting == false)
            {
                playerModle.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                GoUp();
            }

            #endregion


            // MovementSpeed

            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, delta * 10);

            // Rigidbody
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(MoveDirection * currentSpeed, noralVector);
            rb.velocity = projectedVelocity;

            // GetCurrentSpeed();

            /* float movementPerFrame = Vector3.Distance(previousFramePosition, this.transform.position);
             playerSpeed = Mathf.RoundToInt(movementPerFrame / Time.deltaTime);
             previousFramePosition = transform.position;*/

            CurrentV = rb.velocity.magnitude;

            // animatorHandler.UpdateAnimatorValues(playerInputHandler.moveAmount, 0);
            animatorHandler.UpdateAnimatorValues(playerInputHandler.vertical, playerInputHandler.horizontal);
            if (animatorHandler.canRotate && playerInputHandler.vertical >= -0.5 && playerInputHandler.horizontal >= -1)
            {
                HandleRotation(delta);
            }

        }

        public void HandleRotation(float delta)
        {
            Vector3 TargetDirection = Vector3.zero;

            TargetDirection = cameraObject.forward * playerInputHandler.vertical;
            TargetDirection += cameraObject.right * playerInputHandler.horizontal;

            TargetDirection.Normalize();
            TargetDirection.y = 0;

            if (TargetDirection == Vector3.zero)
            {
                TargetDirection = myTransform.forward;
            }
            float rs = RotationSpeed;

            Quaternion TR = Quaternion.LookRotation(TargetDirection);
            Quaternion TargetRotation = Quaternion.Slerp(myTransform.rotation, TR, rs * delta);

            myTransform.rotation = TargetRotation;
        }
        private void Crouch()
        {
            playerCol.height = reducedHeight;
            headT.localPosition = Vector3.up * (-(originalHeight - reducedHeight) / 2);

            playerCol.center = new Vector3(0, -(originalHeight - reducedHeight) / 2, 0);
        }
        private void GoUp()
        {
            playerCol.height = originalHeight;
            headT.localPosition = Vector3.up * (playerCol.height / 2 - 0.1f);

            playerCol.center = new Vector3(0, 0, 0);
        }
        void SetRespawn()
        {
            respawnT = new GameObject(name + " Respawn Point").transform;
            respawnT.position = transform.position;
            respawnT.rotation = transform.rotation;
        }
        public void Respawn()
        {
            // var spawnT = GameObject.FindGameObjectWithTag("Spawn").transform;
            if (respawnT)
            {
                transform.position = respawnT.position;
                // todo rotate camera to actually rotate
                transform.rotation = respawnT.rotation;
            }
        }

    }
}