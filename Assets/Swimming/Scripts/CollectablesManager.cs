using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Swimming
{
    public class CollectablesManager : MonoBehaviour
    {
        private static CollectablesManager _instance;
        [SerializeField] private Image[] shellImages;

        public static CollectablesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CollectablesManager>();
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject("CollectablesManager");
                        _instance = singletonObject.AddComponent<CollectablesManager>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void CollectShell(int index)
        {
            shellImages[index].color = Color.white;
        }
    }
}
