using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubController : MonoBehaviour
{
    private float swingBackLen = 1f;

    public Animator swingAnimator;

    private bool hasNotSwung = true;

    public void swingBack(){
        swingAnimator.CrossFadeInFixedTime("Swing_Back", 0f);
    }

    public void swingForward(){
        swingAnimator.CrossFadeInFixedTime("Swing_Forward", 0f);
    }
}
