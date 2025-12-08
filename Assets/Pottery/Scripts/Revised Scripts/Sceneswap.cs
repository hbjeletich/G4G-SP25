using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Pottery
{

    // Manages UI panels and transitions between them//

    public class UIManager : MonoBehaviour
    {

        // Assign in different panels in Inspector//
        public GameObject mainMenuPanel;
        public GameObject settingsPanel;
        public GameObject galleryPanel;
        public GameObject SelectionPanel;
        public GameObject gameStage_1_Panel;
        public GameObject gameStage_2_Panel;
        public GameObject confirmation_Panel;
        public GameObject pot;
        public GameObject wheel;

        private bool gameStage1Completed = false;

        void Start()
        {
            pot.SetActive(false);
            wheel.SetActive(false);
        }


        // functions to show different panels//

        public void ShowMainMenu()
        {
            SetActivePanel(mainMenuPanel);
        }

        //function to select a button after a frame delay. Give time for the new panel to load so the button will acually be in the scene//
        public void SelectButton(GameObject button)
        {
            StartCoroutine(SelectAfterFrame(button));
        }

        private IEnumerator SelectAfterFrame(GameObject button)
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button);
        }

        public void ShowSettings()
        {
            SetActivePanel(settingsPanel);
        }

        // add delay to any choosen canvas, this allow time for the screenshot script to take a picture of the pot at the end.//
        public void ShowWithDelay(GameObject CanvasToShow)
        {
            StartCoroutine(ShowCanvasWithDelay(CanvasToShow));
        }

        private IEnumerator ShowCanvasWithDelay(GameObject CanvasToShow)
        {
            yield return null;
            yield return null;

            SetActivePanel(CanvasToShow);
        }

        public void ShowGallery()
        {
            SetActivePanel(galleryPanel);
        }


        public void ShowSelection()
        {
            SetActivePanel(SelectionPanel);
        }

        public void ShowGameStage1()
        {
            SetActivePanel(gameStage_1_Panel);
        }

        public void ShowGameStage2()
        {
            SetActivePanel(gameStage_2_Panel);
        }

        public void ShowConfirmationMenu()
        {
            SetActivePanel(confirmation_Panel);
        }


        // function to set only the active panel visible//
        private void SetActivePanel(GameObject activePanel)
        {
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(false);
            galleryPanel.SetActive(false);
            SelectionPanel.SetActive(false);
            gameStage_1_Panel.SetActive(false);
            gameStage_2_Panel.SetActive(false);
            confirmation_Panel.SetActive(false);

            activePanel.SetActive(true);
        }

        // functions to show and hide the pot and wheel//

        public void ShowPot()
        {
            pot.SetActive(true);
            wheel.SetActive(true);
        }

        public void HidePot()
        {
            StartCoroutine(HidePotWithDelay());
        }


        // hide the pot after a delay to allow time for other processes to complete, Screenshot capture etc.//
        private IEnumerator HidePotWithDelay()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            pot.SetActive(false);
            wheel.SetActive(false);
        }
    }
}