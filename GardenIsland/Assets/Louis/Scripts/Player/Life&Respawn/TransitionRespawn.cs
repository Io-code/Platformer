using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRespawn : MonoBehaviour
{
    public Animator transition;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

       // if (Input.GetKeyDown(KeyCode.Space))
            //respawnAnimation(1f, 1f, 1f);
    }
    public void respawnAnimation(float timeBetween, float startSpeed, float endSpeed)
    {
        StartCoroutine(SetRespawnTransition(timeBetween, startSpeed, endSpeed));
    }
    public IEnumerator SetRespawnTransition(float timeBetween, float startSpeed, float endSpeed)
    {
        transition.speed = startSpeed;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(timeBetween);
        transition.speed = endSpeed;
        transition.SetTrigger("End");
    }
}
