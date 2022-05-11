using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoypadMapper : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"Joysticks: {string.Join(", ", Input.GetJoystickNames())}");
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
            if (Input.GetKeyDown(vKey)){
               Debug.Log($"Pressed key: {vKey}");
            }
        }
        /*
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Debug.Log("Button pressed: 0");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            Debug.Log("Button pressed: 1");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            Debug.Log("Button pressed: 2");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            Debug.Log("Button pressed: 3");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            Debug.Log("Button pressed: 4");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            Debug.Log("Button pressed: 5");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton6))
        {
            Debug.Log("Button pressed: 6");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            Debug.Log("Button pressed: 7");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton8))
        {
            Debug.Log("Button pressed: 8");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            Debug.Log("Button pressed: 9");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton10))
        {
            Debug.Log("Button pressed: 10");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton11))
        {
            Debug.Log("Button pressed: 11");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton12))
        {
            Debug.Log("Button pressed: 12");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton13))
        {
            Debug.Log("Button pressed: 13");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton14))
        {
            Debug.Log("Button pressed: 14");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton15))
        {
            Debug.Log("Button pressed: 15");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton16))
        {
            Debug.Log("Button pressed: 16");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton17))
        {
            Debug.Log("Button pressed: 17");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton18))
        {
            Debug.Log("Button pressed: 18");
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton19))
        {
            Debug.Log("Button pressed: 19");
        }
        */

        float x1 = Input.GetAxis("Horizontal1");
        if (x1 != 0)
        {
            Debug.Log($"Horizontal 1 (Reported as X-Axis): {x1}");
        }
        float x2 = Input.GetAxis("Horizontal2");
        if (x2 != 0)
        {
            Debug.Log($"Horizontal 2 (Reported as 3rd axis): {x2}");
        }
        float x3 = Input.GetAxis("Horizontal3");
        if (x3 != 0)
        {
            Debug.Log($"Horizontal 3 (Reported as 7th axis): {x3}");
        }
        float x4 = Input.GetAxis("Horizontal4");
        if (x4 != 0)
        {
            Debug.Log($"Horizontal 4 (Reported as 5th axis): {x4}");
        }
        float y1 = Input.GetAxis("Vertical1");
        if (y1 != 0)
        {
            Debug.Log($"Vertical 1 (Reported as Y-Axis): {y1}");
        }
        float y2 = Input.GetAxis("Vertical2");
        if (y2 != 0)
        {
            Debug.Log($"Vertical 2 (Reported as 4th axis): {y2}");
        }
        float y3 = Input.GetAxis("Vertical3");
        if (y3 != 0)
        {
            Debug.Log($"Vertical 3 (Reported as 8th axis): {y3}");
        }
        float y4 = Input.GetAxis("Vertical4");
        if (y4 != 0)
        {
            Debug.Log($"Vertical 4 (Reported as 6th axis): {y4}");
        }
    }
}
