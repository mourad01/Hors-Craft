// DecompilerFi decompiler from Assembly-CSharp.dll class: FishingModule
using Common.Managers;
using Common.Model;
using UnityEngine;

public class FishingModule : ModelModule
{
	private const string shownFishingReminder = "shownFishingReminder";

	private const string fishingReminderShowedAlready = "fishingReminderShowedAlready";

	private string keyFishingEnabled()
	{
		return "fishing.enabled";
	}

	private string keyLegendaryChance()
	{
		return "fishing.legendary.chance";
	}

	private string keyEpicChance()
	{
		return "fishing.epic.chance";
	}

	private string keyRareChance()
	{
		return "fishing.rare.chance";
	}

	private string keyCommonChance()
	{
		return "fishing.common.chance";
	}

	private string keyTrashChance()
	{
		return "fishing.trash.chance";
	}

	private string keyRodWoodenWidth()
	{
		return "fishing.rod.wooden.width";
	}

	private string keyRodSilverWidth()
	{
		return "fishing.rod.silver.width";
	}

	private string keyRodGoldenWidth()
	{
		return "fishing.rod.golden.width";
	}

	private string keyLegendaryDropScaleBronze()
	{
		return "fishing.legendary.drop.scales.bronze";
	}

	private string keyLegendaryDropScaleSilver()
	{
		return "fishing.legendary.drop.scales.silver";
	}

	private string keyLegendaryDropScaleGolden()
	{
		return "fishing.legendary.drop.scales.golden";
	}

	private string keyEpicDropScaleBronze()
	{
		return "fishing.epic.drop.scales.bronze";
	}

	private string keyEpicDropScaleSilver()
	{
		return "fishing.epic.drop.scales.silver";
	}

	private string keyEpicDropScaleGolden()
	{
		return "fishing.epic.drop.scales.golden";
	}

	private string keyRareDropScaleBronze()
	{
		return "fishing.rare.drop.scales.bronze";
	}

	private string keyRareDropScaleSilver()
	{
		return "fishing.rare.drop.scales.silver";
	}

	private string keyRareDropScaleGolden()
	{
		return "fishing.rare.drop.scales.golden";
	}

	private string keyCommonDropScaleBronze()
	{
		return "fishing.common.drop.scales.bronze";
	}

	private string keyCommonDropScaleSilver()
	{
		return "fishing.common.drop.scales.silver";
	}

	private string keyFishingTutorial1()
	{
		return "fishing.tutorial.popup.1";
	}

	private string keyFishingTutorial2()
	{
		return "fishing.tutorial.popup.2";
	}

	private string keyFishingTutorial3()
	{
		return "fishing.tutorial.popup.3";
	}

	private string keyFishingTutorial4()
	{
		return "fishing.tutorial.popup.4";
	}

	private string keyFishingTutorial5()
	{
		return "fishing.tutorial.popup.5";
	}

	private string keyFishingTutorial6()
	{
		return "fishing.tutorial.popup.6";
	}

	private string keyFishingTutorial7()
	{
		return "fishing.tutorial.popup.7";
	}

	private string keyFishingTutorial8()
	{
		return "fishing.tutorial.popup.8";
	}

	private string keyFishingReminder()
	{
		return "fishing.reminder";
	}

	public string keyFishingStatusPull()
	{
		return "fishing.status.pull";
	}

	public string keyFishingStatusWait()
	{
		return "fishing.status.wait";
	}

	public string keyFishingStatusBreak()
	{
		return "fishing.status.break";
	}

	public string keyFishingStatusBreakTrash()
	{
		return "fishing.status.break.trash";
	}

	public string keyFishingStatusSuccess()
	{
		return "fishing.status.success";
	}

	public string keyFishingStatusSuccessTrash()
	{
		return "fishing.status.success.trash";
	}

	public string keyFishingStatusThrow()
	{
		return "fishing.status.throw";
	}

	public string keyFishingStatusFight()
	{
		return "fishing.status.fight";
	}

	private string keyFishingReminderFirstTime()
	{
		return "fishing.reminder.firsttime";
	}

	private string keyFishingReminderTimeDivider()
	{
		return "fishing.reminder.nexttimedivider";
	}

	private string keyFishingReminderEnabled()
	{
		return "fishing.reminder.enabled";
	}

	private string keyLineUpgrade()
	{
		return "fishing.upgrade.line";
	}

	private string keyFloatUpgrade()
	{
		return "fishing.upgrade.float";
	}

	private string keyReelUpgrade()
	{
		return "fishing.upgrade.reel";
	}

	private string keyHookUpgrade()
	{
		return "fishing.upgrade.hook";
	}

