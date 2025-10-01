using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string targetSceneName; // open game scene by name
    public Image[] attachedImages;

    private Button button;
    private Image buttonImage;
    private Color originalColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if(buttonImage != null )
        {
            originalColor = buttonImage.color;
        }

        if (button != null)
        {
            button.onClick.AddListener(() => SceneManager.LoadScene(targetSceneName));
        }
    }

    private void Start()
    {
        SynchronizeColors(originalColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color hoverColor = button.colors.highlightedColor;
        SynchronizeColors(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SynchronizeColors(button.colors.normalColor);
    }

    private void SynchronizeColors(Color targetColor)
    {
        // Update button color
        if (buttonImage != null)
        {
            buttonImage.color = targetColor;
        }

        // Update attached images colors
        foreach (Image img in attachedImages)
        {
            if (img != null)
            {
                img.color = targetColor;
            }
        }
    }
}
