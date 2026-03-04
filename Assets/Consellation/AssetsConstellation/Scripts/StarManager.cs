using System;
using System.Collections;
using System.Collections.Generic;
using Constellation;
using UnityEngine;

namespace Constellation
{
    public class StarManager : MonoBehaviour
    {
        //the line empty with no stars
        public GameObject blankLine;
        
        //the line once filled by stars
        public GameObject fullLine;

        // the list of star gameobjects
        public List<GameObject> stars =new List<GameObject>();

        // the list of stars that are next to each other
        private List<(StarScript starOne,StarScript starTwo)> relationships =new List<(StarScript starOne, StarScript starTwo)>();

        // the list on instatiated lines SHOULD HAVE IDENTICAL INDEX TO RELATIONSHIPS IF RELATIONSHIPS CHANGES THIS MUST
        private List<GameObject> lines = new List<GameObject>();
    
        // Start is called before the first frame update
        void Awake()
        {
            StarSetup();
        }

        void StarSetup()
        {
            foreach (var star in stars)
            {
                // for each star grab star script and start listening to stars
                StarScript script = star.GetComponent<StarScript>();
                script.starPlaced.AddListener(StarPlaced);

                foreach (var nearStar in script.nearStars)
                {
                    // for each star thats near by grab its script
                    StarScript nearStarScript = nearStar.GetComponent<StarScript>();
                    
                    // build a line for it
                    GameObject temp = Instantiate(blankLine);
                    LineRenderer tempLine=temp.GetComponent<LineRenderer>();
                    tempLine.SetPosition(0,script.destination.transform.position);
                    tempLine.SetPosition(1,nearStarScript.destination.transform.position);

                    // if that relationship is unique
                    if (!relationships.Contains((script, nearStarScript)) && !relationships.Contains((nearStarScript, script)))
                    {
                        // add that relationship and line to list
                        relationships.Add((script, nearStarScript ));
                        lines.Add(temp);
                    }
                    else 
                    {
                        // else save some space
                        Destroy(temp);
                    }
                }
            }

            DataLogger.Instance.LogMinigameEvent("StarStepper", "StarsPlaced", $"Number of stars: {stars.Count}");
        }

        void StarPlaced()
        {
            // when a stars placed check each relationship
            for (int i = 0; i < relationships.Count; i++)
            {
                // if both stars in a relationship are home
                if (relationships[i].starOne.foundHome && relationships[i].starTwo.foundHome)
                {
                    // build a full line
                    GameObject temp=Instantiate(fullLine,transform.position,transform.rotation);
                    LineRenderer tempLine=temp.GetComponent<LineRenderer>();
                    tempLine.SetPosition(0,relationships[i].starOne.destination.transform.position);
                    tempLine.SetPosition(1,relationships[i].starTwo.destination.transform.position);

                    // swap stored line with new completed line
                    GameObject toDelete = lines[i];
                    lines[i] = temp;
                    Destroy(toDelete);
                }
            }
        }
    }
}


