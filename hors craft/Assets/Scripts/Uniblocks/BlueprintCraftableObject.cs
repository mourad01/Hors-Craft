// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlueprintCraftableObject
using Common.Managers;
using States;
using System;
using UnityEngine;

namespace Uniblocks
{
	public class BlueprintCraftableObject : MonoBehaviour, ICustomCraftingItem
	{
		[NonSerialized]
		public BlueprintData blueprintData;

		public string blueprintResourceName;

		public Sprite blueprintSprite;

		public void OnCraftAction()
		{
			Manager.Get<StatsManager>().Blueprint(StatsManager.BlueprintAction.CRAFTED);
		}

		public void OnUseAction(int id)
		{
			if (Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().currentSubstate.substate != 0)
			{
				Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("blueprint.cant.place.1", "You can't place blueprints while in vehicle!"));
				return;
			}
			if (PlayerGraphic.GetControlledPlayerInstance().GetComponent<PlayerMovement>().IsMounted())
			{
				PlayerGraphic.GetControlledPlayerInstance().GetComponent<PlayerMovement>().ForceUnmount();
			}
			SurvivalPhaseContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE);
			if (factContext != null && factContext.isCombat)
			{
				Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("blueprint.cant.place.3", "You can't place blueprints while in Combat!"));
				return;
			}
			blueprintData = BlueprintDataFiles.ReadDataFromResources(blueprintResourceName);
			GameObject gameObject = new GameObject("Blueprint placement");
			gameObject.transform.position = PlayerGraphic.GetControlledPlayerInstance().transform.position;
			IsometricPlaceableBlueprint isometricPlaceableBlueprint = gameObject.AddComponent<IsometricPlaceableBlueprint>();
			isometricPlaceableBlueprint.SetBlueprint(blueprintData, blueprintResourceName);
			isometricPlaceableBlueprint.craftableId = id;
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			Manager.Get<StateMachineManager>().PushState<IsometricObjectPlacementState>(new IsometricObjectPlacementStateStartParameter
			{
				obj = isometricPlaceableBlueprint
			});
		}
	}
}
