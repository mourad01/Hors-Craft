// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ModelManager
using Common.Managers;
using System.Collections.Generic;

namespace Gameplay
{
	public class ModelManager : AbstractModelManager
	{
		public RetentionNotificationsTextsModule retentionNotificationsTexts;

		public IOSAppURLModule iOSAppURL;

		public AdsFreeModule adsFree;

		public TimeBasedRateUsReminderModule timeBasedRateUs;

		public TimeBasedDidYouKnowModule timeBasedDidYouKnow;

		public LevelBasedAdRequirementsModule levelBasedAdRequirements;

		public TimeBasedAdRequirementsModule timeBasedAdRequirements;

		public BlocksUnlockingModule blocksUnlocking;

		//public GlobalBannerModule globalBanner;

		public SeeOtherGamesModule sog;

		public FBFanpageModule fb;

		public ClothesSettingsModule clothesSetting;

		public ChatBotSettingsModule chatbotSettings;

		public CraftingSettings craftingSettings;

		public WorldsSettingsModule worldsSettings;

		public PauseAdTimerModule pauseAdTimer;

		public FishingModule fishingSettings;

		public PetsSettingsModule petSetting;

		public ChestSettingsModule chestSettings;

		public VideoPackageModule videoPackage;

		public DailyChestSettingsModule dailyChestSettings;

		public EngineSettingsModule engineSettings;

		public OfferSettingsModule offerPackSettings;

		public CookingModule cookingSettings;

		public DailyRewardsModule dailyRewardsSettings;

		public WhatsNewModule whatsNewSettings;

		public ProgressModule progressSettings;

		public CurrencyModule currencySettings;

		public HospitalModule hospitalSettings;

		public DressupModule dressupSettings;

		public AdventureQuestModule adventureQuestSettings;

		public TicketsModule ticketsSettings;

		public LoveModule loveSettings;

		public MinigameModule minigameSettings;

		public SurvivalModule survivalSettings;

		public GrowthModule growthSettings;

		public SunModule sunSettings;

		public ItemsUpgradeStatsModule itemsUpgradeStatsSettings;

		public SurvivalTierSpawnModule survivalTierSpawnSettings;

		public ItemsDropModule itemsDropSettings;

		public ItemsUpgradeRequirementsModule itemsUpgradeRequirementsSettings;

		public WaveRewardModule waveRewardSettings;

		public LootModule lootSettings;

		public McpeSteeringModule mcpeSteering;

		public ConfigModule configSettings;

	//	public AdScoringModule adScoringModule;

		protected override int modelVersion => 8;

		protected override List<ModelModule> ConstructModelModules()
		{
			List<ModelModule> list = new List<ModelModule>();
			list.Add(retentionNotificationsTexts = new RetentionNotificationsTextsModule());
			list.Add(adsFree = new AdsFreeModule());
			list.Add(iOSAppURL = new IOSAppURLModule());
			list.Add(timeBasedRateUs = new TimeBasedRateUsReminderModule());
			list.Add(timeBasedDidYouKnow = new TimeBasedDidYouKnowModule());
			list.Add(blocksUnlocking = new BlocksUnlockingModule());
			list.Add(clothesSetting = new ClothesSettingsModule());
			list.Add(chatbotSettings = new ChatBotSettingsModule());
			list.Add(craftingSettings = new CraftingSettings());
			list.Add(worldsSettings = new WorldsSettingsModule());
			list.Add(pauseAdTimer = new PauseAdTimerModule());
			list.Add(fishingSettings = new FishingModule());
			list.Add(chestSettings = new ChestSettingsModule());
			list.Add(petSetting = new PetsSettingsModule());
			list.Add(videoPackage = new VideoPackageModule());
			list.Add(dailyChestSettings = new DailyChestSettingsModule());
			list.Add(engineSettings = new EngineSettingsModule());
			list.Add(offerPackSettings = new OfferSettingsModule());
			list.Add(cookingSettings = new CookingModule());
			list.Add(dailyRewardsSettings = new DailyRewardsModule());
			list.Add(whatsNewSettings = new WhatsNewModule());
			list.Add(progressSettings = new ProgressModule());
			list.Add(currencySettings = new CurrencyModule());
			list.Add(hospitalSettings = new HospitalModule());
			list.Add(dressupSettings = new DressupModule());
			list.Add(adventureQuestSettings = new AdventureQuestModule());
			list.Add(ticketsSettings = new TicketsModule());
			list.Add(loveSettings = new LoveModule());
			list.Add(minigameSettings = new MinigameModule());
			list.Add(survivalSettings = new SurvivalModule());
			list.Add(growthSettings = new GrowthModule());
			list.Add(sunSettings = new SunModule());
			list.Add(itemsUpgradeStatsSettings = new ItemsUpgradeStatsModule());
			list.Add(survivalTierSpawnSettings = new SurvivalTierSpawnModule());
			list.Add(itemsDropSettings = new ItemsDropModule());
			list.Add(itemsUpgradeRequirementsSettings = new ItemsUpgradeRequirementsModule());
			list.Add(waveRewardSettings = new WaveRewardModule());
			list.Add(lootSettings = new LootModule());
			list.Add(mcpeSteering = new McpeSteeringModule());
			list.Add(configSettings = new ConfigModule());
			list.Add(levelBasedAdRequirements = new LevelBasedAdRequirementsModule());
			list.Add(timeBasedAdRequirements = new TimeBasedAdRequirementsModule());
			//list.Add(adScoringModule = new AdScoringModule());
			//list.Add(globalBanner = new GlobalBannerModule());
			list.Add(new TapjoyPlacementsModule(new string[7]
			{
				"Common",
				"Rewarded",
				"Title",
				"Pause",
				"Gameplay",
				"Getblocks",
				"Getworldslot"
			}));
			//list.Add(new AdWaterfallOrdersModule());
			list.Add(sog = new SeeOtherGamesModule());
			list.Add(fb = new FBFanpageModule());
			return list;
		}
	}
}
