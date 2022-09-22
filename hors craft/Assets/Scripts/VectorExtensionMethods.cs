// DecompilerFi decompiler from Assembly-CSharp.dll class: VectorExtensionMethods
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensionMethods
{
	public class Vector3EqualityComparer : IEqualityComparer<Vector3>
	{
		public bool Equals(Vector3 from, Vector3 to)
		{
			return from.x == to.x && from.y == to.y && from.z == to.z;
		}

		public int GetHashCode(Vector3 obj)
		{
			return obj.GetHashCode();
		}
	}

	public static Vector3EqualityComparer Vector3EqualityComparerInstance = new Vector3EqualityComparer();

	public static bool Equals(this Vector2 left, Vector2 right, byte useNoAlloc, byte useNoAlloc2)
	{
		return left.x == right.x && left.y == right.y;
	}

	public static Vector2 XY(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector2 XMinusY(this Vector3 v)
	{
		return new Vector2(v.x, 0f - v.y);
	}

	public static Vector2 XZ(this Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	public static Vector2 MinusXZ(this Vector3 v)
	{
		return new Vector2(0f - v.x, v.z);
	}

	public static Vector2 ZY(this Vector3 v)
	{
		return new Vector2(v.z, v.y);
	}

	public static Vector2 ZX(this Vector3 v)
	{
		return new Vector2(v.z, v.x);
	}

	public static Vector2 YZ(this Vector3 v)
	{
		return new Vector2(v.y, v.z);
	}

	public static Vector2 YX(this Vector3 v)
	{
		return new Vector2(v.y, v.x);
	}

	public static Vector2 MinusZY(this Vector3 v)
	{
		return new Vector2(0f - v.z, v.y);
	}

	public static float Dot(this Vector2 v, Vector2 u)
	{
		return v.x * u.x + v.y * u.y;
	}

	public static Vector3 Abs(this Vector3 v3)
	{
		return new Vector3(Mathf.Abs(v3.x), Mathf.Abs(v3.y), Mathf.Abs(v3.z));
	}

	public static Vector3 MulY(this Vector3 v, float yMul)
	{
		return new Vector3(v.x, v.y * yMul, v.z);
	}

	public static Vector3 WithX(this Vector3 v, float x)
	{
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y)
	{
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector2 WithX(this Vector2 v, float x)
	{
		return new Vector2(x, v.y);
	}

	public static Vector2 WithY(this Vector2 v, float y)
	{
		return new Vector2(v.x, y);
	}

	public static Vector3 WithZ(this Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector3 Rotate180(this Vector3 v)
	{
		return new Vector3(0f - v.x, v.y, 0f - v.z);
	}

	public static Vector3 Rotate90left(this Vector3 v)
	{
		return new Vector3(0f - v.z, v.y, v.x);
	}

	public static Vector3 Rotate90right(this Vector3 v)
	{
		return new Vector3(v.z, v.y, 0f - v.x);
	}

	public static bool IsAnyBigger(this Vector3 v, float value)
	{
		return v.x > value || v.y > value || v.z > value;
	}

	public static float BiggestAxis(this Vector3 v)
	{
		return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
	}

	public static float SmallestAxis(this Vector3 v)
	{
		return Mathf.Min(v.x, Mathf.Min(v.y, v.z));
	}

	public static float FarestAxis(this Vector3 v)
	{
		if (Mathf.Abs(v.x) >= Mathf.Abs(v.y))
		{
			if (Mathf.Abs(v.x) >= Mathf.Abs(v.z))
			{
				return v.x;
			}
			return v.z;
		}
		if (Mathf.Abs(v.y) >= Mathf.Abs(v.z))
		{
			return v.y;
		}
		return v.z;
	}

	public static float FarestAxisAbs(this Vector3 v)
	{
		if (Mathf.Abs(v.x) >= Mathf.Abs(v.y))
		{
			if (Mathf.Abs(v.x) >= Mathf.Abs(v.z))
			{
				return Mathf.Abs(v.x);
			}
			return Mathf.Abs(v.z);
		}
		if (Mathf.Abs(v.y) >= Mathf.Abs(v.z))
		{
			return Mathf.Abs(v.y);
		}
		return Mathf.Abs(v.z);
	}

	public static float CalculateAngle(Vector3 from, Vector3 to)
	{
		Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles;
		return eulerAngles.z;
	}

	public static Vector2 Clamp(this Vector2 v, float clampX, float clampY)
	{
		return new Vector2(Mathf.Min(v.x, clampX), Mathf.Min(v.y, clampY));
	}

	public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
	{
		if (!isNormalized)
		{
			axisDirection.Normalize();
		}
		float d = Vector3.Dot(point, axisDirection);
		return axisDirection * d;
	}

	public static Vector3 NearestPointOnLine(this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
	{
		if (!isNormalized)
		{
			lineDirection.Normalize();
		}
		float d = Vector3.Dot(point - pointOnLine, lineDirection);
		return pointOnLine + lineDirection * d;
	}

	public static Vector3 MulEveryAxis(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector3 DiffEveryAxis(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
	}

	public static Vector2 FromXZ(Vector3 a)
	{
		return new Vector2(a.x, a.z);
	}

	public static bool IsValid(this Vector2 a)
	{
		return a.x.IsValid() && a.y.IsValid();
	}

	public static bool IsValid(this Vector3 a)
	{
		return a.x.IsValid() && a.y.IsValid() && a.z.IsValid();
	}

	public static Vector2 Valid(this Vector2 a)
	{
		return (!a.IsValid()) ? Vector2.zero : a;
	}

	public static Vector3 Valid(this Vector3 a)
	{
		return (!a.IsValid()) ? Vector3.zero : a;
	}

	public static bool IsValid(this float a)
	{
		return !float.IsNaN(a) && !float.IsInfinity(a);
	}
}