	private string keyBaitsUpgrade()
	{
		return "fishing.upgrade.baits";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyFishingEnabled(), defaultValue: false);
		descriptions.AddDescription(keyLegendaryChance(), 4.3f);
		descriptions.AddDescription(keyEpicChance(), 6.88f);
		descriptions.AddDescription(keyRareChance(), 9.07f);
		descriptions.AddDescription(keyCommonChance(), 12.03f);
		descriptions.AddDescription(keyTrashChance(), 9.17f);
		descriptions.AddDescription(keyRodWoodenWidth(), 100f);
		descriptions.AddDescription(keyRodSilverWidth(), 200f);
		descriptions.AddDescription(keyRodGoldenWidth(), 300f);
		descriptions.AddDescription(keyLegendaryDropScaleBronze(), 25);
		descriptions.AddDescription(keyLegendaryDropScaleSilver(), 10);
		descriptions.AddDescription(keyLegendaryDropScaleGolden(), 5);
		descriptions.AddDescription(keyEpicDropScaleBronze(), 10);
		descriptions.AddDescription(keyEpicDropScaleSilver(), 5);
		descriptions.AddDescription(keyEpicDropScaleGolden(), 1);
		descriptions.AddDescription(keyRareDropScaleBronze(), 5);
		descriptions.AddDescription(keyRareDropScaleSilver(), 1);
		descriptions.AddDescription(keyRareDropScaleGolden(), 1);
		descriptions.AddDescription(keyCommonDropScaleBronze(), 3);
		descriptions.AddDescription(keyCommonDropScaleSilver(), 1);
		descriptions.AddDescription(keyFishingReminderFirstTime(), 180);
		descriptions.AddDescription(keyFishingReminderTimeDivider(), 60);
		descriptions.AddDescription(keyFishingReminderEnabled(), defaultValue: true);
		descriptions.AddDescription(keyLineUpgrade(), 1.25f);
		descriptions.AddDescription(keyFloatUpgrade(), 0.8f);
		descriptions.AddDescription(keyReelUpgrade(), 0.85f);
		descriptions.AddDescription(keyHookUpgrade(), 1.1f);
		descriptions.AddDescription(keyBaitsUpgrade(), 1.1f);
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<FishingManager>())
		{
			Manager.Get<FishingManager>().ModelDownloaded();
		}
	}

	public bool IsFishingEnabled()
	{
		return base.settings.GetBool(keyFishingEnabled(), defaultValue: false);
	}

	public float LegendaryChance()
	{
		return base.settings.GetFloat(keyLegendaryChance(), 4.3f);
	}

	public float EpicChance()
	{
		return base.settings.GetFloat(keyEpicChance(), 6.88f);
	}

	public float RareChance()
	{
		return base.settings.GetFloat(keyRareChance(), 9.07f);
	}

	public float CommonChance()
	{
		return base.settings.GetFloat(keyCommonChance(), 12.03f);
	}

	public float TrashChance()
	{
		return base.settings.GetFloat(keyTrashChance(), 9.17f);
	}

	public float RodWoodenBaseWidth()
	{
		return base.settings.GetFloat(keyRodWoodenWidth(), 100f);
	}

	public float RodSilverBaseWidth()
	{
		return base.settings.GetFloat(keyRodSilverWidth(), 200f);
	}

	public float RodGoldenBaseWidth()
	{
		return base.settings.GetFloat(keyRodGoldenWidth(), 300f);
	}

	public int LegendaryBronzeScaleDrop()
	{
		return base.settings.GetInt(keyLegendaryDropScaleBronze(), 25);
	}

	public int LegendarySilverScaleDrop()
	{
		return base.settings.GetInt(keyLegendaryDropScaleSilver(), 10);
	}

	public int LegendaryGoldenScaleDrop()
	{
		return base.settings.GetInt(keyLegendaryDropScaleGolden(), 5);
	}

	public int EpicBronzeScaleDrop()
	{
		return base.settings.GetInt(keyEpicDropScaleBronze(), 10);
	}

	public int EpicSilverScaleDrop()
	{
		return base.settings.GetInt(keyEpicDropScaleSilver(), 5);
	}

	public int EpicGoldenScaleDrop()
	{
		return base.settings.GetInt(keyEpicDropScaleGolden(), 1);
	}

	public int RareBronzeScaleDrop()
	{
		return base.settings.GetInt(keyRareDropScaleBronze(), 5);
	}

	public int RareSilverScaleDrop()
	{
		return base.settings.GetInt(keyRareDropScaleSilver(), 1);
	}

	public int RareGoldenScaleDrop()
	{
		return base.settings.GetInt(keyRareDropScaleGolden(), 1);
	}

	public int CommonBronzeScaleDrop()
	{
		return base.settings.GetInt(keyCommonDropScaleBronze(), 3);
	}

	public int CommonSilverScaleDrop()
	{
		return base.settings.GetInt(keyCommonDropScaleSilver(), 1);
	}

	public string FishingTutorial1Title()
	{
		return base.settings.GetString(keyFishingTutorial1(), "Hello, young Fisherman! I’ll teach you how to catch fish!");
	}

	public string FishingTutorial2Title()
	{
		return base.settings.GetString(keyFishingTutorial2(), "Tap the “fishing rod” button!");
	}

	public string FishingTutorial3Title()
	{
		return base.settings.GetString(keyFishingTutorial3(), "To catch a fish, you need to keep the fish icon inside the inner bar for a few seconds.");
	}

	public string FishingTutorial4Title()
	{
		return base.settings.GetString(keyFishingTutorial4(), "You have to move the bar so that the fish stays inside it! Tap the “Pull” button!");
	}

	public string FishingTutorial5Title()
	{
		return base.settings.GetString(keyFishingTutorial5(), "The inner bar turns green! It means that you are doing great!");
	}

	public string FishingTutorial6Title()
	{
		return base.settings.GetString(keyFishingTutorial6(), "Congratulations! You caught your first fish!");
	}

	public string FishingTutorial7Title()
	{
		return base.settings.GetString(keyFishingTutorial7(), "There are a lot more fish to catch there!");
	}

	public string FishingTutorial8Title()
	{
		return base.settings.GetString(keyFishingTutorial8(), "Collect fish, upgrade your fishing rod and compete with other players in the leaderboard!");
	}

	public string FishingReminder()
	{
		return base.settings.GetString(keyFishingReminder(), "Remember: you have to travel and explore other fishing pools (which are highlighted) to catch specific fish! Good luck!");
	}

	public string FishingStatusPull()
	{
		return base.settings.GetString(keyFishingStatusPull(), "Pull");
	}

	public string FishingStatusWait()
	{
		return base.settings.GetString(keyFishingStatusWait(), "Wait");
	}

	public string FishingStatusBreak()
	{
		return base.settings.GetString(keyFishingStatusBreak(), "Fish breaks!");
	}

	public string FishingStatusBreakTrash()
	{
		return base.settings.GetString(keyFishingStatusBreakTrash(), "Trash breaks!");
	}

	public string FishingStatusSuccessTrash()
	{
		return base.settings.GetString(keyFishingStatusSuccessTrash(), " ");
	}

	public string FishingStatusSuccess()
	{
		return base.settings.GetString(keyFishingStatusSuccess(), " ");
	}

	public string FishingStatusThrow()
	{
		return base.settings.GetString(keyFishingStatusThrow(), "Throw");
	}

	public string FishingStatusFight()
	{
		return base.settings.GetString(keyFishingStatusFight(), "Fight");
	}

	public bool HasToShowFishingReminder(int passedTime)
	{
		if (!IsFishingReminderEnabled())
		{
			return false;
		}
		int @int = base.settings.GetInt(keyFishingReminderFirstTime());
		int int2 = base.settings.GetInt(keyFishingReminderTimeDivider());
		int int3 = PlayerPrefs.GetInt("shownFishingReminder", 0);
		bool flag = false;
		if (int3 == 0)
		{
			if (passedTime > @int)
			{
				flag = true;
			}
		}
		else
		{
			passedTime -= @int;
			passedTime -= (int3 - 1) * Mathf.Max(int2, 1);
			if (passedTime > int2)
			{
				flag = true;
			}
		}
		if (flag)
		{
			PlayerPrefs.SetInt("shownFishingReminder", PlayerPrefs.GetInt("shownFishingReminder", 0) + 1);
		}
		return flag;
	}

	private bool IsFishingReminderEnabled()
	{
		return base.settings.GetBool(keyFishingReminderEnabled());
	}

	public float LineUpgrade()
	{
		return base.settings.GetFloat(keyLineUpgrade(), 1.25f);
	}

	public float FloatUpgrade()
	{
		return base.settings.GetFloat(keyFloatUpgrade(), 0.8f);
	}

	public float ReelUpgrade()
	{
		return base.settings.GetFloat(keyReelUpgrade(), 0.85f);
	}

	public float HookUpgrade()
	{
		return base.settings.GetFloat(keyHookUpgrade(), 1.1f);
	}

	public float BaitsUpgrade()
	{
		return base.settings.GetFloat(keyBaitsUpgrade(), 1.25f);
	}
}
