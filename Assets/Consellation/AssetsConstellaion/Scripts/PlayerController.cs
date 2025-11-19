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

        private InputAction leftFootHeightAction;
        private InputAction rightFootHeightAction;

        //STATISTICS
        //rotation mod gathered by input
        [SerializeField] private float rotationMod;
        //speed mod gathered by input
        [SerializeField] private float speedMod;
        //speed stat handles movement
        [SerializeField] private float speedStat = 5.0f;
        //how fast guy rotates
        [SerializeField] private float rotateStat = 5.0f;

        //
        [SerializeField] private float turnFootThreshold = .2f;

        [SerializeField] private float walkFootThreshold = .06f;

        [SerializeField] private float jumpThreshold=.1f;

        [SerializeField] private float delayTime=.4f;

        private bool doingSomething=false;

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
            leftFootHeightAction = footMap.FindAction("LeftFootPosition");
            rightFootHeightAction = footMap.FindAction("RightFootPosition");
        }

        void OnEnable()
        {
            leftFootHeightAction.Enable();
            rightFootHeightAction.Enable();
        }

        void OnDisable()
        { 
            leftFootHeightAction.Disable();
            rightFootHeightAction.Disable();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Gather Body
            charBody = GetComponent<Rigidbody2D>();
            interact.AddListener(Interact);
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

                float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
                float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;

                //refine the movement
                //Handles Walking
                if ((leftFootY>walkFootThreshold || rightFootY>walkFootThreshold) && !doingSomething)
                {
                    Debug.Log("HIT : walking");
                    speedMod = 1;
                }
                else
                {
                    speedMod = 0;
                }
                
                //Handles Turning
                if (leftFootY>rightFootY && leftFootY>turnFootThreshold)
                {
                    Debug.Log("HIT : turn left?");
                    doingSomething=true;
                    rotationMod = -1;
                }
                if (rightFootY > leftFootY && rightFootY > turnFootThreshold)
                {
                    Debug.Log("HIT : turn right?");
                    doingSomething=true;
                    rotationMod = 1;
                }
                //handles stop turning
                if (leftFootY < turnFootThreshold && rightFootY < turnFootThreshold)
                {
                    doingSomething=false;
                    rotationMod = 0;
                }

                //handles jump
                if (leftFootY > jumpThreshold && rightFootY > jumpThreshold&& !doingSomething)
                {
                    Debug.Log("HIT : interact");
                    doingSomething=true;
                    interact.Invoke();
                }
            }
        }

        void FixedUpdate()
        {
            //applay input
            //actulaly spins character, by changing rotation by rotation mod& rotation speed stat, the -1 makes the character feel uninversed
            charBody.rotation = (-1 * rotationMod * rotateStat);
            //This moves guy forward based on speedMod and Stat
            charBody.AddForce(transform.up*speedMod*speedStat, ForceMode2D.Impulse);

        }

        void Interact()
        {
            Invoke("delay",delayTime);
        }

        void delay()
        {
            doingSomething=false;
        }
    }
}
