// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemWrapperVoxel
using Uniblocks;
using UnityEngine;

public class ItemWrapperVoxel : ItemWrapperGO
{
	public override Sprite GetSprite()
	{
		return prefab.GetComponent<Voxel>().voxelSprite;
	}

	public override string GetName()
	{
		return prefab.GetComponent<Voxel>().GetUniqueKey();
	}
}
