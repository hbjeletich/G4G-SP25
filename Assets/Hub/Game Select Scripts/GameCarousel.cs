using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameCarousel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GameData[] games;
    [SerializeField] private GameObject gameCardPrefab;

    [Header("References")]
    [SerializeField] private RectTransform content;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button playButton;

    [Header("Layout Settings")]
    [SerializeField] private float cardWidth = 800f;
    [SerializeField] private float cardSpacing = 50f;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sound")]
    [SerializeField] private float navigateSoundPitch = 1.0f;

    private int currentIndex = 0;
    private int totalCards;
    private bool isAnimating = false;
    private GameCard[] gameCards;

    private void Start()
    {
        InitializeCarousel();

        leftButton.onClick.AddListener(GoToPrevious);
        rightButton.onClick.AddListener(GoToNext);

        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayCurrentGame);
        }

        UpdateButtonStates();
        UpdateSelectedCard();
    }

    private void InitializeCarousel()
    {
        // Clear any existing children
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        totalCards = games.Length;
        gameCards = new GameCard[totalCards];

        // Instantiate cards
        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(gameCardPrefab, content);
            GameCard card = cardObj.GetComponent<GameCard>();

            if (card != null)
            {
                card.Setup(games[i]);
                gameCards[i] = card;
            }

            // Position the card
            RectTransform cardRect = cardObj.GetComponent<RectTransform>();
            float xPos = i * (cardWidth + cardSpacing);
            cardRect.anchoredPosition = new Vector2(xPos, 0);
        }

        // Size the content to fit all cards
        float totalWidth = totalCards * cardWidth + (totalCards - 1) * cardSpacing;
        content.sizeDelta = new Vector2(totalWidth, content.sizeDelta.y);
    }

    public void GoToNext()
    {
        if (isAnimating || currentIndex >= totalCards - 1) return;

        PlayNavigateSound();
        currentIndex++;
        AnimateToCurrentIndex();

        UpdateSelectedCard();
    }

    public void GoToPrevious()
    {
        if (isAnimating || currentIndex <= 0) return;

        PlayNavigateSound();
        currentIndex--;
        AnimateToCurrentIndex();

        UpdateSelectedCard();
    }

    public void PlayCurrentGame()
    {
        if (isAnimating || games.Length == 0) return;

        GameData currentGame = games[currentIndex];
        if (currentGame != null && !string.IsNullOrEmpty(currentGame.targetSceneName))
        {
            GameSelect.Instance.OpenScene(currentGame.targetSceneName);
        }
    }

    private void AnimateToCurrentIndex()
    {
        float targetX = -currentIndex * (cardWidth + cardSpacing);
        StartCoroutine(AnimateContent(targetX));
    }

    private IEnumerator AnimateContent(float targetX)
    {
        isAnimating = true;
        UpdateButtonStates();

        Vector2 startPos = content.anchoredPosition;
        Vector2 endPos = new Vector2(targetX, startPos.y);

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = easingCurve.Evaluate(elapsed / animationDuration);
            content.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        content.anchoredPosition = endPos;
        isAnimating = false;
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        leftButton.interactable = !isAnimating && currentIndex > 0;
        rightButton.interactable = !isAnimating && currentIndex < totalCards - 1;
    }

    private void PlayNavigateSound()
    {
        if (GameSelect.Instance != null)
        {
            GameSelect.Instance.HoverButton(navigateSoundPitch);
        }
    }

    private void UpdateSelectedCard()
    {
        for (int i = 0; i < gameCards.Length; i++)
        {
            if (gameCards[i] != null)
            {
                gameCards[i].SetSelected(i == currentIndex);
            }
        }
    }

    public GameData GetCurrentGameData()
    {
        if (games.Length > 0 && currentIndex < games.Length)
        {
            return games[currentIndex];
        }
        return null;
    }
}