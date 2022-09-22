// DecompilerFi decompiler from Assembly-CSharp.dll class: STVelocityModule
using Common.Model;
using UnityEngine;

public class STVelocityModule : STModule
{
	public const string VELOCITY_KEY = "Velocity";

	public override void Register(AbstractSaveTransform controller)
	{
		base.Register(controller);
	}

	public override Settings Save(AbstractSaveTransform controller)
	{
		Settings settings = base.Save(controller);
		settings.SetInt("Velocity", 0);
		return settings;
	}

	public override GameObject Spawn(Settings settings)
	{
		return base.Spawn(settings);
	}
}
