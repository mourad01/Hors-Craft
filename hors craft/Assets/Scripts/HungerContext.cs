// DecompilerFi decompiler from Assembly-CSharp.dll class: HungerContext
public class HungerContext : FactContext
{
	public float hunger;

	public override string GetContent()
	{
		return base.GetContent() + "hunger: " + hunger;
	}
}
