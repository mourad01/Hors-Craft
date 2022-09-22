// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CustomMobSpawnerVoxelEvents
using Common.Managers;
using UnityEngine;

namespace Uniblocks
{
	public class CustomMobSpawnerVoxelEvents : MobsSpawnerVoxelEvents, ISpawnableVoxelEvent
	{
		public GameObject mobPrefab;

		public int countFrom = 1;

		public int countTo = 1;

		public bool randomRotation = true;

		public float rotation;

		public GameObject[] GetPrefabs()
		{
			return new GameObject[1]
			{
				mobPrefab
			};
		}

		public override void Register(ChunkData chunk, Vector3 worldPos, byte byteRotation)
		{
			Manager.Get<MobsManager>().RegisterCustomMobSpawner(chunk, mobPrefab, worldPos, countFrom, countTo, (!randomRotation) ? rotation : (-1f));
		}
	}
}
