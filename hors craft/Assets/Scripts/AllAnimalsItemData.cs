// DecompilerFi decompiler from Assembly-CSharp.dll class: AllAnimalsItemData
using System;
using System.Xml.Serialization;
using UnityEngine;

[XmlInclude(typeof(AllAnimalsItemData))]
public class AllAnimalsItemData : PackItemData
{
	public bool grant;

	public AllAnimalsItemData()
	{
		type = PackItemType.AllAnimals;
	}

	public override void FillWithRandom()
	{
		grant = (new System.Random(Mathf.RoundToInt(Time.time)).NextDouble() > 0.5);
	}

	public override Type GetConnectedType()
	{
		return typeof(AllAnimalsItem);
	}

	public override void GrantItem()
	{
		PlayerPrefs.SetInt("overridepetsadsnumber", 1);
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
