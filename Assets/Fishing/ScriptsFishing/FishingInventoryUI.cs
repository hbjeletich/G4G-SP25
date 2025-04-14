using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Fishing
{
public class FishingInventoryUI : MonoBehaviour
{
    public FishData fishData;
    public GameObject fishButtonPrefab;
    public Transform fishButtonContainer;
    public List<GameObject> fishButtons = new List<GameObject>();

        public void Start()
        {
             
        }

        public void Update()
        {
            
        }

        //     public void Start()
        //     {
        //         // Assuming you have a list of fish data to display
        //         List<FishData> fishDataList = new List<FishData>(); // Populate this with your fish data

        //         foreach (FishData data in fishDataList)
        //         {
        //             GameObject button = Instantiate(fishButtonPrefab, fishButtonContainer);
        //             button.GetComponentInChildren<Text>().text = data.fishName;
        //             button.GetComponent<Button>().onClick.AddListener(() => OnFishButtonClicked(data));
        //             fishButtons.Add(button);
        //         }
        //     }

        // // public void BlueCatfishButtonPressed()
        // // {

        // // }
    }
}
