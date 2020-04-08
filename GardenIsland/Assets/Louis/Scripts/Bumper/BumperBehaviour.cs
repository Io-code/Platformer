  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperBehaviour : MonoBehaviour
{
    public int bumperNumber;
    public Vector2 verticalProjection;
    public Vector2 horizontalProjection ;
    [HideInInspector]
    public Vector2 projectionForce;
    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    float gravity = 0.3f;

    BumperController controller;
   
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<BumperController>();
    }

    // Update is called once per frame
    private void Update()
    {
        velocity.y += -gravity;
        
    }

    private void FixedUpdate()
    {
         if (bumperNumber <= 0)
            Destroy(gameObject, 0);

        ProjectionSetting();

        fixePosition();
        controller.Move(velocity * Time.deltaTime);
    }
    public void fixePosition()
    {
        if (controller.collisions.left || controller.collisions.right || controller.collisions.below)
            velocity.y = velocity.x = gravity = 0;
        //if (controller.collisions.stickyWall || controller.collisions.below)

    }

    public void ProjectionSetting()
    {
        if (controller.collisions.left || controller.collisions.right)//(controller.collisions.stickyWall)
            projectionForce = new Vector2 (horizontalProjection.x * ((controller.collisions.right) ? -1 : 1), horizontalProjection.y);
        else if (controller.collisions.below)
            projectionForce = verticalProjection;
    }

}
