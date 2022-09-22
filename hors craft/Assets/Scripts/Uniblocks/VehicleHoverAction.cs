// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VehicleHoverAction
using Common.Managers;
using Gameplay;
using GameUI;
using States;
using UnityEngine;

namespace Uniblocks
{
	public class VehicleHoverAction : HoverAction
	{
		private InteractiveObjectContext vehicleContext;

		private InteractiveObjectContext usedVehicleContext;

		private VoxelEvents hoveredEvents;

		private GameObject usedVehicle;

		private GameObject vehicle;

		public VehicleHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MOVEMENT, new AircraftSteerintContext
			{
				setAscendButton = SetAscendButton,
				setThrustButton = SetThrustButton,
				setDescendButton = SetDescendButton
			});
		}

		private void SetThrustButton(SimpleRepeatButton thrustRepeatButton)
		{
			PlaneController component = usedVehicle.GetComponent<PlaneController>();
			if (component != null)
			{
				thrustRepeatButton.gameObject.SetActive(value: true);
			}
			else
			{
				thrustRepeatButton.gameObject.SetActive(value: false);
			}
			thrustRepeatButton.onFingerDown = delegate
			{
				if (!(usedVehicle == null))
				{
					PlaneController component3 = usedVehicle.GetComponent<PlaneController>();
					if (component3 != null)
					{
						component3.isThrustHold = true;
					}
				}
			};
			thrustRepeatButton.onFingerUp = delegate
			{
				if (!(usedVehicle == null))
				{
					PlaneController component2 = usedVehicle.GetComponent<PlaneController>();
					if (component2 != null)
					{
						component2.isThrustHold = false;
					}
				}
			};
		}

		private void SetAscendButton(SimpleRepeatButton ascendRepeatButton)
		{
			HeliController component = usedVehicle.GetComponent<HeliController>();
			if (component != null)
			{
				ascendRepeatButton.gameObject.SetActive(value: true);
			}
			else
			{
				ascendRepeatButton.gameObject.SetActive(value: false);
			}
			ascendRepeatButton.onFingerDown = delegate
			{
				if (!(usedVehicle == null))
				{
					HeliController component3 = usedVehicle.GetComponent<HeliController>();
					if (component3 != null)
					{
						component3.isAscendHold = true;
					}
				}
			};
			ascendRepeatButton.onFingerUp = delegate
			{
				if (!(usedVehicle == null))
				{
					HeliController component2 = usedVehicle.GetComponent<HeliController>();
					if (component2 != null)
					{
						component2.isAscendHold = false;
					}
				}
			};
		}

		private void SetDescendButton(SimpleRepeatButton descendRepeatButton)
		{
			HeliController component = usedVehicle.GetComponent<HeliController>();
			if (component != null)
			{
				descendRepeatButton.gameObject.SetActive(value: true);
			}
			else
			{
				descendRepeatButton.gameObject.SetActive(value: false);
			}
			descendRepeatButton.onFingerDown = delegate
			{
				if (!(usedVehicle == null))
				{
					HeliController component3 = usedVehicle.GetComponent<HeliController>();
					if (component3 != null)
					{
						component3.isDescendHold = true;
					}
				}
			};
			descendRepeatButton.onFingerUp = delegate
			{
				if (!(usedVehicle == null))
				{
					HeliController component2 = usedVehicle.GetComponent<HeliController>();
					if (component2 != null)
					{
						component2.isDescendHold = false;
					}
				}
			};
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			GetTargetVehicle();
			UpdateInFrontOfVehicle();
			UpdateInVehicle();
		}

		private void GetTargetVehicle()
		{
			vehicle = null;
			if (hitInfo.hit.transform != null && hitInfo.hit.transform.tag.ToUpper().Equals("VEHICLE"))
			{
				vehicle = hitInfo.hit.transform.gameObject;
			}
		}

		private void UpdateInFrontOfVehicle()
		{
			if (vehicle != null)
			{
				if (vehicleContext == null || vehicleContext.obj != vehicle)
				{
					AddInFrontOfVehicleFact(vehicle);
				}
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_VEHICLE, vehicleContext);
				vehicleContext = null;
			}
		}

		private void AddInFrontOfVehicleFact(GameObject vehicle)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_VEHICLE, vehicleContext);
			vehicleContext = new InteractiveObjectContext
			{
				obj = vehicle,
				useAction = OnVehicleUse
			};
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_VEHICLE, vehicleContext);
		}

		private void UpdateInVehicle()
		{
			if (usedVehicle != null)
			{
				if (usedVehicleContext == null || usedVehicleContext.obj != usedVehicle)
				{
					AddInVehicleFact(usedVehicle);
				}
			}
			else
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_VEHICLE, usedVehicleContext);
				usedVehicleContext = null;
			}
		}

		private void AddInVehicleFact(GameObject vehicle)
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_VEHICLE, usedVehicleContext);
			usedVehicleContext = CreateContext(vehicle);
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_VEHICLE, usedVehicleContext);
		}

		private InteractiveObjectContext CreateContext(GameObject vehicle)
		{
			Health componentInChildren = vehicle.GetComponentInChildren<Health>();
			if (componentInChildren != null)
			{
				SurvivalVehicleContext survivalVehicleContext = new SurvivalVehicleContext();
				survivalVehicleContext.obj = vehicle;
				survivalVehicleContext.useAction = OnVehicleUse;
				survivalVehicleContext.health = componentInChildren;
				return survivalVehicleContext;
			}
			InteractiveObjectContext interactiveObjectContext = new InteractiveObjectContext();
			interactiveObjectContext.obj = vehicle;
			interactiveObjectContext.useAction = OnVehicleUse;
			return interactiveObjectContext;
		}

		public void ForceEnterVehicle(GameObject vehicle)
		{
			this.vehicle = vehicle;
			OnVehicleUse();
		}

		public void OnVehicleUse()
		{
			GameObject gameObject = PlayerGraphic.GetControlledPlayerInstance().gameObject;
			PlayerController component = gameObject.GetComponent<PlayerController>();
			if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_VEHICLE) && vehicle != null && !vehicle.GetComponentInChildren<VehicleController>().IsInUse)
			{
				if (usedVehicle != null)
				{
					DiscardVehicle();
					return;
				}
				usedVehicle = vehicle;
				VehicleController componentInChildren = usedVehicle.GetComponentInChildren<VehicleController>();
				if (componentInChildren is IFlyingVehicle)
				{
					component.isInFlyingVehicle = true;
				}
				componentInChildren.VehicleActivate(gameObject);
				SurvivalPhaseContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE);
				GameplayState.Substates substate = componentInChildren.substateToPush;
				if (factContext != null && factContext.isCombat)
				{
					substate = ((!component.isInFlyingVehicle) ? GameplayState.Substates.SURVIVAL_VEHICLE_COMBAT : GameplayState.Substates.SURVIVAL_FLYING_VEHICLE_COMBAT);
				}
				Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().SetSubstate(substate);
			}
			else
			{
				DiscardVehicle();
				SurvivalPhaseContext factContext2 = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE);
				GameplayState.Substates substate2 = (factContext2 != null && factContext2.isCombat) ? GameplayState.Substates.SURVIVAL_COMBAT : GameplayState.Substates.WALKING;
				Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().SetSubstate(substate2);
				if (gameObject.GetComponent<ArmedPlayer>() != null)
				{
					gameObject.GetComponent<ArmedPlayer>().OnVehicleExit();
				}
			}
		}

		private void DiscardVehicle()
		{
			if (!(usedVehicle == null))
			{
				usedVehicle.GetComponentInChildren<VehicleController>().StopUsing();
				usedVehicle = null;
				vehicle = null;
			}
		}
	}
}
