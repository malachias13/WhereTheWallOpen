using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace ManaFlux
{
    public class StartMenu : MonoBehaviour
    {
        public LevelChanger levelChanger;
        public Image titleImage;
        public Image background;
        public Text startText;
        public Text exitText;

        bool isDoneFading = false;
        void Awake()
        {
            // Debug.LogError("hello"); 
            FadeMenu();
            Cursor.lockState = CursorLockMode.None;
        }

        public void Click()
        {
            Debug.Log("Cliked");
            levelChanger.nextScene();
        }
        
        public void QuitGame()
        {
            Debug.Log("Quit Clicked");
            Application.Quit();
        }
        public void FadeMenu()
        {
            background.DOFade(0f, 3f).SetDelay(0.2f).SetEase(Ease.InBack);
            titleImage.DOFade(1f,3f).SetDelay(1f).SetEase(Ease.InBack);
            startText.DOFade(1f, 3f).SetDelay(1.5f).SetEase(Ease.InBack);
            exitText.DOFade(1f, 3f).SetDelay(2f).SetEase(Ease.InBack);
            Invoke("Check", 2.1f);
        }
        public void StartTextHoverOver()
        {
            if (isDoneFading)
            {
                startText.fontSize = 75;
                startText.color = new Color(255, 235, 0);
            }
        }
        public void StartTextUnHover()
        {
            if (isDoneFading)
            {
                startText.fontSize = 67;
                startText.color = Color.white;
            }
        }

        public void ExitTextHoverOver()
        {
            if (isDoneFading)
            {
                exitText.fontSize = 60;
                exitText.color = new Color(255, 235, 0);
            }
        }
        public void ExitTextUnHover()
        {
            if (isDoneFading)
            {
                exitText.fontSize = 51;
                exitText.color = Color.white;
            }
        }
        private void Check()
        {
            isDoneFading = true;
        }



        


    }
}