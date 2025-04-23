using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSceneUIManager : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Animator gameDemonstrationSpriteObject;
    // maybe change layering in runtime to hide/show?
    // otherwise just copy code from fade in

    void Start()
    {
        gameDemonstrationSpriteObject.enabled = false;
    }

    public void ShowCompletionUI()
    {
        gameDemonstrationSpriteObject.enabled = true;
    }
}
