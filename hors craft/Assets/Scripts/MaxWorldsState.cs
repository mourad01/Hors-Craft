// DecompilerFi decompiler from Assembly-CSharp.dll class: MaxWorldsState
using Common.Managers;
using Common.Managers.States;
using Common.Waterfall;
using States;

public class MaxWorldsState : XCraftUIState<MaxWorldsConnector>
{
	public override void StartState(StartParameter startParameter)
	{
		base.StartState(startParameter);
		base.connector.Init(OnReturn, OnWatchAd);
	}

	public void OnReturn()
	{
		Manager.Get<StateMachineManager>().PopState();
	}

	public void OnWatchAd()
	{
		//AdWaterfall.get.ShowInterstitialAd("Getworldslot");
		Manager.Get<StatsManager>().AdShownWithReason(StatsManager.AdReason.XCRAFT_MAX_WORLDS_INCREASE);
		Manager.Get<StateMachineManager>().PushState<WorldNameSelectState>(new WorldNameSelectState.WorldSelectType(create: true, string.Empty));
	}
}
