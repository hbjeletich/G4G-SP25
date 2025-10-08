using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace constellation
{
    public class StarScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
            print("Hit");
        }
    }
}
