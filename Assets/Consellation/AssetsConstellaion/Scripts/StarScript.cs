using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

namespace Constellation
{
    public class StarScript : MonoBehaviour
    {
        private PlayerController playerCont;
        private GameObject player;

        private bool playerTouch = false;

        public GameObject destination;

        private DestinationScript destScript;

        private bool foundHome = false;

        public string starName;
        
        // Start is called before the first frame update
        void Start()
        {
            player=GameObject.FindGameObjectWithTag("Player");
            playerCont = player.GetComponent<PlayerController>();
            if (playerCont != null)
            {
                playerCont.interact.AddListener(Interact);
            }

            destScript = destination.GetComponent<DestinationScript>();
        }

        // Update is called once per frame
        void Update()
        {
            if (playerCont.grabedStar==gameObject)
            {
                transform.position = playerCont.tailSpot.transform.position;
            }

            if (foundHome)
            {
                transform.position = destination.transform.position;
            }
        }

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

        void Interact()
        {
            if (destScript.playerTouch && playerCont.grabedStar == gameObject)
            {
                foundHome = true;
                Global.hit(" home"+starName);
                playerCont.grabedStar = null;
            }
            else if (playerCont.grabedStar == gameObject && !foundHome)
            {
                playerCont.grabedStar = null;
                Global.hit(" drop "+starName);
            }
            else if (playerTouch&&playerCont.grabedStar==null&&!foundHome)
            {
                playerCont.grabedStar = gameObject;
                Global.hit(" grab"+starName);
            }
        }
    }
}
