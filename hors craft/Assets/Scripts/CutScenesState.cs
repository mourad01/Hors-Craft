// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenesState
using Common.Managers.States;
using Common.Utils;
using States;

public class CutScenesState : XCraftUIState<CutScenesStateConnector>
{
	protected override bool hasBackground => false;

	protected override bool hasBackgroundOverlay => false;

	//protected override bool canShowBanner => false;

	public override void StartState(StartParameter startParameter)
	{
		base.StartState(startParameter);
		if (startParameter is CutScenesStateParameter)
		{
			if ((startParameter as CutScenesStateParameter).onInitEnded != null)
			{
				base.connector.ManualStartHandler();
				(startParameter as CutScenesStateParameter).onInitEnded();
			}
			TimeScaleHelper.value = 1f;
		}
	}
}
