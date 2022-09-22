// DecompilerFi decompiler from Assembly-CSharp.dll class: ClothesItemData
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[XmlInclude(typeof(ClothesItemData))]
public class ClothesItemData : PackItemData
{
	public List<int> clothesIds;

	public int count = 1;

	[CompilerGenerated]
	private static Func<string, int> _003C_003Ef__mg_0024cache0;

	public ClothesItemData()
	{
		type = PackItemType.Clothes;
	}

	public override string ToString()
	{
		return $"[ClothesItemData]: {clothesIds.ToStringPretty()}";
	}

	public override string ToParsable()
	{
		return string.Format("{0}:{1}", type, clothesIds.ToStringPretty(","));
	}

	public override void TryParseData(string data)
	{
		clothesIds = data.SplitToList(',', int.Parse);
	}

	public override Type GetConnectedType()
	{
		return typeof(ClothesItem);
	}

	public override bool IsValid()
	{
		return true;
	}

	public override void GrantItem()
	{
		clothesIds.ForEach(delegate(int id)
		{
			Singleton<PlayerData>.get.playerItems.UnlockItemById($"{id}.{BodyPart.Head}");
			Singleton<PlayerData>.get.playerItems.UnlockItemById($"{id}.{BodyPart.Body}");
			Singleton<PlayerData>.get.playerItems.UnlockItemById($"{id}.{BodyPart.Legs}");
		});
	}

	public override void FillWithRandom()
	{
		clothesIds = new List<int>();
		List<int> toRandomize = new List<int>();
		SkinList.instance.possibleSkins.ForEach(delegate(Skin skin)
		{
			bool flag = Singleton<PlayerData>.get.playerItems.IsItemUnlocked($"{skin.id}.{BodyPart.Head}");
			bool flag2 = Singleton<PlayerData>.get.playerItems.IsItemUnlocked($"{skin.id}.{BodyPart.Body}");
			bool flag3 = Singleton<PlayerData>.get.playerItems.IsItemUnlocked($"{skin.id}.{BodyPart.Legs}");
			if (!flag && !flag2 && !flag3)
			{
				toRandomize.Add(skin.id);
			}
		});
		for (int i = 0; i < count; i++)
		{
			clothesIds.Add(toRandomize.GetAndRemoveRandomItem());
		}
	}
}
