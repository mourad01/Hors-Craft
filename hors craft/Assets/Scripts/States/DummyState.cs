// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DummyState
using Common.Managers.States;

namespace States
{
	public class DummyState : XCraftUIState<DummyStateConnector>
	{
		private DummyStateStartParameter startParameter;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			startParameter = (parameter as DummyStateStartParameter);
			if (startParameter != null && startParameter.doOnStart != null)
			{
				startParameter.doOnStart();
			}
		}

		public override void FinishState()
		{
			if (startParameter != null && startParameter.doOnFinish != null)
			{
				startParameter.doOnFinish();
			}
			base.FinishState();
		}
	}
}
