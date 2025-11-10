using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constellation
{
    public class DestinationScript : MonoBehaviour
    {
        //bool that gives that info
        public bool playerTouch = false;
        
        //handles if player is touching destination
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

