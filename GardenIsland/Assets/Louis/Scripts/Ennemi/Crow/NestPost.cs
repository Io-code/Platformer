using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestPost : MonoBehaviour
{
    public Transform[] linkedNest;
    // Update is called once per frame
    void Update()
    {
        ShowPath();
    }
    public void ShowPath()
    {
        for(int i = 0; i < linkedNest.Length; i++)
        {
            Vector3 start = transform.position;
            Vector3 end = linkedNest[i].position;

            
            //Debug.DrawLine(start, end, Color.red);
            Debug.DrawLine(start, start +  0.5f*(end - start), Color.blue);
        }
    }

}
