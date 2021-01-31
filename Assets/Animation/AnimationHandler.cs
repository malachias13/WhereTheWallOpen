using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class AnimationHandler : MonoBehaviour
    {
        public Animator anim;
        PlayerInputHandler playerInputHandler;
        PlayerLocomation playerLocomation;
        Rigidbody pLR;
        EnemyAI enemyAI;
        SoundHandler soundHandler;

        int vertical;
        int horizontal;
        int speed;
        public bool canRotate;
        int iscrouch;
        int isSprinting;
        int moveAmount;
        int Velocity;
        int isPatroling;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            playerInputHandler = GetComponentInParent<PlayerInputHandler>();
            playerLocomation = GetComponentInParent<PlayerLocomation>();
            pLR = playerLocomation.GetComponent<Rigidbody>();
            enemyAI = transform.parent.GetComponent<EnemyAI>();
            soundHandler = GetComponentInParent<SoundHandler>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
            iscrouch = Animator.StringToHash("isCrouching");
            moveAmount = Animator.StringToHash("moveAmount");
            isSprinting = Animator.StringToHash("isSprinting");
            Velocity = Animator.StringToHash("Velocity");
            isPatroling = Animator.StringToHash("isPlayerSpotted");

        }

        public void UpdateAnimatorValues(float VerticalMovement, float HorizontalMovement)
        {
            #region Vertical
            float v;
            if (VerticalMovement > 0 && VerticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (VerticalMovement > 0.55f)
            {
                v = 1;
            }
            else if (VerticalMovement < 0 && VerticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (VerticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h;
            if (HorizontalMovement > 0 && HorizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (HorizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (HorizontalMovement < 0 && HorizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (HorizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion


             anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
             anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);

           /* anim.SetFloat(vertical, v);
            anim.SetFloat(horizontal, h);*/

            anim.SetBool(iscrouch, playerInputHandler.iscrouching);

            if (playerInputHandler.moveAmount >= 0.1f && playerLocomation.canSprint)
            {
                anim.SetBool(isSprinting, playerInputHandler.isSprinting);
            }
            else
            {
                anim.SetBool(isSprinting, false);
            }
                

            anim.SetFloat(moveAmount, playerInputHandler.moveAmount);
            anim.SetFloat(Velocity, playerLocomation.CurrentV);   

        }
        public void EnemyAiAnimations(bool playerSpotted)
        {
            // EnemyAi
            anim.SetBool("isPlayerSpotted", playerSpotted);
        }

        public void stopRotation()
        {
            canRotate = false;
        }
        public void CanRotation()
        {
            canRotate = true;
        }
        public void FootStepAudio()
        {
            soundHandler.PlayFootsteps();
        }
        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            // For Rigidbody
           /* playerLocomation.GetComponent<Rigidbody>().drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
           */

         /*   Vector3 velocity = deltaPosition / delta;
            pLR.velocity = velocity;*/
        }

    }
}
