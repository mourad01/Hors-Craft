// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CustomBlocksFragment
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace States
{
	public class CustomBlocksFragment : Fragment
	{
		public Transform parent;

		public GameObject customBlock;

		private CustomBlocksFilter customBlocksFilter;

		private void Awake()
		{
			customBlocksFilter = GetComponent<CustomBlocksFilter>();
		}

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			ClearGrid();
			FillWithBlocks();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			ClearGrid();
			FillWithBlocks();
		}

		private void ClearGrid()
		{
			while (parent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(parent.GetChild(0).gameObject);
			}
		}

		private void FillWithBlocks()
		{
			List<Voxel> customBlocks = customBlocksFilter.GetCustomBlocks();
			foreach (Voxel item in customBlocks)
			{
				SpawnBlock(item);
			}
		}

		private void SpawnBlock(Voxel voxel)
		{
			GameObject gameObject = Object.Instantiate(customBlock);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			CustomBlockElement component = gameObject.GetComponent<CustomBlockElement>();
			component.Init(voxel);
		}
	}
}
