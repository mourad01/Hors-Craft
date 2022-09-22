// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ColliderEventsSender
using UnityEngine;

namespace Uniblocks
{
	public class ColliderEventsSender : MonoBehaviour
	{
		private Index LastIndex;

		private Chunk LastChunk;

		public void FixedUpdate()
		{
			GameObject gameObject = Engine.PositionToChunk(base.transform.position);
			if (gameObject == null)
			{
				return;
			}
			Chunk component = gameObject.GetComponent<Chunk>();
			Index index = component.chunkData.PositionToVoxelIndex(base.transform.position);
			VoxelInfo voxelInfo = new VoxelInfo(index, component.chunkData);
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(voxelInfo.GetVoxel());
			if (instanceForVoxelId != null)
			{
				if (component != LastChunk || !index.IsEqual(LastIndex))
				{
					instanceForVoxelId.OnBlockEnter(base.gameObject, voxelInfo);
				}
				else
				{
					instanceForVoxelId.OnBlockStay(base.gameObject, voxelInfo);
				}
			}
			LastChunk = component;
			LastIndex = index;
		}
	}
}
