// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.OneTimeSpawnerEvents
using UnityEngine;

namespace Uniblocks
{
	public class OneTimeSpawnerEvents : VoxelEvents, ISpawnableVoxelEvent
	{
		public GameObject prefab;

		protected GameObject instance;

		protected Vector3 worldPos;

		public GameObject[] GetPrefabs()
		{
			return new GameObject[1]
			{
				prefab
			};
		}

		public override void OnBlockPlace(VoxelInfo voxelInfo)
		{
			Spawn(voxelInfo);
		}

		protected virtual void Spawn(VoxelInfo voxelInfo)
		{
			worldPos = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
			instance = Object.Instantiate(prefab);
			voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: false, 0);
		}
	}
}
