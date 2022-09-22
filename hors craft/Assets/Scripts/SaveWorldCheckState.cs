// DecompilerFi decompiler from Assembly-CSharp.dll class: SaveWorldCheckState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using States;
using System;
using Uniblocks;
using UnityEngine;

public class SaveWorldCheckState : XCraftUIState<SaveWorldCheckConnector>
{
	public class SaveWorldStartParameter : StartParameter
	{
		public WorldData data;

		public bool forSave;

		public Action onAccept;

		public Action onRefuse;

		public SaveWorldStartParameter(WorldData data, bool forSave, Action onAccept, Action onRefuse)
		{
			this.data = data;
			this.forSave = forSave;
			this.onAccept = onAccept;
			this.onRefuse = onRefuse;
		}
	}

	private string worldId;

	public override void StartState(StartParameter startParameter)
	{
		base.StartState(startParameter);
		SaveWorldStartParameter saveWorldStartParameter = startParameter as SaveWorldStartParameter;
		worldId = saveWorldStartParameter.data.uniqueId;
		if (saveWorldStartParameter.forSave)
		{
			if (isSurvivalNight())
			{
				base.connector.InitForSurvivalNight(saveWorldStartParameter.data, OnReturn, OnReturn, saveWorldStartParameter.onRefuse);
			}
			else
			{
				base.connector.InitForSaving(saveWorldStartParameter.data, OnReturn, saveWorldStartParameter.onRefuse, saveWorldStartParameter.onAccept);
			}
		}
		else
		{
			base.connector.InitForDelete(saveWorldStartParameter.data, OnReturn, saveWorldStartParameter.onRefuse, saveWorldStartParameter.onAccept);
		}
		bool active = ShouldShowReturnButton();
		base.connector.returnButton.gameObject.SetActive(active);
	}

	private bool ShouldShowReturnButton()
	{
		return !Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate();
	}

	private bool isSurvivalNight()
	{
		bool result = false;
		if (!Manager.Contains<SurvivalManager>())
		{
			UnityEngine.Debug.Log("No survival");
			return result;
		}
		return Manager.Get<SurvivalManager>().IsCombatTime();
	}

	public void OnReturn()
	{
		Manager.Get<StateMachineManager>().PopState();
	}

	public void OnDelete()
	{
		if (!string.IsNullOrEmpty(worldId))
		{
			Manager.Get<SavedWorldManager>().DeleteWorld(worldId);
		}
		Manager.Get<StateMachineManager>().PopState();
	}

	public void OnAccept()
	{
		Engine.SaveWorldInstant();
		Manager.Get<GameCallbacksManager>().FrequentSave();
		if (CheckNumberOfFreeWorlds())
		{
			Manager.Get<StateMachineManager>().PushState<WorldNameSelectState>(new WorldNameSelectState.WorldSelectType(create: true, string.Empty));
		}
		else
		{
			Manager.Get<StateMachineManager>().PushState<MaxWorldsState>(new StartParameter());
		}
	}

	public void OnRefuse()
	{
		if (CheckNumberOfFreeWorlds())
		{
			Manager.Get<StateMachineManager>().PushState<WorldNameSelectState>(new WorldNameSelectState.WorldSelectType(create: true, string.Empty));
		}
		else
		{
			Manager.Get<StateMachineManager>().PushState<MaxWorldsState>(new StartParameter());
		}
	}

	private bool CheckNumberOfFreeWorlds()
	{
		int freeWorlds = Manager.Get<ModelManager>().worldsSettings.GetFreeWorlds();
		int numberOfUsedSlots = Manager.Get<SavedWorldManager>().GetNumberOfUsedSlots();
		return numberOfUsedSlots < freeWorlds;
	}
}
