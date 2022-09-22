// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RankingsState
using Common.Managers;
using Common.Managers.States;
using UnityEngine;

namespace States
{
	public class RankingsState : XCraftUIState<RankingsStateConnector>
	{
		public GameObject rankingFragment;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onReturnButtonClicked = OnReturn;
			GameObject gameObject = Object.Instantiate(rankingFragment, base.connector.fragmentParent);
			gameObject.GetComponent<Fragment>().Init(new FragmentStartParameter());
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
