using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStalker : MonoBehaviour {
    private Transform target;
    private Vector3 relativePos;
    private Quaternion relativeRot;
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
        transform.rotation = Quaternion.Lerp(transform.rotation,target.rotation * relativeRot,0.2f);
        transform.position = Vector3.Lerp(transform.position,target.position + target.rotation*relativePos, 0.2f);
	}
}
