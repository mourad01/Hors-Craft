// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupItemLongBag
public class DressupItemLongBag : DressupItem
{
	public override void ItemCreated(DressupItemContext context)
	{
		if (context is DressupLongBagItemContext && !((context as DressupLongBagItemContext).highHandItemPlacement == null))
		{
			base.transform.SetParent((context as DressupLongBagItemContext).highHandItemPlacement.transform);
		}
	}
}
