using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneButtonCarousel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string targetSceneName;
    public Image[] attachedImages;
    
    [Header("Game Info (for carousel display)")]
    public string gameTitle;
    public Sprite gameImage;

    private Button button;
    private Image buttonImage;
    private Color originalColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }

        // carousel handles this now
        // if (button != null)
        // {
        //     button.onClick.AddListener(() => GameSelect.Instance.OpenScene(targetSceneName));
        // }
    }

    private void Start()
    {
        SynchronizeColors(originalColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color hoverColor = button.colors.highlightedColor;
        SynchronizeColors(hoverColor);
        
        // play hover sound
        if (GameSelect.Instance != null)
        {
            GameSelect.Instance.HoverButton();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SynchronizeColors(button.colors.normalColor);
    }

    private void SynchronizeColors(Color targetColor)
    {
        if (buttonImage != null)
        {
            buttonImage.color = targetColor;
        }

        foreach (Image img in attachedImages)
        {
            if (img != null)
            {
                img.color = targetColor;
            }
        }
    }
}
