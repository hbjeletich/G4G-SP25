using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace constellation
{
    public class StarScript : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerStay2D(Collider2D other)
        {
            print("hit");
            /*
            if (other.gameObject.tag == "Player")
            {
                GameObject player=GameObject.FindGameObjectWithTag("Player");
                PlayerController playerComp = player.GetComponent<PlayerController>();
                if (playerComp.isGrabbing)
                    print("star hit");
            }
            */
        }
    }
}
