using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace ManaFlux
{
    /// <summary>
    /// Plays a dialog line for a duration with subtitles
    /// to use: call DialogPlayer.Instance.PlayLine(dialogLine); from anywhere
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DialogPlayer : MonoBehaviour
    {
        [System.Serializable]
        public class DialogLine
        {
            public AudioClip clip;
            public string text;
            public float duration = 3;
        }

        [SerializeField] DialogLine firstLine;
        [SerializeField] DialogLine firstRespawnLine;
        [SerializeField] List<DialogLine> randomizedRespawnLines = new List<DialogLine>();
        bool firstLinePlayed = false;
        bool firstLoad = true;
        [HideInInspector] public int myLevel;

        List<DialogLine> curLines = new List<DialogLine>();

        Animator subtitleAnim;
        TMP_Text subtitleText;
        AudioSource audioSource;
        public static DialogPlayer Instance;

        // called only once
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            myLevel = SceneManager.GetActiveScene().buildIndex;
            if (!Instance)
            {
                Instance = this;
            } else if (Instance.myLevel != myLevel)
            {
                // we are on a new level, delete the old one
                Destroy(Instance.gameObject);
                Instance = this;
            } else
            {
                // we are a duplicate
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            PlayLine(firstLine);
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += CheckLevel;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= CheckLevel;
        }
        void CheckLevel(Scene scene, LoadSceneMode loadSceneMode)
        {
            OnReload();
        }
        public void OnReload()
        {
            subtitleAnim = GameObject.FindGameObjectWithTag("Subtitle")?.GetComponent<Animator>();
            if (subtitleAnim)
            {
                subtitleText = subtitleAnim.GetComponentInChildren<TMP_Text>();
            }
            if (firstLoad)
            {
                firstLoad = false;
            } else
            {
                PlayRespawnLine();
            }
        }
        void ReloadRespawnLines()
        {
            curLines = new List<DialogLine>(randomizedRespawnLines);
        }
        void PlayRespawnLine()
        {
            if (!firstLinePlayed)
            {
                firstLinePlayed = true;
                PlayLine(firstRespawnLine);
            } else
            {
                if (curLines.Count == 0)
                {
                    ReloadRespawnLines();
                }
                int r = Random.Range(0, curLines.Count);
                DialogLine line = curLines[r];
                curLines.RemoveAt(r);
                PlayLine(line);
            }
        }
        public static float GetDuration(DialogLine line)
        {
            if (line.clip)
            {
                line.duration = line.clip.length;
                return line.clip.length;
            }
            return line.duration;
        }
        public void PlayLine(DialogLine line)
        {

            if (subtitleText)
            {
                subtitleText.text = line.text;
                subtitleAnim.SetBool("Shown", true);
                StopAllCoroutines();
                StartCoroutine(ResetSubtitle(GetDuration(line)));
            }
            if (line.clip)
            {
                audioSource.PlayOneShot(line.clip);
            }
        }
        IEnumerator ResetSubtitle(float dur)
        {
            yield return new WaitForSecondsRealtime(dur);
            if (subtitleAnim != null)
            {
                subtitleAnim.SetBool("Shown", false);
            }
        }
    }
}