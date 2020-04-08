using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

	#region Displacement
	[Header("Horizontal Mouvement")]
	public float accelerationTime = .1f;
	public float decelerationTime = .2f;

	public float groundMoveSpeed = 16;
	#endregion

	#region Air
	#region Jump
	[Header("Jump")]
	public float jumpHeight = 4;
	public float timeToJumpApex = .3f;
	public float timeToJumpFall = .7f;

	public float ghostJumpBuffer = 0.3f;
	#endregion
	#region AirControl
	[Header("Air Control")]
	public float accelerationAirborneModifieur = 0.9f;
	public float airMoveSpeed = 12;
	#endregion

	[Header("Wall")]
	public float wallSlideSpeedMax = 5;
	bool wallSliding;
	#endregion

	#region Gilder Plant
	[Header("Gilder Plant")]
	public float gilderMoveSpeed = 10f;
	public float gilderVelocityY = 3f;
	public float gilderYdeceleration = 0.1f;
	bool hovering;
	#endregion

	#region Sticky Plant
	[Header("Sticky Plant")]
	public float stickyBuffer = 0.3f;
	[HideInInspector]
	public bool stickable;
	bool stickyInput;
	#endregion

	bool isGrounded;

	
	float gravity;
	float defaultGravity;

	float jumpVelocity;
	bool jump;

	public Vector3 velocity;
	float velocityXSmoothing;
	float velocityYSmoothing;

	Controller2D controller;
	BoxCollider2D collider;

	InputHandler inputHandler;
	Vector2 axisInput;

	public bool crowed;
	void Start()
	{
		controller = GetComponent<Controller2D>();
		collider = GetComponent<BoxCollider2D>();
		inputHandler = GetComponent<InputHandler>();

		defaultGravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpFall, 2);
		gravity = defaultGravity;
		jumpVelocity = Mathf.Abs(-(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2)) * timeToJumpApex;
		print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
	}

	void Update()
	{
		axisInput = inputHandler.axisInput;// get input 
		
		if (inputHandler.keyStickyPlant[1])
		{
			stickyInput = true;
			StartCoroutine(StickyBuffer());
		}

		GroundCheck();
		JumpCondition();

		GilderCondition();

		WallCheck();
		StickyContition();
	}

	public void FixedUpdate()
	{
		if (controller.collisions.above || controller.collisions.below) // fixe gravity
			velocity.y = 0;

		float airSpeed = (hovering) ? gilderMoveSpeed : airMoveSpeed;
		float targetVelocityX = (controller.collisions.below) ? groundMoveSpeed * axisInput.x : airSpeed * axisInput.x;

		float accelerationTimeGrounded = (axisInput.x != 0) ? accelerationTime : decelerationTime;
		float accelerationTimeAirborne = (accelerationAirborneModifieur * ((axisInput.x != 0) ? accelerationTime : decelerationTime));

		float accelerationVelocityX = (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne;
		

		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationVelocityX);
		velocity.y += gravity * Time.deltaTime;

		if (hovering)
			velocity.y = Mathf.SmoothDamp(velocity.y, -gilderVelocityY, ref velocityYSmoothing, gilderYdeceleration);

		if (jump)
			StartCoroutine(Jump());
		
		if (stickable)
			HookOn();

		if (controller.triggerInfo.hitBumper)
			Bumpering();

		if (controller.triggerInfo.hitCrow)
			TakeRecoil();

		if (velocity.y <=0)
				gravity = defaultGravity;

		controller.Move(velocity * Time.deltaTime);
	}
	public void TakeRecoil()
	{
		GameObject crow = controller.triggerInfo.hitObject;
		if (crow != null)
		{
			CrowBehaviour crowBehaviour = crow.GetComponent<CrowBehaviour>();

			Vector2 projection = -(crow.transform.position-transform.position) * crowBehaviour.recoilForce;
			float adjustement = crowBehaviour.recoilAdjustement;

			Vector2 projectionForce = new Vector2(projection.x * adjustement* crowBehaviour.recoilForce, projection.y * (1 - adjustement)+10);
			gravity = defaultGravity;

			if (projectionForce != Vector2.zero)
			{
				inputHandler.LockAxis(ref inputHandler.lockAxisInput, true, true, crowBehaviour.lockTime);

				isGrounded = false;
				velocity = projectionForce;
			}
		}
	}
	public void Bumpering()
	{
		GameObject bumper = controller.triggerInfo.hitObject;
		if (bumper != null)
		{
			BumperBehaviour bumperBehaviour = bumper.GetComponent<BumperBehaviour>();

			Vector2 projectionForce = bumperBehaviour.projectionForce;
			gravity = defaultGravity;

			if (projectionForce != Vector2.zero)
			{
				if (projectionForce.x != 0)
					inputHandler.LockAxis(ref inputHandler.lockAxisInput,( projectionForce.x < 0), (projectionForce.x > 0), 0.5f);

				isGrounded = false;
				velocity = projectionForce;
			}
		}
	}

	public void GroundCheck()
	{
		if (controller.collisions.below)
		{
			isGrounded = true;
		}
		if (!controller.collisions.below && velocity.y < 0)
		{
			StartCoroutine(GroundBuffer());
		}
	}
	IEnumerator GroundBuffer()
	{
		yield return new WaitForSeconds(ghostJumpBuffer);
		isGrounded = false;
	}

	public void JumpCondition()
	{
		if (inputHandler.keyJump[1] && (isGrounded || stickable))
			jump = true;
	}
	IEnumerator Jump()
	{

		isGrounded = false;
		jump = false;

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		velocity.y = jumpVelocity;

		yield return new WaitForSeconds(timeToJumpApex);

		gravity = defaultGravity;

	}

	public void GilderCondition()
	{
		if (inputHandler.keyJump[1] && !isGrounded && velocity.y < 0)
			hovering = true;
		if ((!(inputHandler.keyJump[0]) && (!(inputHandler.keyJump[1]))) || isGrounded || velocity.y >= 0)
			hovering = false;
	}
	public void Hover()
	{
		velocity.y = -gilderVelocityY;
	}

	public void WallCheck()
	{
		bool hangOn = false;
		wallSliding = false;

		if ((axisInput.x < 0 && controller.collisions.left) || (axisInput.x > 0 && controller.collisions.right))
		{
			hangOn = true;
		}

		if (hangOn && !controller.collisions.below && velocity.y < 0)
		{
			wallSliding = true;
			if (velocity.y < -wallSlideSpeedMax)
			{
				velocity.y = -wallSlideSpeedMax;
			}
		}
	}
	public void StickyContition()
	{
		if (inputHandler.keyStickyPlant[1] && stickable)
		{
			stickyInput = false;
		}

		if ((!controller.collisions.stickyWall) || (inputHandler.keyStickyPlant[1] && stickable) || jump)
		{
			stickable = false;
		}

		if ((controller.collisions.stickyWall) && stickyInput && !controller.collisions.below)
		{
			stickable = true;
			stickyInput = false;
		}

	}
	IEnumerator StickyBuffer()
	{
		yield return new WaitForSeconds(stickyBuffer);
		stickyInput = false;
	}
	public void HookOn()
	{
		velocity.y = 0;
	}

}
