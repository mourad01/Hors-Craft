// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingState
using Common.Managers;
using Common.Managers.States;

namespace States
{
	public class CraftingState : XCraftUIState<CraftingStateConnector>
	{
		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			base.connector.onReturnButton = onReturn;
			base.connector.onBlockClick = onBlockClick;
			base.connector.onLesserReturnButton = onInsideReturn;
			base.connector.Init(Manager.Get<CraftingManager>().GetResourcesList(), Manager.Get<CraftingManager>().GetCraftableList(), onCraft);
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			base.connector.UpdateResourcesList();
			base.connector.UpdateCraftableList();
		}

		private void onBlockClick(int itemId)
		{
			if (Manager.Get<CraftingManager>().GetCraftable(itemId).status == CraftableStatus.Locked)
			{
				base.connector.SetQuestState(itemId);
			}
			else
			{
				base.connector.SetCraftingState(itemId);
			}
		}

		private void onInsideReturn()
		{
			base.connector.SetListState();
		}

		private void onCraft(int itemId)
		{
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(itemId);
			Singleton<PlayerData>.get.playerItems.Craft(itemId);
			base.connector.EnableCraftButton(craftable.status == CraftableStatus.Craftable);
			base.connector.UpdateAfterCrafting(itemId);
		}

		private void onReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
