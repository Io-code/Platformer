using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RayCastController
{
    public Vector3 move;
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = move * Time.deltaTime;
        transform.Translate(velocity);
    }

    void MovePassengers(Vector3 velocity)
    {
        float DirectionX = Mathf.Sign(velocity.x);
        float DirectionY = Mathf.Sign(velocity.y);
    }
}
