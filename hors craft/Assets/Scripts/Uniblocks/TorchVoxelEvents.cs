// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.TorchVoxelEvents
using UnityEngine;

namespace Uniblocks
{
	public class TorchVoxelEvents : DefaultVoxelEvents
	{
		public GameObject torchLightPrefab;

		public override void OnBlockPlace(VoxelInfo info)
		{
			Vector3 position = info.chunk.VoxelIndexToPosition(info.index);
			GameObject gameObject = Object.Instantiate(torchLightPrefab, position, Quaternion.identity);
			gameObject.transform.parent = Engine.PositionToChunk(position).transform;
		}

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			Vector3 position = chunk.VoxelIndexToPosition(x, y, z);
			GameObject gameObject = Object.Instantiate(torchLightPrefab, position, Quaternion.identity);
			gameObject.transform.parent = Engine.PositionToChunk(position).transform;
		}
	}
}
