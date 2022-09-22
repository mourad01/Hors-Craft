// DecompilerFi decompiler from Assembly-CSharp.dll class: FloatArrayExtensions
using UnityEngine;

public static class FloatArrayExtensions
{
	public static bool DeltaCompare(this float value1, float value2, float delta)
	{
		return Mathf.Abs(value1 - value2) < delta;
	}

	public static float[,] MultiplyScalar(this float[,] array, float scalar)
	{
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(0); j++)
			{
				array[i, j] *= scalar;
			}
		}
		return array;
	}
}
