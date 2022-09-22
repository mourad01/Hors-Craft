// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingFullyUpgradedFragment
using Common.Managers;
using UnityEngine.UI;

namespace States
{
	public class CraftingFullyUpgradedFragment : Fragment
	{
		public CraftItem gigantBlock;

		public Button returnButton;

		private CraftingFragment.CrafttStartParameter startParam;

		public CraftableStatus currentItemStatus;

		public int currentcraftableCount;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			InitBlock();
			InitButtons();
		}

		public void InitBlock()
		{
			int sourceBlockId = startParam.sourceBlockId;
			currentItemStatus = Manager.Get<CraftingManager>().GetCraftableStatus(sourceBlockId);
			currentcraftableCount = Singleton<PlayerData>.get.playerItems.GetCraftableCount(sourceBlockId);
			gigantBlock.Init(sourceBlockId, currentcraftableCount, currentItemStatus, Manager.Get<CraftingManager>().GetCraftable(sourceBlockId).GetGraphic());
		}

		public void InitButtons()
		{
			returnButton.onClick.AddListener(delegate
			{
				try
				{
					startParam.parentFragment.UpdateRecipesFragment();
				}
				catch
				{
				}
				startParam.parentFragment.SetState(CraftingFragment.State.Recipes, startParam);
			});
		}
	}
}
