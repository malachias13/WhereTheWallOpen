using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class EnemyViewTrigger : MonoBehaviour
    {
        public bool inTrigger = false;
        const string playerTag = "Player";
        public delegate void TriggerEvent();
        public event TriggerEvent triggerEvent;

        private void Update()
        {
            if (inTrigger)
            {
                if (triggerEvent != null)
                {
                    triggerEvent();
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                inTrigger = true;
                if (triggerEvent != null)
                {
                    triggerEvent();
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                inTrigger = false;
                if (triggerEvent != null)
                {
                    triggerEvent();
                }
            }
        }
    }
}