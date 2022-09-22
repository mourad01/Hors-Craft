// DecompilerFi decompiler from Assembly-CSharp.dll class: BoatController
using Uniblocks;
using UnityEngine;

[RequireComponent(typeof(Boat))]
public class BoatController : MonoBehaviour
{
	public ushort blockId;

	public Vector3 neighborDirection = new Vector3(0f, -1.5f, 0f);

	public float boatHeightYOffset = 0.5f;

	private Rigidbody body;

	private Boat boat;

	private ushort voxelId;

	private Vector3 freezeVector;

	private void Awake()
	{
		boat = GetComponent<Boat>();
		body = GetComponent<Rigidbody>();
		neighborDirection.y = 0f - boatHeightYOffset - 1f;
	}

	private void Update()
	{
		DisableIfNotOnCorrectBlock();
		freezeVector = body.transform.rotation.eulerAngles;
		freezeVector.x = 0f;
		freezeVector.z = 0f;
		body.transform.rotation = Quaternion.Euler(freezeVector);
		if (boat.IsInUse)
		{
			boat.enabled = true;
			return;
		}
		boat.enabled = false;
		body.velocity = Vector3.zero;
	}

	private void DisableIfNotOnCorrectBlock()
	{
		if (boat.analogInput != null)
		{
			UpdateNeighborPosition();
		}
		try
		{
			voxelId = Engine.PositionToVoxelInfo(base.transform.position + base.transform.rotation * neighborDirection).GetVoxel();
			if (voxelId == blockId)
			{
				boat.enabled = true;
				body.useGravity = false;
			}
			else
			{
				boat.enabled = false;
				body.velocity = Vector3.zero;
			}
		}
		catch
		{
		}
	}

	private void UpdateNeighborPosition()
	{
		Vector2 vector = boat.analogInput.CalculatePosition();
		if (boat.analogInput.IsTouching())
		{
			if (vector.magnitude > 0.1f)
			{
				vector.Normalize();
			}
			else
			{
				vector = Vector2.zero;
			}
			neighborDirection.x = vector.x;
			neighborDirection.z = vector.y;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(base.transform.position + base.transform.rotation * neighborDirection, Vector3.one);
	}
}
