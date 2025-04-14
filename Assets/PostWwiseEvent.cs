using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEvent : MonoBehaviour
{
    public AK.Wwise.Event SwingEvent;
    public AK.Wwise.Event WindUpEvent;

    public void PlaySwingSound()
    {
        SwingEvent.Post(gameObject);
    }

    public void PlayWindUpSound()
    {
        WindUpEvent.Post(gameObject);
    }
}
