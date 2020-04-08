using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
	
	public CollisionInfo collisions;
	public TriggerInfo triggerInfo;
	
	public string whatIsBumper = "Bumper";
	public string whatIsCrow = "Crow";
	public string whatIsRespawn = "Respawn";

	public override void Start()
	{
		base.Start();
		collisions.faceDir = 1;
	}

	public void FixedUpdate()
	{
		TriggerCollision();
	}
	public void Move(Vector3 velocity)
	{
		UpdateRaycastOrigins();
		collisions.Reset();
		//triggerInfo.Reset();
		if (velocity.x != 0) 
		{
			collisions.faceDir =(int) Mathf.Sign(velocity.x);
		}

		HorizontalCollisions(ref velocity);

		if (velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}
		

		transform.Translate(velocity);
		
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		if (Mathf.Abs(velocity.x) < skinWidth)
		{
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit)
			{
				collisions.stickyWall = (hit.collider.tag == "Stickable");

				collisions.spike = (collisions.spike) ? collisions.spike : (hit.collider.tag == "Spike");

				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity)
	{
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit)
			{
				collisions.spike = (collisions.spike) ? collisions.spike : (hit.collider.tag == "Spike");

				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

	void TriggerCollision()
	{
		Collider2D hit = Physics2D.OverlapArea(raycastOrigins.bottomLeft, raycastOrigins.topRight, triggerMask);
		//RaycastHit2D hit = Physics2D.BoxCast(transform.position,transform.localScale,0,Vector2.zero,triggerMask); 
		if (hit)
		{
			triggerInfo.hitBumper = (hit.tag == whatIsBumper);
			triggerInfo.checkRespawn = (hit.tag == whatIsRespawn);
			triggerInfo.hitCrow = (hit.tag == whatIsCrow);

			triggerInfo.hitObject = hit.gameObject;
		}
		else
			triggerInfo.Reset();
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool stickyWall;
		public bool spike;

		public int faceDir;


		public void Reset()
		{
			above = below = false;
			left = right = stickyWall = false;
			spike = false;
		}
	}

	public struct TriggerInfo
	{
		public bool hitBumper, checkRespawn, hitCrow;
		public GameObject hitObject;
		public void Reset()
		{
			hitBumper = checkRespawn = false;
			hitCrow = false;
			hitObject = null;
		}
	}

}
