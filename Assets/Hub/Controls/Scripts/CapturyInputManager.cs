using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturyInputManager : MonoBehaviour
{
    private void OnEnable()
    {
        CapturyInput.Register();
    }
}
