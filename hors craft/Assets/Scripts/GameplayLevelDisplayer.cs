// DecompilerFi decompiler from Assembly-CSharp.dll class: GameplayLevelDisplayer
using Common.Managers;
using States;
using UnityEngine;

public class GameplayLevelDisplayer : MonoBehaviour
{
	public LevelContext context;

	private void Start()
	{
		context = (MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<LevelContext>(Fact.LEVEL) ?? new LevelContext());
		UpdateContext();
	}

	private void Update()
	{
		UpdateContext();
	}

	private void UpdateContext()
	{
		if (Manager.Get<StateMachineManager>().currentState is GameplayState)
		{
			context.level = Manager.Get<ProgressManager>().level;
			context.levelProgress = (float)Manager.Get<ProgressManager>().experience / (float)Manager.Get<ProgressManager>().experienceNeededToNextLevel;
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.LEVEL, context))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.LEVEL);
			}
		}
	}
}
