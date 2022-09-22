// DecompilerFi decompiler from Assembly-CSharp.dll class: IsometricPlaceableBoat
using Uniblocks;
using UnityEngine;

public class IsometricPlaceableBoat : IsometricPlaceableCraft
{
	protected override string GetErrorKey()
	{
		return "water.cant.place";
	}

	protected override string GetErrorDefaultText()
	{
		return "This item have to be placed on water";
	}

	protected override void Update()
	{
		base.Update();
		EnablePlacement(CheckIfValid());
	}

	protected override Vector3 MoveFromCollisions(Vector3 starPos)
	{
		bool flag = false;
		Vector3 vector = Vector3.one;
		while (!flag)
		{
			vector += Vector3.up;
			Bounds bounds = GetBounds();
			bounds.center += starPos + vector;
			if (Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents + Vector3.down * 0.5f, colliders, Quaternion.identity) == 0)
			{
				flag = true;
			}
			else
			{
				previousPosition = base.transform.position;
			}
		}
		bool flag2 = false;
		while (!flag2)
		{
			vector += Vector3.up;
			ushort voxel = Engine.PositionToVoxelInfo(objInstance.transform.position + vector).GetVoxel();
			if (voxel != Engine.usefulIDs.waterBlockID)
			{
				flag2 = true;
			}
			else
			{
				previousPosition = base.transform.position;
			}
		}
		return vector;
	}

	protected override void SnapToGround()
	{
		base.transform.position = Snap(base.transform.position);
	}

	public override bool OnPlace()
	{
		PlaceCraft();
		PlacePlayer();
		DestroyObjects();
		return true;
	}

	protected override void PlaceCraft()
	{
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(objInstance.transform.position);
		if (voxelInfo != null)
		{
			Voxel.PlaceBlock(voxelInfo, ExampleInventory.HeldBlock);
		}
	}

	private bool CheckIfValid()
	{
		Bounds bounds = GetBounds();
		for (int i = 0; i < 2; i++)
		{
			Vector3 a = objInstance.transform.position + bounds.center;
			Vector3 down = Vector3.down;
			Vector3 extents = bounds.extents;
			Vector3 position = a + down * extents.y + i * Vector3.down;
			ushort voxel = Engine.PositionToVoxelInfo(position).GetVoxel();
			if (Engine.usefulIDs.waterBlockID == voxel)
			{
				return true;
			}
		}
		return false;
	}

	protected override void PlacePlayer()
	{
		if (!(playerTransform.parent != null))
		{
			playerTransform.position = objInstance.transform.position + Vector3.up * 5f;
		}
	}
}
