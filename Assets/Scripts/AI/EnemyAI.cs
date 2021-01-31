using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace ManaFlux
{
    public class EnemyAI : MonoBehaviour
    {
        public enum AIState
        {
            IDLE,
            PATROL,
            CHASING,
            LOSTPLAYER,
            SUSPICIOUS,
            CONFUSED,
        }
        // player Stats
        PlayerStats playerStats;

        // Animations Handler
        AnimationHandler animationHandler;

        [SerializeField] AIState aiState = AIState.IDLE;
        [Space]
        [SerializeField] bool usePatrolPoints = false;
        [SerializeField] List<Transform> patrolPath = new List<Transform>();
        [Tooltip("Player needs to be in LOS for this long to start chasing")]
        [SerializeField] float playerDetectionDur = 0.1f;
        [Tooltip("After player is lost, how long until we go back to patrolling?")]
        [SerializeField] float playerForgetDur = 0.1f;
        [SerializeField] float chaseSpeed = 5;
        [SerializeField] float patrolSpeed = 2.5f;
        [SerializeField] float catchDistance = 2.1f;
        [SerializeField] LayerMask ignoreLayerMask = ~(1 << 2);
        [SerializeField] GameObject enableOnAlert;
        // todo get these elsewhere?
        [SerializeField] float maxDist = 50;
        [SerializeField] Vector3 mapCenterOffset = Vector3.zero;
        public UnityEvent caughtEvent;
        public AudioClip seeYouClip;
        public AudioClip caughtClip;
        public AudioClip lostClip;
        public AudioClip readyOrNotClip;
        [Header("Debug")]
        public bool debugLogRaycastResult = false;
        AudioSource audioSource;

        Transform respawnT;

        Transform currentTarget;
        EnemyViewTrigger[] viewTriggers = new EnemyViewTrigger[0];

        bool patrolPathInc = true;
        int patrolTargIndex = 0;
        bool playerCaught = false;

        float lastSawPlayerTime = 0;
        public bool playerSpotted = false;
        float playerForgetTimer = 0;
        Vector3 lastKnownPlayerPos = Vector3.zero;

        // Animations
        bool isPlayerSpotted = false;

        NavMeshAgent agent;
        Transform player;
        Transform playerHead;
        Animator anim;

        private void Start()
        {
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            animationHandler = GetComponentInChildren<AnimationHandler>();
            playerHead = player.GetComponent<PlayerLocomation>().headT;
            SetRespawn();
        }
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }
        private void OnEnable()
        {
            viewTriggers = GetComponentsInChildren<EnemyViewTrigger>();
            foreach (var trigger in viewTriggers)
            {
                trigger.triggerEvent += UpdatePlayerSpotted;
            }
            if (enableOnAlert) enableOnAlert.SetActive(false);
        }
        private void OnDisable()
        {
            foreach (var trigger in viewTriggers)
            {
                trigger.triggerEvent -= UpdatePlayerSpotted;
            }
        }
        private void Update()
        {
            Move();
            RandomDialog();
        }
        private void Move()
        {
            if (aiState != AIState.IDLE)
            {
                agent.isStopped = false;
            }
            switch (aiState)
            {
                case AIState.IDLE:
                    // do nothing
                    agent.isStopped = true;
                    break;

                case AIState.PATROL:
                    // Animation
                    isPlayerSpotted = false;
                    animationHandler.EnemyAiAnimations(isPlayerSpotted);

                    agent.isStopped = false;
                    agent.speed = patrolSpeed;
                    if (!usePatrolPoints)
                    {
                        // move to random points on map
                        if (agent.remainingDistance <= agent.stoppingDistance)
                        {
                            Vector2 ranCir = Random.insideUnitCircle * maxDist;
                            Vector3 randomPos = new Vector3(ranCir.x, 0, ranCir.y) + mapCenterOffset;
                            if (NavMesh.SamplePosition(randomPos, out var hit, 100, NavMesh.AllAreas))
                            {
                                agent.SetDestination(hit.position);
                            }
                        }
                    } else
                    {
                        // go through patrol points
                        if (patrolPath.Count > 1)
                        {
                            if (agent.remainingDistance <= agent.stoppingDistance)
                            {
                                // got to next patrol point
                                if (patrolTargIndex >= patrolPath.Count - 1)
                                {
                                    patrolPathInc = false;
                                } else if (patrolTargIndex <= 0)
                                {
                                    patrolPathInc = true;
                                }
                                patrolTargIndex += patrolPathInc ? 1 : -1;
                            }
                            currentTarget = patrolPath[patrolTargIndex];
                            SetTargetDestination();
                        }
                    }
                    break;

                case AIState.CHASING:
                    // Animations
                    isPlayerSpotted = true;
                    animationHandler.EnemyAiAnimations(isPlayerSpotted);

                    float playerDist = Vector3.Distance(transform.position, player.position);
                    if (playerDist <= catchDistance)
                    {
                        if (!playerCaught)
                        {
                            // caught player
                            Debug.Log("Player Caught!");
                            if (caughtClip) audioSource.PlayOneShot(caughtClip);
                            caughtEvent.Invoke();
                            playerCaught = true;
                            agent.isStopped = true;
                            playerStats.Die();
                        }

                    } else
                    {
                        playerCaught = false;
                        agent.isStopped = false;
                        // chase player
                        currentTarget = player;
                        lastKnownPlayerPos = player.position;
                        agent.speed = chaseSpeed;
                        SetTargetDestination();
                    }
                    break;

                case AIState.LOSTPLAYER:
                    // Animations
                    isPlayerSpotted = false;
                    animationHandler.EnemyAiAnimations(isPlayerSpotted);

                    agent.isStopped = false;
                    playerCaught = false;
                    // go to last known player location
                    agent.SetDestination(lastKnownPlayerPos);

                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        // todo look around
                        playerForgetTimer += Time.deltaTime;
                        if (playerForgetTimer > playerForgetDur)
                        {
                            aiState = AIState.PATROL;
                            Debug.Log("Lost player");
                            if (lostClip) audioSource.PlayOneShot(lostClip);
                            if (enableOnAlert) enableOnAlert.SetActive(false);
                        }
                    }

                    break;

                case AIState.SUSPICIOUS:
                    isPlayerSpotted = true;
                    animationHandler.EnemyAiAnimations(isPlayerSpotted);

                    agent.isStopped = false;
                    // switch to chasing if player remains in sight
                    if (Time.time >= lastSawPlayerTime + playerDetectionDur)
                    {
                        Debug.Log("Player spotted!");
                        if (seeYouClip) audioSource.PlayOneShot(seeYouClip);
                        // todo? play an alert anim to give player time to run
                        if (enableOnAlert) enableOnAlert.SetActive(true);
                        aiState = AIState.CHASING;
                        playerSpotted = true;
                    }
                    break;

                case AIState.CONFUSED:
                    // ?
                    break;
            }
        }
        void SetTargetDestination()
        {
            if (currentTarget.position != agent.destination)
            {
                agent.SetDestination(currentTarget.position);
            }
        }

        void UpdatePlayerSpotted()
        {
            bool inAnyTrigger = false;
            foreach (var trigger in viewTriggers)
            {
                inAnyTrigger |= trigger.inTrigger;
            }
            if (inAnyTrigger && IsPlayerInSight())
            {
                if (aiState != AIState.CHASING && aiState != AIState.SUSPICIOUS)
                {
                    lastSawPlayerTime = Time.time;
                    aiState = AIState.SUSPICIOUS;
                }
            } else
            {
                playerSpotted = false;
                if (aiState == AIState.CHASING)
                {
                    aiState = AIState.LOSTPLAYER;
                    playerForgetTimer = 0;
                } else if (aiState == AIState.SUSPICIOUS)
                {
                    aiState = AIState.PATROL;
                }
            }
        }
        bool IsPlayerInSight()
        {

            bool detected = false;
            // raycast to make sure player is not behind a wall
            // todo? multiple raycasts to different parts of player?
            float headHeight = playerHead.position.y;
            Vector3 playerLookPos = playerHead.position;
            Vector3 myLookPos = transform.position + Vector3.up * headHeight;
            Vector3 toDir = playerLookPos - myLookPos;
            Debug.DrawRay(myLookPos, toDir * 10, Color.red);
            if (Physics.Raycast(myLookPos, toDir, out RaycastHit hit, 100, ignoreLayerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    // hit the player
                    detected = true;
                }
                if (debugLogRaycastResult)
                {
                    Debug.Log("hit " + hit.collider.name);
                }
            }
            return detected;
        }
        void SetRespawn()
        {
            respawnT = new GameObject(name + " Respawn Point").transform;
            respawnT.position = transform.position;
            respawnT.rotation = transform.rotation;
        }
        public void Respawn()
        {
            if (respawnT)
            {
                transform.position = respawnT.position;
                transform.rotation = respawnT.rotation;
            }
        }

        private void RandomDialog()
        {
            float randNum = Random.Range(0, 1000 / Time.deltaTime);
            if (randNum <= 1)
            {
                audioSource.PlayOneShot(readyOrNotClip);
            }
        }


#if UNITY_EDITOR
            private void OnDrawGizmosSelected()
            {
                Vector3 pointOffset = Vector3.up * 0.1f;
                if (agent && agent.destination != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(agent.destination + pointOffset, 0.2f);
                    for (int i = 0; i < agent.path.corners.Length - 1; i++)
                    {
                        Vector3 point = agent.path.corners[i] + pointOffset;
                        Vector3 point2 = agent.path.corners[i + 1] + pointOffset;
                        Gizmos.DrawLine(point, point2);
                    }
                }
                if (usePatrolPoints && patrolPath != null && patrolPath.Count > 1)
                {
                    Gizmos.color = Color.blue;
                    for (int i = 0; i < patrolPath.Count - 1; i++)
                    {
                        Gizmos.DrawLine(patrolPath[i].position + pointOffset, patrolPath[i + 1].position + pointOffset);
                    }
                }
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(mapCenterOffset, 0.3f);
                Handles.DrawWireDisc(mapCenterOffset, Vector3.up, maxDist);
            }
#endif
    }


}