// DecompilerFi decompiler from Assembly-CSharp.dll class: MainQuestContext
using Gameplay.QuestsV2;

public class MainQuestContext : FactContext
{
	public MainQuest currentQuest;

	public override string GetContent()
	{
		return $"{currentQuest.ToString()}";
	}
}
