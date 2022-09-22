// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalVehicleContext
public class SurvivalVehicleContext : InteractiveObjectContext
{
	public Health health;

	public override string GetContent()
	{
		return health.gameObject.name;
	}
}
