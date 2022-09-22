// DecompilerFi decompiler from Assembly-CSharp.dll class: EnemyBumperCar
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class EnemyBumperCar : MonoBehaviour
{
	public List<ushort> blockId;

	private const float NEW_RANDOM_DIRECTION_TIME = 3f;

	private const float ROTATE_TIME = 1f;

	private Vector3 _target;

	public float moveSpeed = 30f;

	public float rotationSpeed = 5f;

	private bool hasToDoUpdate = true;

	private Rigidbody rigidbody;

	private float directionTimer;

	private float insensitiveTimer;

	private Vector3 direction = Vector3.zero;

	private bool toTarget;

	public Vector3 target
	{
		get
		{
			return _target;
		}
		set
		{
			_target = value;
			directionTimer = 0f;
			toTarget = true;
		}
	}

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (hasToDoUpdate)
		{
			DisableIfNotOnCorrectBlock();
			directionTimer += Time.deltaTime;
			insensitiveTimer -= Time.deltaTime;
			ProcessDirection();
		}
	}

	private void FixedUpdate()
	{
		if (hasToDoUpdate)
		{
			if (!direction.Equals(Vector3.zero))
			{
				rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.LookRotation(direction), Time.fixedDeltaTime * rotationSpeed);
			}
			rigidbody.AddForce(rigidbody.transform.forward * moveSpeed);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(insensitiveTimer > 0f))
		{
			directionTimer = 0f;
			insensitiveTimer = 1f;
			direction = collision.contacts[0].normal;
			direction.y = 0f;
			ResetForce();
			UnityEngine.Debug.Log("Bumper colliedr hit");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		UnityEngine.Object.Destroy(other.gameObject);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(base.transform.position, direction);
	}

	private void ProcessDirection()
	{
		if (directionTimer > 3f)
		{
			NewRandomDirection();
		}
		else if (toTarget)
		{
			Vector3 target = this.target;
			Vector3 position = base.transform.position;
			target.y = position.y;
			direction = (target - base.transform.position).normalized;
		}
	}

	private void NewRandomDirection()
	{
		direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;
		directionTimer = 0f;
		toTarget = false;
	}

	private void ResetForce()
	{
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
	}

	private void DisableIfNotOnCorrectBlock()
	{
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(base.transform.position + Vector3.down);
		if (voxelInfo == null || !blockId.Contains(voxelInfo.GetVoxel()))
		{
			DisableCar();
		}
	}

	private void DisableCar()
	{
		ResetForce();
		hasToDoUpdate = false;
	}
}
