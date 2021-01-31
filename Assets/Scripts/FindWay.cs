using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ManaFlux
{
    public class FindWay : MonoBehaviour
    {
        public GameObject wayEffectPrefab;
        public float minSanityToUse = 30f;
        public float sanityCost = 30f;
        List<Transform> targets = new List<Transform>();
        PlayerStats playerStats;

        PlayerControls controls;

        private void Awake()
        {
            controls = new PlayerControls();
            controls.Movement.FindWay.performed += c => TryShowWay();
        }
        private void OnEnable()
        {
            controls.Enable();
        }
        private void OnDisable()
        {
            controls.Disable();
        }
        private void Start()
        {
            FindAllTargets();
            playerStats = transform.parent.GetComponent<PlayerStats>();
        }

        void FindAllTargets()
        {
            // get all collectorobjects sorted
            var sortedCOs = new List<CollectorObject>(GameObject.FindObjectsOfType<CollectorObject>());
            sortedCOs.Sort(
                (a, b) => a.id.CompareTo(b.id)
            );

            foreach (var co in sortedCOs)
            {
                targets.Add(co.transform);
            }
            // level changer is last target
            targets.Add(GameObject.FindGameObjectWithTag("Finish").transform);
        }

        void TryShowWay()
        {
            Debug.Log("finding way...");
            if (playerStats.currentSanity >= minSanityToUse)
            {
                SpawnEffect();
                playerStats.currentSanity -= sanityCost;
                playerStats.UpdateSanityBar();
            }
        }

        void SpawnEffect()
        {
            var waygo = Instantiate(wayEffectPrefab);
            waygo.transform.position = transform.position;
            var forwardDir = targets[playerStats.lastCollected].position - transform.position;
            Quaternion wRot = Quaternion.LookRotation(forwardDir, Vector3.up);
            waygo.transform.rotation = wRot;
        }
    }
}