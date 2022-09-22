// DecompilerFi decompiler from Assembly-CSharp.dll class: CustomSceneMinigame
using Common.Managers;
using Gameplay;
using GameUI;
using States;
using Uniblocks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Minigames/Custom Scene minigame")]
public class CustomSceneMinigame : Minigame
{
	public string sceneToLoad;

	public override void Play(MinigameStartParameter minigameParameter)
	{
		Engine.SaveWorldInstant();
		Manager.Get<GameCallbacksManager>().InFrequentSave();
		Manager.Get<GameCallbacksManager>().FrequentSave();
		SavedWorldManager.UnloadCraftGameplay();
		DummyStateStartParameter dummyStateStartParameter = new DummyStateStartParameter();
		dummyStateStartParameter.doOnStart = delegate
		{
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			Manager.Get<MinigamesManager>().InitMinigame(minigameParameter);
		};
		DummyStateStartParameter parameter = dummyStateStartParameter;
		Manager.Get<StateMachineManager>().SetState<LoadLevelState>(new LoadLevelAndInitStateStartParameter
		{
			sceneToLoadName = sceneToLoad,
			stateType = typeof(DummyState),
			parameter = parameter
		});
	}
}
