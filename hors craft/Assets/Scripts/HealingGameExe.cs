// DecompilerFi decompiler from Assembly-CSharp.dll class: HealingGameExe
using Common.Managers;
using Gameplay;
using States;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealingMinigameExe", menuName = "ScriptableObjects/MinigamesExe/Healing")]
public class HealingGameExe : MiniGameExe
{
	protected override StatsManager.MinigameType minigameType => StatsManager.MinigameType.HEAL;

	public override void Finish()
	{
		base.Finish();
		Manager.Get<StateMachineManager>().PopState();
	}

	public override void Run(Action onWin, Action onFail, int difficultyLevel, bool useFinish = true)
	{
		base.Run(onWin, onFail, difficultyLevel, useFinish);
		ModelManager modelManager = Manager.Get<ModelManager>();
		Manager.Get<StateMachineManager>().PushState<CurePatientState>(new CurePatientState.CurePatientStartParameter
		{
			difficultyLevel = difficultyLevel,
			addingSpeed = modelManager.hospitalSettings.GetMiniGameAddingSpeed(),
			losingSpeed = modelManager.hospitalSettings.GetMiniGameLosingSpeed(),
			startValue = modelManager.hospitalSettings.GetMiniGameStartProgress(),
			onWin = injectedOnWin,
			onFail = injectedOnFail
		});
	}
}
