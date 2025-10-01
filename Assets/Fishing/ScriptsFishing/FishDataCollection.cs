using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishing
{
    [CreateAssetMenu(fileName = "FishDataCollection", menuName = "Fish/Fish Data Collection")]
    public class FishDatabase : ScriptableObject
    {
        public List<FishData> allFish;
    }
}

