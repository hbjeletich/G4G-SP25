using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturyInputManager : MonoBehaviour
{
    // This script should be on an object in the game to register captury input!
    private void OnEnable()
    {
        CapturyInput.Register();
    }
}
