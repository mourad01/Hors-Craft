// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.MobsSpawnerVoxelEvents
using Common.Managers;
using System.Collections;
using UnityEngine;

namespace Uniblocks
{
	public class MobsSpawnerVoxelEvents : DefaultVoxelEvents
	{
		private float timeoutAfter;

		public override void OnBlockPlace(VoxelInfo info)
		{
			StartCoroutine(RegisterWhenChunkWorks(info.chunk, info.chunk.VoxelIndexToPosition(info.index), info.GetVoxelRotation()));
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			StartCoroutine(RegisterWhenChunkWorks(chunk, chunk.VoxelIndexToPosition(x, y, z), chunk.GetVoxelInfo(x, y, z).GetVoxelRotation()));
		}

		public virtual void Register(ChunkData chunk, Vector3 worldPos, byte spawnerRotation)
		{
			Manager.Get<MobsManager>().RegisterSpawnerOn(chunk, worldPos, spawnerRotation);
		}

		protected IEnumerator RegisterWhenChunkWorks(ChunkData chunk, Vector3 worldPos, byte voxelRotation)
		{
			timeoutAfter = Time.time + 10f;
			Chunk ch = ChunkManager.Chunks[chunk.ChunkIndex];
			while (!ch.Fresh)
			{
				if (Time.time > timeoutAfter)
				{
					UnityEngine.Debug.Log("Timeout spawner");
					yield break;
				}
				yield return new WaitForSeconds(0.5f);
			}
			Register(chunk, worldPos, voxelRotation);
		}
	}
}
