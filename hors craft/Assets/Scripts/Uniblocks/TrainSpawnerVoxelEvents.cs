// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.TrainSpawnerVoxelEvents
using UnityEngine;

namespace Uniblocks
{
	public class TrainSpawnerVoxelEvents : OneTimeSaveTransformSpawnerVoxelEvents
	{
		protected override void AfterSpawn(VoxelInfo info, GameObject spawned)
		{
			VoxelInfo voxelInfo = info.chunk.GetVoxelInfo(new Index(info.index.x, info.index.y - 1, info.index.z));
			AutoConnectionRailsVoxelEvents component = voxelInfo.GetVoxelType().GetComponent<AutoConnectionRailsVoxelEvents>();
			if (!(component == null))
			{
				Vector3[] inOutDirections = component.GetInOutDirections(voxelInfo.GetVoxelRotation());
				spawned.transform.forward = inOutDirections[0];
			}
		}
	}
}
