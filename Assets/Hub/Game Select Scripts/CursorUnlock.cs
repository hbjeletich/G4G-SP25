using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorUnlock : MonoBehaviour
{
    void Start()
    {
        // on start enable cursor & visibility
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
