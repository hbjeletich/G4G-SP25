using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GolfScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreUI;

    public GolfBallController golfBall;
    // Start is called before the first frame update
    void Start()
    {
        scoreUI.text = "0000 M";
    }

    // Update is called once per frame
    void Update()
    {
        scoreUI.text = golfBall.getDistanceFromStart().ToString("0") + " m";
    }
}
