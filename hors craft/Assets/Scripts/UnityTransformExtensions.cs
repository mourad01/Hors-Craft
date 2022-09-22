// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityTransformExtensions
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class UnityTransformExtensions
{
	public static void SetPositionX(this Transform t, float newX)
	{
		t.position = t.position.SetPositionX(newX);
	}

	public static Vector3 SetPositionX(this Vector3 v3, float newX)
	{
		return new Vector3(newX, v3.y, v3.z);
	}

	public static void SetPositionY(this Transform t, float newY)
	{
		t.position = t.position.SetPositionY(newY);
	}

	public static Vector3 SetPositionY(this Vector3 v3, float newY)
	{
		return new Vector3(v3.x, newY, v3.z);
	}

	public static void SetPositionZ(this Transform t, float newZ)
	{
		t.position = t.position.SetPositionZ(newZ);
	}

	public static Vector3 SetPositionZ(this Vector3 v3, float newZ)
	{
		return new Vector3(v3.x, v3.y, newZ);
	}

	public static float GetPositionX(this Transform t)
	{
		Vector3 position = t.position;
		return position.x;
	}

	public static float GetPositionY(this Transform t)
	{
		Vector3 position = t.position;
		return position.y;
	}

	public static float GetPositionZ(this Transform t)
	{
		Vector3 position = t.position;
		return position.z;
	}

	private static string GetPath(Transform transform)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Transform transform2 = transform;
		while (transform2 != null)
		{
			stringBuilder.Insert(0, transform2.name + '/');
			transform2 = transform2.parent;
		}
		stringBuilder.Remove(stringBuilder.Length - 1, 1);
		return stringBuilder.ToString();
	}

	public static Transform GetChildByPath(this Transform transform, string path, bool clearFromClone = false)
	{
		string[] array = path.Split('/', '\\');
		if (array.Length == 0)
		{
			return transform;
		}
		Transform transform2 = transform;
		for (int i = 0; i < array.Length; i++)
		{
			if (clearFromClone)
			{
				Transform transform3 = transform2.FindTransform(array[i]);
				if (transform3 == null)
				{
					IEnumerator enumerator = transform2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Transform transform4 = (Transform)enumerator.Current;
							if (transform4.name.Contains(array[i]) && transform4.name.Contains("Clone"))
							{
								transform2 = transform4;
								break;
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
				else
				{
					transform2 = transform3;
				}
			}
			else
			{
				transform2 = transform2.Find(array[i]);
			}
			if (transform2 == null)
			{
				return null;
			}
		}
		return transform2;
	}

	public static Transform FindChildRecursively(this Transform t, string name)
	{
		if (t.name == name)
		{
			return t;
		}
		IEnumerator enumerator = t.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform t2 = (Transform)enumerator.Current;
				Transform transform = t2.FindChildRecursively(name);
				if (transform != null)
				{
					return transform;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		return null;
	}

	public static Transform[] FindChildrenRecursively(this Transform t, string name)
	{
		List<Transform> list = new List<Transform>();
		FindChildrenListRecursively(list, t, name);
		return list.ToArray();
	}

	private static void FindChildrenListRecursively(List<Transform> list, Transform t, string name)
	{
		if (t.name == name)
		{
			list.Add(t);
		}
		for (int i = 0; i < t.childCount; i++)
		{
			FindChildrenListRecursively(list, t.GetChild(i), name);
		}
	}

	public static Transform[] FindChildrenContainingRecursively(this Transform t, string name)
	{
		List<Transform> list = new List<Transform>();
		FindChildrenContainingListRecursively(list, t, name);
		return list.ToArray();
	}

	private static void FindChildrenContainingListRecursively(List<Transform> list, Transform t, string name)
	{
		if (t.name.Contains(name))
		{
			list.Add(t);
		}
		for (int i = 0; i < t.childCount; i++)
		{
			FindChildrenContainingListRecursively(list, t.GetChild(i), name);
		}
	}

	public static void SetLayerRecursively(this Transform t, int layer)
	{
		t.gameObject.layer = layer;
		IEnumerator enumerator = t.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform t2 = (Transform)enumerator.Current;
				t2.SetLayerRecursively(layer);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static Transform FindParentRecursively(this Transform t, string name)
	{
		if (t.name == name)
		{
			return t;
		}
		if (t.parent != null)
		{
			return t.parent.FindParentRecursively(name);
		}
		return null;
	}

	public static void DestroyAllChildren(this Transform t)
	{
		IEnumerator enumerator = t.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}
}
