using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfTransitionHandler : MonoBehaviour
{
    public Animator UIAnimator;
    
    public void transitionGameIn(){
        UIAnimator.CrossFadeInFixedTime("transitionIntoGame", 0);
    }

    public void transitionGameOut(){
        UIAnimator.CrossFadeInFixedTime("transitionOut", 0);
    }
}
