// DecompilerFi decompiler from Assembly-CSharp.dll class: TargetingExtensions
using System.Linq;
using UnityEngine;

public static class TargetingExtensions
{
	private const float FLOAT_MaxSearchRange = 1E+12f;

	public static GameObject FindNearestUnit(this GameObject go, LayerMask maskToSearch)
	{
		Collider[] array = Physics.OverlapSphere(go.transform.position, 1E+12f, maskToSearch);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		return go.FindNearestUnit_List(array.ToList().UnityConvertAll((Collider x) => x.gameObject).ToArray());
	}

	public static GameObject FindNearestUnit_View(this GameObject go, float frustumAngle, LayerMask maskToSearch)
	{
		Collider[] array = Physics.OverlapSphere(go.transform.position, 1E+12f, maskToSearch);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		return go.FindNearestUnit_ListView(frustumAngle, array.ToList().UnityConvertAll((Collider x) => x.gameObject).ToArray());
	}

	public static GameObject FindNearestUnit_Range(this GameObject go, float targetRange, LayerMask maskToSearch)
	{
		Collider[] array = Physics.OverlapSphere(go.transform.position, targetRange, maskToSearch);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		return go.FindNearestUnit_ListRange(targetRange, array.ToList().UnityConvertAll((Collider x) => x.gameObject).ToArray());
	}

	public static GameObject FindNearestUnit_RangeView(this GameObject go, float frustumAngle, float targetRange, LayerMask maskToSearch)
	{
		Collider[] array = Physics.OverlapSphere(go.transform.position, targetRange, maskToSearch);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		return go.FindNearestUnit_ListRangeView(targetRange, frustumAngle, array.ToList().UnityConvertAll((Collider x) => x.gameObject).ToArray());
	}

	public static GameObject FindNearestUnit_ListRange(this GameObject go, float targetRange, GameObject[] goList)
	{
		Vector3 position = go.transform.position;
		float num = 1E+10f;
		int num2 = -1;
		int num3 = goList.Length;
		for (int i = 0; i < num3; i++)
		{
			float num4 = Vector3.Distance(position, goList[i].transform.position);
			if (!(num4 > targetRange) && num4 < num)
			{
				num = num4;
				num2 = i;
			}
		}
		return (num2 <= -1) ? null : goList[num2];
	}

	public static GameObject FindNearestUnit_List(this GameObject go, GameObject[] goList)
	{
		Vector3 position = go.transform.position;
		float num = 1E+10f;
		int num2 = -1;
		int num3 = goList.Length;
		for (int i = 0; i < num3; i++)
		{
			float num4 = Vector3.Distance(position, goList[i].transform.position);
			if (num4 < num)
			{
				num = num4;
				num2 = i;
			}
		}
		return (num2 <= -1) ? null : goList[num2];
	}

	public static GameObject FindNearestUnit_ListView(this GameObject go, float frustumAngle, GameObject[] goList)
	{
		Vector3 position = go.transform.position;
		Vector3 forward = go.transform.forward;
		float num = 1E+10f;
		int num2 = -1;
		int num3 = goList.Length;
		for (int i = 0; i < num3; i++)
		{
			if (!(Vector3.Angle(goList[i].transform.position - position, forward) > frustumAngle))
			{
				float num4 = Vector3.Distance(position, goList[i].transform.position);
				if (num4 < num)
				{
					num = num4;
					num2 = i;
				}
			}
		}
		return (num2 <= -1) ? null : goList[num2];
	}

	public static GameObject FindNearestUnit_ListRangeView(this GameObject go, float targetRange, float frustumAngle, GameObject[] goList)
	{
		Vector3 position = go.transform.position;
		Vector3 forward = go.transform.forward;
		float num = 1E+10f;
		int num2 = -1;
		int num3 = goList.Length;
		for (int i = 0; i < num3; i++)
		{
			float num4 = Vector3.Distance(position, goList[i].transform.position);
			if (!(num4 > targetRange) && !(Vector3.Angle(goList[i].transform.position - position, forward) > frustumAngle) && num4 < num)
			{
				num = num4;
				num2 = i;
			}
		}
		return (num2 <= -1) ? null : goList[num2];
	}
}
