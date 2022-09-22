// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.PrefabSpawnerVoxelEvents
using UnityEngine;

namespace Uniblocks
{
	public class PrefabSpawnerVoxelEvents : DefaultVoxelEvents, ISpawnableVoxelEvent
	{
		[Header("Optional prefab")]
		public GameObject prefabToSpawn;

		public bool setRotation;

		public Vector3 eulerRotation;

		public GameObject prefabInstance;

		[Header("Works only with isometric placement")]
		public bool placeOnWaterSurface;

		public override void OnBlockPlace(VoxelInfo info)
		{
			if (prefabToSpawn != null)
			{
				Spawn(info);
			}
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			if (prefabToSpawn != null)
			{
				Spawn(chunk.GetVoxelInfo(x, y, z));
			}
		}

		protected virtual void Spawn(VoxelInfo voxelInfo)
		{
			ChunkData chunk = voxelInfo.chunk;
			Vector3 position = chunk.VoxelIndexToPosition(voxelInfo.index);
			prefabInstance = Object.Instantiate(prefabToSpawn, position, Quaternion.identity);
			prefabInstance.transform.parent = ChunkManager.Chunks[chunk.ChunkIndex].transform;
			InteractiveObject component = prefabInstance.GetComponent<InteractiveObject>();
			if (component != null)
			{
				component.voxelInfo = voxelInfo;
				component.SetRotation(voxelInfo.GetVoxelRotation());
				component.Init();
			}
			if (setRotation)
			{
				prefabInstance.transform.eulerAngles = eulerRotation;
			}
		}

		public GameObject[] GetPrefabs()
		{
			return new GameObject[1]
			{
				prefabToSpawn
			};
		}
	}
}
