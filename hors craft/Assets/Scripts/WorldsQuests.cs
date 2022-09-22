// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldsQuests
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[Serializable]
public class WorldsQuests
{
	[Serializable]
	public class QuestValueGenerator
	{
		[SerializeField]
		protected int req_a;

		[SerializeField]
		protected int req_b;

		[SerializeField]
		protected int price_a;

		[SerializeField]
		protected int price_b;

		public QuestValueGenerator()
		{
		}

		public QuestValueGenerator(Dictionary<string, object> data)
		{
			req_a = int.Parse(data["req_a"].ToString());
			req_b = int.Parse(data["req_b"].ToString());
			price_a = int.Parse(data["prize_a"].ToString());
			price_b = int.Parse(data["prize_b"].ToString());
		}

		public QuestValueGenerator(int req_a, int req_b, int price_a, int price_b)
		{
			this.req_a = req_a;
			this.req_b = req_b;
			this.price_a = price_a;
			this.price_b = price_b;
		}

		public virtual int GetPriceForQuest(int questIndex)
		{
			return price_a * questIndex + price_b;
		}

		public virtual int GetStepsForQuest(int questIndex)
		{
			return req_a * questIndex + req_b;
		}
	}

	public class QuestWorldDistribution
	{
		private Dictionary<QuestType, float> questWeights;

		private Dictionary<QuestType, float> weightsDistribution;

		private float sumOfWeights;

		public QuestWorldDistribution()
		{
		}

