// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupManager
using Common.Managers;
using Gameplay;

public class DressupManager : Manager
{
	private ModelManager modelManager;

	public override void Init()
	{
		modelManager = Manager.Get<ModelManager>();
	}
}
