// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerQuests
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Uniblocks;
using UnityEngine;

[Serializable]
public class PlayerQuests : IGameCallbacksListener
{
	public class WorldQuestEvent : EventArgs
	{
		public Quest quest;

		public WorldQuestEvent(Quest quest)
		{
			this.quest = quest;
		}
	}

	[Serializable]
	public class QuestProgress
	{
		[XmlAttribute("id")]
		public int id;

		[XmlAttribute("steps")]
		public int steps;

		public QuestProgress()
		{
		}

		public QuestProgress(int id, int steps)
		{
			this.id = id;
			this.steps = steps;
		}
	}

	[Serializable]
	public class WorldQuestProgress
	{
		public string id;

		public int steps;

		public int stepsNeeded;

		public bool passed;

		public WorldQuestProgress()
		{
		}

		public WorldQuestProgress(string id, int steps, int stepsNeeded)
		{
			this.id = id;
			this.steps = steps;
			this.stepsNeeded = stepsNeeded;
		}

		public bool CanBePassed()
		{
			return steps > stepsNeeded;
		}
	}

	private static bool worldQuestShownInSession;

	public float totalVehicleDistance;

	public float totalMountWalkingDistance;

	public float totalWalkingDistance;

	public float totalFlyingDistance;

	public float totalSwimDistance;

	public int lastInformedVehicle;

	public int lastInformedMountWalk;

	public int lastInformedWalk;

	public int lastInformedFly;

	public int lastInformedSwim;

	public int totalWorldQuestPassed;

	private HashSet<int> passedQuest;

	private Dictionary<int, int> currentQuestProgress;

	private Dictionary<string, WorldQuestProgress> currentWorldQuestProgress;

	[XmlArray("QuestProgressArray")]
	[XmlArrayItem("QuestProgress")]
	public List<QuestProgress> questProgressArray;

	public List<WorldQuestProgress> worldQuestProgressArray;

	public event EventHandler<WorldQuestEvent> onWorldQuestChange;

	public event EventHandler<WorldQuestEvent> onWorldQuestMaximum;

