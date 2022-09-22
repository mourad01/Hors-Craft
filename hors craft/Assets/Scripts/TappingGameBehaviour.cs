// DecompilerFi decompiler from Assembly-CSharp.dll class: TappingGameBehaviour
using Common.Managers;
using States;
using UnityEngine;

public abstract class TappingGameBehaviour
{
	public enum GameState
	{
		lost,
		inProgress,
		won
	}

	public TappingGameGraphics tappingGameGraphics;

	protected GameState state;

	public GameState GetState => state;

	public abstract StatsManager.MinigameType GetGameType();

	public virtual void LeftActionButton()
	{
	}

	public virtual void RightActionButton()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void ResetRound()
	{
		state = GameState.inProgress;
	}

	public virtual void SetLevel(int level)
	{
	}

	public virtual bool NextRoundAvailable()
	{
		return false;
	}

	public virtual GenericTutorial.TutorialStep[] GetTutorialSteps(TappingGameState state, TappingGameStateConnector connector, GenericTutorial.TutorialStep[] tutorialSteps)
	{
		int num = 0;
		foreach (GameObject item in tappingGameGraphics.GetThingsForTutorial(connector))
		{
			tutorialSteps[num].element = item;
			num++;
		}
		GenericTutorial.TutorialCallback tutorialCallback = new GenericTutorial.TutorialCallback();
		tutorialCallback.action = delegate
		{
			SaveTutorialStatus();
			state.PrepareForRound();
		};
		tutorialCallback.type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK;
		tutorialSteps[--num].tutorialCallbacks.Add(tutorialCallback);
		return tutorialSteps;
	}

	public virtual void SaveTutorialStatus()
	{
		PlayerPrefs.SetInt("tapping.tutorial.finished", 1);
	}

	public virtual int LoadTutorialStatus()
	{
		return PlayerPrefs.GetInt("tapping.tutorial.finished", 0);
	}
}
