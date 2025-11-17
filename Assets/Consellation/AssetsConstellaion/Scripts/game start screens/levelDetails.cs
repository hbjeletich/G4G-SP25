using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; 

public class levelDetails : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject namePanel;      
    public TMP_Text textObj;         
    [TextArea]
    public string nameText;        

    public void OnPointerEnter(PointerEventData eventData)
    {
        textObj.text = nameText;
        namePanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        namePanel.SetActive(false);
    }
}
