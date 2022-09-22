// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ProgressCraftingFragment
namespace States
{
	public class ProgressCraftingFragment : CraftingFragment
	{
		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			enableResourcesForAd = false;
			resourcesInstance.SetActive(value: true);
			resourcesInstance.transform.SetAsLastSibling();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			resourcesInstance.transform.SetAsLastSibling();
			resourcesInstance.SetActive(value: true);
		}

		public override void SetState(State stateToActive, CrafttStartParameter parameter)
		{
			base.SetState(stateToActive, parameter);
			resourcesInstance.transform.SetAsLastSibling();
		}

		public override void onCraft(int itemId)
		{
			enableResourcesForAd = true;
			base.onCraft(itemId);
			enableResourcesForAd = false;
		}
	}
}
