using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [HideInInspector]
    public enum keyInfo { key, down, up, block };
    
    public KeyCode jump = KeyCode.Joystick1Button0;
    [HideInInspector]
    public bool[] keyJump = { false, false, false,false };
  

    public KeyCode bumper = KeyCode.Joystick1Button5;   
    [HideInInspector]
    public bool[] keyBumper = { false, false, false,false };
  
    public KeyCode stickyPlant = KeyCode.Joystick1Button4;
    [HideInInspector]
    public bool[] keyStickyPlant = { false, false, false,false };
  

    public string axisHorizontal = "Horizontal";
    public string axisVertical = "Vertical";

    public Vector2 axisInput;
    public bool[] lockAxisInput = { false, false };
   
    // Update is called once per frame
    public void Update()
    {
        AxisUpdate(ref axisInput, Input.GetAxis(axisHorizontal), Input.GetAxis(axisVertical), lockAxisInput);
       
        KeyUpdate(jump,keyJump);
        KeyUpdate(bumper, keyBumper);
        KeyUpdate(stickyPlant, keyStickyPlant);
    }

    public void KeyUpdate(KeyCode key, bool[] keyArray)
    {
        if (keyArray[(int)keyInfo.block])
        {
            for(int i =0; i<(keyArray.Length -2); i++)
            {
                keyArray[i] = false;
            }
        }
        else
        {
            keyArray[(int)keyInfo.key] = Input.GetKey(key);
            keyArray[(int)keyInfo.down] = Input.GetKeyDown(key);
            keyArray[(int)keyInfo.up] = Input.GetKeyUp(key);

        }
    }
    public void AxisUpdate(ref Vector2 axis, float xAxis, float yAxis, bool[] block)
    {
        if (xAxis > 0 && block[0]) // block right
            xAxis = 0;

        if (xAxis < 0 && block[1]) // block left
            xAxis = 0;

        axis = new Vector2(xAxis,yAxis);
    }

    public void LockKey(ref bool[] keyArray, float time)
    {
        StartCoroutine(LockKeyTimer(keyArray, time));
    }
    public IEnumerator LockKeyTimer(bool[] keyArray, float time)
    {
        keyArray[(int)keyInfo.block] = true;
        yield return new WaitForSeconds(time);
        keyArray[(int)keyInfo.block] = false;
    }

    public void LockAxis(ref bool[] lockAxis, bool right, bool left, float time)
    {
        StartCoroutine(LockAxisTimer(lockAxis, right, left, time));
    }
    public IEnumerator LockAxisTimer(bool[] lockAxis, bool right, bool left, float time)
    {
        lockAxis[0] = right;
        lockAxis[1] = left;
        yield return new WaitForSeconds(time);

        for (int i = 0; i < lockAxis.Length; i++)
            lockAxis[i] = false;
    }
}
