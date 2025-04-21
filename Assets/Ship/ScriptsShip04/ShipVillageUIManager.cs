using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ship
{
public class ShipVillageUIManager : MonoBehaviour
{
    [Header("Text Box")]
    public TMP_Text textBoxText;
    public CanvasGroup textBoxCanavasGroup;
    public float textTypingSpeed = 0.025f; 

    [Header("Text Sound")]
    public AudioSource audioSource;
    public AudioClip textSound;

    [Header("Character Portrait")]
    public Image characterPortraitImage;
    [HideInInspector] public bool textTyping = false;
    private bool htmlText = false;

    private DialogManager dialogManager;
    private ShipPlayerController playerOverworld; 
    // private TextBoxAnimation textBoxAnimation;

    private void Awake()
    {
        dialogManager = FindObjectOfType<DialogManager>(); 
        playerOverworld = FindObjectOfType<ShipPlayerController>();
        audioSource = gameObject.AddComponent<AudioSource>(); 
        // textBoxAnimation = GetComponentInChildren<TextBoxAnimation>();
    }

    private void Start()
    {
        HideTextBox();
        ClearText();
    }
    
    public void HideTextBox()
    {
        // textBoxAnimation.Close();
        ClearText();
        playerOverworld.EnablePlayerController();
    }

    public void ClearText()
    {
        textBoxText.text = "";
    }
    
    public void ShowText(string aText)
    {
        playerOverworld.DisablePlayerController();
        textBoxCanavasGroup.alpha = 1;
        // textBoxAnimation.Open();
        StartCoroutine(AddOneCharEnumerator(aText));
    }

    private IEnumerator AddOneCharEnumerator(string aText)
    {
        textTyping = true;

        for (int i = 0; i < aText.Length; i++)
        {
            // Handle HTML tags
            if (aText[i] == '<')
            {
                htmlText = true;
            }

            if (!htmlText)
            {
                // Detect the "\!" pause command
                if (i < aText.Length - 1 && aText[i] == '\\' && aText[i + 1] == '!')
                {
                    i++; // Skip the "!" character
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0));
                    continue;
                }

                // Add normal characters to the text
                textBoxText.text += aText[i];

                if (!audioSource.isPlaying) // Play typing sound
                {
                    audioSource.pitch = Random.Range(0.5f, 0.7f);
                    audioSource.PlayOneShot(textSound);
                }
                yield return new WaitForSeconds(textTypingSpeed); // Typing delay/speed
            }

            // Handle HTML tags
            if (aText[i] == '>')
            {
                htmlText = false;

                int htmlStart = aText.LastIndexOf('<', i); 
                string htmlFull = aText.Substring(htmlStart, i - htmlStart + 1); 
                textBoxText.text += htmlFull;  // Add the full HTML tag to the text
            }
        }

        textTyping = false;
    }

    public void ShowOrHidePortrait(Sprite newPortrait)
    {
        if (characterPortraitImage != null && newPortrait != null)
        {
            characterPortraitImage.sprite = newPortrait;
            characterPortraitImage.enabled = true;
        }
        else
        {
            characterPortraitImage.enabled = false;
        }
    }

    public void HidePortrait()
    {
        if (characterPortraitImage == null) return;
        characterPortraitImage.enabled = false;
    }
}
}