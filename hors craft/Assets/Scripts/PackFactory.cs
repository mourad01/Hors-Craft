// DecompilerFi decompiler from Assembly-CSharp.dll class: PackFactory
using UnityEngine;

public class PackFactory
{
	public static PackItemData GetData(PackItemType type)
	{
		switch (type)
		{
		case PackItemType.Blocks:
			return new BlocksItemData();
		case PackItemType.Clothes:
			return new ClothesItemData();
		case PackItemType.GoldDoubler:
			return new GoldDoublerItemData();
		case PackItemType.SoftCurrency:
			return new SoftCurrencyItemData();
		case PackItemType.WorldItem:
			return new WorldItemData();
		case PackItemType.AllBlocks:
			return new AllBlocksItemData();
		case PackItemType.AllAnimals:
			return new AllAnimalsItemData();
		case PackItemType.AllSchematics:
			return new AllSchematicsItemData();
		case PackItemType.AllClothes:
			return new AllClothesItemData();
		case PackItemType.AditionalBlocks:
			return new AditionalBlocksItemData();
		case PackItemType.NoAds:
			return new NoAdsItemData();
		default:
			return null;
		}
	}

	public static PackItem GetGameobjectFromData(PackItemData data)
	{
		GameObject gameObject = new GameObject("PackItem");
		return gameObject.AddComponent(data.GetConnectedType()) as PackItem;
	}

	public static PackItemData ParseData(string data)
	{
		string[] array = data.Split(':');
		PackItemType type = array[0].ToEnum<PackItemType>();
		PackItemData data2 = GetData(type);
		if (data2 == null)
		{
			return null;
		}
		data2.TryParseData(array[1]);
		return data2;
	}
}
