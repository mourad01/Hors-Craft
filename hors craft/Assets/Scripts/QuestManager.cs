// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestManager
using Common.Managers;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class QuestManager : Manager
{
	[Serializable]
	public class QuestWithWeight
	{
		public QuestType questType;

		public float questWeight;
	}

	private static HashSet<QuestType> blocksQuests = new HashSet<QuestType>
	{
		QuestType.PlaceBlocksX
	};

	private static HashSet<QuestType> resourcesQuests = new HashSet<QuestType>
	{
		QuestType.CollectResourcesX
	};

	private static HashSet<QuestType> animalQuests = new HashSet<QuestType>
	{
		QuestType.TameOfX
	};

	private static HashSet<QuestType> craftingQuests = new HashSet<QuestType>
	{
		QuestType.CraftOfX
	};

	public Sprite[] images = new Sprite[Enum.GetValues(typeof(QuestType)).Length];

	public Dictionary<int, Quest> questById;

	private Dictionary<QuestType, List<Quest>> questByType;

	private float timeOfLastJump;

	private HashSet<int> mobsWeHaveChattedWith = new HashSet<int>();

	private HashSet<int> mobsWeHavePetted = new HashSet<int>();

	private HashSet<int> mobsWeHaveDated = new HashSet<int>();

	public SpawnerPool indicatorPool;

	public GameObject questIndicatorPrefab;

	private Dictionary<string, List<GameObject>> indicatorConnectedToQuests = new Dictionary<string, List<GameObject>>();

	public QuestList globalWorldQuestList;

	public WorldsQuests worldsQuests;

	private float timeSinceLastQuestClaim;

	public bool useManagerQuests;

	public string[] questCraftableObjectTypes;

	[SerializeField]
	public QuestWithWeight[] questsWithWeights;

	public static ParameterType GetNeededParameter(QuestType type)
	{
		if (blocksQuests.Contains(type))
		{
			return ParameterType.Block;
		}
		if (resourcesQuests.Contains(type))
		{
			return ParameterType.Resource;
		}
		if (animalQuests.Contains(type))
		{
			return ParameterType.Animal;
		}
		if (craftingQuests.Contains(type))
		{
			return ParameterType.Craftable;
		}
		return ParameterType.None;
	}

	[ContextMenu("Print current quests")]
	private void PrintWorldQuests()
	{
		worldsQuests.PrintCurrentQuests();
	}

	public WorldsQuests GetCurrentWorldQuestsObject()
	{
		return worldsQuests;
	}

	public override void Init()
	{
		worldsQuests = new WorldsQuests();
		worldsQuests.Init();
		InitalizeWithQuestList(globalWorldQuestList);
		indicatorPool = new SpawnerPool("questIndicators", questIndicatorPrefab);
		Singleton<PlayerData>.get.playerQuests.ClearAndAddWorldQuestReadyForClaimListener(RemoveIndicatorsForQuests);
		if (!Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate() && useManagerQuests)
		{
			worldsQuests.FakeModelInit();
		}
	}

	public Quest GetQuestFromPrefab(QuestType type)
	{
		foreach (Quest quest in globalWorldQuestList.quests)
		{
			if (quest.type == type)
			{
				return quest;
			}
		}
		return null;
	}

	private void Update()
	{
		timeSinceLastQuestClaim += Time.deltaTime;
	}

	public void InitalizeWithQuestList(QuestList questList)
	{
		questById = new Dictionary<int, Quest>();
		questByType = new Dictionary<QuestType, List<Quest>>();
		mobsWeHaveChattedWith = new HashSet<int>();
		mobsWeHavePetted = new HashSet<int>();
		mobsWeHaveDated = new HashSet<int>();
		IEnumerator enumerator = Enum.GetValues(QuestType.CollectResourcesAny.GetType()).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				QuestType key = (QuestType)enumerator.Current;
				questByType[key] = new List<Quest>();
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		int num = 0;
		if (!(questList == null))
		{
			foreach (Quest quest in questList.quests)
			{
				quest.id = num;
				questById.Add(num, quest);
				questByType[quest.type].Add(quest);
				num++;
			}
			globalWorldQuestList = questList;
		}
	}

	public QuestType GetQuestTypeById(int id)
	{
		return questById[id].type;
	}

	public List<Quest> GetCurrentWorldQuests()
	{
		if (worldsQuests == null)
		{
			return new List<Quest>();
		}
		return worldsQuests.GetCurrentQuests();
	}

	public List<Quest> GetQuestList()
	{
		if (globalWorldQuestList == null)
		{
			return null;
		}
		return globalWorldQuestList.quests;
	}

	public void ClaimQuest(Quest quest)
	{
		if (worldsQuests.OnQuestClaim(quest.GenerateWorldId()))
		{
			CheckMobsIndicatorState();
			timeSinceLastQuestClaim = 0f;
		}
	}

	public Sprite GetImage(int index)
	{
		return globalWorldQuestList.images[index];
	}

	public Sprite GetImageForQuest(int questId)
	{
		if (!questById.ContainsKey(questId))
		{
			return null;
		}
		Quest quest = questById[questId];
		switch (quest.type)
		{
		case QuestType.CollectResourcesX:
			return Manager.Get<CraftingManager>().GetResourceImage(quest.paramatersInt[0]);
		case QuestType.PlaceBlocksX:
			return VoxelSprite.GetVoxelSprite((ushort)quest.paramatersInt[0]);
		case QuestType.TameOfX:
			return Manager.Get<MobsManager>().GetMobSprite(quest.paramatersInt[0]);
		case QuestType.CraftOfX:
			return Manager.Get<CraftingManager>().GetCraftable(quest.paramatersInt[0]).GetGraphic();
		default:
			return GetDefaultImage(quest);
		}
	}

	private Sprite GetDefaultImage(Quest quest)
	{
		if (quest.paramatersInt == null || quest.paramatersInt.Count == 0)
		{
			return globalWorldQuestList.images[(int)quest.type];
		}
		int num = quest.paramatersInt[0];
		if (num >= globalWorldQuestList.images.Length)
		{
			return null;
		}
		return globalWorldQuestList.images[num];
	}

	public void OnBlockDestroyed(ushort blockId)
	{
		IncreaseQuestOfType(QuestType.DestroyX);
	}

	[ContextMenu("Call OnEnemykilled")]
	public void OnEnemyKilled()
	{
		if (Manager.Contains<SurvivalRankManager>())
		{
			Manager.Get<SurvivalRankManager>().Increase("Kill");
		}
		else
		{
			IncreaseQuestOfType(QuestType.KillX);
		}
	}

	public void OnBlockPut(ushort blockId)
	{
		IncreaseQuestOfType(QuestType.PlaceBlocksAny);
		IncreaseQuestOfTypeParametric(QuestType.PlaceBlocksX, blockId);
		StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.BLOCK_PLACED);
		if (MonoBehaviourSingleton<BlocksCategories>.get.categories != null)
		{
			MonoBehaviourSingleton<BlocksCategories>.get.GetCategory(Engine.GetVoxelType(blockId))?.reward.ClaimReward();
		}
	}

	public void OnJump()
	{
		if (timeOfLastJump + 1f < Time.time)
		{
			timeOfLastJump = Time.time;
			IncreaseQuestOfType(QuestType.JumpXTimes);
		}
	}

	public void OnItemCraft(int itemId)
	{
		IncreaseQuestOfType(QuestType.CraftAny);
		IncreaseQuestOfTypeParametric(QuestType.CraftOfX, itemId);
	}

	public void OnFall(int meters)
	{
		if (meters > 0)
		{
			SetQuestOfTypeIfBigger(QuestType.FallX, meters);
		}
	}

	public void OnChat(int mobId)
	{
		IncreaseQuestOfType(QuestType.ChatXTimes);
		if (!mobsWeHaveChattedWith.Contains(mobId))
		{
			mobsWeHaveChattedWith.Add(mobId);
			IncreaseQuestOfType(QuestType.ChatXPeople);
		}
	}

	public void OnPetHuman(int mobId)
	{
		IncreaseQuestOfType(QuestType.TalkXTimes);
		if (!mobsWeHavePetted.Contains(mobId))
		{
			mobsWeHavePetted.Add(mobId);
			IncreaseQuestOfType(QuestType.TalkXPeople);
		}
	}

	public void OnDateHuman(int mobId)
	{
		IncreaseQuestOfType(QuestType.DateXTimes);
		if (!mobsWeHaveDated.Contains(mobId))
		{
			mobsWeHaveDated.Add(mobId);
			IncreaseQuestOfType(QuestType.DateXPeople);
		}
	}

	public void OnPetThePet()
	{
		IncreaseQuestOfType(QuestType.PetXTimes);
	}

	public void OnPetTame(string name)
	{
		IncreaseQuestOfType(QuestType.Tame);
		IncreaseQuestOfTypeParametric(QuestType.TameOfX, name);
	}

	public void OnXCollectionPoints(int points)
	{
		SetQuestOfType(QuestType.XCollectionPoints, points);
	}

	public void UnlockSilverRod()
	{
		IncreaseQuestOfType(QuestType.UnlockSilverRod);
	}

	public void FishXTimes()
	{
		IncreaseQuestOfType(QuestType.FishXTimes);
	}

	public void CatchedFishBeingOnBoat()
	{
		IncreaseQuestOfType(QuestType.CatchXFishesBeingOnBoat);
	}

	public void CatchedFishBeingOnMotorboat()
	{
		IncreaseQuestOfType(QuestType.CatchXFishesBeingOnMotorboat);
	}

	public void CatchedFishBeingOnYachtBoat()
	{
		IncreaseQuestOfType(QuestType.CatchXFishesBeingOnYachtBoat);
	}

	public void CatchedFishBeingOnFishingBoat()
	{
		IncreaseQuestOfType(QuestType.CatchXFishesBeingOnFishingBoat);
	}

	public void CatchedFishBeingOnTouristBoat()
	{
		IncreaseQuestOfType(QuestType.CatchXFishesBeingOnTouristBoat);
	}

	public void OnLegendaryCatched()
	{
		IncreaseQuestOfType(QuestType.CatchLegendaryFish);
	}

	public void OnEpicCatched()
	{
		IncreaseQuestOfType(QuestType.CatchEpicFish);
	}

	public void OnRareCatched()
	{
		IncreaseQuestOfType(QuestType.CatchRareFish);
	}

	public void OnMarlinCatched()
	{
		IncreaseQuestOfType(QuestType.CatchMarlinFish);
	}

	public void OnStingrayCatched()
	{
		IncreaseQuestOfType(QuestType.CatchStingrayFish);
	}

	public void OnOctopusCatched()
	{
		IncreaseQuestOfType(QuestType.CatchOctopusFish);
	}

	public void OnBlockUnlock()
	{
		IncreaseQuestOfType(QuestType.UnlockBlocks);
	}

	public void OnResourcePickedUp(int resourceId)
	{
		IncreaseQuestOfType(QuestType.CollectResourcesAny);
		IncreaseQuestOfTypeParametric(QuestType.CollectResourcesX, resourceId);
	}

	public void GetXLevel()
	{
		IncreaseQuestOfType(QuestType.GetXLevel);
	}

	public void OnNpcReaction()
	{
		IncreaseQuestOfType(QuestType.GetXNpcReactions);
	}

	public void OnMinigameWon()
	{
		IncreaseQuestOfType(QuestType.WinCatWalkFlawlessy);
	}

	public void SetQuestOfTypeIfBigger(QuestType type, int value)
	{
		if (questByType != null)
		{
			foreach (Quest item in questByType[type])
			{
				if (value > Singleton<PlayerData>.get.playerQuests.GetQuestProgress(item.id))
				{
					Singleton<PlayerData>.get.playerQuests.SetQuestProgress(item.id, value);
				}
			}
			worldsQuests.GetCurrentQuests().ForEach(delegate(Quest quest)
			{
				if (quest.type == type && value > Singleton<PlayerData>.get.playerQuests.GetWorldQuestProgress(quest.GenerateWorldId()))
				{
					Singleton<PlayerData>.get.playerQuests.SetWorldQuestProgress(quest, value);
				}
			});
		}
	}

	private bool CheckInit()
	{
		if (questByType == null)
		{
			UnityEngine.Debug.LogWarning("Quest are not initialized");
			return false;
		}
		return true;
	}

	public void SetQuestOfType(QuestType type, int value)
	{
		if (questByType != null)
		{
			foreach (Quest item in questByType[type])
			{
				Singleton<PlayerData>.get.playerQuests.SetQuestProgress(item.id, value);
			}
			worldsQuests.GetCurrentQuests().ForEach(delegate(Quest quest)
			{
				if (quest.type == type)
				{
					Singleton<PlayerData>.get.playerQuests.SetWorldQuestProgress(quest, value);
				}
			});
		}
	}

	public virtual void IncreaseQuestOfType(QuestType type, int inc = 1)
	{
		MonoBehaviourSingleton<ProgressCounter>.get.Add(type.ToString(), inc);
		if (questByType != null)
		{
			foreach (Quest item in questByType[type])
			{
				Singleton<PlayerData>.get.playerQuests.InformOnQuestIncrease(item.id, inc);
			}
			worldsQuests.GetCurrentQuests().ForEach(delegate(Quest quest)
			{
				if (quest.type == type)
				{
					Singleton<PlayerData>.get.playerQuests.OnWorldQuestIncreased(quest, inc);
				}
			});
		}
	}

	public virtual void OnWorldChange(string id)
	{
		worldsQuests.LoadQuestListForWorld(id);
	}

	public void IncreaseQuestOfTypeParametric(QuestType type, int param)
	{
		if (questByType != null)
		{
			foreach (Quest item in questByType[type])
			{
				if (item.paramatersInt.Count > 0 && item.paramatersInt[0] == param)
				{
					Singleton<PlayerData>.get.playerQuests.InformOnQuestIncrease(item.id, 1);
				}
			}
			worldsQuests.GetCurrentQuests().ForEach(delegate(Quest quest)
			{
				if (quest.type == type && quest.paramatersInt.Count > 0 && quest.paramatersInt[0] == param)
				{
					Singleton<PlayerData>.get.playerQuests.OnWorldQuestIncreased(quest, 1);
				}
			});
		}
	}

	public void IncreaseQuestOfTypeParametric(QuestType type, string param)
	{
		if (questByType != null)
		{
			foreach (Quest item in questByType[type])
			{
				if (item.paramatersString[0] == param)
				{
					Singleton<PlayerData>.get.playerQuests.InformOnQuestIncrease(item.id, 1);
				}
			}
			worldsQuests.GetCurrentQuests().ForEach(delegate(Quest quest)
			{
				if (quest.type == type && quest.paramatersString[0] == param)
				{
					Singleton<PlayerData>.get.playerQuests.OnWorldQuestIncreased(quest, 1);
				}
			});
		}
	}

	public void ReRollWorldQuest()
	{
		worldsQuests.GenerateNewQuestList(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
	}

	public void PassCurrentQuests()
	{
		foreach (Quest currentQuest in worldsQuests.GetCurrentQuests())
		{
			Singleton<PlayerData>.get.playerQuests.SetWorldQuestProgress(currentQuest, currentQuest.stepsNeeded + 10);
		}
	}

	private Quest ShouldHumanHaveIndicator()
	{
		Quest toReturn = null;
		GetCurrentWorldQuests().ForEach(delegate(Quest quest)
		{
			Quest quest2 = null;
			if (quest.type == QuestType.DanceXTimes)
			{
				quest2 = quest;
			}
			else if (quest.type == QuestType.ChatXPeople)
			{
				quest2 = quest;
			}
			else if (quest.type == QuestType.ChatXTimes)
			{
				quest2 = quest;
			}
			if (quest2 != null)
			{
				int worldQuestProgress = Singleton<PlayerData>.get.playerQuests.GetWorldQuestProgress(quest.GenerateWorldId());
				if (worldQuestProgress < quest.stepsNeeded)
				{
					toReturn = quest;
				}
			}
		});
		return toReturn;
	}

	private Quest ShouldMobHaveIndicator(string animalId, bool tamed)
	{
		Quest toReturn = null;
		GetCurrentWorldQuests().ForEach(delegate(Quest quest)
		{
			Quest quest2 = null;
			if (GetNeededParameter(quest.type) == ParameterType.Animal && !tamed)
			{
				if (quest.paramatersString[0].Equals(animalId))
				{
					quest2 = quest;
				}
			}
			else if (quest.type == QuestType.Tame && !tamed)
			{
				quest2 = quest;
			}
			else if (quest.type == QuestType.PetXTimes)
			{
				quest2 = quest;
			}
			if (quest2 != null)
			{
				int worldQuestProgress = Singleton<PlayerData>.get.playerQuests.GetWorldQuestProgress(quest.GenerateWorldId());
				if (worldQuestProgress < quest.stepsNeeded)
				{
					toReturn = quest;
				}
			}
		});
		return toReturn;
	}

	public void ReturnIndicator(GameObject indicator)
	{
		indicatorPool.Despawn(indicator);
	}

	public void RemoveIndicatorsForQuests(Quest quest)
	{
		string key = quest.GenerateWorldId();
		if (indicatorConnectedToQuests.ContainsKey(key))
		{
			indicatorConnectedToQuests[key].ForEach(delegate(GameObject element)
			{
				indicatorPool.Despawn(element);
			});
			indicatorConnectedToQuests.Remove(key);
		}
	}

	public GameObject GetNewIndicatorObject(string connectedQuestId)
	{
		GameObject gameObject = indicatorPool.Spawn(Vector3.zero, Quaternion.identity);
		if (!indicatorConnectedToQuests.ContainsKey(connectedQuestId))
		{
			indicatorConnectedToQuests.Add(connectedQuestId, new List<GameObject>());
		}
		indicatorConnectedToQuests[connectedQuestId].Add(gameObject);
		return gameObject;
	}

	public bool IsPauseFragmentNotified(string pause)
	{
		return worldsQuests.IsFragmentNotified(pause);
	}

	public bool IsThisBlockNotified(int blockId)
	{
		return worldsQuests.IsThisNotified(blockId, ParameterType.Block);
	}

	public bool IsThisCraftableNotified(int craftableId)
	{
		return worldsQuests.IsThisNotified(craftableId, ParameterType.Craftable);
	}

	public bool IsThisResourceNotified(int resourceId)
	{
		return worldsQuests.IsThisNotified(resourceId, ParameterType.Resource);
	}

	public void CheckMobsIndicatorState()
	{
		Manager.Get<MobsManager>().spawnedMobs.ForEach(HandleMobIndicator);
	}

	public void HandleMobIndicator(GameObject mob)
	{
		AnimalMob component = mob.GetComponent<AnimalMob>();
		HumanMob component2 = mob.GetComponent<HumanMob>();
		if (component != null && component2 != null)
		{
			HandleIndicatorsForHuman(component2);
		}
		else if (component != null)
		{
			HandleIndicatorsForAnimal(component);
		}
	}

	private void HandleIndicatorsForAnimal(AnimalMob animal)
	{
		Quest quest = ShouldMobHaveIndicator(animal.name, animal.IsTamed());
		if (quest != null && !animal.mountMode)
		{
			animal.SetQuestIndicator(quest);
		}
		else
		{
			animal.RemoveQuestIndicator();
		}
	}

	private void HandleIndicatorsForHuman(HumanMob human)
	{
		Quest quest = ShouldHumanHaveIndicator();
		if (quest != null)
		{
			human.SetQuestIndicator(quest);
		}
		else
		{
			human.RemoveQuestIndicator();
		}
	}

	public string GetQuestTitle(int questId)
	{
		Quest quest = questById[questId];
		return Manager.Get<TranslationsManager>().GetText($"quests.title.{quest.type.ToString().ToLower()}", getQuestTitleHardCoded(quest.type));
	}

	public string GetQuestDescription(QuestType type, int valueToSet)
	{
		string text = Manager.Get<TranslationsManager>().GetText($"quests.description.{type.ToString().ToLower()}", getQuestDescriptionHardCoded(type));
		return string.Format(text, valueToSet);
	}

	public string GetQuestDescription(int questId)
	{
		Quest quest = questById[questId];
		string text = Manager.Get<TranslationsManager>().GetText($"quests.description.{quest.type.ToString().ToLower()}", getQuestDescriptionHardCoded(quest.type));
		return string.Format(text, Singleton<PlayerData>.get.playerQuests.GetValueNeededToUnlocks(quest.id));
	}

	public Quest GetQuestId(int id)
	{
		if (questById.ContainsKey(id))
		{
			return questById[id];
		}
		return null;
	}

	public Quest GetWorldQuestById(int id)
	{
		return worldsQuests.GetQuestById(id);
	}

	private string getQuestTitleHardCoded(QuestType type)
	{
		switch (type)
		{
		case QuestType.CollectResourcesAny:
			return "Gather blocks";
		case QuestType.PlaceBlocksAny:
			return "Place blocks";
		case QuestType.CollectResourcesX:
			return "Gather resources";
		case QuestType.PlaceBlocksX:
			return "Place blocks";
		case QuestType.MountDistance:
			return "Ride an animal";
		case QuestType.FlyDistance:
			return "Fly";
		case QuestType.WalkDistance:
			return "Walk";
		case QuestType.Tame:
			return "Tame animals";
		case QuestType.PetXTimes:
			return "Pet animals";
		case QuestType.TameOfX:
			return "Tame animals";
		case QuestType.DestroyX:
			return "Destroy";
		case QuestType.FallX:
			return "Fall";
		case QuestType.ReachHeight:
			return "Reach.";
		case QuestType.UnlockBlocks:
			return "Unlock recipes";
		case QuestType.VehicleDistance:
			return "Travel in a vehicle";
		case QuestType.KillX:
			return "Kill";
		case QuestType.FishXTimes:
			return "Go fish";
		case QuestType.UnlockSilverRod:
			return "Unlock sliver rod.";
		case QuestType.CatchXFishesBeingOnBoat:
			return "Catch fish being on boat.";
		case QuestType.CatchXFishesBeingOnMotorboat:
			return "Catch fish being on motorboat.";
		case QuestType.CatchLegendaryFish:
			return "Catch legendary fish.";
		case QuestType.CatchEpicFish:
			return "Catch epic fish.";
		case QuestType.CatchRareFish:
			return "Catch rare fish.";
		case QuestType.CatchMarlinFish:
			return "Catch Marlin.";
		case QuestType.CatchOctopusFish:
			return "Catch Octopus.";
		case QuestType.CatchStingrayFish:
			return "Catch Stingray.";
		case QuestType.XCollectionPoints:
			return "Collection points value.";
		case QuestType.ChatXTimes:
			return "Chat.";
		case QuestType.ChatXPeople:
			return "Chat with diffrent people";
		case QuestType.TalkXTimes:
			return "Be Friendly";
		case QuestType.TalkXPeople:
			return "Be Friendly to diffrent people";
		case QuestType.DateXTimes:
			return "Date";
		case QuestType.DateXPeople:
			return "Date with diffrent people";
		case QuestType.PlayInstrumentXTimes:
			return "Play instrument";
		case QuestType.SwimDistance:
			return "Swim";
		case QuestType.JumpXTimes:
			return "Jump";
		case QuestType.FindTreasureXTimes:
			return "Find Treasure";
		case QuestType.BuyXClothes:
			return "Buy clothes";
		case QuestType.CraftAny:
			return "Craft item";
		case QuestType.CraftOfX:
			return "Craft item";
		case QuestType.CatchXFishesBeingOnYachtBoat:
			return "Catch fish being on sailboat.";
		case QuestType.CatchXFishesBeingOnFishingBoat:
			return "Catch fish being on fishingboat.";
		case QuestType.CatchXFishesBeingOnTouristBoat:
			return "Catch fish being on touristboat.";
		case QuestType.DanceXTimes:
			return "Dance";
		case QuestType.GetXLevel:
			return "Get level";
		default:
			return string.Empty;
		}
	}

	private string getQuestDescriptionHardCoded(QuestType type)
	{
		switch (type)
		{
		case QuestType.CollectResourcesAny:
			return "Gather {0} resources";
		case QuestType.PlaceBlocksAny:
			return "Place {0} blocks";
		case QuestType.CollectResourcesX:
			return "Gather {0} resources";
		case QuestType.PlaceBlocksX:
			return "Place {0} blocks";
		case QuestType.MountDistance:
			return "Travel {0} ft. on mount";
		case QuestType.FlyDistance:
			return "Fly {0} ft.";
		case QuestType.WalkDistance:
			return "Walk {0} ft.";
		case QuestType.Tame:
			return "Tame {0} animals";
		case QuestType.PetXTimes:
			return "Pet animals {0} times";
		case QuestType.TameOfX:
			return "Tame {0} animals";
		case QuestType.DestroyX:
			return "Destroy {0} blocks";
		case QuestType.FallX:
			return "Fall from {0} ft.";
		case QuestType.ReachHeight:
			return "Reach {0} ft.";
		case QuestType.UnlockBlocks:
			return "Unlock {0} recipes";
		case QuestType.VehicleDistance:
			return "Travel {0} ft. in a vehicle";
		case QuestType.KillX:
			return "Kill enemies {0} times";
		case QuestType.FishXTimes:
			return "Go fish {0} times";
		case QuestType.UnlockSilverRod:
			return "Unlock sliver rod.";
		case QuestType.CatchXFishesBeingOnBoat:
			return "Catch fish being on boat.";
		case QuestType.CatchXFishesBeingOnMotorboat:
			return "Catch fish being on motorboat.";
		case QuestType.CatchLegendaryFish:
			return "Catch legendary fish.";
		case QuestType.CatchEpicFish:
			return "Catch epic fish.";
		case QuestType.CatchRareFish:
			return "Catch rare fish.";
		case QuestType.CatchMarlinFish:
			return "Catch Marlin.";
		case QuestType.CatchOctopusFish:
			return "Catch Octopus.";
		case QuestType.CatchStingrayFish:
			return "Catch Stingray.";
		case QuestType.XCollectionPoints:
			return "Collection points value.";
		case QuestType.ChatXTimes:
			return "Chat {0} times.";
		case QuestType.ChatXPeople:
			return "Chat with {0} people.";
		case QuestType.TalkXTimes:
			return "Be Friendly {0} times.";
		case QuestType.TalkXPeople:
			return "Be Friendly to {0} people.";
		case QuestType.DateXTimes:
			return "Date {0} times.";
		case QuestType.DateXPeople:
			return "Date with {0} people.";
		case QuestType.PlayInstrumentXTimes:
			return "Play instrument {0} times.";
		case QuestType.SwimDistance:
			return "Swim {0} ft.";
		case QuestType.JumpXTimes:
			return "Jump {0} times.";
		case QuestType.FindTreasureXTimes:
			return "Find {0} Treasures.";
		case QuestType.BuyXClothes:
			return "Buy {0} clothes.";
		case QuestType.CraftAny:
			return "Craft {0} times.";
		case QuestType.CraftOfX:
			return "Craft {0} items.";
		case QuestType.CatchXFishesBeingOnYachtBoat:
			return "Catch fish being on sailboat.";
		case QuestType.CatchXFishesBeingOnFishingBoat:
			return "Catch fish being on fishingboat.";
		case QuestType.CatchXFishesBeingOnTouristBoat:
			return "Catch fish being on touristboat.";
		case QuestType.DanceXTimes:
			return "Dance {0} times";
		case QuestType.GetXLevel:
			return "Get {0} more levels.";
		case QuestType.GetXWardrobeValue:
			return "Get {0} in glamour value.";
		case QuestType.GetXNpcReactions:
			return "Get {0} NPC reactions.";
		case QuestType.WinCatWalkFlawlessy:
			return "Win catwalk minigame {0} times flawlessly.";
		default:
			return string.Empty;
		}
	}
}
