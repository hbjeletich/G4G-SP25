using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighestScoreIndicator : MonoBehaviour
{
    public static Vector3 golfScoreIndicatorPos = new Vector3(0, 0, 0);

    public GameObject floorIndicator;
    public TextMeshProUGUI floorScore;

    public Animator anim;

    public void updateIndicatorPos(float posZ){
        golfScoreIndicatorPos = new Vector3(0, floorIndicator.transform.position.y, posZ);
    }

    public void placeIndicator(){
        if(golfScoreIndicatorPos.z == 0){
            this.gameObject.SetActive(false);
            return;
        }

        floorIndicator.transform.position = golfScoreIndicatorPos;
        floorScore.text = GolfScoreManager.highestGolfScore + " m";
    }

    public void placeIndicatorWithAnim(){
        this.gameObject.SetActive(true);

        floorIndicator.transform.position = golfScoreIndicatorPos;

        floorScore.text = GolfScoreManager.highestGolfScore + " m";

        anim.CrossFadeInFixedTime("FadeIn", 0);
    }
}
