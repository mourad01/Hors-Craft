// DecompilerFi decompiler from Assembly-CSharp.dll class: GameplayStartedTutorialCallbackExe
public class GameplayStartedTutorialCallbackExe : InitialPopupExecution
{
	public override void Show()
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.GAMEPLAY_STARTED);
	}
}
