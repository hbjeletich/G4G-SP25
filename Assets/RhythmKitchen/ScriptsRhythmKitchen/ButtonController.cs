using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RhythmKitchen
// Worked on by: Jovanna Molina
// Commented by: Jovanna Molina
// Description: This script is used to act as the judgement line for the player to hit the notes on time

{ 
  public class ButtonController : MonoBehaviour
  {
    public GameObject notePrefab; // this might not be necessary and just stick to sprite renderer
    private KeyCode keyToPress;
    
    void Start()
    {
      notePrefab = GetComponent<GameObject>();
    }
    void Update()
    {
      if (Input.GetKeyDown(keyToPress))
      {
        notePrefab.
      }
    }
  }
}