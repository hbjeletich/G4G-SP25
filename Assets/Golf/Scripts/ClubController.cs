using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubController : MonoBehaviour
{
    private float swingBackLen = 1f;

    private Animator swingAnimator;

    private bool hasNotSwung = true;

    void Start(){
        swingAnimator = GetComponent<Animator>();
    }
    public void toStartPosition(){
        swingAnimator.CrossFadeInFixedTime("Idle", 0f);
    }

    public void swingBack(){
        swingAnimator.CrossFadeInFixedTime("Swing_Back", 0f);
    }

    public void swingForward(){
        swingAnimator.CrossFadeInFixedTime("Swing_Forward", 0f);
    }
}
