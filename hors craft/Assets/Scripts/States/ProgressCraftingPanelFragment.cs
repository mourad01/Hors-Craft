// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ProgressCraftingPanelFragment
using UnityEngine;

namespace States
{
	public class ProgressCraftingPanelFragment : ProgressFragment
	{
		private CraftingFragment.CrafttStartParameter startParam;

		private ProgressCraftingFragment progressFragment => startParam.parentFragment as ProgressCraftingFragment;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			base.transform.localScale = Vector3.one;
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
		}
	}
}
