// DecompilerFi decompiler from Assembly-CSharp.dll class: CurrencyModule
using Common.Managers;
using Common.Model;

public class CurrencyModule : ModelModule
{
	private string keySoftCurrencyPerAd()
	{
		return "currency.soft.per.ad";
	}

	private string keySoftCurrencyPerAdPerLevel()
	{
		return "currency.soft.per.ad.per.level";
	}

	private string keyCurrencyPrice(string currencyId, string productId)
	{
		return "currency." + currencyId + ".price." + productId;
	}

	private string keyCurrencyReward(string currencyId, string productId)
	{
		return "currency." + currencyId + ".reward." + productId;
	}

	private string keyHardCurrencyPerPurchase()
	{
		return "currency.hard.per.purchase";
	}

	private string keyHardCurrencyPerAd()
	{
		return "currency.hard.per.ad";
	}

	private string keyGetFreeSoftCooldown()
	{
		return "currency.soft.ad.cooldown";
	}

	private string keyGetFreeSoftMaxCount()
	{
		return "currency.soft.max.ads";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keySoftCurrencyPerAd(), 50);
		descriptions.AddDescription(keySoftCurrencyPerAdPerLevel(), 0);
		descriptions.AddDescription(keyHardCurrencyPerPurchase(), 10);
		descriptions.AddDescription(keyHardCurrencyPerAd(), 25);
		descriptions.AddDescription(keyGetFreeSoftCooldown(), 0);
		descriptions.AddDescription(keyGetFreeSoftMaxCount(), 5);
	}

	public override void OnModelDownloaded()
	{
	}

	public int SoftCurrencyPerAd(int level = 1)
	{
		int @int = base.settings.GetInt(keySoftCurrencyPerAd());
		int int2 = base.settings.GetInt(keySoftCurrencyPerAdPerLevel());
		return @int + level * int2;
	}

	public int GetSoftPriceFor(string id, int defaultPrice = 0)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyCurrencyPrice("soft", id), defaultPrice);
	}

	public int GetHardPriceFor(string id, int defaultPrice = 0)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyCurrencyPrice("hard", id), defaultPrice);
	}

	public int GetSoftRewardFor(string id, int defaultReward = 10)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyCurrencyReward("soft", id), defaultReward);
	}

	public int GetHardRewardFor(string id, int defaultReward = 10)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyCurrencyReward("hard", id), defaultReward);
	}

	public int GetPriceFor(string currencyId, string productId, int defaultPrice)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyCurrencyPrice(currencyId, productId), defaultPrice);
	}

	public int GetRewardFor(string currencyId, string productId, int defaultReward)
	{
		return (int)ModelSettingsHelper.GetFloatFromStringSettings(base.settings, keyCurrencyReward(currencyId, productId), defaultReward);
	}

	public int GetHardCurrencyPerPurchase()
	{
		return base.settings.GetInt(keyHardCurrencyPerPurchase());
	}

	public int GetHardCurrencyPerAd()
	{
		return base.settings.GetInt(keyHardCurrencyPerAd());
	}

	public int GetFreeSoftCooldown()
	{
		return base.settings.GetInt(keyGetFreeSoftCooldown());
	}

	public int GetFreeSoftMaxCount()
	{
		return base.settings.GetInt(keyGetFreeSoftMaxCount());
	}
}
