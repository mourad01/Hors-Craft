// DecompilerFi decompiler from Assembly-CSharp.dll class: LayerMaskExtension
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtension
{
	public static LayerMask Create(params string[] layerNames)
	{
		return NamesToMask(layerNames);
	}

	public static LayerMask Create(params int[] layerNumbers)
	{
		return LayerNumbersToMask(layerNumbers);
	}

	public static LayerMask NamesToMask(params string[] layerNames)
	{
		LayerMask layerMask = 0;
		foreach (string layerName in layerNames)
		{
			layerMask = ((int)layerMask | (1 << LayerMask.NameToLayer(layerName)));
		}
		return layerMask;
	}

	public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
	{
		LayerMask layerMask = 0;
		foreach (int num in layerNumbers)
		{
			layerMask = ((int)layerMask | (1 << num));
		}
		return layerMask;
	}

	public static LayerMask Inverse(this LayerMask original)
	{
		return ~(int)original;
	}

	public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
	{
		return (int)original | (int)NamesToMask(layerNames);
	}

	public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
	{
		LayerMask mask = ~(int)original;
		return ~((int)mask | (int)NamesToMask(layerNames));
	}

	public static string[] MaskToNames(this LayerMask original)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < 32; i++)
		{
			int num = 1 << i;
			if (((int)original & num) == num)
			{
				string text = LayerMask.LayerToName(i);
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
		}
		return list.ToArray();
	}

	public static string MaskToString(this LayerMask original)
	{
		return original.MaskToString(", ");
	}

	public static string MaskToString(this LayerMask original, string delimiter)
	{
		return string.Join(delimiter, original.MaskToNames());
	}

	public static bool IsInLayerMask(this GameObject obj, LayerMask mask)
	{
		return (mask.value & (1 << obj.layer)) > 0;
	}
}
