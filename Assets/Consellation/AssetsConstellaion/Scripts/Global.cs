using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constellation
{
    public class Global : MonoBehaviour
    {
        public static Global Instance { get; private set; }

        public static GameObject player;
        public static PlayerController playerCont;

        public static bool debugControls = false;
        
        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
    
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }
        
        // Start is called before the first frame update
        void Start()
        {
            player=GameObject.FindGameObjectWithTag("Player");
            playerCont = player.GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        public static void hit(string what="")
        {
            print("hit: " + what);
        }
    }
}

