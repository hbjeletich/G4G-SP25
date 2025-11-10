using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Constellation
{
    public class StartMenuButton : MonoBehaviour
    {

        public bool isDebug;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        void clickStuff()
        {
            SceneManager.LoadScene("BigDipper", LoadSceneMode.Additive);
        }
    }
}
