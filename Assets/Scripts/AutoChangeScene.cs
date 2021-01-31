using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ManaFlux
{
    public class AutoChangeScene : MonoBehaviour
    {
        public float changeAfter = 22;
        public GameObject firstScreen;
        public GameObject nextScreen;
        private void Start()
        {
            if (changeAfter >= 0)
            {
                Invoke("NextScene", changeAfter);
            }
        }
        public void NextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        public void ShowNextScreen()
        {
            firstScreen.SetActive(false);
            nextScreen.SetActive(true);
        }
    }
}