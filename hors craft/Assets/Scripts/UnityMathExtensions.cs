// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityMathExtensions
using UnityEngine;

public static class UnityMathExtensions
{
	public static float DistanceTo(this GameObject go, GameObject otherGO)
	{
		return Vector3.Distance(go.transform.position, otherGO.transform.position);
	}

	public static float DistanceTo(this GameObject go, Vector3 pos)
	{
		return Vector3.Distance(go.transform.position, pos);
	}

	public static float DistanceTo(this Vector3 start, Vector3 dest)
	{
		return Vector3.Distance(start, dest);
	}

	public static float DistanceTo(this Transform start, Transform dest)
	{
		return Vector3.Distance(start.position, dest.position);
	}

	public static Vector3 Add(this Vector3 v3, Vector3 value)
	{
		return v3 + value;
	}

	public static Vector3 Add(this Vector3 v3, float x, float y, float z)
	{
		return v3 + new Vector3(x, y, z);
	}

	public static Vector3 Subtract(this Vector3 v3, Vector3 value)
	{
		return v3 - value;
	}

	public static Vector3 Subtract(this Vector3 v3, float x, float y, float z)
	{
		return v3 - new Vector3(x, y, z);
	}

	public static float Remap(this float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}
