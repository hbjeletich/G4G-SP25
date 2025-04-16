using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GolfState { SWINGBACK, ASSESSSTRENGTH, SWING, HIT, FOLLOW, END }

public class GolfSystem : MonoBehaviour
{
    public GolfState state;
    public delegate void StateUpdate();
    StateUpdate stateUpdate;

    public ClubController clubController;
    public GolfBallController golfBallController;
    public GolfCameraController cameraController;

    public GolfTransitionHandler transitionHandler;

    public HighestScoreIndicator highestScoreIndicator;
    
    public GolfControls golfControls;

    public GameObject initialBalPromp;
    public GameObject balancePrompt;

    public float delayBeforeStarting = 1f;

    private float transitionAnimLen = 1.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        highestScoreIndicator.placeIndicator();
        stateUpdate = nullUpdate;
    
        StartCoroutine(startGolfGame());
    }

    void Update(){
        stateUpdate();
    }

    IEnumerator transitionToGameRestart(){

        transitionHandler.transitionGameOut();

        yield return new WaitForSeconds(1f);
        cameraController.toStartTransform();
        clubController.toStartPosition();
        golfBallController.toStartingState();
        SwingStrengthController.instance.toStartingState();
        GolfScoreManager.instance.toStartingState();

        yield return new WaitForSeconds(2f);
        transitionHandler.transitionGameIn();

        StartCoroutine(startGolfGame());
    }

    IEnumerator startGolfGame(){
        yield return new WaitForSeconds(transitionAnimLen);

        stateUpdate = nullUpdate;
        state = GolfState.SWINGBACK;
        StartCoroutine(swingBack());
    }

    IEnumerator swingBack(){
        yield return new WaitForSeconds(delayBeforeStarting);
        //show initial balance promp
        initialBalPromp.SetActive(true);

        //activate golf controls
        golfControls.gameObject.SetActive(true);
    
        while(golfControls.simulatedLegRaise() != true && golfControls.hasFootRaise == false){ //wait for foot to raise before starting golf power assesment. Mouse controls for testing.
            yield return null;
        }

        yield return new WaitForSeconds(.5f); //delay before switching to next state
        //disable intial balance prompt
        initialBalPromp.SetActive(false);

        //swing the golf club back to its starting position. Awaiting to be releases
        clubController.swingBack();

        state = GolfState.ASSESSSTRENGTH;
        StartCoroutine(assesStrength());
    }

    IEnumerator assesStrength(){
        Debug.Log("assesing strength");
        //promp the player to hold an exercise for an amount of time in order to increase the strength of their golf hit
        SwingStrengthController.instance.gameObject.SetActive(true);
        balancePrompt.SetActive(true);

        while(golfControls.simulatedLegLower() != true && golfControls.hasFootRaise == true){ //wait for foot to fall before stopping golf power assesment. Mouse controls for testing
            yield return null;
        }

        //disable golf controls
        golfControls.gameObject.SetActive(false);

        balancePrompt.SetActive(false);
        state = GolfState.SWING;
        StartCoroutine(swing());
    }

    IEnumerator swing(){
        clubController.swingForward();
    
        float swingStrength = SwingStrengthController.instance.getSwingStrength();
        SwingStrengthController.instance.stopMeasuring();
    
        yield return new WaitForSeconds(.2f); //wait for golf club to connect with golf ball

        state = GolfState.HIT;
        StartCoroutine(hitGolfBall(swingStrength));
    }

    IEnumerator hitGolfBall(float swingStrength){
        golfBallController.hitGolfBall(swingStrength);

        yield return new WaitForSeconds(1f); //give a second to view the impact of the golf ball hit

        state = GolfState.FOLLOW;
        stateUpdate = followUpdate;
        StartCoroutine(followGolfBall());
    }

    IEnumerator followGolfBall(){
        yield return new WaitForSeconds(1f);
        GolfScoreManager.instance.displayScoreText();
        
        while(golfBallController.isMoving() == true){ //temp mouse controls before implementing balance controls. Simulates when the player stops holding an exercise
            yield return null;
        }

        state = GolfState.END;
        stateUpdate = nullUpdate;
        StartCoroutine(endGolfSequence());
    }

    IEnumerator endGolfSequence(){
        golfBallController.disableTrail();

        //highest score tracking
        GolfScoreManager.instance.updateHighestScore();
        GolfScoreManager.instance.updateEndScreenScore();
        yield return new WaitForSeconds(4f);

        stateUpdate = nullUpdate;
        StartCoroutine(transitionToGameRestart());
        //GolfReloadManager.instance.reloadSceneInBackground();
    }

    void nullUpdate(){}

    void followUpdate(){
        cameraController.followBall();
    }

}
