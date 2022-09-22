// DecompilerFi decompiler from Assembly-CSharp.dll class: TameContext
public class TameContext : FactContext
{
	public Pettable pettable;

	public bool tameButtonAllowed;

	public override string GetContent()
	{
		return base.GetContent() + "Taming allowed: " + tameButtonAllowed + ((!(pettable != null)) ? string.Empty : (", pettable: " + pettable.gameObject.name));
	}
}
