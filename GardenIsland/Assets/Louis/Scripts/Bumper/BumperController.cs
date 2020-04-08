using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : RaycastController
{
	public CollisionInfo collisions;
	public TriggerInfo triggerInfo;

	public string whatIsPlayer = "Player";
	public string whatIsSpike = "Spike";

	public override void Start()
	{
		base.Start();
	}
	public void Move(Vector3 velocity)
	{
		UpdateRaycastOrigins();
		collisions.Reset();

		if (velocity.x != 0)
		{
			HorizontalCollisions(ref velocity);
		}

		if (velocity.y != 0)
		{
			VerticalCollisions(ref velocity);
		}

		TriggerCollision(ref velocity);

		transform.Translate(velocity);
	}

	void HorizontalCollisions(ref Vector3 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
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
				collisions.stickyWall =(hit.collider.tag == "Stickable");
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

	void TriggerCollision(ref Vector3 velocity)
	{
		Collider2D hit = Physics2D.OverlapArea(raycastOrigins.bottomLeft, raycastOrigins.topRight, triggerMask);
		if (hit)
		{
			triggerInfo.hitPlayer = (hit.tag == whatIsPlayer);
			triggerInfo.hitObject = hit.gameObject;
		}
		else
			triggerInfo.Reset();
		
	}

	public struct CollisionInfo
	{
		public bool above, below, stickyWall;
		public bool left, right;

		public bool spike;
		public void Reset()
		{
			above = below = false;
			left = right = stickyWall  = false;

			spike = false;
		}
	}
	public struct TriggerInfo
	{
		public bool hitPlayer;
		public GameObject hitObject;
		public void Reset()
		{
			hitPlayer = false;
			hitObject = null;
		}
	}
}

