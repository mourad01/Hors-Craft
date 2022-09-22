// DecompilerFi decompiler from Assembly-CSharp.dll class: HospitalModule
using Common.Managers;
using Common.Model;

public class HospitalModule : ModelModule
{
	private string keyMedWidht()
	{
		return "hospital.minigame.med.width";
	}

	private string keySuperstarPatientChance()
	{
		return "hospital.superstar.patient.chance";
	}

	private string keySuperstarBonusMultiplier()
	{
		return "hospital.superstar.bonus.multilpier";
	}

	private string keyPatientSickNextTime()
	{
		return "hospital.patient.sick.nexttime";
	}

	private string keyPatientSickTimer()
	{
		return "hospital.patient.sick.timer";
	}

	private string keyPatientHealthDrop()
	{
		return "hospital.patient.health.drop";
	}

	private string keyPatientGoldValue(int index)
	{
		return "hospital.patient.gold.value." + index;
	}

	private string keyPatientPrestigeValue(int index)
	{
		return "hospital.patient.prestige.value." + index;
	}

	private string keyPatientItemNeededToHeal(int index)
	{
		return "hospital.patient.item.needed.to.heal." + index;
	}

	private string keyBaseCoinsFromRewardedAd()
	{
		return "hospital.get.coins.rewarded.amount";
	}

	private string keyMiniGameStartProgress()
	{
		return "hospital.minigame.start.progress";
	}

	private string keyMiniGameAddingSpeed()
	{
		return "hospital.minigame.adding.speed";
	}

	private string keyMiniGameLosingSpeed()
	{
		return "hospital.minigame.losing.speed";
	}

	private string keyItemUpgradeBonus()
	{
		return "hospital.item.upgrade.bonus";
	}

	private string keyItemUpgradePriceMultiplier()
	{
		return "hospital.item.upgrade.price.multiplier";
	}

	private string keyItemBasePrice(int index)
	{
		return "hospital.item.base.price." + index;
	}

	private string keyItemBaseUpgradePrice(int index)
	{
		return "hospital.item.base.upgrade.price." + index;
	}

	private string keyItemPrestigeLevelRequirement(int index)
	{
		return "hospital.item.prestige.level.requirement." + index;
	}

	private string keyItemPrestigeOnUnlock(int index)
	{
		return "hospital.item.prestige.on.unlock." + index;
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyMedWidht(), 275f);
		descriptions.AddDescription(keySuperstarPatientChance(), 0.05f);
		descriptions.AddDescription(keySuperstarBonusMultiplier(), 10);
		descriptions.AddDescription(keyPatientSickNextTime(), 0.75f);
		descriptions.AddDescription(keyPatientSickTimer(), 1f);
		descriptions.AddDescription(keyPatientHealthDrop(), 1f);
		descriptions.AddDescription(keyBaseCoinsFromRewardedAd(), 50);
		descriptions.AddDescription(keyMiniGameStartProgress(), 25f);
		descriptions.AddDescription(keyMiniGameAddingSpeed(), 0.55f);
		descriptions.AddDescription(keyMiniGameLosingSpeed(), 0.7f);
		descriptions.AddDescription(keyItemUpgradeBonus(), 0.1f);
		descriptions.AddDescription(keyItemUpgradePriceMultiplier(), 1.5f);
		if (Manager.Contains<HospitalManager>())
		{
			HospitalManager hospitalManager = Manager.Get<HospitalManager>();
			for (int i = 0; i < hospitalManager.patients.Length; i++)
			{
				descriptions.AddDescription(keyPatientGoldValue(i), hospitalManager.patients[i].goldValue);
				descriptions.AddDescription(keyPatientPrestigeValue(i), hospitalManager.patients[i].prestigeValue);
				descriptions.AddDescription(keyPatientItemNeededToHeal(i), hospitalManager.patients[i].upgradeIdNeededToHeal);
			}
			for (int j = 0; j < hospitalManager.upgrades.Length; j++)
			{
				descriptions.AddDescription(keyItemBasePrice(j), hospitalManager.upgrades[j].basePrice);
				descriptions.AddDescription(keyItemBaseUpgradePrice(j), hospitalManager.upgrades[j].baseUpgradePrice);
				descriptions.AddDescription(keyItemPrestigeLevelRequirement(j), hospitalManager.upgrades[j].prestigeRequirement);
				descriptions.AddDescription(keyItemPrestigeOnUnlock(j), hospitalManager.upgrades[j].prestigeToAddOnUnlock);
			}
		}
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<HospitalManager>())
		{
			Manager.Get<HospitalManager>().LoadSettingsFromModel();
		}
	}

	public float GetMedWith()
	{
		return base.settings.GetFloat(keyMedWidht(), 145f);
	}

	public float GetSuperstarPatientChance()
	{
		return base.settings.GetFloat(keySuperstarPatientChance(), 0.05f);
	}

	public int GetSuperstarBonusMultiplier()
	{
		return base.settings.GetInt(keySuperstarBonusMultiplier(), 10);
	}

	public float GetPatientSickNextTime()
	{
		return base.settings.GetFloat(keyPatientSickNextTime(), 0.75f);
	}

	public float GetPatientSickTimer()
	{
		return base.settings.GetFloat(keyPatientSickTimer(), 1f);
	}

	public float GetPatientHealthDrop()
	{
		return base.settings.GetFloat(keyPatientHealthDrop(), 1f);
	}

	public int GetPatientGoldValue(int index)
	{
		return base.settings.GetInt(keyPatientGoldValue(index));
	}

	public int GetPatientPrestigeValue(int index)
	{
		return base.settings.GetInt(keyPatientPrestigeValue(index));
	}

	public int GetPatientItemNeededToHeal(int index)
	{
		return base.settings.GetInt(keyPatientItemNeededToHeal(index));
	}

	public int GetBaseCoinsFromRewardedAd()
	{
		return base.settings.GetInt(keyBaseCoinsFromRewardedAd(), 50);
	}

	public float GetMiniGameStartProgress()
	{
		return base.settings.GetFloat(keyMiniGameStartProgress(), 25f);
	}

	public float GetMiniGameAddingSpeed()
	{
		return base.settings.GetFloat(keyMiniGameAddingSpeed(), 0.75f);
	}

	public float GetMiniGameLosingSpeed()
	{
		return base.settings.GetFloat(keyMiniGameLosingSpeed(), 0.6f);
	}

	public float GetItemUpgradeBonus()
	{
		return base.settings.GetFloat(keyItemUpgradeBonus(), 0.1f);
	}

	public float GetItemUpgradePriceMultiplier()
	{
		return base.settings.GetFloat(keyItemUpgradePriceMultiplier(), 1.5f);
	}

	public int GetItemBasePrice(int index)
	{
		return base.settings.GetInt(keyItemBasePrice(index));
	}

	public int GetItemBaseUpgradePrice(int index)
	{
		return base.settings.GetInt(keyItemBaseUpgradePrice(index));
	}

	public int GetItemPrestigeLevelRequirement(int index)
	{
		return base.settings.GetInt(keyItemPrestigeLevelRequirement(index));
	}

	public int GetItemPrestigeOnUnlock(int index)
	{
		return base.settings.GetInt(keyItemPrestigeOnUnlock(index));
	}
}
