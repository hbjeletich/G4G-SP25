using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SetUIElement : MonoBehaviour
{
    [Header("Visualization")]
    [SerializeField] private bool showVisualization;
    [SerializeField] private Color navigationColor = Color.cyan;
    [SerializeField] private GameObject Back;
    [SerializeField] private GameObject Play;

    //private void onDrawGizmos()
    //{
    //    if (!showVisualization)
    //        return;

    //    if (elementToSelect == null)
    //        return;

    //    Gizmos.color = navigationColor;
    //    Gizmos.DrawLine(gameObject.transform.position, elementToSelect.gameObject.transform.position);
    //}

    public void SetSelectedUIElementPlay()
    {
        EventSystem.current.SetSelectedGameObject(Play);
    }

    public void SetSelectedUIElementBack()
    {
        EventSystem.current.SetSelectedGameObject(Back);
    }














}
