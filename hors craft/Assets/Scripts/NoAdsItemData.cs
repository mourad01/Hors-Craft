// DecompilerFi decompiler from Assembly-CSharp.dll class: NoAdsItemData
using System;
using System.Xml.Serialization;
using TsgCommon;
using UnityEngine;

[XmlInclude(typeof(NoAdsItemData))]
public class NoAdsItemData : PackItemData
{
	public bool grant;

	public NoAdsItemData()
	{
		type = PackItemType.NoAds;
	}

	public override void FillWithRandom()
	{
		grant = (new System.Random(Mathf.RoundToInt(Time.time)).NextDouble() > 0.5);
	}

	public override Type GetConnectedType()
	{
		return typeof(NoAdsItem);
	}

	public override void GrantItem()
	{
		TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.SetNoAds(newState: true);
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
