using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    public float maxSpeed;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(-maxSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,speed * Time.deltaTime,0));
    }
}
