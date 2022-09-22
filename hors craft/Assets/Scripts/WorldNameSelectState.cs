// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldNameSelectState
using Common.Managers;
using Common.Managers.States;
using States;

public class WorldNameSelectState : XCraftUIState<WorldNameSelect>
{
	public class WorldSelectType : StartParameter
	{
		public bool create;

		public string currentWorldId;

		public WorldSelectType(bool create, string currentWorldId)
		{
			this.create = create;
			this.currentWorldId = currentWorldId;
		}
	}

	private string currentWorldId = string.Empty;

	public override void StartState(StartParameter startParameter)
	{
		base.StartState(startParameter);
		WorldSelectType worldSelectType = startParameter as WorldSelectType;
		if (worldSelectType != null)
		{
			if (worldSelectType.create)
			{
				base.connector.InitForCreation(OnReturn, OnCreate);
				return;
			}
			base.connector.InitForRename(worldSelectType.currentWorldId, OnReturn, OnRename);
			currentWorldId = worldSelectType.currentWorldId;
		}
	}

	public void OnReturn()
	{
		Manager.Get<StateMachineManager>().PopState();
	}

	public void OnCreate(string name)
	{
		string id = Manager.Get<SavedWorldManager>().AddNewUserWorld(name);
		Manager.Get<SavedWorldManager>().TryToSelectWorldById(id);
		SavedWorldManager.ResetCurrentWorld(Manager.Get<SavedWorldManager>().ShouldSaveAtSelect());
		if (Manager.Get<StateMachineManager>().ContainsState(typeof(ChooseFactionState)))
		{
			Manager.Get<StateMachineManager>().SetState<ChooseFactionState>();
		}
	}

	public void OnRename(string newName)
	{
		Manager.Get<SavedWorldManager>().RenameWorld(currentWorldId, newName);
		Manager.Get<StateMachineManager>().PopState();
	}
}
