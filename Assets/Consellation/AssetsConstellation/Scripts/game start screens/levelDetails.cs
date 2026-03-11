using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; 

public class levelDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject namePanel;      
    public TMP_Text textObj;
    public TMP_Text descriptionObj;
    public string nameText;
    [TextArea]
    public string descriptionText;


    // called when the mouse pointer hovers over the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        textObj.text = nameText;
        descriptionObj.text = descriptionText;
        namePanel.SetActive(true);
    }

    //called when the mouse pointer stops hovering over the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        namePanel.SetActive(false);
    }
}
