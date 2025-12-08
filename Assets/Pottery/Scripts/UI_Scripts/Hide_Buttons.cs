using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide_Buttons : MonoBehaviour
{
    public GameObject buttonToHide;

    // Hides the assigned target button
    public void HideTargetButton()
    {
        gameObject.SetActive(false);

        if (buttonToHide != null)
            buttonToHide.SetActive(false);
    }

    // Reveals BOTH buttons after hiding them
    public void RevealBoth()
    {
        // Reveal this button
        gameObject.SetActive(true);

        // Reveal the assigned button
        if (buttonToHide != null)
            buttonToHide.SetActive(true);
    }
}
