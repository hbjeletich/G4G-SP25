using UnityEngine;

//Because the animal data script is for making scriptable objects, I cannot attach it to prefabs, so this script is for attaching the 
//animal data to the prefabs. 

namespace CameraSnap
{
    public class AnimalIdentifier : MonoBehaviour
    {
        public AnimalData animalData;
    }
}