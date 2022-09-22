// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldItemData
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlInclude(typeof(WorldItemData))]
public class WorldItemData : PackItemData
{
	public string uniqueItemId;

	public WorldItemData()
	{
		type = PackItemType.WorldItem;
	}

	public override string ToString()
	{
		return $"[WorldItemData]: {uniqueItemId}";
	}

	public override void TryParseData(string data)
	{
		uniqueItemId = data;
	}

	public override string ToParsable()
	{
		return $"{type}:{uniqueItemId}";
	}

	public override Type GetConnectedType()
	{
		return typeof(WorldItem);
	}

	public override void GrantItem()
	{
		Singleton<PlayerData>.get.playerWorlds.OnWorldBought(uniqueItemId);
	}

	public override bool IsValid()
	{
		return !string.IsNullOrEmpty(uniqueItemId);
	}

	public override void FillWithRandom()
	{
		List<WorldData> allWorlds = Manager.Get<SavedWorldManager>().GetAllWorlds();
		allWorlds.RemoveAll((WorldData world) => Singleton<PlayerData>.get.playerWorlds.IsWorldBought(world.uniqueId));
		if (allWorlds.Count == 0)
		{
			uniqueItemId = string.Empty;
			return;
		}
		allWorlds.Sort((WorldData a, WorldData b) => b.cost - a.cost);
		uniqueItemId = allWorlds[0].uniqueId;
	}
}