		public QuestWorldDistribution(Dictionary<string, object> weights)
		{
			questWeights = new Dictionary<QuestType, float>();
			foreach (KeyValuePair<string, object> weight in weights)
			{
				float result = 0f;
				if (float.TryParse(weight.Value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				{
					QuestType questType = (QuestType)Enum.Parse(typeof(QuestType), weight.Key);
					if (result > 0f && questType != QuestType.None)
					{
						questWeights.Add(questType, result);
					}
				}
			}
		}

		public QuestWorldDistribution(QuestManager questManager)
		{
			questWeights = new Dictionary<QuestType, float>();
			if (questManager.questsWithWeights != null)
			{
				for (int i = 0; i < questManager.questsWithWeights.Length; i++)
				{
					questWeights.Add(questManager.questsWithWeights[i].questType, questManager.questsWithWeights[i].questWeight);
				}
			}
		}

		public static QuestWorldDistribution GetDistribution(QuestManager questManager)
		{
			if (questManager.questsWithWeights != null)
			{
				return new QuestWorldDistribution(questManager);
			}
			return CreateEqualDistribution();
		}

		public static QuestWorldDistribution CreateEqualDistribution()
		{
			QuestWorldDistribution questWorldDistribution = new QuestWorldDistribution();
			questWorldDistribution.questWeights = new Dictionary<QuestType, float>();
			IEnumerator enumerator = Enum.GetValues(typeof(QuestType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					QuestType questType = (QuestType)enumerator.Current;
					QuestValueGenerator generator = GetGenerator(questType);
					if (generator != null)
					{
						questWorldDistribution.questWeights.Add(questType, 1f);
					}
				}
				return questWorldDistribution;
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void PrepareWeightsDistribution(QuestListData questList)
		{
			sumOfWeights = 0f;
			weightsDistribution = new Dictionary<QuestType, float>();
			foreach (KeyValuePair<QuestType, float> questWeight in questWeights)
			{
				if (questList == null || !questList.IsActiveQuestOfType(questWeight.Key))
				{
					sumOfWeights += questWeight.Value;
					weightsDistribution.Add(questWeight.Key, sumOfWeights);
				}
			}
		}

		public void GenerateAndAddQuestToList(QuestListData questList, WorldsParameters parameters)
		{
			PrepareWeightsDistribution(questList);
			QuestType typeToCreate = GetTypeToCreate();
			QuestValueGenerator questValueGenerator = GetGenerator(typeToCreate);
			if (questValueGenerator == null)
			{
				questValueGenerator = new QuestValueGenerator(1, 1, 50, 50);
			}
			int num = 0;
			num = questList.GetMaxQueueNumber() + 1;
			if (num < 0)
			{
				num = 0;
			}
			Quest quest = new Quest(typeToCreate, questValueGenerator.GetPriceForQuest(num), questValueGenerator.GetStepsForQuest(num), num);
			AddParameterToQuest(quest, parameters);
			questList.AddQuest(quest);
		}

		protected void AddParameterToQuest(Quest quest, WorldsParameters parameters)
		{
			if (parameters == null)
			{
				return;
			}
			switch (QuestManager.GetNeededParameter(quest.type))
			{
			case ParameterType.Block:
				quest.paramatersInt.Add(parameters.GetRandomBlock());
				break;
			case ParameterType.Animal:
				quest.paramatersString.Add(parameters.GetRandomAnimal());
				break;
			case ParameterType.Resource:
				quest.paramatersInt.Add(parameters.GetRandomResource());
				break;
			case ParameterType.Craftable:
			{
				int item = parameters.GetRandomCraftable();
				if (Manager.Get<QuestManager>().useManagerQuests && Manager.Get<QuestManager>().questCraftableObjectTypes.Length > 0)
				{
					item = GetIndexOfQuestManagerTypes(parameters);
				}
				quest.paramatersInt.Add(item);
				break;
			}
			}
		}

		private static int GetIndexOfQuestManagerTypes(WorldsParameters parameters)
		{
			int randomCraftable;
			while (true)
			{
				CraftingManager craftingManager = Manager.Get<CraftingManager>();
				randomCraftable = parameters.GetRandomCraftable();
				CraftableList craftableListInstance = craftingManager.GetCraftableListInstance();
				if ((bool)craftableListInstance)
				{
					Craftable craftable = craftableListInstance.GetCraftable(randomCraftable);
					if (craftable != null && (bool)craftable.customCraftableObject && IsObjectOfQuestType(craftable))
					{
						break;
					}
				}
			}
			return randomCraftable;
		}

		private static bool IsObjectOfQuestType(Craftable craftable)
		{
			string[] questCraftableObjectTypes = Manager.Get<QuestManager>().questCraftableObjectTypes;
			foreach (string type in questCraftableObjectTypes)
			{
				if ((bool)craftable.customCraftableObject.GetComponent(type))
				{
					return true;
				}
			}
			return false;
		}

		public QuestType GetTypeToCreate()
		{
			float num = UnityEngine.Random.Range(0f, sumOfWeights);
			foreach (KeyValuePair<QuestType, float> item in weightsDistribution)
			{
				if (num <= item.Value)
				{
					return item.Key;
				}
			}
			return QuestType.None;
		}

		public void Test()
		{
			Dictionary<QuestType, int> dictionary = new Dictionary<QuestType, int>();
			for (int i = 0; i < 10000; i++)
			{
				PrepareWeightsDistribution(null);
				QuestType typeToCreate = GetTypeToCreate();
				if (dictionary.ContainsKey(typeToCreate))
				{
					dictionary[typeToCreate]++;
				}
				else
				{
					dictionary[typeToCreate] = 1;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<QuestType, int> item in dictionary)
			{
				stringBuilder.Append(item.Key);
				stringBuilder.Append(" ");
				stringBuilder.Append(item.Value);
				stringBuilder.Append(Environment.NewLine);
			}
			UnityEngine.Debug.LogError(stringBuilder.ToString());
		}

		public int GetCount()
		{
			return questWeights.Count;
		}
	}

	[Serializable]
	public class WorldsParameters
	{
		public string world_id;

		public List<string> animals;

		public string resources_list;

		public string blocks_list;

		public string craftable_list;

		public List<int> resourcesList;

		public List<int> blocksList;

		public List<int> craftableList;

		[CompilerGenerated]
		private static Func<string, int> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Func<string, int> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Func<string, int> _003C_003Ef__mg_0024cache2;

		public void Init()
		{
			resourcesList = resources_list.SplitToList(',', int.Parse);
			blocksList = blocks_list.SplitToList(',', int.Parse);
			craftableList = craftable_list.SplitToList(',', int.Parse);
		}

		public int GetRandomBlock()
		{
			return blocksList.GetRandomItem();
		}

		public int GetRandomCraftable()
		{
			return blocksList.GetRandomItem();
		}

		public int GetRandomResource()
		{
			return resourcesList.GetRandomItem();
		}

		public string GetRandomAnimal()
		{
			animals.RemoveAll(delegate(string mobName)
			{
				GameObject mobByName = Manager.Get<MobsManager>().mobsContainer.GetMobByName(mobName);
				return (mobByName != null) ? ((mobByName.GetComponent<HumanMob>() != null || mobByName.GetComponent<Pettable>() == null) ? true : false) : true;
			});
			return animals.GetRandomItem();
		}
	}

	public static int numberOfActiveQuests = 3;

	private static Dictionary<QuestType, QuestValueGenerator> questGenerators;

	private static Dictionary<string, QuestWorldDistribution> questDistributions;

	private static QuestWorldDistribution equalDistribution;

	private static Dictionary<string, WorldsParameters> questWorldParameters;

	protected QuestListData currentWorldQuestList;

	private HashSet<int> blockToBeNotified;

	private HashSet<int> resourcesToBeNotified;

	private HashSet<int> craftableToBeNotified;

	private HashSet<string> pauseFragmentsToNotify;

	public virtual void Init()
	{
		equalDistribution = QuestWorldDistribution.CreateEqualDistribution();
	}

	public virtual List<Quest> GetCurrentQuests()
	{
		if (currentWorldQuestList == null || currentWorldQuestList.quests == null)
		{
			return new List<Quest>();
		}
		return currentWorldQuestList.quests;
	}

	public static void AddNewDistribution(string worldId)
	{
		if (questDistributions == null)
		{
			questDistributions = new Dictionary<string, QuestWorldDistribution>();
		}
		questDistributions[worldId] = QuestWorldDistribution.GetDistribution(Manager.Get<QuestManager>());
	}

	public void PrintCurrentQuests()
	{
		if (currentWorldQuestList != null && currentWorldQuestList.quests != null)
		{
			UnityEngine.Debug.Log("#######");
			foreach (Quest quest in currentWorldQuestList.quests)
			{
				UnityEngine.Debug.Log($"type: {quest.type}, steps Needed: {quest.stepsNeeded}, current level {quest.CurrentLevel}, maxLevel: {quest.maxLevel}");
			}
			UnityEngine.Debug.Log("#######");
		}
	}

	private void OnError(string error)
	{
		UnityEngine.Debug.LogError(error);
	}

	public static QuestValueGenerator GetGenerator(QuestType type)
	{
		if (questGenerators != null && questGenerators.ContainsKey(type))
		{
			return questGenerators[type];
		}
		return null;
	}

	public void FakeModelInit()
	{
		questGenerators = new Dictionary<QuestType, QuestValueGenerator>();
		questDistributions = new Dictionary<string, QuestWorldDistribution>();
		questWorldParameters = new Dictionary<string, WorldsParameters>();
		LoadQuestListForWorld("xcraft.world1");
	}

	public void FakeModelInitCustom(Dictionary<QuestType, QuestValueGenerator> customGenerators)
	{
		questGenerators = customGenerators;
		questDistributions = new Dictionary<string, QuestWorldDistribution>();
		questWorldParameters = new Dictionary<string, WorldsParameters>();
		LoadQuestListForWorld("xcraft.world1");
	}

	public virtual void OnDownloadModel(Dictionary<string, object> baseModel)
	{
		questGenerators = new Dictionary<QuestType, QuestValueGenerator>();
		questDistributions = new Dictionary<string, QuestWorldDistribution>();
		questWorldParameters = new Dictionary<string, WorldsParameters>();
		Dictionary<string, object> dictionary = baseModel["questList"] as Dictionary<string, object>;
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			Dictionary<string, object> data = item.Value as Dictionary<string, object>;
			QuestType key = (QuestType)Enum.Parse(typeof(QuestType), item.Key);
			questGenerators.Add(key, new QuestValueGenerator(data));
		}
		Dictionary<string, object> dictionary2 = baseModel["questDistribution"] as Dictionary<string, object>;
		foreach (KeyValuePair<string, object> item2 in dictionary2)
		{
			QuestWorldDistribution value = (!Manager.Get<QuestManager>().useManagerQuests) ? new QuestWorldDistribution(item2.Value as Dictionary<string, object>) : new QuestWorldDistribution(Manager.Get<QuestManager>());
			questDistributions.Add(item2.Key, value);
		}
		List<object> list = baseModel["questParams"] as List<object>;
		foreach (object item3 in list)
		{
			WorldsParameters worldsParameters = JSONHelper.Deserialize<WorldsParameters>(item3);
			worldsParameters.Init();
			questWorldParameters.Add(worldsParameters.world_id, worldsParameters);
		}
		LoadQuestListForWorld(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
	}

	protected string questListWorldKey(string worldId)
	{
		return $"quest.list.{worldId}";
	}

	public Quest GetQuestById(int id)
	{
		if (currentWorldQuestList == null)
		{
			return null;
		}
		return currentWorldQuestList.GetQuestById(id);
	}

	public virtual void LoadQuestListForWorld(string worldId)
	{
		string @string = PlayerPrefs.GetString(questListWorldKey(worldId), string.Empty);
		int @int = PlayerPrefs.GetInt(questListWorldKey(worldId) + "_max", -1);
		if (string.IsNullOrEmpty(@string) || @string.Equals("null"))
		{
			UnityEngine.Debug.Log("No quest list");
			GenerateNewQuestList(worldId);
		}
		else
		{
			currentWorldQuestList = new QuestListData();
			currentWorldQuestList.quests = JSONHelper.Deserialize<List<Quest>>(@string);
			currentWorldQuestList.questListNumber = @int;
			if (currentWorldQuestList == null || currentWorldQuestList.quests == null)
			{
				GenerateNewQuestList(worldId);
			}
		}
		if (currentWorldQuestList.quests.Count < numberOfActiveQuests)
		{
			for (int i = currentWorldQuestList.quests.Count; i < numberOfActiveQuests; i++)
			{
				AddRandomToQuestList(worldId);
			}
		}
		SaveQuestList(worldId);
		PrintCurrentQuests();
	}

	public virtual void GenerateNewQuestList(string worldId)
	{
		currentWorldQuestList = new QuestListData();
		currentWorldQuestList.quests = new List<Quest>();
		for (int i = 0; i < numberOfActiveQuests; i++)
		{
			AddRandomToQuestList(worldId);
		}
		SaveQuestList(worldId);
		UnityEngine.Debug.Log("Quest list generated");
	}

	public virtual void AddRandomToQuestList(string worldId)
	{
		QuestWorldDistribution questWorldDistribution = equalDistribution;
		if (questDistributions != null && questDistributions.ContainsKey(worldId))
		{
			questWorldDistribution = questDistributions[worldId];
		}
		else if (Manager.Get<QuestManager>().useManagerQuests)
		{
			questWorldDistribution = new QuestWorldDistribution(Manager.Get<QuestManager>());
		}
		WorldsParameters parameters = null;
		if (questWorldParameters == null)
		{
			questWorldParameters = new Dictionary<string, WorldsParameters>();
		}
		if (questWorldParameters.ContainsKey(worldId))
		{
			parameters = questWorldParameters[worldId];
		}
		UnityEngine.Debug.Log("questPassed:" + Singleton<PlayerData>.get.playerQuests.GetQuestCountPassedInWorld(worldId));
		if (Manager.Get<ModelManager>().worldsSettings.AreWorldQuestUnlimited() || CheckQuestNumberLimit(worldId))
		{
			questWorldDistribution.GenerateAndAddQuestToList(currentWorldQuestList, parameters);
			RecreateNotificationInformation();
			Manager.Get<QuestManager>().CheckMobsIndicatorState();
		}
	}

	private bool CheckQuestNumberLimit(string worldId)
	{
		int num = 40;
		WorldData worldById = Manager.Get<SavedWorldManager>().GetWorldById(worldId);
		if (worldById != null)
		{
			num = worldById.questLimit;
		}
		return Singleton<PlayerData>.get.playerQuests.GetQuestCountPassedInWorld(worldId) < num;
	}

	public virtual bool OnQuestClaim(string id)
	{
		int num = 0;
		foreach (Quest quest2 in currentWorldQuestList.quests)
		{
			if (id.Equals(quest2.GenerateWorldId()))
			{
				if (Manager.Contains<AbstractSoftCurrencyManager>())
				{
					Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(quest2.prize);
				}
				else
				{
					int chestCoins = Manager.Get<ModelManager>().chestSettings.GetChestCoins();
					CraftingManager craftingManager = Manager.Get<CraftingManager>();
					List<CraftableList.ResourceSpawn> resourcesList = craftingManager.GetCraftableListInstance().resourcesList;
					for (int i = 0; i < chestCoins; i++)
					{
						int id2 = resourcesList[UnityEngine.Random.Range(0, resourcesList.Count)].id;
						ResourceSprite.GatherResource(id2);
					}
					if (Manager.Contains<SurvivalRankManager>())
					{
						SurvivalRankManager survivalRankManager = Manager.Get<SurvivalRankManager>();
						survivalRankManager.Increase("AnyQuest");
					}
				}
				break;
			}
			num++;
		}
		if (num == currentWorldQuestList.quests.Count)
		{
			return false;
		}
		Quest quest = currentWorldQuestList.quests[num];
		currentWorldQuestList.quests.RemoveAt(num);
		AddRandomToQuestList(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
		SaveQuestList(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId);
		int currentGold = 0;
		if (Manager.Contains<AbstractSoftCurrencyManager>())
		{
			currentGold = Manager.Get<AbstractSoftCurrencyManager>().GetCurrencyAmount();
		}
		Manager.Get<StatsManager>().OnWorldQuestFinished(Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId, quest.type.ToString(), Singleton<PlayerData>.get.playerQuests.totalWorldQuestPassed, currentGold);
		return true;
	}

	private void SpawnResources(Vector3 lootPosition, int currencyValue)
	{
		CraftingManager craftingManager = Manager.Get<CraftingManager>();
		if ((bool)craftingManager)
		{
			for (int i = 0; i < currencyValue; i++)
			{
				craftingManager.SpawnRandomResource(lootPosition, spawnWithRandomForce: true);
			}
		}
	}

	public void SaveQuestList(string worldId)
	{
		string value = JSONHelper.ToJSON(currentWorldQuestList.quests);
		PlayerPrefs.SetString(questListWorldKey(worldId), value);
		PlayerPrefs.SetInt(questListWorldKey(worldId) + "_max", currentWorldQuestList.questListNumber);
		PlayerPrefs.Save();
	}

	protected void RecreateNotificationInformation()
	{
		blockToBeNotified = new HashSet<int>();
		resourcesToBeNotified = new HashSet<int>();
		craftableToBeNotified = new HashSet<int>();
		pauseFragmentsToNotify = new HashSet<string>();
		currentWorldQuestList.quests.ForEach(delegate(Quest quest)
		{
			if (!quest.paramatersInt.IsNullOrEmpty())
			{
				switch (QuestManager.GetNeededParameter(quest.type))
				{
				case ParameterType.Block:
					blockToBeNotified.Add(quest.paramatersInt[0]);
					break;
				case ParameterType.Resource:
					resourcesToBeNotified.Add(quest.paramatersInt[0]);
					break;
				case ParameterType.Craftable:
					craftableToBeNotified.Add(quest.paramatersInt[0]);
					break;
				default:
					if (quest.type == QuestType.CraftAny || quest.type == QuestType.CraftOfX)
					{
						pauseFragmentsToNotify.Add("Crafting");
					}
					break;
				}
			}
		});
	}

	public bool IsFragmentNotified(string name)
	{
		return pauseFragmentsToNotify.Contains(name);
	}

	public bool IsThisNotified(int itemId, ParameterType type)
	{
		if (!Manager.Get<ModelManager>().worldsSettings.AreWorldQuestEnabled())
		{
			return false;
		}
		if (blockToBeNotified == null)
		{
			RecreateNotificationInformation();
		}
		switch (type)
		{
		case ParameterType.Block:
			return blockToBeNotified.Contains(itemId);
		case ParameterType.Resource:
			return resourcesToBeNotified.Contains(itemId);
		case ParameterType.Craftable:
			return craftableToBeNotified.Contains(itemId);
		default:
			return false;
		}
	}
}
