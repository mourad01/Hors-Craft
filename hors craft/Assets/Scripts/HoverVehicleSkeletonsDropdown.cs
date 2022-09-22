// DecompilerFi decompiler from Assembly-CSharp.dll class: HoverVehicleSkeletonsDropdown
using System.Collections.Generic;
using UnityEngine;

public class HoverVehicleSkeletonsDropdown : PropertyAttribute
{
	public HoverVehicleSkeleton[] Skeletons
	{
		get;
		private set;
	}

	public string[] Names
	{
		get;
		private set;
	}

	public HoverVehicleSkeletonsDropdown()
	{
		Skeletons = AllVehicleSkeletons();
		Names = GetNamesArray(Skeletons);
	}

	public HoverVehicleSkeleton[] AllVehicleSkeletons()
	{
		HoverVehicleSkeletonsList[] allInstances = GetAllInstances<HoverVehicleSkeletonsList>();
		if (allInstances.Length > 0)
		{
			return allInstances[0].skeletons;
		}
		return null;
	}

	public T[] GetAllInstances<T>() where T : ScriptableObject
	{
		return null;
	}

	public string[] GetNamesArray(HoverVehicleSkeleton[] list)
	{
		List<string> list2 = new List<string>();
		foreach (HoverVehicleSkeleton hoverVehicleSkeleton in list)
		{
			list2.Add(hoverVehicleSkeleton.name);
		}
		return list2.ToArray();
	}
}
