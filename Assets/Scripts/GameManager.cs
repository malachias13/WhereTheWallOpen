using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class GameManager : MonoBehaviour
    {
        PlayerInputHandler playerInputHandler;
        PlayerLocomation PlayerLocomation;
        CameraController cameraController;


        void Start()
        {
          //  cameraController = GetComponentInChildren<CameraController>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            PlayerLocomation = GetComponent<PlayerLocomation>();

            playerInputHandler.mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;

           // cameraController.CameraMove(delta);

        }
        private void FixedUpdate()
        {

            float delta = Time.fixedDeltaTime;
            playerInputHandler.PlayerAniming();
            playerInputHandler.TickInput(delta);
            PlayerLocomation.HandleMovement(delta);
        }
    }
}