using System;
using System.Collections;
using System.Collections.Generic;
using Constellation;
using UnityEngine;

namespace Constellation
{
    public class StarManager : MonoBehaviour
    {
        public float setupDelay=0.1f;
        
        //the line empty with no stars
        public GameObject blankLine;
        
        //the line once filled by stars
        public GameObject fullLine;

        public List<GameObject> stars =new List<GameObject>();

        private List<StarScript> starScripts =new List<StarScript>();

        private List<(StarScript starOne,StarScript starTwo,GameObject line)> relationships =new List<(StarScript starOne, StarScript starTwo,GameObject line)>();

        //private List<(string nameOne, string nameTwo)> relationshipNames = new List<(string nameOne, string nameTwo)>();
    
        // Start is called before the first frame update
        void Start()
        {
            StarSetup();
            //Invoke("StarSetup",setupDelay);
        }

        void StarSetup()
        {
            foreach (var star in stars)
            {
                starScripts.Add(star.GetComponent<StarScript>());
            }

            foreach (var script in starScripts)
            {
                script.starPlaced.AddListener(StarPlaced);
                
                foreach (var nearStar in script.nearStars)
                {
                    StarScript nearStarScript = nearStar.GetComponent<StarScript>();
                    
                    if (!relationships.Contains((script, nearStarScript,blankLine)) && !relationships.Contains((nearStarScript, script,blankLine)))
                    {
                        relationships.Add((script, nearStarScript,blankLine));
                    }
                }
            }

            foreach (var pair in relationships)
            {
                Instantiate(pair.line,transform.position,transform.rotation);
                LineRenderer tempLine=pair.line.GetComponent<LineRenderer>();
                tempLine.SetPosition(0,pair.starOne.destination.transform.position);
                tempLine.SetPosition(1,pair.starTwo.destination.transform.position);
            }
        }

        void StarPlaced()
        {
            foreach (var pair in relationships)
            {
                if (pair.starOne.foundHome&&pair.starTwo.foundHome)
                {
                    GameObject temp=Instantiate(fullLine,transform.position,transform.rotation);
                    LineRenderer tempLine=temp.GetComponent<LineRenderer>();
                    tempLine.SetPosition(0,pair.starOne.destination.transform.position);
                    tempLine.SetPosition(1,pair.starTwo.destination.transform.position);
                }
            }
        }
    }
}


