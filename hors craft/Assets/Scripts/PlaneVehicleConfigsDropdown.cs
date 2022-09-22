// DecompilerFi decompiler from Assembly-CSharp.dll class: PlaneVehicleConfigsDropdown
using System.Collections.Generic;
using UnityEngine;

public class PlaneVehicleConfigsDropdown : PropertyAttribute
{
	public PlaneVehicleConfig[] Configs
	{
		get;
		private set;
	}

	public string[] Names
	{
		get;
		private set;
	}

	public PlaneVehicleConfigsDropdown()
	{
		Configs = AllVehicleConfigs();
		Names = GetNamesArray(Configs);
	}

	public PlaneVehicleConfig[] AllVehicleConfigs()
	{
		return GetAllInstances<PlaneVehicleConfig>();
	}

	public T[] GetAllInstances<T>() where T : ScriptableObject
	{
		return null;
	}

	public string[] GetNamesArray(PlaneVehicleConfig[] list)
	{
		List<string> list2 = new List<string>();
		foreach (PlaneVehicleConfig planeVehicleConfig in list)
		{
			list2.Add(planeVehicleConfig.name);
		}
		return list2.ToArray();
	}
}
