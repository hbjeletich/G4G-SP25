using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorsSceneUIManager : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Animator gameDemonstrationSpriteObject;
    public Animator previousGameDemonstrationSpriteObject;
    public SpriteRenderer gameDemonstrationSpriteRenderer;
    public SpriteRenderer previousGameDemonstrationSpriteRenderer;


    void Start()
    {
        gameDemonstrationSpriteObject.enabled = false;
        gameDemonstrationSpriteRenderer.enabled = false;
        previousGameDemonstrationSpriteObject.enabled = true;
        previousGameDemonstrationSpriteRenderer.enabled = true;
    }

    public void ScissorsShowCompletionUI()
    {
        gameDemonstrationSpriteObject.enabled = true;
        gameDemonstrationSpriteRenderer.enabled = true;
        previousGameDemonstrationSpriteObject.enabled = false;
        previousGameDemonstrationSpriteRenderer.enabled = false;
    }
}
