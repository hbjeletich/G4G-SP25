using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Sewing
{
    public class UIManager : MonoBehaviour
    {
        public GameObject[] shownArray;
        public GameObject[] hiddenArray;

        void Start()
        {
            foreach (GameObject obj in shownArray)
            {
                obj.SetActive(true);
            }

            foreach (GameObject obj in hiddenArray)
            {
                obj.SetActive(false);
            }
        }

        public void ShowCompletionUI()
        {
            foreach (GameObject obj in shownArray)
            {
                obj.SetActive(false);
            }

            foreach (GameObject obj in hiddenArray)
            {
                obj.SetActive(true);
            }
        }
    }
}
