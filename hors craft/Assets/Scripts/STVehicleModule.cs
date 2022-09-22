// DecompilerFi decompiler from Assembly-CSharp.dll class: STVehicleModule
using Common.Model;
using System;
using Uniblocks;
using UnityEngine;

[CreateAssetMenu(fileName = "STVehicleModule", menuName = "SaveTransformsModule/STVehicleModule")]
public class STVehicleModule : STModule
{
	public const string BLOCK_ID_KEY = "SpawnedByBlock";

	protected override Func<Settings, bool> transferSelectionFunction => TransferSelector;

	protected override Func<Settings, Settings> transferFunction => TransferFunction;

	protected override string transferPrefsKey => "STModule_" + GetType().BaseType;

	public override Settings Save(AbstractSaveTransform controller)
	{
		Settings settings = base.Save(controller);
		settings.SetInt("SpawnedByBlock", controller.GetComponentInChildren<VehicleController>().SpawnedByBlock);
		return settings;
	}

	public override GameObject Spawn(Settings settings)
	{
		GameObject gameObject = base.Spawn(settings);
		if (gameObject == null || settings == null)
		{
			return null;
		}
		VehicleController componentInChildren = gameObject.GetComponentInChildren<VehicleController>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.InitAfterSpawnByPlayer(settings.GetInt("SpawnedByBlock", -1));
		}
		CarMob componentInChildren2 = gameObject.GetComponentInChildren<CarMob>(includeInactive: true);
		CarNavigator componentInChildren3 = gameObject.GetComponentInChildren<CarNavigator>(includeInactive: true);
		if ((bool)componentInChildren2)
		{
			UnityEngine.Object.Destroy(componentInChildren2);
		}
		if ((bool)componentInChildren3)
		{
			UnityEngine.Object.Destroy(componentInChildren3);
		}
		return gameObject;
	}

	private bool TransferSelector(Settings settings)
	{
		GameObject prefab = GetPrefab(GetName(settings));
		if (prefab == null)
		{
			return false;
		}
		if (prefab.GetComponentInChildren<SaveTransform>().module == this)
		{
			return true;
		}
		return false;
	}

	private Settings TransferFunction(Settings settings)
	{
		if (!(Engine.EngineInstance != null))
		{
			settings.SetInt("SpawnedByBlock", -1);
		}
		settings.SetInt("SpawnedByBlock", -1);
		return settings;
	}
}
