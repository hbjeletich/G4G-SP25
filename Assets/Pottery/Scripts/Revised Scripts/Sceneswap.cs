using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class UIManager : MonoBehaviour
{

    // UI panels references
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
        //ShowMainMenu();
        pot.SetActive(false);
        wheel.SetActive(false);
    }


    public void ShowMainMenu()
    {
        SetActivePanel(mainMenuPanel);
    }


    public void SelectButton(GameObject button)
    {
        StartCoroutine(SelectAfterFrame(button));
    }


    private IEnumerator SelectAfterFrame(GameObject button)
    {
        yield return null; // wait 1 frame so UI activates
        EventSystem.current.SetSelectedGameObject(null); // optional: clear previous selection
        EventSystem.current.SetSelectedGameObject(button); // now select
    }


    public void ShowSettings()
    {
        SetActivePanel(settingsPanel);
        
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
        
        //set gameStage1Completed = true; // For demonstration, mark stage 1 as completed when accessed
        SetActivePanel(gameStage_1_Panel);
    }

   
    public void ShowGameStage2()
    {

        SetActivePanel(gameStage_2_Panel);


        //if (gameStage1Completed)
        //{
        //    SetActivePanel(gameStage_2_Panel);
        //}
        //else
        //{
        //    Debug.Log("Complete Game Stage 1 to access Game Stage 2.");
        //}
    }

    public void ShowConfirmationMenu()
    {
        SetActivePanel(confirmation_Panel);
    }

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

    public void ShowPot()
    {
        pot.SetActive(true);
        wheel.SetActive(true);
    }

    public void HidePot()
    {
        pot.SetActive(false);
        wheel.SetActive(false);
    }

}

