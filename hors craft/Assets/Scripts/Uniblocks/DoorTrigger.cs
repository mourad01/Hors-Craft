// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.DoorTrigger
using UnityEngine;

namespace Uniblocks
{
	public class DoorTrigger : MonoBehaviour
	{
		private VoxelInfo voxelInfo;

		public void AssignVoxelInfo(VoxelInfo info)
		{
			voxelInfo = info;
		}

		public VoxelInfo GetVoxelInfo()
		{
			return voxelInfo;
		}
	}
}
