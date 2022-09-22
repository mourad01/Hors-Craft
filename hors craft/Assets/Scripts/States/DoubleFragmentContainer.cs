// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DoubleFragmentContainer
using UnityEngine;

namespace States
{
	public class DoubleFragmentContainer : Fragment
	{
		public Transform fragmentsParent;

		public GameObject firstPrefab;

		public GameObject secondPrefab;

		private Fragment firstFragment;

		private Fragment secondFragment;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			firstFragment = Object.Instantiate(firstPrefab, fragmentsParent).GetComponent<Fragment>();
			firstFragment.Init(startParameter);
			secondFragment = Object.Instantiate(secondPrefab, fragmentsParent).GetComponent<Fragment>();
			secondFragment.Init(startParameter);
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			firstFragment.UpdateFragment();
			secondFragment.UpdateFragment();
		}

		public override void Destroy()
		{
			secondFragment.Destroy();
			firstFragment.Destroy();
			base.Destroy();
		}
	}
}
