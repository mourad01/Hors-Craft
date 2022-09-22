// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.SwitchVoxelEvents
using UnityEngine;

namespace Uniblocks
{
	public class SwitchVoxelEvents : PrefabSpawnerVoxelEvents
	{
		public GameObject voxelToSwitch;

		public void Switch(VoxelInfo info)
		{
			info.chunk.SetVoxelSimple(info.index, voxelToSwitch.GetComponent<Voxel>().GetUniqueID());
		}
	}
}
