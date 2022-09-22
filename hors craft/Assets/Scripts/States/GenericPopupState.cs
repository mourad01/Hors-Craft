// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GenericPopupState
using Common.Managers.States;

namespace States
{
	public class GenericPopupState : XCraftUIState<GenericPopupStateConnector>
	{
		protected GenericPopupStateStartParameter startParameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			startParameter = (parameter as GenericPopupStateStartParameter);
			startParameter.configureLeftButton(base.connector.leftButton, base.connector.leftButtonText);
			startParameter.configureRightButton(base.connector.rightButton, base.connector.rightButtonText);
			startParameter.configureMessage(base.connector.message);
			if (startParameter.configureTitle != null)
			{
				if (base.connector.title != null)
				{
					startParameter.configureTitle(base.connector.title);
					base.connector.title.ForceRefresh();
				}
			}
			else
			{
				if (base.connector.title != null)
				{
					base.connector.title.gameObject.SetActive(value: false);
				}
				if (base.connector.line != null)
				{
					base.connector.line.gameObject.SetActive(value: false);
				}
			}
			base.connector.leftButtonText.ForceRefresh();
			base.connector.rightButtonText.ForceRefresh();
			base.connector.message.ForceRefresh();
		}

		public override void UpdateState()
		{
			base.UpdateState();
		}

		public override void FinishState()
		{
			base.FinishState();
		}
	}
}
