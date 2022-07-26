﻿using Core.Managers;
using Interfaces.Core.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler

    {
        //TODO make World Map component style from here and start main menu all together
        private IGameStateManager _gameStateManager;
        public Text topText;

        public GameObject volumePanel;
        public GameObject soundSlider;
        public GameObject musicSlider;

        public bool volumePanelActive;

        private void Start()
        {
            _gameStateManager = FindObjectOfType<GameStateManager>();
            _gameStateManager.ConfigNewGame();

            int currentHighScore = PlayerPrefs.GetInt("highScore", 0);
            topText.text = "TOP- " + currentHighScore.ToString("D6");

            if (!PlayerPrefs.HasKey("soundVolume")) {
                PlayerPrefs.SetFloat("soundVolume", 1);
            }

            if (!PlayerPrefs.HasKey("musicVolume")) {
                PlayerPrefs.SetFloat("musicVolume", 1);
            }

            soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("soundVolume");
            musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("musicVolume");

            Debug.Log(this.name + " Start: Volume Setting sound=" + PlayerPrefs.GetFloat("soundVolume")
                      + "; music=" + PlayerPrefs.GetFloat("musicVolume"));
        }
#if ENABLE_LEGACY_INPUT_MANAGER
        public void OnMouseHover(Button button)
        {
            Debug.Log("Mouse Hover: " + button.name);
            if (volumePanelActive) return;
            GameObject cursor = button.transform.Find("Cursor").gameObject;
            cursor.SetActive(true);
        }

        public void OnMouseHoverExit(Button button)
        {
            if (volumePanelActive) return;
            GameObject cursor = button.transform.Find("Cursor").gameObject;
            cursor.SetActive(false);
        }
#endif

#if ENABLE_INPUT_SYSTEM
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Mouse Hover: " + eventData.hovered);
            if (volumePanelActive) return;
            GameObject cursor = eventData.pointerEnter.transform.Find("Cursor").gameObject;
            cursor.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!eventData.fullyExited) return;
            Debug.Log("Mouse Hover: " + eventData.hovered);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Mouse Hover: " + eventData.hovered);
        }
#endif

        public void StartNewGame()
        {
            if (volumePanelActive) return;
            _gameStateManager.SceneToLoad = "World 1-1";
            SceneManager.LoadScene("Level Start Screen");
        }

        public void StartWorld1_2()
        {
            if (volumePanelActive) return;
            _gameStateManager.SceneToLoad = "World 1-2";
            SceneManager.LoadScene("Level Start Screen");
        }

        public void StartWorld1_3()
        {
            if (volumePanelActive) return;
            _gameStateManager.SceneToLoad = "World 1-3";
            SceneManager.LoadScene("Level Start Screen");
        }


        public void StartWorld1_4()
        {
            if (volumePanelActive) return;
            _gameStateManager.SceneToLoad = "World 1-4";
            SceneManager.LoadScene("Level Start Screen");
        }

        public void QuitGame()
        {
            if (!volumePanelActive) {
                Application.Quit();
            }
        }

        public void SelectVolume()
        {
            volumePanel.SetActive(true);
            volumePanelActive = true;
        }

        public void SetVolume()
        {
            PlayerPrefs.SetFloat("soundVolume", soundSlider.GetComponent<Slider>().value);
            PlayerPrefs.SetFloat("musicVolume", musicSlider.GetComponent<Slider>().value);
            volumePanel.SetActive(false);
            volumePanelActive = false;
        }

        public void CancelSelectVolume()
        {
            soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("soundVolume");
            musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("musicVolume");
            volumePanel.SetActive(false);
            volumePanelActive = false;
        }
    }
}