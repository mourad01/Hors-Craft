// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.StatsManager
using Common.Connection;
using Common.Utils;
using Common.Waterfall;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Common.Managers
{
	public class StatsManager : Manager
	{
		[Serializable]
		public abstract class Stat
		{
			public string serversideId;
		}

		public enum AdReason
		{
			XCRAFT_TITLE,
			XCRAFT_PAUSE,
			XCRAFT_BLOCKS_ALL,
			XCRAFT_BLOCKS_BASIC,
			XCRAFT_BLOCKS_ORGANIC,
			XCRAFT_BLOCKS_CUSTOM,
			XCRAFT_BLOCKS_FURNITURE,
			XCRAFT_CLOTHES,
			XCRAFT_REMOVEADS,
			XCRAFT_PETS,
			DATING_TITLE,
			DATING_HOME,
			DATING_MAKEUP,
			DATING_CLOTHES,
			XCRAFT_CURRENCY,
			XCRAFT_DOUBLE_REWARD,
			COOKING_BETWEEN_STAGE,
			COOKING_RENEW_ITEMS,
			COOKING_CURRENCY,
			TANKS_ITEM,
			TANKS_TANK_BUY,
			TANKS_ARMORY,
			TANKS_SUMMARY,
			TANKS_TITLE,
			XCRAFT_MAX_WORLDS_INCREASE,
			XCRAFT_RESPAWN,
			XCRAFT_GET_RESOURCES,
			XCRAFT_UNLOCK_CRAFTABLE,
			XCRAFT_CURE,
			XCRAFT_FISHING,
			XCRAFT_BASE_UI_STATE,
			SKIP_TIME,
			XSURVIVAL_DOUBLE_DPS,
			XSURVIVAL_GET_POTIONS,
			XSURVIVAL_DEATH,
			XCRAFT_BLOCKS_REFILL,
			XCRAFT_BLUEPRINT_FILL,
			LOWPOLY_REMOVEADS,
			LOWPOLY_BUILDING_SKIP,
			LOWPOLY_CATALOG_BUILDING,
			LOWPOLY_GET_TELEPORTS,
			LOWPOLY_FLYING_PROMO,
			LOWPOLY_GET_GOLD,
			XCRAFT_UNLOCK_BLOCK,
			XCRAFT_LOVE_DATE,
			XCRAFT_LOVE_SMS,
			XSURVIVAL_AMMO,
			XCRAFT_FLY
		}

		public enum BlueprintAction
		{
			INVALID,
			BASIC_BLOCK,
			MANUALLY_FILLED,
			AUTO_FILLED,
			CRAFTED
		}

		public enum BoughtProduct
		{
			INVALID,
			BASIC_BLOCK,
			FURNITURE_BLOCK,
			ORGANIC_BLOCK,
			CUSTOM_BLOCK,
			UNLOCK_ALL_BASIC,
			UNLOCK_ALL_FURNITURE,
			UNLOCK_ALL_CUSTOM,
			UNLOCK_ALL_ORGANIC,
			CLOTHES,
			TANK,
			CONSUMABLE
		}

		public enum MinigameType
		{
			INVALID,
			DUCKS,
			DANCE,
			HAMMERTIME,
			FISH,
			HEAL,
			INSTRUMENTS,
			WALKWAY,
			MASHING
		}

		[Serializable]
		public class SeeOtherGamesClickedStat : Stat
		{
			public SeeOtherGamesClickedStat()
			{
				serversideId = "clickedSeeOtherGames";
			}
		}

		[Serializable]
		public class WaterfallStepTriedStat : Stat
		{
			public string stepid;

			public int stepnumber;

			public string steptype;

			public WaterfallStepTriedStat()
			{
				serversideId = "waterfallStepTried";
			}
		}

		[Serializable]
		public class AdShownStat : Stat
		{
			public AdShownStat()
			{
				serversideId = "shownAd";
			}
		}

		[Serializable]
		public class AdShownWithReasonStat : Stat
		{
			public string adReason;

			public AdShownWithReasonStat()
			{
				serversideId = "shownAdWithReason";
			}
		}

		[Serializable]
		public class AdShownSuccessBasedOnCallbacksStat : Stat
		{
			public string stepid;

			public AdShownSuccessBasedOnCallbacksStat()
			{
				serversideId = "shownAdSuccessFromCallback";
			}
		}

		[Serializable]
		public class AdShownSuccessBasedOnTimeStat : Stat
		{
			public string stepid;

			public AdShownSuccessBasedOnTimeStat()
			{
				serversideId = "shownAdSuccessFromTime";
			}
		}

		[Serializable]
		public class AdClickedStat : Stat
		{
			public AdClickedStat()
			{
				serversideId = "clickedAd";
			}
		}

		[Serializable]
		public class RewardedAdShownStat : Stat
		{
			public RewardedAdShownStat()
			{
				serversideId = "rewardedAdShown";
			}
		}

		[Serializable]
		public class RemoveAdsClickedStat : Stat
		{
			public RemoveAdsClickedStat()
			{
				serversideId = "clickedRemoveAds";
			}
		}

		[Serializable]
		public class QuestStartedStat : Stat
		{
			public int questNo;

			public QuestStartedStat()
			{
				serversideId = "questStarted";
			}
		}

		[Serializable]
		public class QuestCompletedStat : Stat
		{
			public int questNo;

			public QuestCompletedStat()
			{
				serversideId = "questCompleted";
			}
		}

		[Serializable]
		public class QuestCompletedWithStarsStat : Stat
		{
			public int questNo;

			public int stars;

			public QuestCompletedWithStarsStat()
			{
				serversideId = "questCompletedWithStars";
			}
		}

		[Serializable]
		public class QuestCompletedWithStarsUniqueStat : Stat
		{
			public int questNo;

			public int stars;

			public QuestCompletedWithStarsUniqueStat()
			{
				serversideId = "questCompletedWithStarsUnique";
			}
		}

		[Serializable]
		public class ErrorsOccurredStat : Stat
		{
			public int count;

			public ErrorsOccurredStat()
			{
				serversideId = "errorsOccured";
			}
		}

		[Serializable]
		public class NotificationsAcceptedStat : Stat
		{
			public NotificationsAcceptedStat()
			{
				serversideId = "notificationsAccepted";
			}
		}

		[Serializable]
		public class NotificationOpenedStat : Stat
		{
			public string notificationtype;

			public NotificationOpenedStat()
			{
				serversideId = "notificationOpened";
			}
		}

		[Serializable]
		public class NotificationOpenedUniqueStat : Stat
		{
			public string notificationtype;

			public NotificationOpenedUniqueStat()
			{
				serversideId = "notificationOpenedUnique";
			}
		}

		[Serializable]
		public class ItemBoughtStat : Stat
		{
			public string itemid;

			public int count;

			public ItemBoughtStat()
			{
				serversideId = "itemBought";
			}
		}

		[Serializable]
		public class PaymentInitStat : Stat
		{
			public string productId;

			public PaymentInitStat()
			{
				serversideId = "paymentInit";
			}
		}

		[Serializable]
		public class PaymentSuccessStat : Stat
		{
			public string productId;

			public PaymentSuccessStat()
			{
				serversideId = "paymentSuccess";
			}
		}

		[Serializable]
		public class CrosspromoShownStat : Stat
		{
			public CrosspromoShownStat()
			{
				serversideId = "crosspromoShown";
			}
		}

		[Serializable]
		public class CrosspromoClickedStat : Stat
		{
			public CrosspromoClickedStat()
			{
				serversideId = "crosspromoClicked";
			}
		}

		[Serializable]
		public class AdditionalButtonClickedStat : Stat
		{
			public int buttonNo;

			public AdditionalButtonClickedStat()
			{
				serversideId = "additionalButtonClicked";
			}
		}

		[Serializable]
		public class XCraftBlocksPlacedStat : Stat
		{
			public int count;

			public XCraftBlocksPlacedStat()
			{
				serversideId = "xcraftBlocksPlaced";
			}
		}

		[Serializable]
		public class ItemCraftedStat : Stat
		{
			public ItemCraftedStat()
			{
				serversideId = "itemCrafted";
			}
		}

		[Serializable]
		public class BlockBoughtStat : Stat
		{
			public int blockId;

			public BoughtProduct category;

			public int currency;

			public int currencyDelta;

			public BlockBoughtStat()
			{
				serversideId = "blockBought";
			}
		}

		[Serializable]
		public class BlockUnlockCategoryStat : Stat
		{
			public BoughtProduct category;

			public int currency;

			public int currencyDelta;

			public BlockUnlockCategoryStat()
			{
				serversideId = "unlockCategory";
			}
		}

		[Serializable]
		public class ClothBoughtStat : Stat
		{
			public string id;

			public int currency;

			public int currencyDelta;

			public ClothBoughtStat()
			{
				serversideId = "clothesBought";
			}
		}

		[Serializable]
		public class OfferPackBoughtStat : Stat
		{
			public string id;

			public OfferPackBoughtStat()
			{
				serversideId = "offerPackBought";
			}
		}

		[Serializable]
		public class WorldBoughtStat : Stat
		{
			public string worldId;

			public int currency;

			public int currencyDelta;

			public WorldBoughtStat()
			{
				serversideId = "worldBought";
			}
		}

		[Serializable]
		public class WorldEnteredStat : Stat
		{
			public string worldId;

			public int currency;

			public int currencyDelta;

			public WorldEnteredStat()
			{
				serversideId = "worldEntered";
			}
		}

		[Serializable]
		public class WorldQuestFinished : Stat
		{
			public string worldId;

			public string questType;

			public int currency;

			public int numberOfQuest;

			public WorldQuestFinished()
			{
				serversideId = "worldQuestFinished";
			}
		}

		[Serializable]
		public class DailyChestOpenStat : Stat
		{
			public DailyChestOpenStat()
			{
				serversideId = "dailyChestOpen";
			}
		}

		[Serializable]
		public class TankBoughtStat : Stat
		{
			public TankBoughtStat()
			{
				serversideId = "tankBought";
			}
		}

		[Serializable]
		public class ConsumableBoughtStat : Stat
		{
			public string item_id;

			public ConsumableBoughtStat()
			{
				serversideId = "consumableBought";
			}

			public ConsumableBoughtStat(string item_id)
			{
				serversideId = "consumableBought";
				this.item_id = item_id;
			}
		}

		[Serializable]
		public class ChatbotLaunchedStat : Stat
		{
			public ChatbotLaunchedStat()
			{
				serversideId = "chatbotLaunched";
			}
		}

		[Serializable]
		public class ChatbotMessageSendStat : Stat
		{
			public ChatbotMessageSendStat()
			{
				serversideId = "chatbotMessageSend";
			}
		}

		[Serializable]
		public class ShareClickedStat : Stat
		{
			public ShareClickedStat()
			{
				serversideId = "shareClicked";
			}
		}

		[Serializable]
		public class DrawingStartedStat : Stat
		{
			public int category;

			public DrawingStartedStat()
			{
				serversideId = "drawingStarted";
			}
		}

		[Serializable]
		public class OnMultiRoomCreatedStat : Stat
		{
			public OnMultiRoomCreatedStat()
			{
				serversideId = "multiRoomCreated";
			}
		}

		[Serializable]
		public class OnMultiRoomJoinedStat : Stat
		{
			public OnMultiRoomJoinedStat()
			{
				serversideId = "multiRoomJoined";
			}
		}

		[Serializable]
		public class BlueprintStat : Stat
		{
			public string action;

			public BlueprintStat()
			{
				serversideId = "blueprint";
			}
		}

		[Serializable]
		public class PingStat : Stat
		{
			public PingStat()
			{
				serversideId = "ping";
			}
		}

		[Serializable]
		public class FishingSuccessStat : Stat
		{
			public FishingSuccessStat()
			{
				serversideId = "fishingSuccess";
			}
		}

		[Serializable]
		public class FishingFailureStat : Stat
		{
			public FishingFailureStat()
			{
				serversideId = "fishingFailure";
			}
		}

		[Serializable]
		public class BoughtUpgradeStat : Stat
		{
			public BoughtUpgradeStat()
			{
				serversideId = "boughtUpgrade";
			}
		}

		[Serializable]
		public class RankingClickedStat : Stat
		{
			public RankingClickedStat()
			{
				serversideId = "rankingClicked";
			}
		}

		[Serializable]
		public class AchievementsClickedStat : Stat
		{
			public AchievementsClickedStat()
			{
				serversideId = "achievementsClicked";
			}
		}

		[Serializable]
		public class CookingUpgradeBoughtStat : Stat
		{
			public CookingUpgradeBoughtStat()
			{
				serversideId = "cookingUpgradeBought";
			}
		}

		[Serializable]
		public class AdsRemovedStat : Stat
		{
			public AdsRemovedStat()
			{
				serversideId = "adsRemoved";
			}
		}

		[Serializable]
		public class XcraftCraftAttemptedStat : Stat
		{
			public bool success;

			public string itemCategory;

			public string itemName;

			public XcraftCraftAttemptedStat()
			{
				serversideId = "xcraftCraftAttempted";
			}
		}

		[Serializable]
		public class ItemPlacedStat : Stat
		{
			public string itemCategory;

			public string itemName;

			public ItemPlacedStat()
			{
				serversideId = "itemPlaced";
			}
		}

		[Serializable]
		public class ItemUnlockedStat : Stat
		{
			public string itemCategory;

			public string itemName;

			public ItemUnlockedStat()
			{
				serversideId = "itemUnlocked";
			}
		}

		[Serializable]
		public class ChestOpenedStat : Stat
		{
			public string rarity;

			public ChestOpenedStat()
			{
				serversideId = "chestOpened";
			}
		}

		[Serializable]
		public class XcraftBlueprintCompletedStat : Stat
		{
			public string blueprintId;

			public bool wasInstant;

			public float instantPercentageFilled;

			public XcraftBlueprintCompletedStat()
			{
				serversideId = "xcraftBlueprintCompleted";
			}
		}

		[Serializable]
		public class MinigameFinishedStat : Stat
		{
			public string minigameId;

			public bool success;

			public MinigameFinishedStat()
			{
				serversideId = "minigameFinished";
			}
		}

		public class AdventureQuestStartedStat : Stat
		{
			public int questId;

			public AdventureQuestStartedStat()
			{
				serversideId = "adventureQuestStarted";
			}
		}

		[Serializable]
		public class AdventureQuestFinishedStat : Stat
		{
			public int questId;

			public AdventureQuestFinishedStat()
			{
				serversideId = "adventureQuestFinished";
			}
		}

		[Serializable]
		public class XcraftPlayerDeathStat : Stat
		{
			public int wave;

			public XcraftPlayerDeathStat()
			{
				serversideId = "xcraftPlayerDeath";
			}
		}

		[Serializable]
		public class LevelUpStat : Stat
		{
			public int playerLevel;

			public LevelUpStat()
			{
				serversideId = "levelUp";
			}
		}

		[Serializable]
		public class RelationshipLevelUpStat : Stat
		{
			public int relationshipLevel;

			public RelationshipLevelUpStat()
			{
				serversideId = "relationshipLevelUp";
			}
		}

		[Serializable]
		public class PlayerMovementStat : Stat
		{
			public int distance;

			public PlayerMovementStat()
			{
				serversideId = "playerMovement";
			}
		}

		[Serializable]
		public class FarmingSowStat : Stat
		{
			public FarmingSowStat()
			{
				serversideId = "farmingSow";
			}
		}

		[Serializable]
		public class FarmingHarvestStat : Stat
		{
			public FarmingHarvestStat()
			{
				serversideId = "farmingHarvest";
			}
		}

		[Serializable]
		public class FarmingWaterStat : Stat
		{
			public FarmingWaterStat()
			{
				serversideId = "farmingWater";
			}
		}

		[Serializable]
		public class FarmingPlantStat : Stat
		{
			public string plantId;

			public FarmingPlantStat()
			{
				serversideId = "farmingPlant";
			}
		}

		[Serializable]
		public class CommonWorldEnteredStat : Stat
		{
			public int worldId;

			public CommonWorldEnteredStat()
			{
				serversideId = "commonWorldEntered";
			}
		}

		[Serializable]
		public class ClickerPlayerBatchStat : Stat
		{
			public int generator1Count;

			public int generator2Count;

			public int generator3Count;

			public int generator4Count;

			public int generator5Count;

			public int generator6Count;

			public int generator7Count;

			public int generator8Count;

			public int generator9Count;

			public double softAmount;

			public double hardAmount;

			public double eliteAmount;

			public int softResetCount;

			public int clicksBase;

			public int clicksNpcs;

			public double softCoinsIncome;

			public ClickerPlayerBatchStat()
			{
				serversideId = "clickerPlayerBatch";
			}
		}

		[Serializable]
		public class ClickerSoftUpgradeBoughtStat : Stat
		{
			public string itemId;

			public string test;

			public ClickerSoftUpgradeBoughtStat()
			{
				serversideId = "clickerSoftUpgradeBought";
			}
		}

		[Serializable]
		public class ClickerEliteUpgradeBoughtStat : Stat
		{
			public string itemId;

			public ClickerEliteUpgradeBoughtStat()
			{
				serversideId = "clickerEliteUpgradeBought";
			}
		}

		[Serializable]
		public class ClickerHardItemBoughtStat : Stat
		{
			public string itemId;

			public ClickerHardItemBoughtStat()
			{
				serversideId = "clickerHardItemBought";
			}
		}

		[Serializable]
		public class IAPShopOpenedStat : Stat
		{
			public IAPShopOpenedStat()
			{
				serversideId = "iapShopOpened";
			}
		}

		[Serializable]
		public class LowpolyDistrictsVisitedStat : Stat
		{
			public int districtsCount;

			public int currency;

			public int currencyDelta;

			public LowpolyDistrictsVisitedStat()
			{
				serversideId = "districtsVisited";
			}
		}

		[Serializable]
		public class LowpolyMinigameFinishedStat : Stat
		{
			public string minigameId;

			public bool success;

			public int currency;

			public int currencyDelta;

			public LowpolyMinigameFinishedStat()
			{
				serversideId = "lowpolyMinigameFinished";
			}
		}

		[Serializable]
		public class PlaceableBoughtStat : Stat
		{
			public string placeableId;

			public int placeableLevel;

			public int currency;

			public int currencyDelta;

			public int wood;

			public int woodDelta;

			public int stone;

			public int stoneDelta;

			public int iron;

			public int ironDelta;

			public PlaceableBoughtStat()
			{
				serversideId = "placeableBought";
			}
		}

		[Serializable]
		public class PassivePlaceablePlacedStat : Stat
		{
			public PassivePlaceablePlacedStat()
			{
				serversideId = "passivePlaceablePlaced";
			}
		}

		[Serializable]
		public class PlaceableUpgradedStat : Stat
		{
			public PlaceableUpgradedStat()
			{
				serversideId = "placeableUpgraded";
			}
		}

		[Serializable]
		public class StructurePlacedStat : Stat
		{
			public StructurePlacedStat()
			{
				serversideId = "structurePlaced";
			}
		}

		[Serializable]
		public class TeleportUsedStat : Stat
		{
			public int districtId;

			public string biomId;

			public bool isHome;

			public int currency;

			public int currencyDelta;

			public TeleportUsedStat()
			{
				serversideId = "teleportUsed";
			}
		}

		[Serializable]
		public class MapRewardClaimedStat : Stat
		{
			public int currency;

			public int currencyDelta;

			public MapRewardClaimedStat()
			{
				serversideId = "mapRewardClaimed";
			}
		}

		[Serializable]
		public class PetTamedStat : Stat
		{
			public string petName;

			public PetTamedStat()
			{
				serversideId = "petTamed";
			}
		}

		[Serializable]
		public class ResourceGatheredStat : Stat
		{
			public ResourceGatheredStat()
			{
				serversideId = "resourceGathered";
			}
		}

		[Serializable]
		public class BankCollectedStat : Stat
		{
			public int currency;

			public int currencyDelta;

			public BankCollectedStat()
			{
				serversideId = "bankCollected";
			}
		}

		private Stats stats;

		private Queue<Stat> statsQueue;

		private List<StatReporter> reporters;

		private const float PING_INTERVALS = 10f;

		private float nextPingTime = 10f;

		public override void Init()
		{
			ConnectionInfoManager connectionInfoManager = Manager.Get<ConnectionInfoManager>();
			stats = new Stats(connectionInfoManager.gameName, connectionInfoManager.homeURL);
			statsQueue = new Queue<Stat>();
			reporters = new List<StatReporter>
			{
				new FPSStatReporter(),
				StartupFunnelStatsReporter.Instance
			};
			StartCoroutine(stats.OnInit(this));
			//AdWaterfall.get.statAdShownSuccessBasedOnTime = AdShownSuccessBasedOnTime;
			//AdWaterfall.get.statWaterfallStepTried = WaterfallStepTried;
		}

		public void EnqueueStat(Stat stat)
		{
			statsQueue.Enqueue(stat);
		}

		public void AddReporter(StatReporter statReporter)
		{
			reporters.Add(statReporter);
		}

		public void SeeOtherGamesClicked()
		{
			statsQueue.Enqueue(new SeeOtherGamesClickedStat());
		}

		public void WaterfallStepTried(string stepid, int stepNumber, string type)
		{
			statsQueue.Enqueue(new WaterfallStepTriedStat
			{
				stepid = stepid,
				stepnumber = stepNumber,
				steptype = type
			});
		}

		public void AdShown()
		{
			statsQueue.Enqueue(new AdShownStat());
		}

		public void AdShownWithReason(AdReason reason)
		{
			statsQueue.Enqueue(new AdShownWithReasonStat
			{
				adReason = reason.ToString().ToLower()
			});
		}

		public void AdShownSuccessBasedOnCallbacks(string stepid)
		{
			statsQueue.Enqueue(new AdShownSuccessBasedOnCallbacksStat
			{
				stepid = stepid
			});
		}

		public void AdShownSuccessBasedOnTime(string stepid)
		{
			statsQueue.Enqueue(new AdShownSuccessBasedOnTimeStat
			{
				stepid = stepid
			});
		}

		public void AdClicked()
		{
			statsQueue.Enqueue(new AdClickedStat());
		}

		public void RewardedAdShown()
		{
			statsQueue.Enqueue(new RewardedAdShownStat());
		}

		public void RemoveAdsClicked()
		{
			statsQueue.Enqueue(new RemoveAdsClickedStat());
		}

		public void QuestStarted(int questNo)
		{
			statsQueue.Enqueue(new QuestStartedStat
			{
				questNo = questNo
			});
		}

		public void QuestCompleted(int questNo)
		{
			statsQueue.Enqueue(new QuestCompletedStat
			{
				questNo = questNo
			});
		}

		public void QuestCompletedWithStars(int questNo, int stars)
		{
			statsQueue.Enqueue(new QuestCompletedWithStarsStat
			{
				questNo = questNo,
				stars = stars
			});
		}

		public void QuestCompletedWithStarsUnique(int questNo, int stars)
		{
			statsQueue.Enqueue(new QuestCompletedWithStarsUniqueStat
			{
				questNo = questNo,
				stars = stars
			});
		}

		public void ErrorsOccurred(int count)
		{
			statsQueue.Enqueue(new ErrorsOccurredStat
			{
				count = count
			});
		}

		public void NotificationsAccepted()
		{
			statsQueue.Enqueue(new NotificationsAcceptedStat());
		}

		public void NotificationOpened(string type)
		{
			statsQueue.Enqueue(new NotificationOpenedStat
			{
				notificationtype = type
			});
		}

		public void NotificationOpenedUnique(string type)
		{
			statsQueue.Enqueue(new NotificationOpenedStat
			{
				notificationtype = type
			});
		}

		public void ItemBought(string itemId, int count)
		{
			statsQueue.Enqueue(new ItemBoughtStat
			{
				itemid = itemId,
				count = count
			});
		}

		public void PaymentInit(string productId)
		{
			statsQueue.Enqueue(new PaymentInitStat
			{
				productId = productId
			});
		}

		public void PaymentSuccess(string productId)
		{
			statsQueue.Enqueue(new PaymentSuccessStat
			{
				productId = productId
			});
		}

		public void CrosspromoShown()
		{
			statsQueue.Enqueue(new CrosspromoShownStat());
		}

		public void CrosspromoClicked()
		{
			statsQueue.Enqueue(new CrosspromoClickedStat());
		}

		public void AdditionalButtonClicked(int buttonNo)
		{
			statsQueue.Enqueue(new AdditionalButtonClickedStat
			{
				buttonNo = buttonNo
			});
		}

		public void XCraftBlocksPlaced(int count)
		{
			statsQueue.Enqueue(new XCraftBlocksPlacedStat
			{
				count = count
			});
		}

		public void ItemCrafted()
		{
			statsQueue.Enqueue(new ItemCraftedStat());
		}

		public void BlockBought(int boughtBlock, BoughtProduct blockCategory, int currentGold, int delta)
		{
			statsQueue.Enqueue(new BlockBoughtStat
			{
				blockId = boughtBlock,
				category = blockCategory,
				currency = currentGold,
				currencyDelta = delta
			});
		}

		public void BlockCategoryUnlock(BoughtProduct blockCategory, int currentGold, int delta)
		{
			statsQueue.Enqueue(new BlockBoughtStat
			{
				category = blockCategory,
				currency = currentGold,
				currencyDelta = delta
			});
		}

		public void ClothBought(string clothesId, int currentGold, int delta)
		{
			statsQueue.Enqueue(new ClothBoughtStat
			{
				id = clothesId,
				currency = currentGold,
				currencyDelta = delta
			});
		}

		public void OfferPackBought(string offerId)
		{
			statsQueue.Enqueue(new OfferPackBoughtStat
			{
				id = offerId
			});
		}

		public void OnWorldBought(string id, int currentGold, int delta)
		{
			if (!id.Contains("pregenerated"))
			{
				statsQueue.Enqueue(new WorldBoughtStat
				{
					worldId = id,
					currency = currentGold
				});
			}
		}

		public void OnWorldEntered(string id, int currentGold)
		{
			if (id.Contains("pregenerated"))
			{
				id = "xcraft.base";
			}
			statsQueue.Enqueue(new WorldEnteredStat
			{
				worldId = id,
				currency = currentGold
			});
		}

		public void OnWorldQuestFinished(string currentWorldId, string type, int questNumber, int currentGold)
		{
			if (currentWorldId.Contains("pregenerated"))
			{
				currentWorldId = "xcraft.base";
			}
			statsQueue.Enqueue(new WorldQuestFinished
			{
				worldId = currentWorldId,
				questType = type,
				currency = currentGold,
				numberOfQuest = questNumber
			});
		}

		public void DailyChestOpened()
		{
			statsQueue.Enqueue(new DailyChestOpenStat());
		}

		public void TankBought()
		{
			statsQueue.Enqueue(new TankBoughtStat());
		}

		public void ConsumableBought(string item)
		{
			statsQueue.Enqueue(new ConsumableBoughtStat(item));
		}

		public void ChatbotLaunched()
		{
			statsQueue.Enqueue(new ChatbotLaunchedStat());
		}

		public void ChatbotMessageSend()
		{
			statsQueue.Enqueue(new ChatbotMessageSendStat());
		}

		public void ShareClicked()
		{
			statsQueue.Enqueue(new ShareClickedStat());
		}

		public void DrawingStarted(int category)
		{
			statsQueue.Enqueue(new DrawingStartedStat
			{
				category = category
			});
		}

		public void OnMultiRoomCreated()
		{
			statsQueue.Enqueue(new OnMultiRoomCreatedStat());
		}

		public void OnMultiRoomJoined()
		{
			statsQueue.Enqueue(new OnMultiRoomJoinedStat());
		}

		public void Blueprint(BlueprintAction action)
		{
			statsQueue.Enqueue(new BlueprintStat
			{
				action = action.ToString().ToLower()
			});
		}

		private void Ping()
		{
			statsQueue.Enqueue(new PingStat());
			SendAll();
		}

		public void FishingSuccess()
		{
			statsQueue.Enqueue(new FishingSuccessStat());
		}

		public void FishingFailure()
		{
			statsQueue.Enqueue(new FishingFailureStat());
		}

		public void BoughtUpgrade()
		{
			statsQueue.Enqueue(new BoughtUpgradeStat());
		}

		public void RankingClicked()
		{
			statsQueue.Enqueue(new RankingClickedStat());
		}

		public void AchievementsClicked()
		{
			statsQueue.Enqueue(new AchievementsClickedStat());
		}

		public void CookingUpgradeBought()
		{
			statsQueue.Enqueue(new CookingUpgradeBoughtStat());
		}

		public void AdsRemoved()
		{
			statsQueue.Enqueue(new AdsRemovedStat());
		}

		public void XcraftCraftAttempted(bool success, string itemCategory, string itemName)
		{
			statsQueue.Enqueue(new XcraftCraftAttemptedStat
			{
				success = success,
				itemCategory = itemCategory,
				itemName = itemName
			});
		}

		public void ItemPlaced(string itemCategory, string itemName)
		{
			statsQueue.Enqueue(new ItemPlacedStat
			{
				itemCategory = itemCategory,
				itemName = itemName
			});
		}

		public void ItemUnlocked(string itemCategory, string itemName)
		{
			statsQueue.Enqueue(new ItemUnlockedStat
			{
				itemCategory = itemCategory,
				itemName = itemName
			});
		}

		public void ChestOpened(string rarity)
		{
			statsQueue.Enqueue(new ChestOpenedStat
			{
				rarity = rarity
			});
		}

		public void XcraftBlueprintCompleted(string blueprintId, bool wasInstant, float instantPercentageFilled)
		{
			statsQueue.Enqueue(new XcraftBlueprintCompletedStat
			{
				blueprintId = blueprintId,
				wasInstant = wasInstant,
				instantPercentageFilled = instantPercentageFilled
			});
		}

		public void MinigameFinished(MinigameType minigameId, bool success)
		{
			statsQueue.Enqueue(new MinigameFinishedStat
			{
				minigameId = minigameId.ToString().ToLower(),
				success = success
			});
		}

		public void MinigameFinished(string minigameId, bool success)
		{
			statsQueue.Enqueue(new MinigameFinishedStat
			{
				minigameId = minigameId.ToString().ToLower(),
				success = success
			});
		}

		public void AdventureQuestStarted(int questId)
		{
			statsQueue.Enqueue(new AdventureQuestStartedStat
			{
				questId = questId
			});
		}

		public void AdventureQuestFinished(int questId)
		{
			statsQueue.Enqueue(new AdventureQuestFinishedStat
			{
				questId = questId
			});
		}

		public void XcraftPlayerDeath(int wave)
		{
			statsQueue.Enqueue(new XcraftPlayerDeathStat
			{
				wave = wave
			});
		}

		public void LevelUp(int playerLevel)
		{
			statsQueue.Enqueue(new LevelUpStat
			{
				playerLevel = playerLevel
			});
		}

		public void RelationshipLevelUp(int relationshipLevel)
		{
			statsQueue.Enqueue(new RelationshipLevelUpStat
			{
				relationshipLevel = relationshipLevel
			});
		}

		public void PlayerMovement(int distance)
		{
			statsQueue.Enqueue(new PlayerMovementStat
			{
				distance = distance
			});
		}

		public void FarmingSow()
		{
			statsQueue.Enqueue(new FarmingSowStat());
		}

		public void FarmingHarvest()
		{
			statsQueue.Enqueue(new FarmingHarvestStat());
		}

		public void FarmingWater()
		{
			statsQueue.Enqueue(new FarmingWaterStat());
		}

		public void FarmingPlant(string id)
		{
			statsQueue.Enqueue(new FarmingPlantStat
			{
				plantId = id
			});
		}

		public void CommonWorldEntered(int id)
		{
			statsQueue.Enqueue(new CommonWorldEnteredStat
			{
				worldId = id
			});
		}

		public void ClickerPlayerBatch(int generator1Count, int generator2Count, int generator3Count, int generator4Count, int generator5Count, int generator6Count, int generator7Count, int generator8Count, int generator9Count, double softAmount, double hardAmount, double eliteAmount, int softResetCount, int clicksBase, int clicksNpcs, double softCoinsIncome)
		{
			statsQueue.Enqueue(new ClickerPlayerBatchStat
			{
				generator1Count = generator1Count,
				generator2Count = generator2Count,
				generator3Count = generator3Count,
				generator4Count = generator4Count,
				generator5Count = generator5Count,
				generator6Count = generator6Count,
				generator7Count = generator7Count,
				generator8Count = generator8Count,
				generator9Count = generator9Count,
				softAmount = softAmount,
				hardAmount = hardAmount,
				eliteAmount = eliteAmount,
				softResetCount = softResetCount,
				clicksBase = clicksBase,
				clicksNpcs = clicksNpcs,
				softCoinsIncome = softCoinsIncome
			});
		}

		public void ClickerSoftUpgradeBought(string itemId)
		{
			statsQueue.Enqueue(new ClickerSoftUpgradeBoughtStat
			{
				itemId = itemId
			});
		}

		public void ClickerEliteUpgradeBought(string itemId)
		{
			statsQueue.Enqueue(new ClickerEliteUpgradeBoughtStat
			{
				itemId = itemId
			});
		}

		public void ClickerHardItemBought(string itemId)
		{
			statsQueue.Enqueue(new ClickerHardItemBoughtStat
			{
				itemId = itemId
			});
		}

		public void IAPShopOpened()
		{
			statsQueue.Enqueue(new IAPShopOpenedStat());
		}

		public void LowpolyDistrictsVisited(int districtsCount, int currency, int currencyDelta)
		{
			statsQueue.Enqueue(new LowpolyDistrictsVisitedStat
			{
				districtsCount = districtsCount,
				currency = currency,
				currencyDelta = currencyDelta
			});
		}

		public void LowpolyMinigameFinished(string minigameId, bool success, int currency, int currencyDelta)
		{
			statsQueue.Enqueue(new LowpolyMinigameFinishedStat
			{
				minigameId = minigameId.ToString().ToLower(),
				success = success,
				currency = currency,
				currencyDelta = currencyDelta
			});
		}

		public void PlaceableBought(string placeableId, int placeableLevel, int currentGold, int delta, int currentWood, int woodDelta, int currentStone, int stoneDelta, int currentIron, int ironDelta)
		{
			statsQueue.Enqueue(new PlaceableBoughtStat
			{
				placeableId = placeableId,
				placeableLevel = placeableLevel,
				currency = currentGold,
				currencyDelta = delta,
				wood = currentWood,
				woodDelta = woodDelta,
				stone = currentStone,
				stoneDelta = stoneDelta,
				iron = currentIron,
				ironDelta = ironDelta
			});
		}

		public void PassivePlaceablePlaced()
		{
			statsQueue.Enqueue(new PassivePlaceablePlacedStat());
		}

		public void PlaceableUpgraded()
		{
			statsQueue.Enqueue(new PlaceableUpgradedStat());
		}

		public void StructurePlaced()
		{
			statsQueue.Enqueue(new StructurePlacedStat());
		}

		public void TeleportUsed(int districtId, string biomId, bool isHome, int currentGold, int delta)
		{
			if (biomId.Equals("OCEAN"))
			{
				biomId = "BEACH";
			}
			statsQueue.Enqueue(new TeleportUsedStat
			{
				districtId = districtId,
				biomId = biomId.ToLower(),
				currency = currentGold,
				currencyDelta = delta,
				isHome = isHome
			});
		}

		public void MapRewardClaimed(int currentGold, int delta)
		{
			statsQueue.Enqueue(new MapRewardClaimedStat
			{
				currency = currentGold,
				currencyDelta = delta
			});
		}

		public void PetTamed(string petName)
		{
			statsQueue.Enqueue(new PetTamedStat
			{
				petName = petName.ToLower()
			});
		}

		public void ResourceGathered()
		{
			statsQueue.Enqueue(new ResourceGatheredStat());
		}

		public void BankCollected(int currency, int currencyDelta)
		{
			statsQueue.Enqueue(new BankCollectedStat
			{
				currency = currency,
				currencyDelta = currencyDelta
			});
		}

		private void OnGUI()
		{
			if (Time.realtimeSinceStartup > nextPingTime)
			{
				nextPingTime = Time.realtimeSinceStartup + 10f;
				Ping();
			}
		}

		private void Update()
		{
			if (reporters == null)
			{
				return;
			}
			for (int i = 0; i < reporters.Count; i++)
			{
				if (reporters[i].UpdateAndCheck())
				{
					statsQueue.Enqueue(reporters[i].GetStat());
				}
			}
		}

		public bool TryToGetReporter<T>(out T reporter) where T : StatReporter
		{
			StatReporter reporter2 = GetReporter<T>();
			if (reporter2 != null)
			{
				reporter = (reporter2 as T);
				return true;
			}
			reporter = (T)null;
			return false;
		}

		public StatReporter GetReporter<T>() where T : StatReporter
		{
			return reporters.Find((StatReporter rep) => rep is T);
		}

		public void SendAll()
		{
			string queueJson = DequeueToJSON();
			StartCoroutine(stats.TrackStatsQueue(queueJson));
		}

		private string DequeueToJSON()
		{
			StringBuilder stringBuilder = new StringBuilder("{ \"body\" : [");
			bool flag = true;
			while (statsQueue.Count > 0)
			{
				if (!flag)
				{
					stringBuilder.Append(",\n");
				}
				Stat ob = statsQueue.Dequeue();
				stringBuilder.Append(JSONHelper.ToJSON(ob));
				flag = false;
			}
			stringBuilder.Append("] }");
			return stringBuilder.ToString();
		}
	}
}
