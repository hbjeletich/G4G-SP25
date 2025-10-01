using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwingStrengthController : MonoBehaviour
{
    public float swingStrength;
    public Slider strengthMeter;

    private float maxStrength = 100f;
    private float targetStrength;
    private float currStrengthGoal = 1;
    private float barRate = 5f;
    private float strengthMultiplier = 1.2f;

    // wwise audio integration
    [SerializeField] private AK.Wwise.Event tickingEvent;
    private uint tickingSoundID;
    private bool isPlayingTick = false;

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
        toStartingState();
    }

    void Update()
    {
        swingStrength = Mathf.MoveTowards(swingStrength, targetStrength, barRate * Time.deltaTime);
        strengthMeter.value = swingStrength; //(Visualizing purposes)

        if (isPlayingTick)
        {
            AkSoundEngine.SetRTPCValue("Swing_Hold_Time", swingStrength, gameObject);
        }
    }

    public void toStartingState(){
        swingStrength = 0;
        targetStrength = maxStrength;
        //slider for visualizing purpose.
        strengthMeter.value = swingStrength;
        strengthMeter.maxValue = maxStrength;

        startTicking();
    }

    public void stopMeasuring(){
        //stops measuring strength of swing
        targetStrength = swingStrength; //stops the swing strength from increasing
        stopTicking();
        Invoke("deactiveSelf", 1f);
    }

    private void startTicking()
    {
        if (!isPlayingTick)
        {
            tickingSoundID = tickingEvent.Post(gameObject);
            isPlayingTick = true;
        }
    }

    private void stopTicking()
    {
        if (isPlayingTick)
        {
            AkSoundEngine.StopPlayingID(tickingSoundID);
            isPlayingTick = false;
        }
    }

    public void deactiveSelf(){
        this.gameObject.SetActive(false);
    }

    public float getSwingStrength(){
        return swingStrength * strengthMultiplier;
    }
}
