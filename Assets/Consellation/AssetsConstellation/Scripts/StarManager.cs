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

        public List<GameObject> stars =new List<GameObject>();

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
                //new
                StarScript script = star.GetComponent<StarScript>();
                script.starPlaced.AddListener(StarPlaced);

                foreach (var nearStar in script.nearStars)
                {
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
        }

        void StarPlaced()
        {
            for (int i = 0; i < relationships.Count; i++)
            {
                if (relationships[i].starOne.foundHome && relationships[i].starTwo.foundHome)
                {
                    GameObject temp=Instantiate(fullLine,transform.position,transform.rotation);
                    LineRenderer tempLine=temp.GetComponent<LineRenderer>();
                    tempLine.SetPosition(0,relationships[i].starOne.destination.transform.position);
                    tempLine.SetPosition(1,relationships[i].starTwo.destination.transform.position);

                    GameObject toDelete = lines[i];
                    lines[i] = temp;
                    Destroy(toDelete);
                    
                    
                }
            }

            /*
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
            */
        }
    }
}


