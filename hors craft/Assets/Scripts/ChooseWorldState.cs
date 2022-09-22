// DecompilerFi decompiler from Assembly-CSharp.dll class: ChooseWorldState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using States;
using System.Collections;
using Uniblocks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseWorldState : XCraftUIState<ChooseWorldConnector>
{
	public static bool haveShownChooseWorld;

	public override void StartState(StartParameter startParameter)
	{
		haveShownChooseWorld = true;
		base.StartState(startParameter);
		base.connector.Init(OnReturn, OnWorldSelect, OnWorldAdd, OnWorldPlay, OnWorldDelete, OnWorldRename);
		SetCurrentWorldElement();
	}

	public override void UnfreezeState()
	{
		base.connector.CheckList();
		base.connector.fakeLoading.SetActive(value: false);
		SetCurrentWorldElement();
		base.UnfreezeState();
	}

	private void SetCurrentWorldElement()
	{
		if (Manager.Get<SavedWorldManager>().GetWorldById(base.connector.GetSelectedId()) == Manager.Get<SavedWorldManager>().GetCurrentWorld())
		{
			base.connector.deleteButton.gameObject.SetActive(value: false);
		}
		else
		{
			base.connector.deleteButton.gameObject.SetActive(value: true);
		}
	}

	public void OnReturn()
	{
		if (Manager.Get<SavedWorldManager>().CanReturnToGame())
		{
			Manager.Get<StateMachineManager>().PopState();
			return;
		}
		Manager.Get<SavedWorldManager>().OnReturnToTitleScreen();
		SceneManager.UnloadScene("Gameplay");
		Manager.Get<StateMachineManager>().SetState<TitleState>();
	}

	public void OnWorldSelect(string id)
	{
		if (Manager.Get<SavedWorldManager>().GetWorldById(id) == Manager.Get<SavedWorldManager>().GetCurrentWorld())
		{
			base.connector.deleteButton.gameObject.SetActive(value: false);
		}
		else
		{
			base.connector.deleteButton.gameObject.SetActive(value: true);
		}
	}

	public void OnWorldDelete(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			Manager.Get<StateMachineManager>().PushState<SaveWorldCheckState>(new SaveWorldCheckState.SaveWorldStartParameter(Manager.Get<SavedWorldManager>().GetWorldById(id), forSave: false, delegate
			{
				if (!string.IsNullOrEmpty(id))
				{
					Manager.Get<SavedWorldManager>().DeleteWorld(id);
				}
				Manager.Get<StateMachineManager>().PopState();
			}, delegate
			{
				Manager.Get<StateMachineManager>().PopState();
			}));
		}
	}

	public void OnWorldRename(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			Manager.Get<StateMachineManager>().PushState<WorldNameSelectState>(new WorldNameSelectState.WorldSelectType(create: false, id));
		}
	}

	public void OnWorldPlay(string id)
	{
		if (Manager.Get<SavedWorldManager>().IsWorldReadyToPlay(id))
		{
			IfWorldIsValidForPlay(id);
		}
		else
		{
			GetSelectedElement().downloader = Manager.Get<SavedWorldManager>().DownloadWorldForPlayer(id, delegate(bool isReady)
			{
				if (isReady)
				{
					IfWorldIsValidForPlay(id);
				}
				else if (base.connector != null)
				{
					base.connector.CheckList();
				}
			});
		}
	}

	private WorldSelectElement GetSelectedElement()
	{
		return base.connector.GetSelectedElement();
	}

	private void IfWorldIsValidForPlay(string id)
	{
		if (Manager.Get<SavedWorldManager>().CheckIfCurrentAndValid(id))
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		}
		else
		{
			SelectWorldIfNotValid(id);
		}
		Manager.Get<SavedWorldManager>().OnPlayWorld(id);
	}

	private void SelectWorldIfNotValid(string id)
	{
		if (Manager.Get<SavedWorldManager>().CanReturnToGame())
		{
			Manager.Get<StateMachineManager>().PushState<SaveWorldCheckState>(new SaveWorldCheckState.SaveWorldStartParameter(Manager.Get<SavedWorldManager>().GetCurrentWorld(), forSave: true, delegate
			{
				Manager.Get<StateMachineManager>().PopState();
				StartCoroutine(SaveAndGo(save: true, id));
			}, delegate
			{
				Manager.Get<StateMachineManager>().PopState();
				StartCoroutine(SaveAndGo(save: false, id));
			}));
			return;
		}
		Manager.Get<SavedWorldManager>().TryToSelectWorldById(id);
		StartCoroutine(GoToTheAnotherWorld());
	}

	private IEnumerator SaveAndGo(bool save, string id)
	{
		yield return new WaitForSecondsRealtime(0.05f);
		base.connector.fakeLoading.SetActive(value: true);
		yield return new WaitForSecondsRealtime(0.05f);
		if (save)
		{
			Engine.SaveWorldInstant();
			Manager.Get<GameCallbacksManager>().FrequentSave();
		}
		Manager.Get<SavedWorldManager>().TryToSelectWorldById(id);
		StartCoroutine(GoToTheAnotherWorld());
	}

	private IEnumerator GoToTheAnotherWorld()
	{
		base.connector.fakeLoading.SetActive(value: true);
		yield return new WaitForSecondsRealtime(0.05f);
		SavedWorldManager.ResetCurrentWorld(Manager.Get<SavedWorldManager>().ShouldSaveAtSelect());
	}

	public void OnWorldAdd()
	{
		Manager.Get<StateMachineManager>().PushState<SaveWorldCheckState>(new SaveWorldCheckState.SaveWorldStartParameter(Manager.Get<SavedWorldManager>().GetCurrentWorld(), forSave: true, OnAcceptSave, OnRefuseSave));
	}

	public void OnAcceptSave()
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

	public void OnRefuseSave()
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
