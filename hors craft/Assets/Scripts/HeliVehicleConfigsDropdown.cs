// DecompilerFi decompiler from Assembly-CSharp.dll class: HeliVehicleConfigsDropdown
using System.Collections.Generic;
using UnityEngine;

public class HeliVehicleConfigsDropdown : PropertyAttribute
{
	public HeliVehicleConfig[] Configs
	{
		get;
		private set;
	}

	public string[] Names
	{
		get;
		private set;
	}

	public HeliVehicleConfigsDropdown()
	{
		Configs = AllVehicleConfigs();
		Names = GetNamesArray(Configs);
	}

	public HeliVehicleConfig[] AllVehicleConfigs()
	{
		return GetAllInstances<HeliVehicleConfig>();
	}

	public T[] GetAllInstances<T>() where T : ScriptableObject
	{
		return null;
	}

	public string[] GetNamesArray(HeliVehicleConfig[] list)
	{
		List<string> list2 = new List<string>();
		foreach (HeliVehicleConfig heliVehicleConfig in list)
		{
			list2.Add(heliVehicleConfig.name);
		}
		return list2.ToArray();
	}
}
