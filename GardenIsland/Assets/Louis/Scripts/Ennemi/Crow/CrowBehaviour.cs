using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowBehaviour : MonoBehaviour
{
    public float speed;
    public float waitTime;

    public bool flying = true;
    Vector3 dodge;

    public float recoilForce;
    [Range(0f, 1f)]
    public float recoilAdjustement;
    public float lockTime;

    public Transform[] linkedNest;

    Vector3 nestTarget;
    Vector3 lastNest = Vector3.zero;
    Vector3 nestTransitor;

    public bool nestled = false;
    public bool searchingNest = false;

    BoxCollider2D collider;
    public string WhatIsNest;

    DetectionCorner detectionCorner;
    public LayerMask detectionMask;

    public LayerMask playerMask;
    
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        nestTarget = transform.position;
    }
    private void Update()
    {
        #region find nest
        if (nestled && searchingNest)// contdition to trigger research
            FindNest();
        if (!nestled)// reset condition
        {
            searchingNest = true;
            lastNest = nestTransitor;
        }
        #endregion
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        DetecteNest();
        UpdateDetection();

        PlayerCollision();
        Flying();
    }
    public void Flying()
    {
        if (flying)
            transform.position = Vector2.MoveTowards(transform.position, nestTarget, speed * Time.deltaTime);
        else
            transform.Translate(dodge*Time.deltaTime*speed*0.5f); ;
    }
    IEnumerator Waiting(float time, int target)
    {
        yield return new WaitForSeconds(time);
        nestTarget = linkedNest[target].position;
    }
    IEnumerator Staying(float time, float time2)
    {
        yield return new WaitForSeconds(time);
        dodge = Vector3.zero;
        yield return new WaitForSeconds(time);
        flying = true;
        
    }
    public void PlayerCollision()
    {
        Collider2D hit = Physics2D.OverlapArea(detectionCorner.bottomLeft, detectionCorner.topRight, playerMask);
        if (hit)
        {
            GameObject player = hit.gameObject;
     
            flying = false;
            StartCoroutine(Staying(0.3f,0.2f));

            Vector2 projection = (transform.position - player.transform.position)*1.5f;
            projection.y = Mathf.Abs(projection.y);

            dodge = projection;
        }
        
    }

    public void DetecteNest()
    {
        Collider2D hit = Physics2D.OverlapArea(detectionCorner.bottomLeft, detectionCorner.topRight, detectionMask);
        if (hit)
        {
            if (hit.tag == WhatIsNest)
            {
                
                GameObject nest = hit.gameObject;
                NestPost nestPost = nest.GetComponent<NestPost>();
                
                nestled = true;
                nestTransitor = nest.transform.position;
                linkedNest = nestPost.linkedNest;
            }
        }
        else
            nestled = false; 
       
    }
    public void FindNest()
    {
        if (linkedNest != null)
        {
            int target = Random.Range(0, linkedNest.Length);

            if (linkedNest[target].position == lastNest)
                target = Random.Range(0, linkedNest.Length);

            StartCoroutine(Waiting(waitTime, target));
            
        }
        searchingNest = false;
    }

   

    public void UpdateDetection()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(-0.3f);
        detectionCorner.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        detectionCorner.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        detectionCorner.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        detectionCorner.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    public struct DetectionCorner
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
