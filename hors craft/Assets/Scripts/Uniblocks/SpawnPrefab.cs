// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.SpawnPrefab
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public class SpawnPrefab : DefaultVoxelEvents, ISpawnableVoxelEvent
	{
		public GameObject trainPrefab;

		public Voxel[] canSpawnOnTracks;

		public void Spawn(VoxelInfo info)
		{
			Index index = new Index(info.index.x, info.index.y, info.index.z);
			ushort voxelId = info.chunk.GetVoxel(index);
			if (!canSpawnOnTracks.Any((Voxel tracks) => tracks.GetUniqueID() == voxelId))
			{
				index = new Index(info.index.x, info.index.y + 1, info.index.z);
				info.chunk.SetVoxel(index, canSpawnOnTracks[0].GetUniqueID(), updateMesh: true, 0);
			}
			Vector3 position = info.chunk.VoxelIndexToPosition(index);
			byte voxelRotation = info.chunk.GetVoxelRotation(index);
			float num = 0f;
			switch (voxelRotation)
			{
			case 0:
				num = 90f;
				break;
			case 1:
				num = 180f;
				break;
			case 2:
				num = 270f;
				break;
			default:
				num = 180f;
				break;
			}
			GameObject gameObject = Object.Instantiate(trainPrefab, position, Quaternion.identity);
			if (gameObject.transform.parent == null)
			{
				gameObject.transform.parent = Engine.PositionToChunk(position).transform;
			}
			gameObject.transform.Rotate(0f, num, 0f);
		}

		public GameObject[] GetPrefabs()
		{
			return new GameObject[1]
			{
				trainPrefab
			};
		}
	}
}