	public PlayerQuests()
	{
		passedQuest = new HashSet<int>();
		currentQuestProgress = new Dictionary<int, int>();
		currentWorldQuestProgress = new Dictionary<string, WorldQuestProgress>();
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	public void ClearAndAddWorldQuestListener(Action listener)
	{
		this.onWorldQuestChange = null;
		onWorldQuestChange += delegate
		{
			listener();
		};
	}

	public void ClearAndAddWorldQuestReadyForClaimListener(Action<Quest> listener)
	{
		this.onWorldQuestMaximum = null;
		onWorldQuestMaximum += delegate(object sender, WorldQuestEvent e)
		{
			listener(e.quest);
		};
	}

	public static bool HaveToShowOnStart()
	{
		if (!worldQuestShownInSession)
		{
			worldQuestShownInSession = true;
			return true;
		}
		return false;
	}

	public static string GetCurrentSaveKey()
	{
		return $"player.quests";
	}

	public static string GetCurrentWorldSaveKey()
	{
		return $"player.quests.{Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId}";
	}

	public void LoadForWorld()
	{
	}

	public static PlayerQuests LoadFromPlayerPrefs()
	{
		string @string = PlayerPrefs.GetString(GetCurrentSaveKey());
		PlayerQuests playerQuests = (!string.IsNullOrEmpty(@string)) ? JsonUtility.FromJson<PlayerQuests>(@string) : new PlayerQuests();
		playerQuests.initAfterSerialization();
		return playerQuests;
	}

	private void SaveToPrefs()
	{
		beforeSerialization();
		PlayerPrefs.SetString(GetCurrentSaveKey(), JsonUtility.ToJson(this));
	}

	public int GetWorldQuestProgress(string id)
	{
		if (currentWorldQuestProgress != null && currentWorldQuestProgress.ContainsKey(id))
		{
			return currentWorldQuestProgress[id].steps;
		}
		return 0;
	}

	private string PrintCurrentWorldQuests()
	{
		if (currentWorldQuestProgress == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, WorldQuestProgress> item in currentWorldQuestProgress)
		{
			stringBuilder.Append(item.Key);
			stringBuilder.Append(" | ");
			stringBuilder.Append(JSONHelper.ToJSON(item.Value));
			stringBuilder.Append(Environment.NewLine);
		}
		return stringBuilder.ToString();
	}

	public int GetQuestProgress(int id)
	{
		if (currentQuestProgress.ContainsKey(id))
		{
			return currentQuestProgress[id];
		}
		return 0;
	}

	public int GetValueNeededToUnlocks(int i)
	{
		Quest quest = Manager.Get<QuestManager>().questById[i];
		return quest.stepsNeeded - GetQuestProgress(i);
	}

	private void beforeSerialization()
	{
		questProgressArray = new List<QuestProgress>();
		worldQuestProgressArray = new List<WorldQuestProgress>();
		foreach (KeyValuePair<int, int> item in currentQuestProgress)
		{
			questProgressArray.Add(new QuestProgress(item.Key, item.Value));
		}
		foreach (KeyValuePair<string, WorldQuestProgress> item2 in currentWorldQuestProgress)
		{
			worldQuestProgressArray.Add(item2.Value);
		}
	}

	public void InformOnPlayerWalk(Vector3 position, float distance, PlayerMovement.Mode mode, bool inWater)
	{
		if (distance > 33f)
		{
			return;
		}
		switch (mode)
		{
		case PlayerMovement.Mode.WALKING:
			if (inWater)
			{
				CheckIncreaseOfWalkinQuest(distance, ref totalSwimDistance, ref lastInformedSwim, QuestType.SwimDistance);
			}
			else
			{
				CheckIncreaseOfWalkinQuest(distance, ref totalWalkingDistance, ref lastInformedWalk, QuestType.WalkDistance);
			}
			break;
		case PlayerMovement.Mode.FLYING:
			CheckIncreaseOfWalkinQuest(distance, ref totalFlyingDistance, ref lastInformedFly, QuestType.FlyDistance);
			break;
		case PlayerMovement.Mode.MOUNTED:
			CheckIncreaseOfWalkinQuest(distance, ref totalMountWalkingDistance, ref lastInformedMountWalk, QuestType.MountDistance);
			break;
		case PlayerMovement.Mode.MOUNTED_FLYING:
			CheckIncreaseOfWalkinQuest(distance, ref totalMountWalkingDistance, ref lastInformedMountWalk, QuestType.MountDistance);
			break;
		case PlayerMovement.Mode.MOUNTED_VEHICLE:
			CheckIncreaseOfWalkinQuest(distance, ref totalVehicleDistance, ref lastInformedVehicle, QuestType.VehicleDistance);
			break;
		}
		SetHeightQuest(position.y);
		Manager.Get<AbstractAchievementManager>()?.RegisterEvent("traveled.distance", distance);
	}

	private void SetHeightQuest(float positionY)
	{
		Manager.Get<QuestManager>().SetQuestOfTypeIfBigger(QuestType.ReachHeight, Mathf.FloorToInt(positionY));
	}

	private void CheckIncreaseOfWalkinQuest(float distance, ref float total, ref int lastInfo, QuestType type)
	{
		total += distance;
		if (total > (float)(lastInfo + 1))
		{
			lastInfo = Mathf.FloorToInt(total);
			Manager.Get<QuestManager>().IncreaseQuestOfType(type, Mathf.CeilToInt(distance));
		}
	}

	private void initAfterSerialization()
	{
		currentQuestProgress = new Dictionary<int, int>();
		passedQuest = new HashSet<int>();
		currentWorldQuestProgress = new Dictionary<string, WorldQuestProgress>();
		if (questProgressArray != null)
		{
			foreach (QuestProgress item in questProgressArray)
			{
				currentQuestProgress[item.id] = item.steps;
				if (Manager.Get<QuestManager>().questById == null)
				{
					Manager.Get<QuestManager>().InitalizeWithQuestList(Manager.Get<QuestManager>().globalWorldQuestList);
				}
				if (Manager.Get<QuestManager>().questById.ContainsKey(item.id))
				{
					Quest quest = Manager.Get<QuestManager>().questById[item.id];
					if (quest != null && quest.stepsNeeded <= item.steps)
					{
						passedQuest.Add(item.id);
					}
				}
			}
			foreach (WorldQuestProgress item2 in worldQuestProgressArray)
			{
				currentWorldQuestProgress[item2.id] = item2;
			}
		}
	}

	public void SetQuestProgress(int id, int value)
	{
		if (!passedQuest.Contains(id))
		{
			if (currentQuestProgress == null)
			{
				currentQuestProgress = new Dictionary<int, int>();
			}
			currentQuestProgress[id] = value;
			Quest worldQuestById = Manager.Get<QuestManager>().GetWorldQuestById(id);
			if (worldQuestById != null && worldQuestById.stepsNeeded <= currentQuestProgress[id])
			{
				OnQuestFinish(id);
			}
		}
	}

	public void SetWorldQuestProgress(Quest quest, int value)
	{
		if (currentWorldQuestProgress == null)
		{
			currentWorldQuestProgress = new Dictionary<string, WorldQuestProgress>();
		}
		string key = quest.GenerateWorldId();
		if (currentWorldQuestProgress.ContainsKey(key))
		{
			currentWorldQuestProgress[quest.GenerateWorldId()].steps = value;
		}
		else
		{
			currentWorldQuestProgress[quest.GenerateWorldId()] = new WorldQuestProgress(quest.GenerateWorldId(), value, quest.stepsNeeded);
		}
		CheckWorldQuestPass(quest);
		this.onWorldQuestChange(this, new WorldQuestEvent(quest));
	}

	public void OnWorldQuestIncreased(Quest quest, int increase)
	{
		if (currentWorldQuestProgress == null)
		{
			currentWorldQuestProgress = new Dictionary<string, WorldQuestProgress>();
		}
		string key = quest.GenerateWorldId();
		if (currentWorldQuestProgress.ContainsKey(key))
		{
			currentWorldQuestProgress[key].steps += increase;
		}
		else
		{
			currentWorldQuestProgress[key] = new WorldQuestProgress(quest.GenerateWorldId(), increase, quest.stepsNeeded);
		}
		CheckWorldQuestPass(quest);
		if (this.onWorldQuestChange != null)
		{
			this.onWorldQuestChange(this, new WorldQuestEvent(quest));
		}
	}

	private void CheckWorldQuestPass(Quest quest)
	{
		if (currentWorldQuestProgress[quest.GenerateWorldId()].steps >= quest.stepsNeeded && !currentWorldQuestProgress[quest.GenerateWorldId()].passed)
		{
			currentWorldQuestProgress[quest.GenerateWorldId()].passed = true;
			OnWorldQuestFinish(quest);
		}
	}

	public void InformOnQuestIncrease(int id, int increase)
	{
		if (!passedQuest.Contains(id))
		{
			if (currentQuestProgress == null)
			{
				currentQuestProgress = new Dictionary<int, int>();
			}
			currentQuestProgress.AddToValueOrCreate(id, increase);
			Quest quest = Manager.Get<QuestManager>().questById[id];
			if (quest != null && quest.stepsNeeded <= currentQuestProgress[id])
			{
				OnQuestFinish(id);
			}
		}
	}

	public bool IsQuestPassed(List<int> questIds)
	{
		if (questIds == null || questIds.Count == 0 || passedQuest == null)
		{
			return true;
		}
		foreach (int questId in questIds)
		{
			if (!passedQuest.Contains(questId))
			{
				return false;
			}
		}
		return true;
	}

	public void OnWorldQuestFinish(Quest quest)
	{
		GameplayState gameplayState = Manager.Get<StateMachineManager>().currentState as GameplayState;
		totalWorldQuestPassed++;
		if (gameplayState != null && Manager.Get<ModelManager>().worldsSettings.AreWorldQuestEnabled())
		{
			this.onWorldQuestMaximum(this, new WorldQuestEvent(quest));
			gameplayState.ShowQuestCompleteNotification(quest.type);
		}
	}

	public int GetQuestCountPassedInWorld(string worldUniqueId)
	{
		if (string.IsNullOrEmpty(worldUniqueId))
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<string, WorldQuestProgress> item in currentWorldQuestProgress)
		{
			string[] array = item.Key.Split(".".ToCharArray(), 2);
			if (array[1].Equals(worldUniqueId) && item.Value.passed)
			{
				num++;
			}
		}
		return num;
	}

	public void OnQuestFinish(int id)
	{
		passedQuest.Add(id);
		CheckIfBlockUnlock(id);
	}

	public void CheckIfBlockUnlock(int unlocked)
	{
		GameplayState gameplayState = Manager.Get<StateMachineManager>().currentState as GameplayState;
		foreach (Craftable craftable in Manager.Get<CraftingManager>().GetCraftableList())
		{
			if (craftable.isQuestOnList(unlocked) && craftable.IsQuestPassed())
			{
				Manager.Get<StatsManager>().ItemUnlocked(craftable.GetStatsCategory(), craftable.GetStatsName());
				if (gameplayState != null)
				{
					gameplayState.ShowBlockNotification(Manager.Get<TranslationsManager>().GetText("crafting.unlock", "Block unlocked!"), craftable.id);
				}
				Manager.Get<QuestManager>().OnBlockUnlock();
			}
		}
	}

	public void OnGameSavedFrequent()
	{
		SaveToPrefs();
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayStarted()
	{
	}

	public void OnGameplayRestarted()
	{
	}
}
