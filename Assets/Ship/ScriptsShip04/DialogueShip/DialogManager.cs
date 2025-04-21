using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Ship
{
public class DialogManager : MonoBehaviour
{
    [Header("Components (Add here)")]
    public TextAsset dialogFile;
    public ShipVillageUIManager uiManager;
    public System.Action onDialogComplete;
    public Queue<string> dialogQueue = new Queue<string>();

    [Header("Auto-Advancing Dialog Settings")]
    public bool autoAdvanceDialog = false; // Enable auto mode
    public float autoAdvanceDelay = 2.5f;  // Time between lines

    [Header("Character Portraits")]
    public Dictionary<string, CharacterPortraitScriptable> characterPortraits = new Dictionary<string, CharacterPortraitScriptable>();
    [SerializeField] private List<CharacterPortraitScriptable> characterPortraitList;

    // Internal references
    private Dictionary<string, string> dialogMap;
    private HashSet<string> completedDialogKeys = new HashSet<string>();
    private string myDialogKey;
    [HideInInspector] public enum DialogueState { Normal, Waiting }
    [HideInInspector] public DialogueState currentState = DialogueState.Normal;

    // Components    
    private ShipPlayerController playerOverworld;

    private void Awake()
    {
        playerOverworld = FindObjectOfType<ShipPlayerController>();
        dialogMap = new Dictionary<string, string>();

        foreach (var characterPortrait in characterPortraitList)  // Populate char portrait dictionary
        {
            if (!characterPortraits.ContainsKey(characterPortrait.characterName))
            {
                characterPortraits.Add(characterPortrait.characterName, characterPortrait);
            }
        }
        PopulateDialogMap();
    }

    private void PopulateDialogMap()
    {
        if (dialogMap.Count > 0 || dialogFile == null)
            return;

        string[] lines = dialogFile.text.Split(new[] { '\n' }, System.StringSplitOptions.None);

        string currentKey = null;
        string currentValue = "";

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line)) continue;

            // Check if this is a new key=value line
            if (line.Contains("="))
            {
                // If we were building a previous value, save it
                if (!string.IsNullOrEmpty(currentKey))
                {
                    dialogMap[currentKey] = currentValue.Trim();
                }

                string[] split = line.Split(new[] { '=' }, 2);
                currentKey = split[0].Trim();
                currentValue = split[1].Trim() + "\n"; // start the new value
            }
            else if (!string.IsNullOrEmpty(currentKey))
            {
                // Continuation of previous dialog
                currentValue += line + "\n";
            }
        }

        // Save the last block
        if (!string.IsNullOrEmpty(currentKey))
        {
            dialogMap[currentKey] = currentValue.Trim();
        }

        Debug.Log($"Loaded {dialogMap.Count} dialog entries.");
    }


    public string GetDialogFromKey(string aKey)
    {
        if (dialogMap.TryGetValue(aKey, out string value))
        {
            return value;
        }
        return aKey;
    }

    public void PlayScene(string aDialogKey)
    {
        // If the dialog has already been completed or is currently playing, return
        if (completedDialogKeys.Contains(aDialogKey) || dialogQueue.Count > 0) 
        {
            PlayNextDialog();
            return;
        }

        myDialogKey = aDialogKey; 
        string dialogText = GetDialogFromKey(aDialogKey);
        string[] lines = dialogText.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            dialogQueue.Enqueue(line);
        }
        completedDialogKeys.Add(aDialogKey); 
        Debug.Log($"Queue populated with {dialogQueue.Count} lines for key: {aDialogKey}");
        PlayNextDialog();
    }

    public void PlayNextDialog()
    {
        if (uiManager.textTyping || currentState == DialogueState.Waiting) return;

        if (dialogQueue.Count > 0)
        {
            string dialogLine = dialogQueue.Dequeue();

            // Parse dialog line
            string[] parts = dialogLine.Split(new[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries);
            string dialogText = parts[0].Trim(); 
            string emotion = parts.Length > 1 ? parts[1].Trim() : null;

            // Get character portrait
            string characterName = myDialogKey.Split('.')[0];
            if (characterPortraits.TryGetValue(characterName, out CharacterPortraitScriptable characterData))
            {
                Sprite portrait = characterData.defaultPortrait;
                if (!string.IsNullOrEmpty(emotion))
                {
                    characterData.GetEmotionPortraits().TryGetValue(emotion, out portrait);
                }

                uiManager.ShowOrHidePortrait(portrait);
            }
            uiManager.ShowText(dialogText);

            if (autoAdvanceDialog)
            {
                StartCoroutine(AutoAdvanceNextLine());
            }
        }
        else
        {
            EndDialog();
        }
    }

    public IEnumerator AutoAdvanceNextLine()
    {
        yield return new WaitUntil(() => uiManager.textTyping == false);
        yield return new WaitForSeconds(autoAdvanceDelay);
        uiManager.ClearText();
        PlayNextDialog();
    }

    public void EndDialog()
    {
        Debug.Log("EndDialog called.");
        
        uiManager.HideTextBox();
        uiManager.ClearText();
        uiManager.HidePortrait();
        ResetDialogForKey(myDialogKey);
        playerOverworld.EnablePlayerController();

        onDialogComplete?.Invoke();
    }

    public void ResetDialogForKey(string aDialogKey)
    {
        if (completedDialogKeys.Contains(aDialogKey))
        {
            completedDialogKeys.Remove(aDialogKey);
        }
    }
}
}