// DecompilerFi decompiler from Assembly-CSharp.dll class: ClothesSettingsModule
using Common.Managers;
using Common.Model;

public class ClothesSettingsModule : ModelModule
{
	private string keyClothesEnabled()
	{
		return "clothes.enabled";
	}

	private string keyClothesPopupEnabled()
	{
		return "clothes.popup.enabled";
	}

	private string keyClothesPerAds()
	{
		return "clothes.adsUnlock";
	}

	private string keyFreeClothes()
	{
		return "clothes.freeItemsOnStart";
	}

	private string keyModelOfClothesUnlock()
	{
		return "clothes.unlocking.type";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyClothesEnabled(), 1);
		descriptions.AddDescription(keyClothesPopupEnabled(), 1);
		descriptions.AddDescription(keyClothesPerAds(), 2);
		descriptions.AddDescription(keyFreeClothes(), 2);
		descriptions.AddDescription(keyModelOfClothesUnlock(), 1);
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetFreeClothes()
	{
		if (base.contextIAP.isAdsFree)
		{
			return 9999;
		}
		return base.settings.GetInt(keyFreeClothes());
	}

	public ItemsUnlockModel GetUnlockType()
	{
		return (ItemsUnlockModel)base.settings.GetInt(keyModelOfClothesUnlock(), 0);
	}

	public bool GetClothesEnabled()
	{
		return (base.settings.GetInt(keyClothesEnabled()) != 0) ? true : false;
	}

	public bool GetClothesPopupEnabled()
	{
		return (base.settings.GetInt(keyClothesPopupEnabled()) != 0) ? true : false;
	}

	public int GetClothesPerAds()
	{
		return base.settings.GetInt(keyClothesPerAds());
	}
}
