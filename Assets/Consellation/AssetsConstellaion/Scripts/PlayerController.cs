using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Constellation
{
    public class PlayerController : MonoBehaviour
    {
        //Declaration Area


        [SerializeField] private InputActionAsset inputActions;

        private InputAction isWalking;

        //STATISTICS
        //rotation mod gathered by input
        [SerializeField] private float rotationMod;
        //speed mod gathered by input
        [SerializeField] private float speedMod;
        //speed stat handles movement
        [SerializeField] private float speedStat = 5.0f;
        //how fast guy rotates
        [SerializeField] private float rotateStat = 5.0f;

        //The Event to try and grab
        public UnityEvent interact;
        
        //THE RIGID BODY
        private Rigidbody2D charBody;

        //The invisible gameobject that acts purly as a position to hold the grabbed star, SHOULD BE IMPROVED
        public GameObject tailSpot;

        // The storage spot so that the player knows which star is currently held
        public GameObject grabedStar;

        public bool debugControls = false;

        void Awake()
        { 
            var footMap= inputActions.FindActionMap("Foot");
            isWalking = footMap.FindAction("IsWalking");
        }

        void OnEnable()
        {
            isWalking.Enable();
        }

        void OnDisable()
        { 
            isWalking.Disable();
        }

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

            //this movement is used when the keyboard is wanted to test features
            if (debugControls)
            {
                speedMod = Input.GetAxis("Vertical");
                rotationMod = Input.GetAxis("Horizontal");


                if (Input.GetKeyDown("space"))
                {
                    interact.Invoke();
                }
            }
            else     //THis branch is used when caputry movemnt is used MUST BE TESTED IN LIM
            {

                if (Input.GetKeyDown("w"))
                {
                    speedMod = 1;
                }
                if (Input.GetKeyUp("w"))
                {
                    speedMod = 0;
                }


                if (Input.GetKeyDown("a"))
                {
                    rotationMod = -1;
                }
                if (Input.GetKeyDown("d"))
                {
                    rotationMod = 1;
                }
                if (Input.GetKeyUp("d")||Input.GetKeyUp("a"))
                {
                    rotationMod = 0;
                }

                if (Input.GetKeyDown("space"))
                {
                    interact.Invoke();
                }
            }

        }

        void FixedUpdate()
        {
            //applay input
            //actulaly spins character, by changing rotation by rotation mod& rotation speed stat, the -1 makes the character feel uninversed
            charBody.rotation += (-1 * rotationMod * rotateStat);
            //This moves guy forward based on speedMod and Stat
            charBody.AddForce(transform.up*speedMod*speedStat, ForceMode2D.Impulse);

        }
    }
}
