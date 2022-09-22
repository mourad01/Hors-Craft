// DecompilerFi decompiler from Assembly-CSharp.dll class: LoveContext
public class LoveContext : FactContext
{
	public float loveValue;

	public float maxLoveValue;

	public override string GetContent()
	{
		return base.GetContent() + loveValue + "/" + maxLoveValue;
	}
}
