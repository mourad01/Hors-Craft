// DecompilerFi decompiler from Assembly-CSharp.dll class: AllClothesItemData
using System;
using System.Xml.Serialization;
using UnityEngine;

[XmlInclude(typeof(AllClothesItemData))]
public class AllClothesItemData : PackItemData
{
	public bool grant;

	public AllClothesItemData()
	{
		type = PackItemType.AllClothes;
	}

	public override void FillWithRandom()
	{
		grant = (new System.Random(Mathf.RoundToInt(Time.time)).NextDouble() > 0.5);
	}

	public override Type GetConnectedType()
	{
		return typeof(AllClothesItem);
	}

	public override void GrantItem()
	{
		PlayerPrefs.SetInt("overrideclothesadsnumber", 1);
		PlayerPrefs.Save();
	}

	public override bool IsValid()
	{
		throw new NotImplementedException();
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
