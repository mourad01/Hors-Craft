// DecompilerFi decompiler from Assembly-CSharp.dll class: AirStrikeKit.FlightSystem
using UnityEngine;

namespace AirStrikeKit
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class FlightSystem : MonoBehaviour
	{
		public float AccelerationSpeed = 0.5f;

		public float Speed = 50f;

		public float SpeedMax = 60f;

		public float RotationSpeed = 10f;

		public float SpeedTakeoff = 40f;

		public float SpeedPitch = 2f;

		public float SpeedRoll = 3f;

		public float SpeedYaw = 1f;

		public float DampingTarget = 10f;

		public bool AutoPilot;

		private float MoveSpeed;

		public float VelocitySpeed;

		[HideInInspector]
		public bool SimpleControl;

		[HideInInspector]
		public bool FollowTarget;

		[HideInInspector]
		public Vector3 PositionTarget = Vector3.zero;

		private Vector3 positionTarget = Vector3.zero;

		private Quaternion mainRot = Quaternion.identity;

		[HideInInspector]
		public float roll;

		[HideInInspector]
		public float pitch;

		[HideInInspector]
		public float yaw;

		public Vector2 LimitAxisControl = new Vector2(2f, 1f);

		public bool FixedX;

		public bool FixedY;

		public bool FixedZ;

		public bool IsLanding;

		[HideInInspector]
		public float yMul = 5f;

		private float gravityVelocity;

		private Rigidbody rigidBody;

		[HideInInspector]
		public float normalFlySpeed;

		private float speedDelta;

		private void Start()
		{
			mainRot = base.transform.rotation;
			MoveSpeed = Speed;
			rigidBody = GetComponent<Rigidbody>();
			rigidBody.velocity = Vector3.zero;
		}

		private float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
		{
			float num = Vector3.Angle(a, b);
			float num2 = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));
			return num * num2;
		}

		private void FixedUpdate()
		{
			if (!rigidBody)
			{
				return;
			}
			Quaternion identity = Quaternion.identity;
			Vector3 a = Vector3.zero;
			normalFlySpeed = Mathf.Clamp(VelocitySpeed / (SpeedTakeoff * 2f), 0f, 1f);
			if (AutoPilot)
			{
				if (FollowTarget)
				{
					Vector3 vector = PositionTarget - base.transform.position;
					float num = Vector3.Dot(base.transform.forward.normalized, vector.normalized);
					float num2 = 1f;
					if (num > 0.3f)
					{
						num2 = 1f;
						positionTarget = Vector3.Lerp(positionTarget, PositionTarget, Time.fixedDeltaTime * DampingTarget);
					}
					else
					{
						Vector3 b = Vector3.Reflect(vector.normalized, base.transform.forward);
						b.x *= 0.3f;
						b.y *= 0.5f;
						positionTarget = Vector3.Slerp(positionTarget, base.transform.position + b, Time.fixedDeltaTime * DampingTarget);
						num2 = 0.5f;
						if ((double)num < -0.9)
						{
							positionTarget.y += 0.1f;
						}
					}
					mainRot = Quaternion.LookRotation(positionTarget - base.transform.position);
					Vector3 normalized = base.transform.InverseTransformPoint(positionTarget).normalized;
					rigidBody.rotation = Quaternion.Lerp(rigidBody.rotation, mainRot, num2 * 0.1f * RotationSpeed * Time.fixedDeltaTime);
					rigidBody.rotation *= Quaternion.Euler(0f, 0f, (0f - normalized.x) * 150f * SpeedRoll * Time.fixedDeltaTime);
					mainRot = ConstraintRotation(mainRot);
				}
				a = rigidBody.rotation * Vector3.forward * VelocitySpeed;
			}
			else
			{
				if (!IsLanding)
				{
					identity.eulerAngles = new Vector3(pitch + (1f - normalFlySpeed) * 0.5f, yaw, 0f - roll);
				}
				else
				{
					identity.eulerAngles = new Vector3(pitch, yaw, 0f - roll);
				}
				mainRot *= identity;
				if (SimpleControl)
				{
					Quaternion b2 = mainRot;
					Vector3 eulerAngles = mainRot.eulerAngles;
					float x = eulerAngles.x;
					Vector3 eulerAngles2 = mainRot.eulerAngles;
					float y = eulerAngles2.y;
					Vector3 eulerAngles3 = mainRot.eulerAngles;
					Vector3 eulerAngles4 = new Vector3(x, y, eulerAngles3.z);
					if (FixedX)
					{
						eulerAngles4.x = 1f;
					}
					if (FixedY)
					{
						eulerAngles4.y = 1f;
					}
					if (FixedZ)
					{
						eulerAngles4.z = 1f;
					}
					b2.eulerAngles = eulerAngles4;
					mainRot = Quaternion.Lerp(mainRot, b2, Time.fixedDeltaTime * 2f);
				}
				a = rigidBody.rotation * Vector3.forward * VelocitySpeed;
				rigidBody.rotation = mainRot;
				mainRot = ConstraintRotation(mainRot);
			}
			if (IsLanding)
			{
				Quaternion b3 = mainRot;
				Vector3 eulerAngles5 = mainRot.eulerAngles;
				float x2 = eulerAngles5.x;
				Vector3 eulerAngles6 = mainRot.eulerAngles;
				float y2 = eulerAngles6.y;
				Vector3 eulerAngles7 = mainRot.eulerAngles;
				Vector3 eulerAngles8 = new Vector3(x2, y2, eulerAngles7.z);
				eulerAngles8.x = 1f;
				eulerAngles8.z = 1f;
				gravityVelocity = 0f;
				b3.eulerAngles = eulerAngles8;
				mainRot = Quaternion.Lerp(mainRot, b3, Time.fixedDeltaTime * 2f);
			}
			else if (rigidBody.useGravity)
			{
				float num3 = gravityVelocity;
				Vector3 gravity = Physics.gravity;
				gravityVelocity = num3 + gravity.y * (1f - Mathf.Clamp(VelocitySpeed / (SpeedTakeoff * 2f), 0f, 1f) + Vector3.Dot(Physics.gravity.normalized, a.normalized + Vector3.up * 0.5f)) * Time.fixedDeltaTime;
				gravityVelocity = Mathf.Clamp(gravityVelocity, float.MinValue, 0f);
				a.y += gravityVelocity;
			}
			yaw = Mathf.Lerp(yaw, 0f, Time.deltaTime * yMul);
			Vector3 force = a - rigidBody.velocity;
			rigidBody.AddForce(force, ForceMode.VelocityChange);
			if (IsLanding)
			{
				roll = Mathf.Lerp(roll, 0f, 0.5f);
				VelocitySpeed = MoveSpeed;
				if (speedDelta < 1f)
				{
					MoveSpeed = Mathf.Lerp(MoveSpeed, 0f, Time.fixedDeltaTime * 0.2f);
				}
			}
			else
			{
				VelocitySpeed = Speed + MoveSpeed;
				if (speedDelta < 1f)
				{
					MoveSpeed = Mathf.Lerp(MoveSpeed, Speed, Time.fixedDeltaTime * 0.1f);
				}
			}
			IsLanding = false;
			mainRot = ConstraintRotation(mainRot);
			positionTarget.y = Mathf.Clamp(positionTarget.y, 60f, 200f);
		}

		private Quaternion ConstraintRotation(Quaternion rot)
		{
			Vector3 eulerAngles = rot.eulerAngles;
			eulerAngles = new Vector3(Mathf.Clamp(eulerAngles.x, -30f, 30f), eulerAngles.y, eulerAngles.z);
			return Quaternion.Euler(eulerAngles);
		}

		public void Landing()
		{
			IsLanding = true;
		}

		public void AxisControl(Vector2 axis)
		{
			if (SimpleControl)
			{
				LimitAxisControl.y = LimitAxisControl.x;
			}
			if (!IsLanding)
			{
				roll = Mathf.Clamp(axis.x, 0f - LimitAxisControl.x, LimitAxisControl.x) * SpeedRoll;
			}
			if (VelocitySpeed > SpeedTakeoff || !IsLanding)
			{
				pitch = Mathf.Clamp(axis.y, 0f - LimitAxisControl.y, LimitAxisControl.y) * SpeedPitch;
			}
		}

		public void TurnControl(float turn)
		{
			yaw += turn * Time.deltaTime * SpeedYaw;
		}

		public void SpeedUp(float delta)
		{
			if (delta < 0f)
			{
				delta = 0f;
			}
			if (delta > 0f)
			{
				MoveSpeed = Mathf.Clamp(Mathf.Lerp(MoveSpeed, SpeedMax, Time.deltaTime * AccelerationSpeed), 0f, SpeedMax);
			}
			speedDelta = delta;
		}

		public void SpeedUp()
		{
			MoveSpeed = Mathf.Clamp(Mathf.Lerp(MoveSpeed, SpeedMax, Time.deltaTime * AccelerationSpeed), 0f, SpeedMax);
			speedDelta = 1f;
		}
	}
}
