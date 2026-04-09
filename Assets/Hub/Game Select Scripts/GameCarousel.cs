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

    [Header("Focused View (3 visible)")]
    [SerializeField] private float cardScaleMultiplier = 1.50f;
    [SerializeField] private float focusedSpreadX = 500f;
    [SerializeField] private float focusedNeighborScale = 0.75f;
    [SerializeField] private float focusedNeighborAlpha = 0.6f;
    [SerializeField] private float focusedNeighborOffsetY = -30f;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.35f;
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sound")]
    [SerializeField] private float navigateSoundPitch = 1.0f;

    private int currentIndex = 0;
    private int totalCards;
    private bool isAnimating = false;
    private GameCard[] gameCards;
    private RectTransform[] cardRects;
    private CanvasGroup[] cardGroups;

    private void Start()
    {
        InitializeCarousel();

        leftButton.onClick.AddListener(GoToPrevious);
        rightButton.onClick.AddListener(GoToNext);

        if (playButton != null)
            playButton.onClick.AddListener(PlayCurrentGame);

        LayoutCardsImmediate();
        UpdateSelectedCard();
    }

    private void InitializeCarousel()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        totalCards = games.Length;
        gameCards = new GameCard[totalCards];
        cardRects = new RectTransform[totalCards];
        cardGroups = new CanvasGroup[totalCards];

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(gameCardPrefab, content);
            GameCard card = cardObj.GetComponent<GameCard>();

            if (card != null)
            {
                card.Setup(games[i]);
                gameCards[i] = card;
            }

            cardRects[i] = cardObj.GetComponent<RectTransform>();
            cardRects[i].anchorMin = new Vector2(0.5f, 0.5f);
            cardRects[i].anchorMax = new Vector2(0.5f, 0.5f);
            cardRects[i].pivot = new Vector2(0.5f, 0.5f);
            cardRects[i].anchoredPosition = Vector2.zero;

            cardGroups[i] = cardObj.GetComponent<CanvasGroup>();
            if (cardGroups[i] == null)
                cardGroups[i] = cardObj.AddComponent<CanvasGroup>();
        }
    }

    public void GoToNext()
    {
        if (isAnimating || totalCards == 0) return;
        PlayNavigateSound();
        currentIndex = (currentIndex + 1) % totalCards;
        AnimateLayout();
        UpdateSelectedCard();
    }

    public void GoToPrevious()
    {
        if (isAnimating || totalCards == 0) return;
        PlayNavigateSound();
        currentIndex = (currentIndex - 1 + totalCards) % totalCards;
        AnimateLayout();
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

    public GameData GetCurrentGameData()
    {
        if (games.Length > 0 && currentIndex < games.Length)
            return games[currentIndex];
        return null;
    }

    private struct CardLayout
    {
        public Vector2 position;
        public float scale;
        public float alpha;
        public int siblingIndex;
    }

    private CardLayout GetFocusedLayout(int cardIndex)
    {
        CardLayout layout;
        int offset = ShortestOffset(cardIndex, currentIndex, totalCards);

        if (offset == 0)
        {
            layout.position = Vector2.zero;
            layout.scale = cardScaleMultiplier;
            layout.alpha = 1f;
            layout.siblingIndex = totalCards;
        }
        else if (Mathf.Abs(offset) == 1)
        {
            layout.position = new Vector2(offset * focusedSpreadX, focusedNeighborOffsetY);
            layout.scale = focusedNeighborScale;
            layout.alpha = focusedNeighborAlpha;
            layout.siblingIndex = totalCards - 1;
        }
        else
        {
            float sign = (offset > 0) ? 1f : -1f;
            layout.position = new Vector2(sign * focusedSpreadX * 2.5f, 0f);
            layout.scale = 0.5f;
            layout.alpha = 0f;
            layout.siblingIndex = 0;
        }

        return layout;
    }

    private int ShortestOffset(int from, int to, int count)
    {
        if (count == 0) return 0;
        int raw = from - to;
        int half = count / 2;
        if (raw > half) raw -= count;
        if (raw < -half) raw += count;
        return raw;
    }

    private void LayoutCardsImmediate()
    {
        for (int i = 0; i < totalCards; i++)
        {
            CardLayout target = GetFocusedLayout(i);
            ApplyLayout(i, target);
        }
    }

    private void ApplyLayout(int i, CardLayout layout)
    {
        cardRects[i].anchoredPosition = layout.position;
        cardRects[i].localScale = Vector3.one * layout.scale;
        cardGroups[i].alpha = layout.alpha;
        cardRects[i].SetSiblingIndex(layout.siblingIndex);
        cardGroups[i].blocksRaycasts = layout.alpha > 0.1f;
    }

    private void AnimateLayout()
    {
        StartCoroutine(AnimateAllCards());
    }

    private IEnumerator AnimateAllCards()
    {
        isAnimating = true;

        Vector2[] startPos = new Vector2[totalCards];
        float[] startScale = new float[totalCards];
        float[] startAlpha = new float[totalCards];
        CardLayout[] targets = new CardLayout[totalCards];

        for (int i = 0; i < totalCards; i++)
        {
            startPos[i] = cardRects[i].anchoredPosition;
            startScale[i] = cardRects[i].localScale.x;
            startAlpha[i] = cardGroups[i].alpha;
            targets[i] = GetFocusedLayout(i);
            cardRects[i].SetSiblingIndex(targets[i].siblingIndex);
        }

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = easingCurve.Evaluate(Mathf.Clamp01(elapsed / animationDuration));

            for (int i = 0; i < totalCards; i++)
            {
                cardRects[i].anchoredPosition = Vector2.Lerp(startPos[i], targets[i].position, t);
                float s = Mathf.Lerp(startScale[i], targets[i].scale, t);
                cardRects[i].localScale = Vector3.one * s;
                cardGroups[i].alpha = Mathf.Lerp(startAlpha[i], targets[i].alpha, t);
            }

            yield return null;
        }

        for (int i = 0; i < totalCards; i++)
            ApplyLayout(i, targets[i]);

        isAnimating = false;
    }

    private void PlayNavigateSound()
    {
        if (GameSelect.Instance != null)
            GameSelect.Instance.HoverButton(navigateSoundPitch);
    }

    private void UpdateSelectedCard()
    {
        for (int i = 0; i < gameCards.Length; i++)
        {
            if (gameCards[i] != null)
                gameCards[i].SetSelected(i == currentIndex);
        }
    }
}