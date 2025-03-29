using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwingStrengthController : MonoBehaviour
{
    public float swingStrength;
    public Slider strengthMeter;

    private float maxStrength = 50f;
    private float targetStrength;
    private float currStrengthGoal = 1;
    private float barRate = 10f;
    private float strengthMultiplier = 1.2f;

    // singleton set up
    public static SwingStrengthController instance { get; private set; }

    private void Awake(){
        if (instance != null && instance != this){
            //only allow one instance of this singleton. Destroy duplicates
            Destroy(this);
        } else {
            instance = this;
        }
        this.gameObject.SetActive(false);
    }

    void Start()
    {
        swingStrength = 0;
        targetStrength = maxStrength;
        //slider for visualizing purpose.
        strengthMeter.value = swingStrength;
        strengthMeter.maxValue = maxStrength;
    }

    void Update()
    {
        swingStrength = Mathf.MoveTowards(swingStrength, targetStrength, barRate * Time.deltaTime);
        strengthMeter.value = swingStrength; //(Visualizing purposes)
    }

    public void stopMeasuring(){
        //stops measuring strength of swing
        targetStrength = swingStrength; //stops the swing strength from increasing
        Invoke("deactiveSelf", 1f);
    }

    public void deactiveSelf(){
        this.gameObject.SetActive(false);
    }

    public float getSwingStrength(){
        return swingStrength * strengthMultiplier;
    }
}
