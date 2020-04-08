using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBehaviour : MonoBehaviour
{
    public LayerMask collisionMask;
    public Vector3 respawnPoint;
    public float skinWith = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = transform.position;
        
    }
    void Update()
    {
        SetRespawnPoint(ref respawnPoint);
    }

    // Update is called once per frame
    void SetRespawnPoint(ref Vector3 respawnPoint)
    {
        Vector2 ray0rigin = new Vector2(transform.position.x, transform.position.y);
        float rayLength = transform.localScale.y*0.5f ;

        RaycastHit2D hit = Physics2D.Raycast(ray0rigin, Vector2.down, rayLength, collisionMask);
        Debug.DrawRay(ray0rigin, Vector2.down *rayLength, Color.red);

        if (hit)
        {
            respawnPoint.y = (transform.position.y-hit.distance+skinWith);
        }
    }
}
