// DecompilerFi decompiler from Assembly-CSharp.dll class: GoldDoublerItemData
using Common.Managers;
using System;
using System.Xml.Serialization;

[XmlInclude(typeof(GoldDoublerItemData))]
public class GoldDoublerItemData : PackItemData
{
	public float value = 2f;

	public GoldDoublerItemData()
	{
		type = PackItemType.GoldDoubler;
	}

	public override string ToString()
	{
		return $"[GoldDoublerItemData]: {value}";
	}

	public override void TryParseData(string data)
	{
		value = float.Parse(data);
	}

	public override string ToParsable()
	{
		return $"{type}:{value}";
	}

	public override bool IsValid()
	{
		return true;
	}

	public override Type GetConnectedType()
	{
		return typeof(GoldDoublerItem);
	}

	public override void GrantItem()
	{
		Manager.Get<DailyChestManager>().SetDailyChestMultiplier(value);
	}

	public override void FillWithRandom()
	{
	}
}
