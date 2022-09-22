// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WhatsNewPopUpState
using Common.Managers;
using Common.Managers.States;
using UnityEngine;

namespace States
{
	public class WhatsNewPopUpState : XCraftUIState<WhatsNewPopUpStateConnector>
	{
		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onReturnButtonClicked = OnReturn;
			base.connector.UpdateFeatureList();
			PlayerPrefs.SetInt("whatsnew.popup.showed." + Application.version, 1);
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
