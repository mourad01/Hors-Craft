// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SettingsFragment
using UnityEngine;

namespace States
{
	public class SettingsFragment : Fragment
	{
		public Transform fragmentsParent;

		public GameObject buttonsPrefab;

		public GameObject slidersPrefab;

		private Fragment buttonsFragment;

		private Fragment slidersFragment;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			slidersFragment = Object.Instantiate(slidersPrefab, fragmentsParent).GetComponent<Fragment>();
			slidersFragment.Init(startParameter);
			buttonsFragment = Object.Instantiate(buttonsPrefab, fragmentsParent).GetComponent<Fragment>();
			buttonsFragment.Init(startParameter);
		}

		public override void Destroy()
		{
			buttonsFragment.Destroy();
			slidersFragment.Destroy();
			base.Destroy();
		}
	}
}
