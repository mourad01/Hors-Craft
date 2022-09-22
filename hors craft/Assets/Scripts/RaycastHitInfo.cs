// DecompilerFi decompiler from Assembly-CSharp.dll class: RaycastHitInfo
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public struct RaycastHitInfo
	{
		public RaycastHit hit;

		public VoxelInfo voxelHit;

		public List<VoxelInfo> gridRaycast;

		public VoxelInfo gridRaycastHit
		{
			get
			{
				if (gridRaycast == null || gridRaycast.Count == 0)
				{
					return null;
				}
				return gridRaycast.FirstOrDefault((VoxelInfo gr) => gr.GetVoxel() != 0);
			}
		}

		public VoxelInfo gridRaycastHitNoWater
		{
			get
			{
				if (gridRaycast == null || gridRaycast.Count == 0)
				{
					return null;
				}
				return gridRaycast.FirstOrDefault((VoxelInfo gr) => gr.GetVoxel() != 0 && gr.GetVoxel() != 12);
			}
		}
	}
}
