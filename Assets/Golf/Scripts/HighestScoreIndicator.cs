using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighestScoreIndicator : MonoBehaviour
{
    public static Vector3 golfScoreIndicatorPos = new Vector3(0, 0, 0);

    public GameObject floorIndicator;

    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateIndicatorPos(float posZ){
        golfScoreIndicatorPos = new Vector3(0, floorIndicator.transform.position.y, posZ);
    }

    public void placeIndicator(){
        if(golfScoreIndicatorPos.z == 0){
            this.gameObject.SetActive(false);
            return;
        }

        floorIndicator.transform.position = golfScoreIndicatorPos;
    }

    public void placeIndicatorWithAnim(){
        this.gameObject.SetActive(true);

        floorIndicator.transform.position = golfScoreIndicatorPos;
        anim.CrossFadeInFixedTime("FadeIn", 0);
    }
}
