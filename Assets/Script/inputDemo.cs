using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputDemo : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 0"))//X
        {
            Debug.Log("button0");
        }
        if (Input.GetKeyDown("joystick button 1"))//Y
        {
            Debug.Log("button1");
        }
        if (Input.GetKeyDown("joystick button 2"))//A
        {
            Debug.Log("button2");
        }
        if (Input.GetKeyDown("joystick button 3"))//B
        {
            Debug.Log("button3");
        }
        if (Input.GetKeyDown("joystick button 4"))//LB
        {
            Debug.Log("button4");
        }
        if (Input.GetKeyDown("joystick button 5"))//RB
        {
            Debug.Log("button5");
        }
        if (Input.GetKeyDown("joystick button 6"))//LT
        {
            Debug.Log("button6");
        }
        if (Input.GetKeyDown("joystick button 7"))//RT
        {
            Debug.Log("button7");
        }
        if (Input.GetKeyDown("joystick button 8"))//L3
        {
            Debug.Log("button8");
        }
        if (Input.GetKeyDown("joystick button 9"))//R3
        {
            Debug.Log("button9");
        }
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        if ((hori != 0) || (vert != 0))
        {
            Debug.Log("stick:" + hori + "," + vert);
        }
    }
}
