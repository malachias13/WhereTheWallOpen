using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ManaFlux
{
    public class PauseMenu : MonoBehaviour
    {
        PlayerControls controls;
        bool isPaused = false;
        CanvasGroup canvasGroup;
        public CanvasGroup optionsCanvasGroup;
        public AudioMixer audioMixer;
        bool showingOptions = false;
        CamSensitivity camSensitivity;
        PlayerStats playerStats;
        public GameObject pauseVolume;

        private void Awake()
        {
            playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
            canvasGroup = GetComponent<CanvasGroup>();
            camSensitivity = GameObject.FindObjectOfType<CamSensitivity>();
            controls = new PlayerControls();
            controls.Movement.Pause.performed += c => TogglePause();
            Unpause();
        }
        private void OnEnable()
        {
            controls.Enable();
        }
        private void OnDisable()
        {
            controls.Disable();
        }
        public void Pause()
        {
            isPaused = true;
            ShowMenu(optionsCanvasGroup, false);
            ShowMenu(canvasGroup, true);
            Time.timeScale = 0;
            SetCursorLock(false);
            if (pauseVolume) pauseVolume.SetActive(true);
        }
        public void Unpause()
        {
            isPaused = false;
            ShowMenu(canvasGroup, false);
            ShowMenu(optionsCanvasGroup, false);
            Time.timeScale = 1;
            SetCursorLock(true);
            if (pauseVolume) pauseVolume.SetActive(false);
        }
        void ShowMenu(CanvasGroup menu, bool shown = true)
        {
            if (shown)
            {
                menu.alpha = 1;
                menu.interactable = true;
                menu.blocksRaycasts = true;
            } else
            {
                menu.alpha = 0;
                menu.interactable = false;
                menu.blocksRaycasts = false;
            }
        }
        public void TogglePause()
        {
            if (isPaused)
            {
                Unpause();
            } else
            {
                Pause();
            }
        }
        public bool IsPaused()
        {
            return isPaused;
        }
        void SetCursorLock(bool locked)
        {
            if (locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                // Cursor.visible = false;
            } else
            {
                Cursor.lockState = CursorLockMode.None;
                // Cursor.visible = true;
            }
        }
        public void SetMusicVolume(float val)
        {
            SetVolume("Music", val);
        }
        public void SetSfxVolume(float val)
        {
            SetVolume("Sfx", val);
        }
        public void SetDialogVolume(float val)
        {
            SetVolume("Dialog", val);
        }
        void SetVolume(string mixerName, float val)
        {
            val = Mathf.Clamp(val, 0.0001f, 2f);
            float volume = Mathf.Log10(val) * 20;
            Debug.Log("setting volume to " + volume + " (" + val + ")");
            audioMixer.SetFloat(mixerName + "Volume", volume);
        }
        public void SetCamSensitivityX(float value)
        {
            if (!camSensitivity)
                camSensitivity = GameObject.FindObjectOfType<CamSensitivity>();
            camSensitivity.SetSensitivityX(value);
        }
        public void SetCamSensitivityY(float value)
        {
            if (!camSensitivity)
                camSensitivity = GameObject.FindObjectOfType<CamSensitivity>();
            camSensitivity.SetSensitivityY(value);
        }
        void RemapButtonClicked(InputAction actionToRebind)
        {
            var rebindOperation = actionToRebind.PerformInteractiveRebinding()
                        // To avoid accidental input from mouse motion
                        .WithControlsExcluding("Mouse")
                        .OnMatchWaitForAnother(0.1f)
                        .Start();
        }
        public void GoToOptionsMenu()
        {
            ShowMenu(optionsCanvasGroup, true);
            ShowMenu(canvasGroup, false);
        }
        public void GoToPauseMenu()
        {
            ShowMenu(canvasGroup, true);
            ShowMenu(optionsCanvasGroup, false);
        }
        public void RestartLevel()
        {
            if (playerStats)
            {
                playerStats.ReloadLevel();
            }
            Unpause();
        }
        public void BackToStart()
        {
            Unpause();
            SceneManager.LoadScene(0);
        }
        public void ToggleFullScreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}