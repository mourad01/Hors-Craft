// DecompilerFi decompiler from Assembly-CSharp.dll class: VoxelListDataProvider
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class VoxelListDataProvider : ScrollableListDataProvider
{
	private List<GameObject> voxelList;

	public override void Init(ScrollableListRawContent content)
	{
		voxelList = (content as VoxelListRawContent).voxels;
	}

	public override List<ScrollableListElement> PrepareData()
	{
		List<ScrollableListElement> list = new List<ScrollableListElement>();
		foreach (GameObject voxel in voxelList)
		{
			VoxelDataElement voxelDataElement = ConstructDataElement(voxel.GetComponent<Voxel>());
			if (voxelDataElement != null)
			{
				list.Add(voxelDataElement);
			}
		}
		return list;
	}

	private VoxelDataElement ConstructDataElement(Voxel voxel)
	{
		ItemConfig.Item item = Manager.Get<ItemConfig>().GetItem(voxel.GetUniqueKey());
		VoxelDataElement voxelDataElement = new VoxelDataElement();
		voxelDataElement.icon = voxel.voxelSprite;
		voxelDataElement.configItem = item;
		voxelDataElement.voxel = voxel;
		voxelDataElement.name = voxel.GetUniqueKey();
		return voxelDataElement;
	}
}
