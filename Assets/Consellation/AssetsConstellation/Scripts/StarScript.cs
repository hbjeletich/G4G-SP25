using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Constellation
{
    public class StarScript : MonoBehaviour
    {
        // player script and player
        private PlayerController playerCont;
        private GameObject player;
        // is the player touching me
        public bool playerTouch = false;
        //the stars that are adjacent to the star 
        public GameObject[] nearStars;
        // the attached destination
        public GameObject destination;
        // that destinations script
        private DestinationScript destScript;
        // has the star gotten home yet
        public bool foundHome = false;

        public bool playerTouchingHomeStar=false;
        //the debug string to test interactions
        public string starName;

        public GameObject effect;

        // the event to send out when players place a star
        public UnityEvent starPlaced;
        
        // Start is called before the first frame update
        void Start()
        {
            // grab player
            player=GameObject.FindGameObjectWithTag("Player");
            playerCont = player.GetComponent<PlayerController>();
            
            //adds interaction listener
            if (playerCont != null)
            {
                playerCont.interact.AddListener(Interact);
            }
            //gets dest scritp
            
            destScript = destination.GetComponent<DestinationScript>();
            
        }

        private bool wasNearDestination = false;

        // Update is called once per frame
        void Update()
        {
            //if star is grabbed put it in tailspot
            if (playerCont.grabedStar==gameObject)
            {
                transform.position = playerCont.tailSpot.transform.position;
                if (destScript.playerTouch)
                {
                    playerTouchingHomeStar = true;
                    if (!wasNearDestination)
                    {
                        G4G.ExerciseIndicatorManager.Instance?.Show(ExerciseType.LegLift);
                        wasNearDestination = true;
                    }
                }
                else
                {
                    playerTouchingHomeStar=false;
                    if (wasNearDestination)
                    {
                        G4G.ExerciseIndicatorManager.Instance?.Hide();
                        wasNearDestination = false;
                    }
                }
            }
            else
            {
                wasNearDestination = false;
            }
            // if home lock star to home
            if (foundHome)
            {
                //transform.position = destination.transform.position;
                transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y, destination.transform.position.z - 1);
                destination.SetActive(false);
            }
        }

        //player touch handle
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerTouch = true;
                // Show indicator if player can pick up this star
                if (playerCont.grabedStar == null && !foundHome)
                    G4G.ExerciseIndicatorManager.Instance?.Show(ExerciseType.LegLift);
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerTouch = false;
                // Hide if player was near this star
                if (playerCont.grabedStar == null)
                    G4G.ExerciseIndicatorManager.Instance?.Hide();
            }
        }

        //handles the the stars reaction to player interact button press
        void Interact()
        {
            //if player is over stars home and players grabbed star is me
            if (destScript.playerTouch && playerCont.grabedStar == gameObject)
            {
                //set me home, drop star, and tell star manager a star was placed
                foundHome = true;
                starPlaced.Invoke();
                playerCont.grabedStar = null;
                effect.SetActive(false);
                G4G.ExerciseIndicatorManager.Instance?.Hide();
            }
            else if (playerCont.grabedStar == gameObject && !foundHome)     // if player has star and not home
            {
                //drop star
                playerCont.grabedStar = null;
                effect.SetActive(false);
                G4G.ExerciseIndicatorManager.Instance?.Hide();
            }
            else if (playerTouch&&playerCont.grabedStar==null&&!foundHome)     //if player doesn't have a star and is touching me
            {
                //grab star
                playerCont.grabedStar = gameObject;
                effect.SetActive(true);
                G4G.ExerciseIndicatorManager.Instance?.Hide();
            }
        }
    }
}