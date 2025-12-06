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

        private List<(StarScript starOne,StarScript starTwo)> relationships =new List<(StarScript starOne, StarScript starTwo)>();

        private List<GameObject> lines = new List<GameObject>();
    
        // Start is called before the first frame update
        void Awake()
        {
            StarSetup();
            //Invoke("StarSetup",setupDelay);
        }

        void StarSetup()
        {
            Debug.Log("HIT : SETUP");
            foreach (var star in stars)
            {
                starScripts.Add(star.GetComponent<StarScript>());
            }

            foreach (var script in starScripts)
            {
                script.starPlaced.AddListener(StarPlaced);
                Debug.Log("HIT : StarScfript");

                foreach (var nearStar in script.nearStars)
                {
                    Debug.Log("HIT : if");
                    StarScript nearStarScript = nearStar.GetComponent<StarScript>();
                    
                    GameObject temp = Instantiate(blankLine);
                    LineRenderer tempLine=temp.GetComponent<LineRenderer>();
                    tempLine.SetPosition(0,script.destination.transform.position);
                    tempLine.SetPosition(1,nearStarScript.destination.transform.position);

                    if (!relationships.Contains((script, nearStarScript)) && !relationships.Contains((nearStarScript, script)))
                    {
                        relationships.Add((script, nearStarScript ));
                        lines.Add(temp);
                        Debug.Log("HIT : if");
                    }
                    else 
                    {
                        Destroy(temp);
                        Debug.Log("HIT : else");
                    }
                }
            }

            foreach (var pair in relationships)
            {
                //GameObject tempInst=Instantiate(pair.line,new Vector3(0,0,0),Quaternion.Euler(0,0,0));
                
                Debug.Log("HIT : placed");
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


