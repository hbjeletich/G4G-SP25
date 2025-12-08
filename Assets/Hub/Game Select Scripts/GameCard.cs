using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image gameImage;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Scale Animation")]
    [SerializeField] private float selectedScaleMultiplier = 1.1f;
    [SerializeField] private float deselectedScaleMultiplier = 0.9f;
    [SerializeField] private float scaleAnimationDuration = 0.25f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private GameData data;
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;
    private bool isSelected = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

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

        // start at deselected scale
        transform.localScale = originalScale * deselectedScaleMultiplier;
    }

    public void SetSelected(bool selected)
    {
        if (isSelected == selected) return;

        isSelected = selected;
        float targetMultiplier = selected ? selectedScaleMultiplier : deselectedScaleMultiplier;
        Vector3 targetScale = originalScale * targetMultiplier;

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
        scaleCoroutine = StartCoroutine(AnimateScale(targetScale));
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;

        float elapsed = 0f;

        while (elapsed < scaleAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = scaleCurve.Evaluate(elapsed / scaleAnimationDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        scaleCoroutine = null;
    }

    public GameData GetData()
    {
        return data;
    }
}