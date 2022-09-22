// DecompilerFi decompiler from Assembly-CSharp.dll class: AllBlocksItemData
using System;
using System.Xml.Serialization;
using UnityEngine;

[XmlInclude(typeof(AllBlocksItemData))]
public class AllBlocksItemData : PackItemData
{
	public bool grant;

	public AllBlocksItemData()
	{
		type = PackItemType.AllBlocks;
	}

	public override void FillWithRandom()
	{
		grant = (new System.Random(Mathf.RoundToInt(Time.time)).NextDouble() > 0.5);
	}

	public override Type GetConnectedType()
	{
		return typeof(AllBlocksItem);
	}

	public override void GrantItem()
	{
		PlayerPrefs.SetInt("overrideblocksadsnumber", 1);
		PlayerPrefs.Save();
	}

	public override bool IsValid()
	{
		return true;
	}

	public override string ToParsable()
	{
		return $"{type}:{grant}";
	}

	public override void TryParseData(string data)
	{
		bool.TryParse(data, out grant);
	}
}
