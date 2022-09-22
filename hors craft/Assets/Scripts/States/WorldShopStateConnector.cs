// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WorldShopStateConnector
using Common.Managers.States.UI;
using System.Collections;
using UnityEngine;

namespace States
{
	public class WorldShopStateConnector : UIConnector
	{
		public void Init()
		{
			StartCoroutine(InitializeFromInjector());
		}

		private IEnumerator InitializeFromInjector()
		{
			yield return new WaitForSecondsRealtime(0.01f);
			GetComponentInChildren<WorldsFragment>().Init(new WorldsFragment.WorldsFragmentStart(initFromPause: false));
		}
	}
}
