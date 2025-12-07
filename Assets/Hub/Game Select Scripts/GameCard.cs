using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image gameImage;
    [SerializeField] private TextMeshProUGUI titleText;

    private GameData data;

    public void Setup(GameData gameData)
    {
        data = gameData;

        if (gameImage != null && data.gameImage != null)
        {
            gameImage.sprite = data.gameImage;
        }

        if (titleText != null)
        {
            titleText.text = data.gameTitle;
        }
    }

    public GameData GetData()
    {
        return data;
    }
}
