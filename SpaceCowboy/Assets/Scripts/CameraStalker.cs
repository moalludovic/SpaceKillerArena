using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStalker : MonoBehaviour {
    private RaycastHit hit;
    private Transform target;
    private Vector3 relativePos;
    private Quaternion relativeRot;

    private float speed = 10;
    private float collisionOffset = 1;
    
    private float magnitude = 0.05f;
    public bool shake = false;

    // private 
    // Use this for initialization
    void Start () {
        target = transform.parent;
        relativePos = Quaternion.Inverse(target.rotation)*(transform.position-target.position);
        relativeRot = transform.localRotation;
        transform.parent = null;

	}

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation * relativeRot, speed * Time.deltaTime);
        Vector3 desiredPos = target.position + target.rotation * relativePos;

        if (Physics.Linecast(target.position, desiredPos, out hit))
        {
            Vector3 vecDir = Vector3.Normalize(desiredPos - target.position);
            desiredPos = target.position + vecDir * (hit.distance - collisionOffset);
        }

        if (shake)
        {
            Vector3 shake = new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude));
            transform.position = Vector3.Lerp(transform.position, desiredPos, speed * Time.deltaTime) + shake;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPos, speed * Time.deltaTime);
        }
    }
}
