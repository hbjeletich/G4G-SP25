using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constellation
{
    public class DestinationScript : MonoBehaviour
    {
        public bool playerTouch = false;
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                playerTouch = true;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                playerTouch = false;
        }
    }
}

