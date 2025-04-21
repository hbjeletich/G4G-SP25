using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterPortaits", menuName = "Character Portaits")]
public class CharacterPortraitScriptable : ScriptableObject
{
    public string characterName;
    public Sprite defaultPortrait;

    [System.Serializable]
    public class EmotionPortrait
    {
        public string emotion;
        public Sprite portrait;
    }

    public List<EmotionPortrait> emotionPortraitList = new List<EmotionPortrait>();

    private Dictionary<string, Sprite> emotionPortraits;

    public Dictionary<string, Sprite> GetEmotionPortraits()
    {
        if (emotionPortraits == null)
        {
            emotionPortraits = new Dictionary<string, Sprite>();
            foreach (var emotionPortrait in emotionPortraitList)
            {
                if (!emotionPortraits.ContainsKey(emotionPortrait.emotion))
                {
                    emotionPortraits.Add(emotionPortrait.emotion, emotionPortrait.portrait);
                }
            }
        }
        return emotionPortraits;
    }
}
