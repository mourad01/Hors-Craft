// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.OneTimeVehicleSpawnerEvents
using UnityEngine;

namespace Uniblocks
{
	public class OneTimeVehicleSpawnerEvents : OneTimeSpawnerEvents
	{
		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			base.OnBlockPlace(voxelInfo);
			GameObject instance = base.instance;
			instance.GetComponentInChildren<VehicleController>().InitAfterSpawnByPlayer(ExampleInventory.HeldBlock);
			instance.name = "alfa";
			instance.transform.position = worldPos.Add(0f, 2f, 0f);
			voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: false, 0);
		}
	}
}
