// DecompilerFi decompiler from Assembly-CSharp.dll class: Quest
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class Quest
{
	private const string GENERATOR_KEY = "quest.generator.id";

	public int id;

	public QuestType type;

	public int prize;

	public int questQueueNumber;

	public int stepsNeeded = 1;

	public bool claimAble;

	[HideInInspector]
	public List<int> paramatersInt = new List<int>();

	[HideInInspector]
	public List<string> paramatersString = new List<string>();

	public Prizes specialPrizes;

	public int maxLevel = -1;

	[SerializeField]
	public int currentLevel;

	private static int idGenerator
	{
		get
		{
			return PlayerPrefs.GetInt("quest.generator.id", 0);
		}
		set
		{
			PlayerPrefs.SetInt("quest.generator.id", value);
		}
	}

	public int CurrentLevel
	{
		get
		{
			return currentLevel;
		}
		set
		{
			currentLevel = value;
		}
	}

	public virtual int Prize => prize;

	public Quest()
	{
	}

	public Quest(QuestType type, int prize, int stepsNeeded, int queueNumber)
	{
		id = idGenerator++;
		this.type = type;
		this.prize = prize;
		questQueueNumber = queueNumber;
		this.stepsNeeded = stepsNeeded;
	}

	public bool TryIncreaseLevel(string worldId)
	{
		currentLevel++;
		if (maxLevel < 0)
		{
			return true;
		}
		if (maxLevel == 0)
		{
			currentLevel = 0;
			return false;
		}
		if (currentLevel >= maxLevel)
		{
			currentLevel = maxLevel;
			return false;
		}
		UnityEngine.Debug.LogError("Increasing level");
		return true;
	}

	public string GenerateWorldId()
	{
		return $"{id}.{Manager.Get<SavedWorldManager>().GetCurrentWorld().uniqueId}";
	}

	public override string ToString()
	{
		StringBuilder intParamsBuilder = new StringBuilder();
		paramatersInt.ForEach(delegate(int value)
		{
			intParamsBuilder.Append(value + " ");
		});
		StringBuilder stringBuilder = new StringBuilder();
		paramatersInt.ForEach(delegate(int value)
		{
			intParamsBuilder.Append(value + " ");
		});
		return $"[Quest]  type: {type}, steps {stepsNeeded}, pI {intParamsBuilder.ToString()}, pS {stringBuilder.ToString()}";
	}
}
