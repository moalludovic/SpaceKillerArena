using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public float tempTime;

    ////timer
    public GameObject timerObject;
    private Text Timer;
    public float timerDuration = 3;
    private float timerState;
    private bool isTimerActive = false;
    private bool fadeDown = false;
    
    ////death
    public Image blackFade;
    public bool isDead = false;
    private float fadeState = 0;
    private ApplicationManager app;


    float myDeltaTime;

    // Start is called before the first frame update
    void Start()
    {
        app = GetComponent<ApplicationManager>();
        Timer = timerObject.GetComponent<Text>();
        tempTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        myDeltaTime = Time.realtimeSinceStartup - tempTime;
        tempTime = Time.realtimeSinceStartup;

        if (isTimerActive){
            if (timerState <= 0)
            {
                Time.timeScale = 1;
                timerState = 0;
                isTimerActive = false;
                fadeDown = true;
                Timer.text = "0";
            }
            else
            {
                timerState -= myDeltaTime;
                Timer.text = ((int)(timerState + 1)).ToString();
            }
        }

        FadeDown();
        BlackFade();
    }

    public void StartTimer(){
        Time.timeScale = 0;
        timerState = timerDuration;
        isTimerActive = true;
    }

    public void FadeDown(){
        if(fadeDown){
            if(Timer.color.a > 0){
                //opacité
                Color tempColor = Timer.color;
                tempColor.a -= 1 * Time.deltaTime;
                Timer.color = tempColor;
            }
            else{
                fadeDown = false;
                timerObject.SetActive(false);
            }
        }
    }

    public void BlackFade()
    {
        if (isDead)
        {
            if (blackFade.color.a < 1)
            {
                //opacité
                Color tempColor = blackFade.color;
                tempColor.a += 2f * myDeltaTime;
                blackFade.color = tempColor;
            }
            else
            {
                Time.timeScale = 1;
                app.MainMenu();
            }
        }
    }

    public void Death(){
        Time.timeScale = 0;
        isDead = true;
    }
}
