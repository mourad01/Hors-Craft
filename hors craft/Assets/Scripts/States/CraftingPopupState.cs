// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingPopupState
using Common.Managers;
using Common.Managers.States;
using UnityEngine;

namespace States
{
	public class CraftingPopupState : XCraftUIState<CraftingPopupStateConnector>
	{
		public GameObject craftingFragment;

		public GameObject recipesFragment;

		public GameObject ticketRecipesFragment;

		private CraftingFragment crafting;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			CraftingPopupStateStartParameter craftingPopupStateStartParameter = startParameter as CraftingPopupStateStartParameter;
			craftingPopupStateStartParameter.blueprintTicketsEnabled = Manager.Contains<TicketsManager>();
			crafting = Object.Instantiate(craftingFragment, base.connector.parent, worldPositionStays: false).GetComponent<CraftingFragment>();
			if (craftingPopupStateStartParameter.blueprintTicketsEnabled)
			{
				crafting.recipesPrefab = ticketRecipesFragment;
				crafting.Init(null);
			}
			else
			{
				crafting.recipesPrefab = recipesFragment;
				crafting.Init(null);
			}
			CraftingRecipesFragment componentInChildren = crafting.GetComponentInChildren<CraftingRecipesFragment>();
			if (componentInChildren is CraftingPopupRecipesFragment)
			{
				((CraftingPopupRecipesFragment)componentInChildren).listType = craftingPopupStateStartParameter.type;
			}
			componentInChildren.UpdateFragment();
			base.connector.onReturnButton = OnReturn;
			base.connector.AdjustWindow();
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			crafting.UpdateFragment();
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		}
	}
}
