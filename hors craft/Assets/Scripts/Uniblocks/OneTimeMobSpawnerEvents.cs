// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.OneTimeMobSpawnerEvents
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class OneTimeMobSpawnerEvents : OneTimeSpawnerEvents
	{
		public MobsManager.MobSpawnConfig spawnConfig;

		protected override void Spawn(VoxelInfo voxelInfo)
		{
			Vector3 pos = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
			spawnConfig = Engine.GetVoxelType(voxelInfo.GetVoxel()).GetComponent<OneTimeMobSpawnerEvents>().spawnConfig;
			Manager.Get<MobsManager>().PlanSpawn(new MobsManager.MobSpawnConfig(prefab), pos);
			voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: true, 0);
		}
	}
}
