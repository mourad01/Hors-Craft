// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ModesFragment
using Common.Managers;
using Gameplay;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ModesFragment : Fragment
	{
		public Button resetWorldButton;

		public Button changeWorldButton;

		public Button resetPosition;

		public Button cookingButton;

		private void Awake()
		{
			if (Manager.Get<ModelManager>().worldsSettings.GetWorldsEnabled())
			{
				resetWorldButton.gameObject.SetActive(value: false);
				changeWorldButton.gameObject.SetActive(!Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate());
				changeWorldButton.onClick.AddListener(delegate
				{
					OnChangeWorld();
				});
				resetPosition.gameObject.SetActive(value: true);
				resetPosition.onClick.AddListener(delegate
				{
					OnResetPlayerPosition();
				});
			}
			else
			{
				resetPosition.gameObject.SetActive(value: false);
				changeWorldButton.gameObject.SetActive(value: false);
				resetWorldButton.gameObject.SetActive(value: true);
				resetWorldButton.onClick.AddListener(delegate
				{
					OnResetWorld();
				});
			}
			cookingButton.gameObject.SetActive(Manager.Contains<CookingManager>());
			cookingButton.onClick.AddListener(delegate
			{
				OnCooking();
			});
		}

		private void OnResetWorld()
		{
			Manager.Get<StateMachineManager>().PushState<ResetWorldPopUpState>();
		}

		private void OnChangeWorld()
		{
			Manager.Get<StateMachineManager>().PushState<ChooseWorldState>();
		}

		private void OnResetPlayerPosition()
		{
			PlayerMovement playerMovement = Object.FindObjectOfType<PlayerMovement>();
			if (!(playerMovement == null))
			{
				if (playerMovement.IsMounted())
				{
					MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.MOUNTED_MOB);
					playerMovement.Unmount();
				}
				ArmedPlayer componentInParent = playerMovement.GetComponentInParent<ArmedPlayer>();
				VehicleController componentInParent2 = playerMovement.GetComponentInParent<VehicleController>();
				if ((bool)componentInParent2)
				{
					VehicleHoverAction hoverAction = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<CameraEventsSender>().GetHoverAction<VehicleHoverAction>();
					hoverAction.OnVehicleUse();
				}
				if ((bool)componentInParent)
				{
					componentInParent.OnVehicleExit();
				}
				Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
				playerMovement.ClearPositionFromThisWorld();
				playerMovement.OnGameplayStarted();
			}
		}

		private void OnCooking()
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			InteractiveKitchen.GoToCooking("modes.kitchen", Manager.Get<CookingManager>().defaultSceneName);
		}
	}
}
