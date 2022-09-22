// DecompilerFi decompiler from Assembly-CSharp.dll class: ArmorPercentageMod
public class ArmorPercentageMod : Armor
{
	public float damageReduction => base.armorValue / 100f;

	public override float TakeDamage(float damage)
	{
		return (1f - damageReduction) * damage;
	}

	public override void TryDestroy()
	{
	}
}
