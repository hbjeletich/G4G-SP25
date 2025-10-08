using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace constellation
{
    public class PlayerController : MonoBehaviour
    {
        //Declaration Area
        [SerializeField] private float rotationMod;
        [SerializeField] private float speedMod;
        [SerializeField] private float speedStat = 5.0f;
        [SerializeField] private float rotateStat = 5.0f;
        [SerializeField] private float maxSpeed = 5.0f;

        private Rigidbody2D charBody;


        // Start is called before the first frame update
        void Start()
        {
            charBody = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            speedMod = Input.GetAxis("Vertical");
            rotationMod = Input.GetAxis("Horizontal");
        }

        void FixedUpdate()
        {
            charBody.rotation += (-1 * rotationMod * rotateStat);
            //if (speedMod > 0)
            //{
            charBody.AddForce(transform.up*speedMod*speedStat, ForceMode2D.Impulse);
            //}
            /*
            else if (speedMod < 0)
            {
                charBody.AddForce(transform.up*-1, ForceMode2D.Impulse);
            }
            */
        }
    }
}
