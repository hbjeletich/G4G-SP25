using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Constellation
{
    public enum ControlsScheme
    {
        Keyboard,Workout,Standard,DebugHead
    }

    public class PlayerController : MonoBehaviour
    {
        //Declaration Area

        // ACTIONS

        // this is the Input actions of captury
        [SerializeField] private InputActionAsset inputActions;
        
        private InputAction leftFootHeightAction;
        private InputAction rightFootHeightAction;
        private InputAction walkingAction;

        private InputAction headPositionAction; 

        //STATISTICS
        
        //rotation mod gathered by input
        [SerializeField] private float rotationMod;
        //speed mod gathered by input
        [SerializeField] private float speedMod;
        //speed stat handles movement
        [SerializeField] private float speedStat = 5.0f;
        //how fast guy rotates
        [SerializeField] private float rotateStat = 5.0f;
        // these handle multiple interaction spam 
        [SerializeField] private float delayTime = 1f;

        // THRESHOLDS

        // These are stats for thresholds of foot height
        [SerializeField] private float turnFootThreshold = .2f;

        [SerializeField] private float walkFootThreshold = .06f;

        [SerializeField] private float jumpThreshold=.1f;

        //UNITY OBJECTS

        //The Event to try and grab
        public UnityEvent interact;
        
        //THE RIGID BODY
        private Rigidbody2D charBody;

        //The invisible gameobject that acts purly as a position to hold the grabbed star, SHOULD BE IMPROVED
        public GameObject tailSpot;

        // The storage spot so that the player knows which star is currently held
        public GameObject grabedStar;

        // the control scheme being used currently
        public ControlsScheme controls = ControlsScheme.Workout;

        public Camera mainCam; 

        private bool interacting=false;

        [SerializeField] private GameObject fakeHead;

        void Awake()
        { 
            // grabs input actions on awake
            var footMap= inputActions.FindActionMap("Foot");
            leftFootHeightAction = footMap.FindAction("LeftFootPosition");
            rightFootHeightAction = footMap.FindAction("RightFootPosition");
            walkingAction = footMap.FindAction("IsWalking");

            var headMap = inputActions.FindActionMap("Head");
            headPositionAction = headMap.FindAction("HeadPosition");
        }

        void OnEnable()
        {
            // turns on actions
            leftFootHeightAction.Enable();
            rightFootHeightAction.Enable();
            headPositionAction.Enable();
        }

        void OnDisable()
        { 
            //turns off actions
            leftFootHeightAction.Disable();
            rightFootHeightAction.Disable();
            headPositionAction.Disable();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Gather Body
            charBody = GetComponent<Rigidbody2D>();
            mainCam = GameObject.FindObjectOfType<Camera>();
            //interact.AddListener(Interact);
        }

        // Update is called once per frame
        void Update()
        {
            //take input should ne changed with movement aspects

            //this movement is used when the keyboard is wanted to test features
            if (controls == ControlsScheme.Keyboard)
            {
                clamp();

                speedMod = Input.GetAxis("Vertical");
                rotationMod = Input.GetAxis("Horizontal");


                if (Input.GetKeyDown("space"))
                {
                    interact.Invoke();
                }
            }
            // THis implements workout controls
            else if (controls == ControlsScheme.Workout)
            {
                clamp();

                float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
                float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;

                //refine the movement
                //Handles Walking
                if ((leftFootY > walkFootThreshold || rightFootY > walkFootThreshold))
                {
                    Debug.Log("HIT : walking");
                    speedMod = 1;
                }
                else
                {
                    speedMod = 0;
                }

                //Handles Turning
                if (leftFootY > rightFootY && leftFootY > turnFootThreshold)
                {
                    Debug.Log("HIT : turn left?");
                    rotationMod = -1;
                }

                if (rightFootY > leftFootY && rightFootY > turnFootThreshold)
                {
                    Debug.Log("HIT : turn right?");
                    rotationMod = 1;
                }

                //handles stop turning
                if (leftFootY < turnFootThreshold && rightFootY < turnFootThreshold)
                {
                    rotationMod = 0;
                }

                //handles jump
                if (leftFootY > jumpThreshold && rightFootY > jumpThreshold && !interacting)
                {
                    Debug.Log("HIT : interact");
                    interactCaptureHandle();
                }
            }
            // This implements standard Controls
            else if (controls == ControlsScheme.Standard)
            {
                //clamp();

                float leftFootY = leftFootHeightAction.ReadValue<Vector3>().y;
                float rightFootY = rightFootHeightAction.ReadValue<Vector3>().y;


             
                // slap postion equal to head within room
                // probably needs to be changed
                Vector3 headPos = headPositionAction.ReadValue<Vector3>();
                transform.position = headPos;

                Debug.Log("Debug: "+headPos.x +" : "+ headPos.z);

                //transform head x to game x and head z to game y. 

                transform.position = mainCam.ViewportToWorldPoint(new Vector3(map(headPos.x, -4, 4, 0, 1), map(headPos.z, -3, 3, 0, 1), 0));
                /*

                Vector3 viewpoint = mainCam.WorldToViewportPoint(transform.position);
                transform.position = new Vector3(Mathf.Lerp(0,1,), Mathf.Lerp(0,1), 0);

                */
                // handles interact
                if ((leftFootY > walkFootThreshold || rightFootY > walkFootThreshold))
                {
                    interactCaptureHandle();
                }

            }
            // this is to debug standard movement by moving an object
            else if (controls == ControlsScheme.DebugHead)
            {
                transform.position = mainCam.ViewportToWorldPoint(new Vector3(map(fakeHead.transform.position.x, -4, 4, 0, 1), map(fakeHead.transform.position.y, -3, 3, 0, 1), 0));
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

        // this function flips doing something after a certain amount of time.
        void delay()
        {
            interacting=false;
        }

        // this is the interaction code that runs when a captury system wants to interact
        void interactCaptureHandle()
        {
            interact.Invoke();
            interacting = true;
            Invoke("delay", delayTime);
        }
    
        void clamp()
        {
            Vector3 viewpoint = mainCam.WorldToViewportPoint(transform.position);
            viewpoint.x = Mathf.Clamp01(viewpoint.x);
            viewpoint.y = Mathf.Clamp01(viewpoint.y);
            transform.position = mainCam.ViewportToWorldPoint(viewpoint);
            //Debug.Log(viewpoint);
        }

        float map(float val, float inMin, float inMax, float outMin, float outMax)
        {
            return (val - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;    
        }
    }
}
