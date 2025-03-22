using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GolfState { SWINGBACK, ASSESSSTRENGTH, SWING, HIT, FOLLOW}

public class GolfSystem : MonoBehaviour
{
    public GolfState state;
    public delegate void StateUpdate();
    StateUpdate stateUpdate;

    public ClubController clubController;
    public GolfBallController golfBallController;
    public GolfCameraController cameraController;
    
    
    // Start is called before the first frame update
    void Start()
    {
        stateUpdate = nullUpdate;
        state = GolfState.SWINGBACK;
        StartCoroutine(swingBack());
    }

    void Update(){
        stateUpdate();
    }

    IEnumerator swingBack(){
        //swing the golf club back to its starting position. Awaiting to be releases
        clubController.swingBack();
        yield return new WaitForSeconds(1f);  //wait for swing back animation to play

        state = GolfState.ASSESSSTRENGTH;
        StartCoroutine(assesStrength());
    }

    IEnumerator assesStrength(){
        //promp the player to hold an exercise for an amount of time in order to increase the strength of their golf hit
        SwingStrengthController.instance.gameObject.SetActive(true);

        while(Input.GetMouseButtonDown(0) != true){ //temp mouse controls before implementing balance controls. Simulates when the player stops holding an exercise
            yield return null;
        }
        
        state = GolfState.SWING;
        StartCoroutine(swing());
    }

    IEnumerator swing(){
        clubController.swingForward();
    
        float swingStrength = SwingStrengthController.instance.swingStrength;
        SwingStrengthController.instance.stopMeasuring();
    
        yield return new WaitForSeconds(.5f); //wait for golf club to connect with golf ball

        state = GolfState.HIT;
        StartCoroutine(hitGolfBall(swingStrength));
    }

    IEnumerator hitGolfBall(float swingStrength){
        golfBallController.hitGolfBall(swingStrength);

        yield return new WaitForSeconds(1f); //give a second to view the impact of the golf ball hit

        state = GolfState.FOLLOW;
        StartCoroutine(followGolfBall());
    }

    IEnumerator followGolfBall(){
        stateUpdate = followUpdate;
        yield return new WaitForSeconds(1f);
    }

    void nullUpdate(){}

    void followUpdate(){
        cameraController.followBall();
    }

}
