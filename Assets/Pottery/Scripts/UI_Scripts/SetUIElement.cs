using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetUIElement : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Selectable elementToSelect;

    [Header("Visualization")]
    [SerializeField] private bool showVisualization;
    [SerializeField] private Color navigationColor = Color.cyan;

    private void onDrawGizmos()
    {
        if (!showVisualization)
            return;

        if (elementToSelect == null)
            return;

        Gizmos.color = navigationColor;
        Gizmos.DrawLine(gameObject.transform.position, elementToSelect.gameObject.transform.position);
    }


    private void Reset() 
    {
    
        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem == null)
        Debug.Log("did not find Event System in your scene, please add one");

    }

    public void SetSelectedUIElement()
    {
        if (eventSystem == null)
            return;
            Debug.Log("Event System is null");

        if (elementToSelect == null)
            return;

           Debug.Log("Element to select is null");


        eventSystem.SetSelectedGameObject(elementToSelect.gameObject);
    }















}
