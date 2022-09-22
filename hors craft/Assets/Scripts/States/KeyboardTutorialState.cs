// DecompilerFi decompiler from Assembly-CSharp.dll class: States.KeyboardTutorialState
using Common.Managers;
using Common.Managers.States;
using UnityEngine;

namespace States
{
	public class KeyboardTutorialState : XCraftUIState<KeyboardTutorialStateConnector>
	{
		private GameObject keyboardtutorialFragment;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			base.connector.onApplyButtonClicked = OnApply;
			keyboardtutorialFragment = Object.Instantiate(base.connector.fragmentPrefab, base.connector.fragmentParent);
			keyboardtutorialFragment.transform.SetParent(base.connector.transform);
			keyboardtutorialFragment.transform.SetAsFirstSibling();
			keyboardtutorialFragment.GetComponent<Fragment>().Init(null);
		}

		private void OnApply()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		public override void FinishState()
		{
			keyboardtutorialFragment.GetComponent<Fragment>().Destroy();
			base.FinishState();
		}
	}
}
