// DecompilerFi decompiler from Assembly-CSharp.dll class: RectExtensions
using System.Collections.Generic;
using UnityEngine;

public static class RectExtensions
{
	public static List<Vector2> GetEdgesCenter(this Rect rect)
	{
		List<Vector2> list = new List<Vector2>();
		list.Add(new Vector2((rect.xMax + rect.xMin) * 0.5f, rect.yMin));
		list.Add(new Vector2(rect.xMax, (rect.yMax + rect.yMin) * 0.5f));
		list.Add(new Vector2(rect.xMin, (rect.yMax + rect.yMin) * 0.5f));
		list.Add(new Vector2((rect.xMax + rect.xMin) * 0.5f, rect.yMax));
		return list;
	}

	public static List<Vector2> GetCorners(this Rect rect)
	{
		List<Vector2> list = new List<Vector2>();
		list.Add(new Vector2(rect.xMin, rect.yMin));
		list.Add(new Vector2(rect.xMax, rect.yMin));
		list.Add(new Vector2(rect.xMin, rect.yMax));
		list.Add(new Vector2(rect.xMax, rect.yMax));
		return list;
	}

	public static Rect GetOverlappingRect(this Rect r1, Rect r2)
	{
		float num = r1.x;
		float num2 = r1.y;
		float x = r2.x;
		float y = r2.y;
		float num3 = num;
		num3 += r1.width;
		float num4 = num2;
		num4 += r1.height;
		float num5 = x;
		num5 += r2.width;
		float num6 = y;
		num6 += r2.height;
		if (num < x)
		{
			num = x;
		}
		if (num2 < y)
		{
			num2 = y;
		}
		if (num3 > num5)
		{
			num3 = num5;
		}
		if (num4 > num6)
		{
			num4 = num6;
		}
		num3 -= num;
		num4 -= num2;
		if (num3 < float.MinValue)
		{
			num3 = float.MinValue;
		}
		if (num4 < float.MinValue)
		{
			num4 = float.MinValue;
		}
		return new Rect(num, num2, num3, num4);
	}
}
