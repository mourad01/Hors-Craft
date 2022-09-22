// DecompilerFi decompiler from Assembly-CSharp.dll class: DanceMiniGame
using Common.Managers;
using Gameplay.RhythmicMinigame;
using States;
using System;
using Uniblocks;
using UnityEngine;

[CreateAssetMenu(fileName = "DanceMinigameExe", menuName = "ScriptableObjects/MinigamesExe/Dance")]
public class DanceMiniGame : MiniGameExe
{
	public string animatorPath = "prefabs/dance_mob_animator";

	public Vector3 mobOffset;

	protected override StatsManager.MinigameType minigameType => StatsManager.MinigameType.DANCE;

	public override void Finish()
	{
		base.Finish();
		Manager.Get<StateMachineManager>().PopState();
	}

	public override void Run(Action onWin, Action onFail, int difficultyLevel, bool useFinish = true)
	{
		base.Run(onWin, onFail, difficultyLevel, useFinish);
		Rigidbody rigidbody = UnityEngine.Object.FindObjectOfType<CameraEventsSender>().inFrontObject.rigidbody;
		if (rigidbody == null)
		{
			UnityEngine.Debug.LogError("Error Error, no object in front");
			return;
		}
		PlayerGraphic componentInChildren = rigidbody.GetComponentInChildren<PlayerGraphic>();
		Pettable componentInChildren2 = rigidbody.GetComponentInChildren<Pettable>();
		if (componentInChildren2 == null || componentInChildren == null)
		{
			UnityEngine.Debug.LogError($"Error Error, wrong object in front {rigidbody.name}");
			return;
		}
		injectedOnWin = (Action)Delegate.Combine(injectedOnWin, (Action)delegate
		{
			Manager.Get<StateMachineManager>().GetStateInstance<RhythmicMinigameState>().StopAllCoroutines();
		});
		Manager.Get<StateMachineManager>().PushState<RhythmicMinigameState>(new RhythmicMinigameStateStartParameter
		{
			graphicScene = new PonyDanceScene(PlayerGraphic.GetControlledPlayerInstance(), rigidbody.GetComponentInChildren<PlayerGraphic>(), rigidbody.GetComponentInChildren<Pettable>(), new PonyDanceScene.DancePathes("prefabs/dance_player_animator", animatorPath), new PonyDanceScene.DanceOffsets(PonyDanceScene.DanceOffsets.defaultPlayerOffset, mobOffset, PonyDanceScene.DanceOffsets.defaultDiscoBallOffset)),
			onWin = injectedOnWin,
			onFail = injectedOnFail
		});
	}
}
