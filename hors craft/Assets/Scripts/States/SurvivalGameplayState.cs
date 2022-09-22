// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SurvivalGameplayState
using Common.Behaviours;
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using UnityEngine;

namespace States
{
	public class SurvivalGameplayState : GameplayState
	{
		public bool hungerActivated;

		private GameObject _player;

		private Weapon weapon;

		protected GameObject player
		{
			get
			{
				if (_player == null)
				{
					PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
					if (controlledPlayerInstance != null)
					{
						_player = controlledPlayerInstance.gameObject;
					}
					if (_player != null && hungerActivated)
					{
						InitHunger();
					}
				}
				return _player;
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			if (hungerActivated)
			{
				InitHunger();
			}
			GlobalSettings.mode = GlobalSettings.MovingMode.WALKING;
		}

		public override void OnPause()
		{
			Manager.Get<StateMachineManager>().PushState<PauseState>(new PauseStateStartParameter
			{
				canSave = !Manager.Get<SurvivalManager>().IsCombatTime(),
				allowTimeChange = MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper,
				categoryToOpen = null
			});
		}

		private void InitHunger()
		{
			player.GetComponentInChildren<Hunger>().Activate();
		}

		public override void FreezeState()
		{
			if (player != null)
			{
				player.GetComponent<CharacterMotor>().enabled = false;
			}
			base.FreezeState();
		}

		public override void UnfreezeState()
		{
			if (player != null)
			{
				player.GetComponent<CharacterMotor>().enabled = true;
			}
			base.UnfreezeState();
		}
	}
}
