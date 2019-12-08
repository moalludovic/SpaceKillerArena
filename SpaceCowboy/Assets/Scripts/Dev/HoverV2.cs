﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverV2 : MonoBehaviour {
    private RaycastHit hit;
    private Rigidbody rb;

    //vitesse
    public float axeleration = 2000;

    //force
    public float force = 150f;
    public float rayDist = 10f;
    public float groundDistance = 1f;
    public List<Transform> reactors;

    //rotation
    public bool quaternionStabilisation = true;
    public float StabilisationLerp = 50f;
    public float StabilisationThreshold = 0.05f;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        bool grounded = false;
        Vector3 groundNormal = Vector3.zero;
        ////forces

        for (int i = 0; i < reactors.Count; ++i)
        {
            if (Physics.Raycast(reactors[i].position, -transform.up, out hit,rayDist))
            {
                grounded = true;
                groundNormal += hit.normal;
                Debug.DrawLine(reactors[i].position, hit.point, Color.green);
                if (hit.distance != 0)
                {
                    Vector3 targetPos = hit.point + Vector3.Normalize(transform.position - hit.point) * groundDistance;
                    rb.AddForceAtPosition(force*(targetPos - reactors[i].position), reactors[i].position);
                }
                else
                {
                    Debug.Log("le pivot du vaisseau touche le sol");
                }
            }
            else
            {
                rb.AddForceAtPosition(Vector3.Normalize(reactors[i].position - hit.point) * force * Time.deltaTime, reactors[i].position);
                Debug.DrawLine(reactors[i].position, reactors[i].position - transform.up* rayDist, Color.red);
            }
        }
        //speed
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(transform.forward * axeleration * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(-transform.forward * axeleration * Time.deltaTime);
        }

        ////rotation
        float turnAxis = Input.GetAxis("Horizontal");
        //rb.AddRelativeTorque(hit.normal * turnAxis * 2);

       if(grounded){
            if (quaternionStabilisation)
            {
                float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(hit.normal, transform.up));//vecteurs normalisés
                Debug.Log(angle);
                if (angle > StabilisationThreshold)
                {
                    Vector3 axis = Vector3.Cross(hit.normal, transform.up);//vecteurs normalisés
                    transform.rotation = Quaternion.Euler(-axis * angle * StabilisationLerp * Time.deltaTime) * transform.rotation;
                }
            }
            Vector3.Normalize(groundNormal);
            //rb.AddTorque(transform.up * turnAxis * 2);//modéle en fonction du vaisseau
            rb.AddTorque(groundNormal * turnAxis * 2);//modèle en fonction du sol
        }
        //else
        //{
        //    float angle = Mathf.Acos(Vector3.Dot(Vector3.up, transform.up));//vecteurs normalisés
        //    Debug.Log(angle);
        //    if(angle>StabilisationThreshold)
        //    {
        //        Vector3 axis = Vector3.Cross(Vector3.up, transform.up);//vecteurs normalisés
        //        transform.rotation = Quaternion.Euler(axis * angle * StabilisationLerp * Time.deltaTime) * transform.rotation;
        //    }
        //}
    }//fin fixed update


}//fin classe