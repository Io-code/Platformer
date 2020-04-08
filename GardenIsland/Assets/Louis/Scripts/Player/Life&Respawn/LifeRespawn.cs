using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class LifeRespawn : MonoBehaviour
{
    Controller2D controller;
    InputHandler inputHandler;
    Player playerBehaviour;

    [Header("Respawn Param")]
    public Vector3 respawnPoint;

    public float respawnDuration = 1f;
    public float timeBeforeRespawn = 5f;
    public float timeAfterRespaw = 0.4f;

    public bool animTransition = true;
    public bool respawning;

    public GameObject respawnTransition;
    public TransitionRespawn transition;

    [Header("Life Param")]
    public int lifePointMax = 2;
    public int lifePoint;

    public bool invincible;
    public float invincibleTime = 0.3f;

    public int spikeDamage = 2;
    public int crowDamage = 1;

    public bool hitCrow ;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
        inputHandler = GetComponent<InputHandler>();
        playerBehaviour = GetComponent<Player>();

        transition = respawnTransition.GetComponent<TransitionRespawn>();

        lifePoint = lifePointMax;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRespawnPoint(controller.triggerInfo.checkRespawn, controller.triggerInfo.hitObject,ref respawnPoint);


    }
    private void FixedUpdate()
    {
        if (controller.collisions.spike && !controller.triggerInfo.hitBumper)
            lifePoint -= spikeDamage;

        hitCrow = (controller.triggerInfo.hitCrow);
        if (controller.triggerInfo.hitCrow && !invincible)
        {
            lifePoint -= 1;
            invincible = true;
        }

        if (invincible)
            StartCoroutine(invincibleTimer(invincibleTime));

        if (lifePoint <= 0)
        {
            respawning = true;
            playerBehaviour.velocity = Vector2.zero;
            if (animTransition)
            {
                transition.respawnAnimation(1f, 1, 1);
                animTransition = false;
            }
            StartCoroutine(WaitRespawn(timeBeforeRespawn, timeAfterRespaw));
        }
        else
        {
            animTransition = true;
            respawning = false;
        }
    }

    public void collisionsCrow()
    {
        hitCrow = (controller.triggerInfo.hitCrow && !invincible);
    }

    public void UpdateRespawnPoint(bool checkRespawn,GameObject objRespawn, ref Vector3 respawnPoint)
    {
        if (checkRespawn)
        {
            RespawnBehaviour respawnBehaviour = objRespawn.GetComponent<RespawnBehaviour>();
            Vector3 targetRespawnPoint = respawnBehaviour.respawnPoint;

            if (respawnPoint != targetRespawnPoint)
                respawnPoint = targetRespawnPoint;
        }
    }
    public void Respawn(float duration, float timeBefore, float timeAfter)
    {
        StartCoroutine(WaitRespawn(timeBefore, timeAfter));  
    }
    public IEnumerator WaitRespawn(float timeBefore, float timeAfter)
    {
        float globalTime = timeBefore + timeAfter;

        // lock input
        inputHandler.LockAxis(ref inputHandler.lockAxisInput,true,true,globalTime+1);

        inputHandler.LockKey(ref inputHandler.keyBumper, globalTime);
        inputHandler.LockKey(ref inputHandler.keyJump, globalTime);

        
        yield return new WaitForSeconds(timeBefore );

        // reset position
        transform.position = respawnPoint;
        lifePoint = lifePointMax;

    }
    public IEnumerator invincibleTimer(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
}
