using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingSceneUIManager : MonoBehaviour
{

    public SpriteRenderer gameDemonstrationArrowRenderer;
    // Start is called before the first frame update
    void Start()
    {

        gameDemonstrationArrowRenderer.enabled = false;
        
    }

    // Update is called once per frame
    public void SewingShowCompletionUI()
    {
        gameDemonstrationArrowRenderer.enabled = true;
    }
}
