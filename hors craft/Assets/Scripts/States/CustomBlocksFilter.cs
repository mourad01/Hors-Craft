// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CustomBlocksFilter
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace States
{
	public abstract class CustomBlocksFilter : MonoBehaviour
	{
		public abstract List<Voxel> GetCustomBlocks();
	}
}
