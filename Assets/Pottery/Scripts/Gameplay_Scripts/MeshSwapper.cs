using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pottery
{
    // Swaps meshes based on slider value and manages progress  for gameplay//

    public class MeshSwapper : MonoBehaviour
    {

        
        Settings_Script settingsscript;
        private bool isResetting = false;

        public float Progress = 0f;
        public float MaxProgress = 40f;

        public Slider slider;
        public int slidervalue = 0;


        //assign meshes in inspector//
        [Header("Assign your 4 meshes here")]
        public Mesh mesh01;
        public Mesh mesh02;
        public Mesh mesh03;
        public Mesh mesh04;
        public Mesh mesh05;
        public Mesh mesh06;

        private MeshFilter meshFilter;
        private int currentMeshIndex = 0;



        //void Awake()
        //{
        //    GameObject go = GameObject.Find("GameView");
        //    if (go != null)
        //    {
        //        settingsscript = go.GetComponent<Settings_Script>();
        //        if (settingsscript == null)
        //        {
        //            Debug.LogError("Settings_Script not found on GameView.");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("GameObject named 'GameView' not found in scene.");
        //    }
        //}

        // get meshfilter component and set initial mesh//
        void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogError("No MeshFilter found on this GameObject!");
                return;
            }

            meshFilter.mesh = mesh01;
        }

        void Update()
        {
            updateMesh();
        }

        // ----------------------------------------------------
        // RESET PROGRESS WITH 5-FRAME DELAY
        // ----------------------------------------------------
        public void ResetProgress()
        {
            StartCoroutine(ResetProgressDelayed());
        }

        private IEnumerator ResetProgressDelayed()
        {
            isResetting = true;  // 

            // Wait 5 frames
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Progress = 0;
            slidervalue = 0;

            if (slider != null)
                slider.value = 0;

            if (meshFilter != null)
                meshFilter.mesh = mesh01;

            isResetting = false; 
        }

        // ----------------------------------------------------
        // MESH PROGRESSION BASED ON SLIDER VALUE
        // ----------------------------------------------------
        void updateMesh()
        {
            if (isResetting) return;  // ⛔ do nothing if resetting

            slider.value = slidervalue;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                slidervalue = (slidervalue + 1);
                Debug.Log("space pressed");
            }

            switch (slidervalue)
            {
                case 0:
                    meshFilter.mesh = mesh01;
                    break;
                case 7:
                    meshFilter.mesh = mesh02;
                    break;
                case 14:
                    meshFilter.mesh = mesh03;
                    break;
                case 21:
                    meshFilter.mesh = mesh04;
                    break;
                case 35:
                    meshFilter.mesh = mesh05;
                    break;
                case 40:
                    meshFilter.mesh = mesh06;
                    break;
            }
        }
    }
}
