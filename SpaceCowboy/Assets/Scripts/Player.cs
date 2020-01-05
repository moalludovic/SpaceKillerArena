using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private HoverV3 hover;
    ////speed and jump
    public Vector2 barRotationRange = new Vector2(20, 110);
    //speed
    public GameObject speedUI;
    public Color speedBar;
    public Color speedBarFilled;
    public Color speedBarBoost;
    public float maxSpeed = 172;
    //jump
    public GameObject jumpUI;
    public Color jumpBar;
    public Color jumpBarEmpty;
    public Color jumpBarFilled;

    public UIManager UI;


    // Start is called before the first frame update
    void Start()
    {
        hover = GetComponent<HoverV3>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = Vector3.Magnitude(GetComponent<Rigidbody>().velocity) * 3.6f;
        float speedProportion = Mathf.Min(speed / maxSpeed, 1);
        speedUI.transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(-barRotationRange.x, -barRotationRange.y, speedProportion), 0);
        jumpUI.transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(barRotationRange.x, barRotationRange.y, speedProportion), 0);

        //rotation
        speedUI.transform.GetChild(0).GetComponent<Image>().fillAmount = speedProportion;
        jumpUI.transform.GetChild(0).GetComponent<Image>().fillAmount = 1 - hover.jumpUsage;

        //couleur barre de vitesse
        if (speedProportion == 1)
        {
            speedUI.transform.GetChild(0).GetComponent<Image>().color = speedBarFilled;
        }
        else
        {
            speedUI.transform.GetChild(0).GetComponent<Image>().color = speedBar;
        }

        //couleur barre de saut
        if (hover.isJumpLocked)
        {
            jumpUI.transform.GetChild(0).GetComponent<Image>().color = jumpBarEmpty;
        }
        else if (hover.jumpUsage == 0)
        {
            jumpUI.transform.GetChild(0).GetComponent<Image>().color = jumpBarFilled;
        }
        else
        {
            jumpUI.transform.GetChild(0).GetComponent<Image>().color = jumpBar;
        }
    }

    public void Die()
    {
        Debug.Log("Tu es mort");
        GetComponent<AudioSource>().Play();
        UI.Death();
    }

    public void Dash()
    {
        hover.SetBoostActive();
    }
}

//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.gameObject.tag == "DeathZone"){
//            Debug.Log("Tu es mort");
//            GetComponent<AudioSource>().Play();
//            UI.Death();
//        }
//        if(other.gameObject.tag == "Dash"){
//            hover.SetBoostActive();
//        }
//    }
//}
