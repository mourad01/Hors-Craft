// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityRotateExtensions
using UnityEngine;

public static class UnityRotateExtensions
{
	public static void Rotate_DegreesPerSecond(this GameObject go, Vector3 direction, float timeInSeconds)
	{
		go.transform.Rotate_DegreesPerSecond(direction, timeInSeconds);
	}

	public static void Rotate_DegreesPerSecond(this Transform goTrans, Vector3 direction, float timeInSeconds)
	{
		goTrans.Rotate(direction * timeInSeconds * Time.deltaTime);
	}

	public static void RotateAroundAxis_X(this GameObject go, float degrees, float timeInSeconds)
	{
		go.transform.RotateAroundAxis_X(degrees, timeInSeconds);
	}

	public static void RotateAroundAxis_X(this Transform goTrans, float degrees, float timeInSeconds)
	{
		goTrans.Rotate_DegreesPerSecond(new Vector3(degrees, 0f, 0f), timeInSeconds);
	}

	public static void RotateAroundAxis_Y(this GameObject go, float degrees, float timeInSeconds)
	{
		go.transform.RotateAroundAxis_Y(degrees, timeInSeconds);
	}

	public static void RotateAroundAxis_Y(this Transform goTrans, float degrees, float timeInSeconds)
	{
		goTrans.Rotate_DegreesPerSecond(new Vector3(0f, degrees, 0f), timeInSeconds);
	}

	public static void RotateAroundAxis_Z(this GameObject go, float degrees, float timeInSeconds)
	{
		go.transform.RotateAroundAxis_Z(degrees, timeInSeconds);
	}

	public static void RotateAroundAxis_Z(this Transform goTrans, float degrees, float timeInSeconds)
	{
		goTrans.Rotate_DegreesPerSecond(new Vector3(0f, 0f, degrees), timeInSeconds);
	}

	public static void LookAt(this GameObject go, GameObject targetGo)
	{
		go.LookAt(targetGo.transform);
	}

	public static void LookAt(this GameObject go, Transform targetTrans)
	{
		go.transform.LookAt(targetTrans);
	}

	public static void LookAt(this GameObject go, Vector3 targetVector)
	{
		go.transform.LookAt(targetVector);
	}
}
