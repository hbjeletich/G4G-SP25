using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{

    public GameObject playerLocal;

    public float speed=1f;

    public float minDistance=0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(playerLocal.transform.position.x - transform.position.x, playerLocal.transform.position.y - transform.position.y);
        float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, rotation-90);
        
        float distance = Vector3.Distance(transform.position, playerLocal.transform.position);

        if (distance > minDistance)
        {
            transform.position=Vector3.MoveTowards(transform.position,playerLocal.transform.position,speed);
        }
    }
}
