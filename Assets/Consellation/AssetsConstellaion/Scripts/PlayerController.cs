using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace constellation
{
    public class PlayerController : MonoBehaviour
    {
        //Declaration Area
        //rotation mod gathered by input
        [SerializeField] private float rotationMod;
        //speed mod gathered by input
        [SerializeField] private float speedMod;
        //speed stat handles movement
        [SerializeField] private float speedStat = 5.0f;
        //how fast guy rotates
        [SerializeField] private float rotateStat = 5.0f;
        //if the player is grabbing
        public bool isGrabbing = false;

        //THE RIGID BODY
        private Rigidbody2D charBody;


        // Start is called before the first frame update
        void Start()
        {
            //Gather Body
            charBody = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            //take input should ne changed with movement aspects
            speedMod = Input.GetAxis("Vertical");
            rotationMod = Input.GetAxis("Horizontal");
            if (Input.GetKeyDown("space"))
            {
                isGrabbing = true;
            }
        }

        void FixedUpdate()
        {
            //applay input
            //actulaly spins character, by changing rotation by rotation mod& rotation speed stat, the -1 makes the character feel uninversed
            charBody.rotation += (-1 * rotationMod * rotateStat);
            //This moves guy forward based on speedMod and Stat
            charBody.AddForce(transform.up*speedMod*speedStat, ForceMode2D.Impulse);
            if (isGrabbing)
            {
                print("player hit");
                isGrabbing = false;
            }

        }
    }
}
