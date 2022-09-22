// DecompilerFi decompiler from Assembly-CSharp.dll class: BlocksItemData
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Uniblocks;

[XmlInclude(typeof(BlocksItemData))]
public class BlocksItemData : PackItemData
{
	public int count = 5;

	public List<ushort> blocks;

	[CompilerGenerated]
	private static Func<string, ushort> _003C_003Ef__mg_0024cache0;

	public BlocksItemData()
	{
		type = PackItemType.Blocks;
	}

	public override string ToString()
	{
		return $"[BlocksItemData]: {blocks.ToStringPretty()}";
	}

	public override Type GetConnectedType()
	{
		return typeof(BlocksItem);
	}

	public override bool IsValid()
	{
		return true;
	}

	public override string ToParsable()
	{
		return string.Format("{0}:{1}", type, blocks.ToStringPretty(","));
	}

	public override void TryParseData(string data)
	{
		blocks = data.SplitToList(',', ushort.Parse);
	}

	public override void GrantItem()
	{
		blocks.ForEach(delegate(ushort id)
		{
			Singleton<PlayerData>.get.playerItems.OnBlockUnlock(id);
		});
	}

	public override void FillWithRandom()
	{
		blocks = new List<ushort>();
		List<Voxel.Category> list = new List<Voxel.Category>();
		list.Add(Voxel.Category.basic);
		list.Add(Voxel.Category.organic);
		list.Add(Voxel.Category.furniture);
		list.Add(Voxel.Category.custom);
		List<Voxel.Category> list2 = list;
		List<Voxel> list3 = new List<Voxel>();
		Voxel[] array = Engine.Blocks;
		foreach (Voxel voxel in array)
		{
			if (!(voxel == null) && list2.Contains(voxel.blockCategory) && !voxel.editorOnly && !Singleton<PlayerData>.get.playerItems.IsBlockUnlocked(voxel.GetUniqueID()))
			{
				list3.Add(voxel);
			}
		}
		for (int j = 0; j < count; j++)
		{
			blocks.Add(list3.GetAndRemoveRandomItem().GetUniqueID());
		}
	}
}
