// DecompilerFi decompiler from Assembly-CSharp.dll class: StatsProgressManager
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsProgressManager : ProgressManager, IGameCallbacksListener
{
	[Serializable]
	public class StatsProgressConfig
	{
		public PlayerStats stats;

		public float toAdd;

		public Sprite sprite;
	}

	public bool showLevelUpPopup = true;

	public StatsProgressConfig[] stats;

	public override void Init()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		onLevelUpCallbacks = (Action)Delegate.Combine(onLevelUpCallbacks, new Action(OnLevelUp));
		base.Init();
	}

	public void OnGameplayRestarted()
	{
		OnLoaded();
	}

	public void OnGameplayStarted()
	{
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}

	protected override void Load()
	{
		base.Load();
		OnLoaded();
	}

	private void OnLevelUp()
	{
		List<LevelUpStat> list = new List<LevelUpStat>();
		for (int i = 0; i < stats.Length; i++)
		{
			stats[i].stats.Add(new PlayerStats.Modifier
			{
				value = stats[i].toAdd,
				priority = 2,
				Action = ((float toAction, float value) => value + toAction)
			});
			list.Add(new LevelUpStat
			{
				statIco = stats[i].sprite,
				statText = "+" + stats[i].toAdd.ToString()
			});
		}
		if (showLevelUpPopup)
		{
			Manager.Get<StateMachineManager>().PushState<LevelUpWithStatsPopupState>(new LevelUpWithStatsPopupStartParameter
			{
				statsConfigs = list,
				level = base.level
			});
		}
	}

	private void OnLoaded()
	{
		for (int i = 1; i < base.level; i++)
		{
			for (int j = 0; j < stats.Length; j++)
			{
				stats[j].stats.Add(new PlayerStats.Modifier
				{
					value = stats[j].toAdd,
					priority = 2,
					Action = ((float toAction, float value) => value + toAction)
				});
			}
		}
	}
}
