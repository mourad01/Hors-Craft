// DecompilerFi decompiler from Assembly-CSharp.dll class: SoftCurrencyItemData
using Common.Managers;
using System;
using System.Xml.Serialization;

[XmlInclude(typeof(SoftCurrencyItemData))]
public class SoftCurrencyItemData : PackItemData
{
	public int value;

	public SoftCurrencyItemData()
	{
		type = PackItemType.SoftCurrency;
	}

	public override string ToString()
	{
		return $"[SoftCurrencyItemData]: {value}";
	}

	public override void TryParseData(string data)
	{
		value = int.Parse(data);
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
		return typeof(SoftCurrencyItem);
	}

	public override void GrantItem()
	{
		Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(value);
	}

	public override void FillWithRandom()
	{
		PackageData packageData = Singleton<UltimateCraftModelDownloader>.get.GetIapPackages().Find((PackageData package) => package.iapIdentifier.Contains("medium"));
		if (packageData == null)
		{
			value = 5000;
		}
		else
		{
			value = packageData.currencyCount;
		}
	}
}
