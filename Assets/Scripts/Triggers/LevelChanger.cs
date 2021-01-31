using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace ManaFlux {
    public class LevelChanger : MonoBehaviour
    {
        [SerializeField] GameObject myLight;
        [SerializeField] Image FadeToBlackImage;
        SoundHandler soundHandler;
        PlayerStats playerStats;

        bool hasFadeToBlack = false;
        float delay = 3.2f;

        private void Start()
        {
            playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
            soundHandler = playerStats?.GetComponent<SoundHandler>();
        }

        private void OnTriggerEnter(Collider other)
        {

            myLight.GetComponent<Animator>().SetBool("LightOn", false);

            playerStats.Interacting();
            playerStats.InteractionReset(1);
            FadeToBlack();
            LoadScene();
            Invoke("GrainPlayerMovement", 1.9f);
            Invoke("UnFade", 3.2f);
            Debug.Log("LEVEL loaded");
        }


        public void LoadScene()
        {
            Invoke("nextScene", delay);
        }
        public void nextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }

        public void GrainPlayerMovement()
        {
            playerStats.isInteracting = false;
        }
        public void FadeToBlack()
        {
            if (!hasFadeToBlack)
            {
                FadeToBlackImage.DOFade(1f, 0.2f);
                delay = 0;
                hasFadeToBlack = true;
                if (soundHandler.endSound != null)
                {
                    delay = 3.2f;
                    soundHandler.PlayEndSound();
                    
                }

            }
        }
        public void UnFade()
        {
            if (hasFadeToBlack)
            {
                FadeToBlackImage.DOFade(0f, 0.2f);
                hasFadeToBlack = false;
            }
        }
    }
}