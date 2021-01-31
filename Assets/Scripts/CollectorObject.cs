using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManaFlux
{
    public class CollectorObject : MonoBehaviour
    {
        PlayerStats playerStats;
        SoundHandler soundHandler;
        public int id;

        private void Awake() {
            playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
            soundHandler = GameObject.FindGameObjectWithTag("Player")?.GetComponent<SoundHandler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                playerStats.AddToCollection(id);
                playerStats.Interacting();
                StartCoroutine(playerStats.InteractionReset(soundHandler.narrations[id - 1].duration));

                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}