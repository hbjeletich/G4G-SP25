using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
public class ShipCameraScroll : MonoBehaviour
{
    public Transform player; 
    public float scrollSpeed = 3f; 

    private void FixedUpdate()
    {
        if (player != null)
        {
            transform.position += new Vector3(0, scrollSpeed * Time.deltaTime, 0);
        }
    }
}
}

