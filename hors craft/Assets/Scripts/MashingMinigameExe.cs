// DecompilerFi decompiler from Assembly-CSharp.dll class: MashingMinigameExe
using Common.Managers;
using States;
using System;
using Uniblocks;
using UnityEngine;

[CreateAssetMenu(fileName = "MashingMinigameExe", menuName = "ScriptableObjects/MinigamesExe/Mashing")]
public class MashingMinigameExe : MiniGameExe
{
	public GameObject graphicsPrefab;

	public float tapStrength = 5f;

	public float opponentCD = 0.1f;

	public float defaultProgress = 50f;

	public AnimationCurve[] opponentBehaviours;

	public GameObject healingStation;

	public RuntimeAnimatorController patientAnimator;

	protected override StatsManager.MinigameType minigameType => StatsManager.MinigameType.MASHING;

	public override void Run(Action onWin, Action onFail, int difficultyLevel, bool useFinish = true)
	{
		base.Run(onWin, onFail, difficultyLevel, useFinish);
		MashToFillBarHealingGraphics component = graphicsPrefab.GetComponent<MashToFillBarHealingGraphics>();
		component.healingStationPrefab = healingStation;
		component.patientAnimator = patientAnimator;
		component.realPatient = UnityEngine.Object.FindObjectOfType<CameraEventsSender>().inFrontObject.collider.attachedRigidbody.gameObject;
		TappingGameState stateInstance = Manager.Get<StateMachineManager>().GetStateInstance<TappingGameState>();
		stateInstance.onWin = injectedOnWin;
		stateInstance.onFail = injectedOnFail;
		Manager.Get<StateMachineManager>().PushState<TappingGameState>(new TappingGameStateStartParameter
		{
			graphicPrefab = graphicsPrefab,
			gameBehaviour = new MashToFillBarHealingGame(opponentBehaviours, tapStrength, opponentCD, defaultProgress)
		});
	}

	public override void Finish()
	{
		base.Finish();
		Manager.Get<StateMachineManager>().PopState();
	}
}
