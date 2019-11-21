using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverV3 : MonoBehaviour {
    private RaycastHit hit;
    private Rigidbody rb;
    public Text Velocity;
    public Transform body;

    //vitesse
    public float axeleration = 4000; //8000 (setup superspeed)
    public float rotationForce = 8f;

    //force
    public float force = 100f;//attire vers la cible et éloigne du sol (500 setup superspeed)
    public float RayDist = 10f;//sistance max pour choper les normales au sol
    public float maxTargetDist = 0.2f;//0.0025 (setup superspeed)
    public bool adjustForceWithMaxTargetDist = true;
    public float groundDistance = 1f;
    public float forceStabilisationAmplitude = 2f;//ajouter 1 pour avoir la distance au sol prise en compte max //8(setup superspeed)
    public float forceStabilisationExponent = 2f;//3(setup superspeed)

    public float repulsionMinDist = 0.4f;
    public float repulsionExponent = 2f;
    public float repulsionMult = 1f;

    public Transform propulsorsParent;
    public List<Transform> reactors;

    //rotation
    public bool quaternionStabilisation = true;
    public float rotStabilisationLerp = 20f;
    public float rotStabilisationThreshold = 0.05f;

    //fall
    public Vector3 gravityDirection;
    public Transform gravityCenter;

    //jump (déplace la target)
    public float jumpProportion=1.5f;
    public float jumpDelay=0.5f;
    private float currentJumpProportion = 0;

	// Use this for initialization
	void Start ()
    {
        if (adjustForceWithMaxTargetDist)
        {
            force /= maxTargetDist;
        }
        //récupération du rigidbody
        rb = GetComponent<Rigidbody>();
        //ajout des propulseurs dans la liste
        if (propulsorsParent!=null){
            for(int i = 0;i<propulsorsParent.childCount;++i){
                reactors.Add(propulsorsParent.GetChild(i));
            }
        }
        //initialisation centre de gravité
        if (gravityCenter==null){
            if(gravityDirection==Vector3.zero){
                gravityDirection = -Vector3.up;
            }
            else{
                Vector3.Normalize(gravityDirection);
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //gestion de la gravité
        if(gravityCenter!=null){
            gravityDirection = Vector3.Normalize(gravityCenter.position-transform.position);
        }
        //bool grounded = false;
        Vector3 groundNormal = Vector3.zero; //on récupère la normale pour chaque réacteur afin de faire la moyenne
        Vector3 targetPos = Vector3.zero;   //on récupère la position cible pour chaque réacteur afin de faire la moyenne
        float currentGroundDistance = 0; //on récupère la distance au sol pour chaque réacteur afin de faire la moyenne
        
        //saut préparation
        if (Input.GetKey(KeyCode.Space)){
            if(jumpDelay == 0){
                    currentJumpProportion = jumpProportion;
            }
            else{
                currentJumpProportion = Mathf.Min(currentJumpProportion + jumpProportion * Time.deltaTime / jumpDelay,jumpProportion);
            }
        }
        else{
            if (jumpDelay == 0)
            {
                currentJumpProportion = 0;
            }
            else
            {
                currentJumpProportion = Mathf.Max(currentJumpProportion - jumpProportion * Time.deltaTime / jumpDelay, 0);
            }
        }
        
        //calculs dedss variables pour les forces de lévitation
        if (currentJumpProportion!=0)//saut
        {
            groundNormal = transform.up;
            targetPos = transform.position + transform.up * jumpProportion;
            currentGroundDistance = forceStabilisationAmplitude + groundDistance;
        }
        else//lévitation
        {
            ////forces
            for (int i = 0; i < reactors.Count; ++i)
            {
                if (Physics.Raycast(reactors[i].position, -transform.up, out hit, RayDist))
                {
                    //grounded = true;
                    groundNormal += hit.normal;
                    if (hit.distance < forceStabilisationAmplitude + groundDistance)//c'est inutile de mettre plus car on sort du rayon d'action de la stabilisation et ça provoqerais un gros déséquilibre quand on est au bord d'un trou
                    {
                        currentGroundDistance += hit.distance;
                    }
                    else
                    {
                        currentGroundDistance += forceStabilisationAmplitude + groundDistance;
                    }

                    if (hit.distance - groundDistance < maxTargetDist)
                    {
                        Vector3 reactorTargetPoint = hit.point + (Vector3.Normalize(reactors[i].position - hit.point) * groundDistance);
                        targetPos += reactorTargetPoint;
                        Debug.DrawLine(reactors[i].position, hit.point, Color.green);
                    }
                    else
                    {
                        Vector3 reactorTargetPoint = reactors[i].position - transform.up * (maxTargetDist);
                        targetPos += reactorTargetPoint;
                        Debug.DrawLine(reactors[i].position, reactors[i].position - transform.up * (maxTargetDist + groundDistance), Color.blue);
                    }
                }
                else
                {
                    groundNormal += -gravityDirection;
                    currentGroundDistance += forceStabilisationAmplitude + groundDistance;
                    targetPos += reactors[i].position - transform.up * (maxTargetDist);
                    Debug.DrawLine(reactors[i].position, reactors[i].position - transform.up * (maxTargetDist + groundDistance), Color.red);
                }
            }
            //moyennes des valeurs récupérées
            groundNormal /= reactors.Count;
            targetPos /= reactors.Count;
            currentGroundDistance /= reactors.Count;
        }
        //calcul d'un coefficient (repulsion) qui fait augmenter la force si le faisseau s'approche du sol //en phase de test
        float halfShipSize = 0.25f;
        float repulsion = Mathf.Min( Mathf.Max((currentGroundDistance- halfShipSize) /(groundDistance - halfShipSize), repulsionMinDist),1); //valeur entre 0 (ajusté) et 1
        repulsion = Mathf.Pow(repulsion, repulsionExponent);
        repulsion = 1 / repulsion - 1;
        repulsion *= repulsionMult;
        //Debug.Log(repulsion);

        //ajout des foreces pour faire léviter le vaisseau
        //la distance entre les 2 points n'est pas normalisée car elle sert pour le saut
        rb.AddForce((targetPos - transform.position) * force * Time.deltaTime *
            (Mathf.Min(1f, Mathf.Pow(Mathf.Abs(currentGroundDistance - groundDistance) / forceStabilisationAmplitude, forceStabilisationExponent)))//réduction de la force quand on se rapproche du centre
        );
        rb.AddForce((targetPos - transform.position) * force * Time.deltaTime * repulsion);//force de répulsion

        //stabilisation(rotation)
        if (quaternionStabilisation)
        {
            float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(groundNormal, transform.up));//vecteurs normalisés
            if (angle > rotStabilisationThreshold)
            {
                Vector3 axis = Vector3.Cross(groundNormal, transform.up);//vecteurs normalisés
                transform.rotation = Quaternion.Euler(-axis * angle * rotStabilisationLerp * Time.deltaTime) * transform.rotation;
            }
        }
        //rotation
        float turnAxis = Input.GetAxis("Horizontal");
        rb.AddTorque(groundNormal * turnAxis * rotationForce * Time.deltaTime);//modèle prenant en compte les normales du sol

        float animRot = Mathf.LerpAngle(body.localEulerAngles.z, -turnAxis * 50, 2f * Time.deltaTime);
        body.localEulerAngles = new Vector3(body.localEulerAngles.x, body.localEulerAngles.y, animRot);
        //déplacement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(transform.forward * axeleration * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(-transform.forward * axeleration * Time.deltaTime);
        }
        Velocity.text = (Vector3.Magnitude(rb.velocity) * 3.6f).ToString();
    }//fin fixed update


    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("collision");
    }
}//fin classe
