// DecompilerFi decompiler from Assembly-CSharp.dll class: PetsSettingsModule
using Common.Managers;
using Common.Model;

public class PetsSettingsModule : ModelModule
{
	private string keyPetsEnabled()
	{
		return "pets.enabled";
	}

	private string keyPetsPerAds()
	{
		return "pets.adsUnlock";
	}

	private string keyFreePets()
	{
		return "pets.freeItemsOnStart";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyPetsEnabled(), 1);
		descriptions.AddDescription(keyPetsPerAds(), 1);
		descriptions.AddDescription(keyFreePets(), 3);
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetFreePets()
	{
		if (base.contextIAP.isAdsFree)
		{
			return 9999;
		}
		return base.settings.GetInt(keyFreePets());
	}

	public bool GetPetsEnabled()
	{
		return (base.settings.GetInt(keyPetsEnabled()) != 0) ? true : false;
	}

	public int GetPetsPerAds()
	{
		return base.settings.GetInt(keyPetsPerAds());
	}
}
