// DecompilerFi decompiler from Assembly-CSharp.dll class: BoatVehicleConfigsDropdown
using System.Collections.Generic;
using UnityEngine;

public class BoatVehicleConfigsDropdown : PropertyAttribute
{
	public BoatVehicleConfig[] Configs
	{
		get;
		private set;
	}

	public string[] Names
	{
		get;
		private set;
	}

	public BoatVehicleConfigsDropdown()
	{
		Configs = AllVehicleConfigs();
		Names = GetNamesArray(Configs);
	}

	public BoatVehicleConfig[] AllVehicleConfigs()
	{
		return GetAllInstances<BoatVehicleConfig>();
	}

	public T[] GetAllInstances<T>() where T : ScriptableObject
	{
		return null;
	}

	public string[] GetNamesArray(BoatVehicleConfig[] list)
	{
		List<string> list2 = new List<string>();
		foreach (BoatVehicleConfig boatVehicleConfig in list)
		{
			list2.Add(boatVehicleConfig.name);
		}
		return list2.ToArray();
	}
}
