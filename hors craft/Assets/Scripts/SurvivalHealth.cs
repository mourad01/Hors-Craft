// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalHealth
public class SurvivalHealth : FactContext
{
	public Health healthComponent;

	public override string GetContent()
	{
		return healthComponent.gameObject.name;
	}
}
