// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WorldShopState
using Common.Managers.States;

namespace States
{
	public class WorldShopState : XCraftUIState<WorldShopStateConnector>
	{
		public static bool stateVisitedAtThisSession;

		public override void StartState(StartParameter parameter)
		{
			stateVisitedAtThisSession = true;
			base.StartState(parameter);
			base.connector.Init();
		}
	}
}
