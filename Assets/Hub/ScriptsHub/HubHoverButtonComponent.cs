using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HubHoverButtonComponent : MonoBehaviour, IPointerEnterHandler
{
    // play sound on hover button through game select singleton
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameSelect.Instance.HoverButton(Random.Range(0.75f, 0.85f));

    }
}
