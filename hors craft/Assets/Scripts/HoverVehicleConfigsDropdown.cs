// DecompilerFi decompiler from Assembly-CSharp.dll class: HoverVehicleConfigsDropdown
using System.Collections.Generic;
using UnityEngine;

public class HoverVehicleConfigsDropdown : PropertyAttribute
{
	public HoverVehicleConfig[] Configs
	{
		get;
		private set;
	}

	public string[] Names
	{
		get;
		private set;
	}

	public HoverVehicleConfigsDropdown()
	{
		Configs = AllVehicleConfigs();
		Names = GetNamesArray(Configs);
	}

	public HoverVehicleConfig[] AllVehicleConfigs()
	{
		return GetAllInstances<HoverVehicleConfig>();
	}

	public T[] GetAllInstances<T>() where T : ScriptableObject
	{
		return null;
	}

	public string[] GetNamesArray(HoverVehicleConfig[] list)
	{
		List<string> list2 = new List<string>();
		foreach (HoverVehicleConfig hoverVehicleConfig in list)
		{
			list2.Add(hoverVehicleConfig.name);
		}
		return list2.ToArray();
	}
}
