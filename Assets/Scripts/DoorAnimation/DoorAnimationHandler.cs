using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class DoorAnimationHandler : MonoBehaviour
    {
        public Animator anim;
        int canOpen;

        void Start()
        {
            anim = GetComponent<Animator>();

            canOpen = Animator.StringToHash("OpenDoor");

        }



    }
}