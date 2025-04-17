using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEvent : MonoBehaviour
{
    public AK.Wwise.Event SwingEvent;
    public AK.Wwise.Event WindUpEvent;
    public AK.Wwise.Event UISwipe;

    public void PlaySwingSound()
    {
        SwingEvent.Post(gameObject);
    }

    public void PlayTransitionSound()
    {
        UISwipe.Post(gameObject);
    }

    public void PlayWindUpSound()
    {
        WindUpEvent.Post(gameObject);
    }
}