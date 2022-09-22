// DecompilerFi decompiler from Assembly-UnityScript-firstpass.dll class: CharacterMotor
using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
[AddComponentMenu("Character/Character Motor")]
[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024SubtractNewPlatformVelocity_00241 : GenericGenerator<object>
	{
		internal CharacterMotor _0024self__00244;

		public _0024SubtractNewPlatformVelocity_00241(CharacterMotor self_)
		{
			_0024self__00244 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			yield return null;
		}
	}

	public bool canControl;

	public bool useFixedUpdate;

	[NonSerialized]
	public Vector3 inputMoveDirection;

	[NonSerialized]
	public bool inputJump;

	public CharacterMotorMovement movement;

	public CharacterMotorJumping jumping;

	public CharacterMotorMovingPlatform movingPlatform;

	public CharacterMotorSliding sliding;

	[NonSerialized]
	public bool grounded;

	[NonSerialized]
	public Vector3 groundNormal;

	private Vector3 lastGroundNormal;

	private Transform tr;

	private CharacterController controller;

	[NonSerialized]
	public bool isKnockedBack;

	[NonSerialized]
	public float knockBackTimer;

	[NonSerialized]
	public Vector3 knockBackForce;

	public CharacterMotor()
	{
		canControl = true;
		useFixedUpdate = true;
		inputMoveDirection = Vector3.zero;
		movement = new CharacterMotorMovement();
		jumping = new CharacterMotorJumping();
		movingPlatform = new CharacterMotorMovingPlatform();
		sliding = new CharacterMotorSliding();
		grounded = true;
		groundNormal = Vector3.zero;
		lastGroundNormal = Vector3.zero;
		knockBackForce = Vector3.zero;
	}

	public void Awake()
	{
		controller = (CharacterController)GetComponent(typeof(CharacterController));
		tr = transform;
	}

	public void UpdateFunction()
	{
		Vector3 velocity = movement.velocity;
		if (!(knockBackTimer <= 0f))
		{
			inputMoveDirection = knockBackForce;
			inputJump = true;
			knockBackTimer -= Time.deltaTime;
		}
		velocity = ApplyInputVelocityChange(velocity);
		velocity = ApplyGravityAndJumping(velocity);
		Vector3 zero = Vector3.zero;
		if (MoveWithPlatform())
		{
			Vector3 a = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
			zero = a - movingPlatform.activeGlobalPoint;
			if (zero != Vector3.zero)
			{
				controller.Move(zero);
			}
			Quaternion lhs = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
			Vector3 eulerAngles = (lhs * Quaternion.Inverse(movingPlatform.activeGlobalRotation)).eulerAngles;
			float y = eulerAngles.y;
			if (y != 0f)
			{
				tr.Rotate(0f, y, 0f);
			}
		}
		Vector3 position = tr.position;
		Vector3 vector = velocity * Time.deltaTime;
		float d = Mathf.Max(controller.stepOffset, new Vector3(vector.x, 0f, vector.z).magnitude);
		if (grounded)
		{
			vector -= d * Vector3.up;
		}
		movingPlatform.hitPlatform = null;
		groundNormal = Vector3.zero;
		if (controller.enabled)
		{
			movement.collisionFlags = controller.Move(vector);
		}
		movement.lastHitPoint = movement.hitPoint;
		lastGroundNormal = groundNormal;
		if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform && movingPlatform.hitPlatform != null)
		{
			movingPlatform.activePlatform = movingPlatform.hitPlatform;
			movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
			movingPlatform.newPlatform = true;
		}
		Vector3 vector2 = new Vector3(velocity.x, 0f, velocity.z);
		if (Time.timeScale == 0f)
		{
			movement.velocity = new Vector3(0f, 0f, 0f);
		}
		else
		{
			movement.velocity = (tr.position - position) / Time.deltaTime;
		}
		Vector3 lhs2 = new Vector3(movement.velocity.x, 0f, movement.velocity.z);
		if (vector2 == Vector3.zero)
		{
			movement.velocity = new Vector3(0f, movement.velocity.y, 0f);
		}
		else
		{
			float value = Vector3.Dot(lhs2, vector2) / vector2.sqrMagnitude;
			movement.velocity = vector2 * Mathf.Clamp01(value) + movement.velocity.y * Vector3.up;
		}
		if (!(movement.velocity.y >= velocity.y - 0.001f))
		{
			if (!(movement.velocity.y >= 0f))
			{
				movement.velocity.y = velocity.y;
			}
			else
			{
				jumping.holdingJumpButton = false;
			}
		}
		if (grounded && !IsGroundedTest())
		{
			grounded = false;
			if (movingPlatform.enabled && (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
			{
				movement.frameVelocity = movingPlatform.platformVelocity;
				movement.velocity += movingPlatform.platformVelocity;
			}
			SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			tr.position += d * Vector3.up;
		}
		else if (!grounded && IsGroundedTest())
		{
			grounded = true;
			inputJump = false;
			jumping.jumping = false;
			StartCoroutine(SubtractNewPlatformVelocity());
			SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
		}
		if (MoveWithPlatform())
		{
			CharacterMotorMovingPlatform characterMotorMovingPlatform = movingPlatform;
			Vector3 position2 = tr.position;
			Vector3 up = Vector3.up;
			Vector3 center = controller.center;
			characterMotorMovingPlatform.activeGlobalPoint = position2 + up * (center.y - controller.height * 0.5f + controller.radius);
			movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
			movingPlatform.activeGlobalRotation = tr.rotation;
			movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation;
		}
	}

	public void FixedUpdate()
	{
		if (!movingPlatform.enabled)
		{
			return;
		}
		if (movingPlatform.activePlatform != null)
		{
			if (!movingPlatform.newPlatform)
			{
				Vector3 platformVelocity = movingPlatform.platformVelocity;
				movingPlatform.platformVelocity = (movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint) - movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)) / Time.deltaTime;
			}
			movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
			movingPlatform.newPlatform = false;
		}
		else
		{
			movingPlatform.platformVelocity = Vector3.zero;
		}
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		if (!canControl)
		{
			inputMoveDirection = Vector3.zero;
		}
		Vector3 vector = default(Vector3);
		if (grounded && TooSteep())
		{
			vector = new Vector3(groundNormal.x, 0f, groundNormal.z).normalized;
			Vector3 vector2 = Vector3.Project(inputMoveDirection, vector);
			vector = vector + vector2 * sliding.speedControl + (inputMoveDirection - vector2) * sliding.sidewaysControl;
			vector *= sliding.slidingSpeed;
		}
		else
		{
			vector = GetDesiredHorizontalVelocity();
		}
		if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		{
			vector += movement.frameVelocity;
			vector.y = 0f;
		}
		if (grounded)
		{
			vector = AdjustGroundVelocityToNormal(vector, groundNormal);
		}
		else
		{
			velocity.y = 0f;
		}
		float num = GetMaxAcceleration(grounded) * Time.deltaTime;
		Vector3 b = vector - velocity;
		if (!(b.sqrMagnitude <= num * num))
		{
			b = b.normalized * num;
		}
		if (grounded || canControl)
		{
			velocity += b;
		}
		if (grounded)
		{
			velocity.y = Mathf.Min(velocity.y, 0f);
		}
		return velocity;
	}

	private Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		if (!inputJump || !canControl)
		{
			jumping.holdingJumpButton = false;
			jumping.lastButtonDownTime = -100f;
		}
		if (inputJump && !(jumping.lastButtonDownTime >= 0f) && canControl)
		{
			jumping.lastButtonDownTime = Time.time;
		}
		if (grounded)
		{
			velocity.y = Mathf.Min(0f, velocity.y) - movement.gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;
			if (jumping.jumping && jumping.holdingJumpButton && !(Time.time >= jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)))
			{
				velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
			}
			velocity.y = Mathf.Max(velocity.y, 0f - movement.maxFallSpeed);
		}
		if (grounded)
		{
			if (jumping.enabled && canControl && !(Time.time - jumping.lastButtonDownTime >= 0.2f))
			{
				grounded = false;
				jumping.jumping = true;
				jumping.lastStartTime = Time.time;
				jumping.lastButtonDownTime = -100f;
				jumping.holdingJumpButton = true;
				if (TooSteep())
				{
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
				}
				else
				{
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
				}
				velocity.y = 0f;
				velocity += jumping.jumpDir * CalculateJumpVerticalSpeed(jumping.baseHeight);
				if (movingPlatform.enabled && (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
				{
					movement.frameVelocity = movingPlatform.platformVelocity;
					velocity += movingPlatform.platformVelocity;
				}
				SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				jumping.holdingJumpButton = false;
			}
		}
		return velocity;
	}

	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Vector3 normal = hit.normal;
		if (normal.y <= 0f)
		{
			return;
		}
		Vector3 normal2 = hit.normal;
		if (normal2.y <= groundNormal.y)
		{
			return;
		}
		Vector3 moveDirection = hit.moveDirection;
		if (!(moveDirection.y >= 0f))
		{
			if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001f || lastGroundNormal == Vector3.zero)
			{
				groundNormal = hit.normal;
			}
			else
			{
				groundNormal = lastGroundNormal;
			}
			movement.hitPoint = hit.point;
			movement.frameVelocity = Vector3.zero;
		}
	}

	private IEnumerator SubtractNewPlatformVelocity()
	{
		return new _0024SubtractNewPlatformVelocity_00241(this).GetEnumerator();
	}

	private bool MoveWithPlatform()
	{
		int num = movingPlatform.enabled ? 1 : 0;
		if (num != 0)
		{
			num = (grounded ? 1 : 0);
			if (num == 0)
			{
				num = ((movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked) ? 1 : 0);
			}
		}
		if (num != 0)
		{
			num = ((movingPlatform.activePlatform != null) ? 1 : 0);
		}
		return (byte)num != 0;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		Vector3 vector = tr.InverseTransformDirection(inputMoveDirection);
		float num = MaxSpeedInDirection(vector);
		if (grounded)
		{
			Vector3 normalized = movement.velocity.normalized;
			float time = Mathf.Asin(normalized.y) * 57.29578f;
			num *= movement.slopeSpeedMultiplier.Evaluate(time);
		}
		return tr.TransformDirection(vector * num);
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 lhs = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(lhs, groundNormal).normalized * hVelocity.magnitude;
	}

	private bool IsGroundedTest()
	{
		return groundNormal.y > 0.01f;
	}

	public float GetMaxAcceleration(bool grounded)
	{
		return (!grounded) ? movement.maxAirAcceleration : movement.maxGroundAcceleration;
	}

	public float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * movement.gravity);
	}

	public bool IsJumping()
	{
		return jumping.jumping;
	}

	public bool IsSliding()
	{
		int num = grounded ? 1 : 0;
		if (num != 0)
		{
			num = (sliding.enabled ? 1 : 0);
		}
		if (num != 0)
		{
			num = (TooSteep() ? 1 : 0);
		}
		return (byte)num != 0;
	}

	public bool IsTouchingCeiling()
	{
		return (movement.collisionFlags & CollisionFlags.Above) != CollisionFlags.None;
	}

	public bool IsGrounded()
	{
		return grounded;
	}

	public bool TooSteep()
	{
		return !(groundNormal.y > Mathf.Cos(controller.slopeLimit * ((float)Math.PI / 180f)));
	}

	public Vector3 GetDirection()
	{
		return inputMoveDirection;
	}

	public void SetControllable(bool controllable)
	{
		canControl = controllable;
	}

	public float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		float result;
		if (desiredMovementDirection == Vector3.zero)
		{
			result = 0f;
		}
		else
		{
			float num = ((desiredMovementDirection.z <= 0f) ? movement.maxBackwardsSpeed : movement.maxForwardSpeed) / movement.maxSidewaysSpeed;
			Vector3 normalized = new Vector3(desiredMovementDirection.x, 0f, desiredMovementDirection.z / num).normalized;
			float num2 = new Vector3(normalized.x, 0f, normalized.z * num).magnitude * movement.maxSidewaysSpeed;
			result = num2;
		}
		return result;
	}

	public void SetVelocity(Vector3 velocity)
	{
		grounded = false;
		movement.velocity = velocity;
		movement.frameVelocity = Vector3.zero;
		SendMessage("OnExternalVelocity");
	}

	public void KnockBack(Vector3 force)
	{
		isKnockedBack = true;
		knockBackTimer = 0.5f;
		knockBackForce = force;
	}

	public void Main()
	{
	}
}
