// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.StartMashingGame
using Common.Managers;
using States;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class StartMashingGame : InteractiveObject
	{
		public GameObject graphicsPrefab;

		public float tapStrength = 5f;

		public float opponentCD = 0.1f;

		public float defaultProgress = 50f;

		public AnimationCurve[] opponentBehaviours;

		public Animator enemyAnimator;

		public GameObject ourArm;

		public Transform cameraPivot;

		public override void OnUse()
		{
			base.OnUse();
			MashToFillBarWrestlingGraphics component = graphicsPrefab.GetComponent<MashToFillBarWrestlingGraphics>();
			component.ourArm = ourArm;
			component.enemyAnim = enemyAnimator;
			component.cameraPivot = cameraPivot;
			Manager.Get<StateMachineManager>().PushState<TappingGameState>(new TappingGameStateStartParameter
			{
				graphicPrefab = graphicsPrefab,
				gameBehaviour = new MashToFillBarWrestlingGame(opponentBehaviours, tapStrength, opponentCD, defaultProgress)
			});
		}
	}
}
