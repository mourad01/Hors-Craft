// DecompilerFi decompiler from Assembly-CSharp.dll class: HeldBlockContext
public class HeldBlockContext : FactContext
{
	public ushort block;

	public override string GetContent()
	{
		return base.GetContent() + "heldBlock: " + block;
	}
}
