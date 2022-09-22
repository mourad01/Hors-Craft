// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.DuckShooter
using Common.Managers;
using Gameplay.RhythmicMinigame;
using States;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class DuckShooter : InteractiveObject
	{
		public GameObject hitParticles;

		public GameObject missParticles;

		public GameObject[] ducks;

		public GameObject shotgun;

		public override void OnUse()
		{
			base.OnUse();
			TicketsManager.TakeEntranceFeeIfPossible(StartMinigame);
		}

		private void StartMinigame()
		{
			Manager.Get<StateMachineManager>().PushState<RhythmicMinigameState>(new RhythmicMinigameStateStartParameter
			{
				graphicScene = new DuckScene(new DuckScene.DucksConfiguration
				{
					ducks = ducks,
					shootRange = base.gameObject,
					hitParticles = hitParticles,
					missParticles = missParticles,
					shotgun = shotgun
				})
			});
		}
	}
}
