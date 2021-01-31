using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ManaFlux
{
    public class PlayerStats : MonoBehaviour
    {

        [SerializeField] SoundHandler soundHandler;
        PlayerInputHandler playerInput;
        PlayerLocomation playerLocomation;
        AnimationHandler animationHandler;

        public float maxSanity = 100f;
        public float currentSanity;
        [SerializeField] float sanityRefillAmount;
        private bool canRefresh;

        [Header("Death settings")]
        public float reloadDelay = 1;
        public bool reloadScene = true;
        public bool resetDoors = false;

        [Header("Doors")]
        public GameObject door1;
        public GameObject door2;
        public GameObject door3;

        [HideInInspector] public int lastCollected = 0;

        public SanityBar sanityBar;
        [Header("Player Checks")]
        public bool isDead = false;
        public bool isInteracting = false;



        void Start()
        {
            playerInput = this.GetComponent<PlayerInputHandler>();
            playerLocomation = this.GetComponent<PlayerLocomation>();
            animationHandler = this.GetComponentInChildren<AnimationHandler>();
            currentSanity = maxSanity;
            sanityBar.SetMaxSanity(maxSanity);

        }
        private void Update()
        {
            //  soundHandler.PlayDeadSound(isDead);
            if (Time.timeScale == 1)
            {
                SanityRefillOverTime();
            }
            SprintChecker();

        }
        public void LostOfSanity(float lostSanity)
        {
            currentSanity -= lostSanity * Time.deltaTime;
            if (currentSanity < 0)
                currentSanity = 0;

            UpdateSanityBar();
        }
        public void UpdateSanityBar()
        {
            sanityBar.SetCurrentSanity(currentSanity);
        }
        public void AddToCollection(int id)
        {
            var doors = new List<GameObject>() { door1, door2, door3 };
            var door = doors[id - 1];
            door.transform.GetChild(0).GetComponent<Animator>().SetBool("OpenDoor", true);

            lastCollected = id;
            soundHandler.PlayNarration(id);
        }
        public void ResetCollection()
        {
            var doors = new List<GameObject>() { door1, door2, door3 };
            foreach (var door in doors)
            {
                door.transform.GetChild(0).GetComponent<Animator>().SetBool("OpenDoor", false);
            }

            lastCollected = 0;
            // soundHandler.PlayNarration(id);
        }
        public void Die()
        {
            isDead = true;
            ReloadLevel();
        }
        public void ReloadLevel()
        {
            StartCoroutine(LevelReload());
        }
        public void Interacting()
        {
            isInteracting = true;
            playerInput.inputActions.Disable();
            playerLocomation.CurrentV = 0;
            animationHandler.UpdateAnimatorValues(0, 0);

        }
        IEnumerator LevelReload()
        {
            // todo anim and sound
            // fade to black?
            yield return new WaitForSecondsRealtime(reloadDelay);
            // reload level
            Debug.Log("Restarting level " + SceneManager.GetActiveScene().buildIndex);
            if (reloadScene)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } else
            {
                // reset player, enemy, and doors
                var enemyGos = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemyGos)
                {
                    enemy.GetComponent<EnemyAI>().Respawn();
                }
                CameraController.Instance.StopCameraShake();
                playerLocomation.Respawn();
                currentSanity = maxSanity;
                UpdateSanityBar();
                if (resetDoors)
                {
                    ResetCollection();
                }
                DialogPlayer.Instance.OnReload();
            }
        }
        IEnumerator SprintCooldown()
        {
            yield return new WaitForSeconds(1f);
            canRefresh = true;
        }
        public IEnumerator InteractionReset(float time)
        {
            yield return new WaitForSeconds(time);
            isInteracting = false;
            playerInput.inputActions.Enable();

        }

        private void SprintChecker()
        {
            if (playerInput.isSprinting == false && !canRefresh)
            {
                StartCoroutine(SprintCooldown());
            } else if (playerInput.isSprinting == true)
            {
                canRefresh = false;
            }
        }
        private void SanityRefillOverTime()
        {
            if (currentSanity < maxSanity && canRefresh)
            {
                currentSanity += sanityRefillAmount;
                UpdateSanityBar();
            }
        }

    }
}