// DecompilerFi decompiler from Assembly-CSharp.dll class: ChooseFactionState
using Common.Managers;
using Common.Managers.States;
using States;

public class ChooseFactionState : XCraftUIState<ChooseFactionStateConnector>
{
	public override void StartState(StartParameter startParameter)
	{
		base.StartState(startParameter);
		base.connector.Init(OnEast, OnWest);
	}

	private void OnEast()
	{
		Faction east = base.connector.GetEast();
		east.SetFaction();
		Manager.Get<StateMachineManager>().PopState();
	}

	private void OnWest()
	{
		Faction west = base.connector.GetWest();
		west.SetFaction();
		Manager.Get<StateMachineManager>().PopState();
	}
}
