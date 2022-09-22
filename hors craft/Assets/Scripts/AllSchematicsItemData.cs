// DecompilerFi decompiler from Assembly-CSharp.dll class: AllSchematicsItemData
using System;
using System.Xml.Serialization;
using UnityEngine;

[XmlInclude(typeof(AllSchematicsItemData))]
public class AllSchematicsItemData : PackItemData
{
	public bool grant;

	public AllSchematicsItemData()
	{
		type = PackItemType.AllSchematics;
	}

	public override void FillWithRandom()
	{
		grant = (new System.Random(Mathf.RoundToInt(Time.time)).NextDouble() > 0.5);
	}

	public override Type GetConnectedType()
	{
		return typeof(AllSchematicsItem);
	}

	public override void GrantItem()
	{
		PlayerPrefs.SetInt("overrideschematicsadsnumber", 1);
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
