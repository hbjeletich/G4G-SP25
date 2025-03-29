using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GolfScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreUI;
    public static int highestGolfScore = 0;
    public int score;

    public HighestScoreIndicator HighestScoreIndicator;

    // singleton set up
    public static GolfScoreManager instance { get; private set; }

    private void Awake(){
        if (instance != null && instance != this){
            //only allow one instance of this singleton. Destroy duplicates
            Destroy(this);
        } else {
            instance = this;
        }
    }

    public GolfBallController golfBall;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreUI.text = "0000 M";
    }

    // Update is called once per frame
    void Update()
    {
        score = golfBall.getDistanceFromStart();
        scoreUI.text = score + " m";
    }

    public void updateHighestScore(){
        if(score > highestGolfScore){
            highestGolfScore = score;
            HighestScoreIndicator.updateIndicatorPos(golfBall.transform.position.z);
            HighestScoreIndicator.placeIndicatorWithAnim();
        }
    }
}
