// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldsChanger
using Common.Managers;
using Gameplay;
using States;
using System.Collections;
using Uniblocks;
using UnityEngine;

public class WorldsChanger : MonoBehaviour
{
	public void ChangeWorld(string id, bool noSaveWindow)
	{
		UnityEngine.Debug.Log("Start changing world");
		SelectWorldIfNotValid(id, noSaveWindow);
	}

	private void SelectWorldIfNotValid(string id, bool noSaveWindow)
	{
		if (noSaveWindow)
		{
			ShowLoadingAndGo(save: true, id);
			return;
		}
		if (Manager.Get<SavedWorldManager>().CanReturnToGame())
		{
			Manager.Get<StateMachineManager>().PushState<SaveWorldCheckState>(new SaveWorldCheckState.SaveWorldStartParameter(Manager.Get<SavedWorldManager>().GetCurrentWorld(), forSave: true, delegate
			{
				ShowLoadingAndGo(save: true, id);
			}, delegate
			{
				ShowLoadingAndGo(save: false, id);
			}));
			return;
		}
		Manager.Get<SavedWorldManager>().TryToSelectWorldById(id);
		StartCoroutine(GoToTheAnotherWorld());
	}

	private void ShowLoadingAndGo(bool save, string id)
	{
		Manager.Get<StateMachineManager>().PushState<EmptyLoadingState>(new LoadingStartParameter("changing", "Changing World"));
		StartCoroutine(SaveAndGo(save, id));
	}

	private IEnumerator SaveAndGo(bool save, string id)
	{
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
		yield return new WaitForSecondsRealtime(0.05f);
		SavedWorldManager.ResetCurrentWorld(Manager.Get<SavedWorldManager>().ShouldSaveAtSelect());
	}
}
