// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.PlaytimeHumanMobSpawnerVoxelEvents
using UnityEngine;

namespace Uniblocks
{
	public class PlaytimeHumanMobSpawnerVoxelEvents : DefaultVoxelEvents, ISpawnableVoxelEvent
	{
		public GameObject prefabToSpawn;

		public GameObject[] GetPrefabs()
		{
			return new GameObject[1]
			{
				prefabToSpawn
			};
		}

		public override void OnBlockPlace(VoxelInfo info)
		{
			if (prefabToSpawn != null)
			{
				Spawn(info.chunk, info.index.x, info.index.y, info.index.z, info);
			}
		}

		private void Spawn(ChunkData chunk, int x, int y, int z, VoxelInfo voxelInfo)
		{
			Vector3 position = chunk.VoxelIndexToPosition(x, y, z);
			position.y += 0.5f;
			GameObject gameObject = Object.Instantiate(prefabToSpawn, position, Quaternion.identity);
			SaveTransform component = gameObject.GetComponent<SaveTransform>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			InteractiveObject component2 = gameObject.GetComponent<InteractiveObject>();
			if (component2 != null)
			{
				component2.voxelInfo = voxelInfo;
				component2.SetRotation(voxelInfo.GetVoxelRotation());
			}
			gameObject.GetComponentInChildren<HumanMob>().Init(new HumanMob.HumanParameters(Random.value));
		}
	}
}
