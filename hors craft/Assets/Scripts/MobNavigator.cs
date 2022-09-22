// DecompilerFi decompiler from Assembly-CSharp.dll class: MobNavigator
using com.ootii.Cameras;
using System;
using Uniblocks;
using UnityEngine;

public class MobNavigator : MonoBehaviour
{
	private Mob _mob;

	private Transform _cameraTransform;

	public bool dontMoveIfTooFar = true;

	[HideInInspector]
	public float speed;

	[HideInInspector]
	public float jumpPower;

	protected Vector3 destination;

	protected float lastJumpTime;

	private float movementTimeout;

	public bool canSwim;

	protected const float MIN_OFFSET_TO_APPLY_DESTINATION = 1.25f;

	protected const float JUMP_BLOCK_DURATION = 0.5f;

	protected const float ROTATION_SPEED = 90f;

	private const float TIMEOUT_AFTER = 6f;

	private const float MAX_DISTANCE_TO_MOVE = 100f;

	protected Mob mob
	{
		get
		{
			if (_mob == null)
			{
				_mob = GetComponent<Mob>();
			}
			return _mob;
		}
	}

	private Transform cameraTransform
	{
		get
		{
			if (_cameraTransform == null && (bool)CameraController.instance.MainCamera)
			{
				_cameraTransform = CameraController.instance.MainCamera.transform;
			}
			if (_cameraTransform == null)
			{
				_cameraTransform = Camera.main.transform;
			}
			return _cameraTransform;
		}
	}

	public bool moving
	{
		get;
		protected set;
	}

	protected bool grounded => mob.groundIndicator.active;

	protected bool obstacleAhead => mob.forwardObstacleIndicator.active;

	protected virtual void Start()
	{
	}

	private void FixedUpdate()
	{
	}

	protected virtual void Update()
	{
		if ((!dontMoveIfTooFar || !IsTooFarToMove()) && moving)
		{
			UpdateMovement();
			UpdateJump();
			if (EndMovementCondition())
			{
				EndMovement();
			}
		}
	}

	protected virtual bool EndMovementCondition()
	{
		Vector3 vector = destination - mob.groundPosition;
		vector.y = 0f;
		return vector.magnitude < 1.25f || Time.time > movementTimeout;
	}

	public void ForceEndMovement()
	{
		EndMovement();
	}

	private bool IsTooFarToMove()
	{
		Vector3 position = cameraTransform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		float num = Mathf.Abs(x - position2.x);
		if (num > 100f)
		{
			return true;
		}
		Vector3 position3 = cameraTransform.position;
		float z = position3.z;
		Vector3 position4 = base.transform.position;
		num = Mathf.Abs(z - position4.z);
		return num > 100f;
	}

	protected void StartMovement()
	{
		moving = true;
		if (mob.animator != null && Array.Exists(mob.animator.parameters, (AnimatorControllerParameter parametr) => parametr.name == "walking"))
		{
			mob.animator.SetBool("walking", value: true);
		}
		movementTimeout = Time.time + 6f;
	}

	protected virtual void UpdateMovement()
	{
		Vector3 vector = destination - mob.groundPosition;
		vector.y = 0f;
		vector.Normalize();
		if (!obstacleAhead)
		{
			Vector3 vector2 = vector * speed;
			if (!grounded)
			{
				vector2 /= 2f;
			}
			Vector3 velocity = mob.body.velocity;
			vector2.y = velocity.y;
			mob.body.velocity = vector2;
		}
		if (mob.transform.forward != vector)
		{
			Quaternion to = Quaternion.LookRotation(vector);
			mob.body.MoveRotation(Quaternion.RotateTowards(mob.transform.rotation, to, Time.deltaTime * 90f));
		}
	}

	public void Jump()
	{
		if (grounded && !(Time.time < lastJumpTime + 0.5f))
		{
			Rigidbody body = mob.body;
			Vector3 velocity = mob.body.velocity;
			float x = velocity.x;
			float y = jumpPower;
			Vector3 velocity2 = mob.body.velocity;
			body.velocity = new Vector3(x, y, velocity2.z);
			lastJumpTime = Time.time;
		}
	}

	protected virtual void UpdateJump()
	{
		if (grounded && obstacleAhead && !(Time.time < lastJumpTime + 0.5f) && !mob.forwardObstacleIndicator.activeCollider.gameObject.IsInLayerMask(LayerMask.GetMask("Ignore Raycast", "Mobs")))
		{
			Rigidbody body = mob.body;
			Vector3 velocity = mob.body.velocity;
			float x = velocity.x;
			float y = jumpPower;
			Vector3 velocity2 = mob.body.velocity;
			body.velocity = new Vector3(x, y, velocity2.z);
			lastJumpTime = Time.time;
		}
	}

	protected virtual void EndMovement()
	{
		mob.body.velocity = Vector3.zero;
		moving = false;
		if (mob.animator != null)
		{
			mob.animator.SetBool("walking", value: false);
		}
	}

	public virtual Vector3 GetDestination()
	{
		return destination;
	}

	public virtual void SetDestination(Vector3 pos)
	{
		if (!Physics.Raycast(pos + Vector3.up * 100f, Vector3.down, out RaycastHit hitInfo, 200f))
		{
			return;
		}
		if (!canSwim)
		{
			ushort? num = Engine.PositionToVoxelInfo(hitInfo.point + Vector3.up)?.GetVoxel();
			ushort num2 = (ushort)(num.HasValue ? num.Value : 0);
			if (num2 == Engine.usefulIDs.waterBlockID)
			{
				return;
			}
		}
		destination = hitInfo.point;
		Vector3 vector = destination - base.transform.position;
		vector.y = 0f;
		if (vector.magnitude >= 1.25f)
		{
			StartMovement();
		}
	}

	protected virtual void DrawDebugGizmos()
	{
		UnityEngine.Debug.DrawLine(mob.groundPosition + Vector3.up * 0.01f, destination + Vector3.up * 0.01f, Color.red);
	}
}
