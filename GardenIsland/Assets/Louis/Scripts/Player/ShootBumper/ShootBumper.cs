using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(Player))]
public class ShootBumper : MonoBehaviour
{

	[Header("Aim")]
	public float aimTime = 0.3f;
	public float minDistance = 1f;
	public float maxDistance = 5f;

	public GameObject pointerPrefab;
	GameObject pointer;

	bool aiming;
	float aimDistance;
	float aimHeight = 0.5f;

	[Header("Shoot")]
	public float shootForceMultiplier = 10;
	[Range(0f, 1f)]
	public float forceAdjusment = 0.5f;
	public ShootInfo shootInfo;

	[Header("BumperControl")]
	public int bumberAllowed = 4;
	public float bumperSize = 0.4f;
	GameObject[] bumperList;
	
	public GameObject bumperPrefab;
	public LayerMask obstructMask;

	Controller2D controller;
	BoxCollider2D collider;

	InputHandler inputHandler;
	Player player;
	// Start is called before the first frame update
	void Start()
    {
		controller = GetComponent<Controller2D>();
		collider = GetComponent<BoxCollider2D>();

		inputHandler = GetComponent<InputHandler>();
		player = GetComponent<Player>();

		pointer = Instantiate(pointerPrefab, transform.position, transform.rotation);
	}

    // Update is called once per frame
    void Update()
    {
		BumperAim(inputHandler.keyBumper[1], minDistance, aimTime, maxDistance);
		UpdateTargetPosition();

		Shoot(inputHandler.keyBumper[2], shootInfo.targetPosition, shootInfo.shootForce);
    }

	public void Shoot(bool input, Vector2 targetPosition, Vector2 projectionForce)
	{
		if (input && aiming)
		{
			GameObject bumper = Instantiate(bumperPrefab, new Vector3(targetPosition.x, targetPosition.y, 0), transform.rotation);
			BumperBehaviour bumperBehaviour = bumper.GetComponent<BumperBehaviour>();

			bumperBehaviour.bumperNumber = (bumberAllowed +1);
			ControlBumperNumber();

			bumperBehaviour.velocity = new Vector3(projectionForce.x, projectionForce.y, 0);
			aiming = false;
		}
	}
	public void UpdateTargetPosition()
	{
		Vector2 startPos ;
		Vector2 targetPos;
		float targetDistance = aimDistance;

		Vector2 direction;
		Vector2 projectionDir;

		FirePoint firePoint;
		Bounds bounds = collider.bounds;

		// set Start position
		bounds.Expand(aimHeight * -2);

		firePoint.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		firePoint.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		firePoint.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		firePoint.topRight = new Vector2(bounds.max.x, bounds.max.y);

		if (player.stickable)
		{
			bool axisDir = (Mathf.Sign(inputHandler.axisInput.y) == 1); 
			direction = (axisDir) ? Vector2.up : Vector2.down;

			if (controller.collisions.right)
			{
				startPos = (axisDir) ? firePoint.topRight : firePoint.bottomRight;
				projectionDir = Vector2.right; 
			}
			else
			{
				startPos = (axisDir) ? firePoint.topLeft : firePoint.bottomLeft;
				projectionDir = Vector2.left;
			}		
		}
		else
		{
			startPos = (controller.collisions.faceDir == 1) ? firePoint.bottomRight : firePoint.bottomLeft;
			direction = (controller.collisions.faceDir == 1) ? Vector2.right : Vector2.left;
			projectionDir = Vector2.down;
		}
		// initialisation target position
		RaycastHit2D hit = Physics2D.Raycast(startPos, direction, targetDistance+bumperSize, obstructMask);
		if(hit)// check collision
			targetDistance = hit.distance-bumperSize;
		
		targetPos = startPos + direction * (targetDistance);

		Debug.DrawLine(startPos, targetPos, Color.blue);

		// change shoot info settings
		shootInfo.targetPosition = targetPos;
		pointer.transform.position = targetPos;
		shootInfo.shootForce =( projectionDir * forceAdjusment + direction* (1-forceAdjusment)) * shootForceMultiplier;
	}

	public void BumperAim(bool input, float minDistance, float aimTime, float maxDistance)
	{
		Renderer pointerRenderer = pointer.GetComponent<Renderer>();
		float increasedDistance = maxDistance / aimTime;
		
		// Aim conditions
		if (input)
			StartCoroutine(AimTimer(aimTime));

		// Aim application
		if (aiming)
			aimDistance += increasedDistance * Time.deltaTime;
		else
			aimDistance = minDistance;

		pointerRenderer.enabled = (aiming);
			
	}
	IEnumerator AimTimer(float time)
	{
		aiming = true;
		yield return new WaitForSeconds(time);
		aiming = false;
	}

	public void ControlBumperNumber()
	{
		bumperList = GameObject.FindGameObjectsWithTag("Bumper");
		foreach(GameObject bumper in bumperList)
		{
			BumperBehaviour bumperBehaviour = bumper.GetComponent<BumperBehaviour>();
			bumperBehaviour.bumperNumber -= 1;
		}
		
	}

	public struct FirePoint
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
	public struct ShootInfo
	{
		public Vector2 targetPosition;
		public Vector2 shootForce;
	}

}
