// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityExtensions
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class UnityExtensions
{
	public static string ToV3String(this Vector3 v3)
	{
		return $"{v3.x}, {v3.y}, {v3.z}";
	}

	public static Vector3 ZeroY(this Vector3 v3)
	{
		return new Vector3(v3.x, 0f, v3.z);
	}

	public static Vector3 RotateAroundY(this Vector3 v3, Vector3 goV3)
	{
		return new Vector3(v3.x, goV3.y, v3.z);
	}

	public static string ToStringParentHierarchy(this GameObject go)
	{
		if (go == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		if (go.transform.parent != null)
		{
			text = go.transform.parent.gameObject.ToStringParentHierarchy();
		}
		return $"{(string.IsNullOrEmpty(text) ? string.Empty : $"{text} > ")}{go.name}";
	}

	public static T LoadFromResources<T>(this string resourcePath, bool errorOnNull) where T : UnityEngine.Object
	{
		T val = Resources.Load<T>(resourcePath);
		if ((UnityEngine.Object)val == (UnityEngine.Object)null)
		{
			UnityEngine.Debug.LogError($"Unable to load resource: {resourcePath}");
		}
		return val;
	}

	public static byte[] UnityStringToBytes(this string source)
	{
		if (string.IsNullOrEmpty(source))
		{
			return null;
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
			{
				streamWriter.Write(source);
				streamWriter.Close();
				return memoryStream.ToArray();
			}
		}
	}

	public static string UnityBytesToString(this byte[] source)
	{
		if (source.IsNullOrEmpty())
		{
			return string.Empty;
		}
		using (MemoryStream stream = new MemoryStream(source))
		{
			using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
			{
				return streamReader.ReadToEnd();
			}
		}
	}

	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static List<OutputType> UnityConvertAll<InputType, OutputType>(this List<InputType> inputList, Func<InputType, OutputType> converter)
	{
		int count = inputList.Count;
		List<OutputType> list = new List<OutputType>(count);
		for (int i = 0; i < count; i++)
		{
			list.Add(converter(inputList[i]));
		}
		return list;
	}

	public static void EnableComponentIfExist<T>(this GameObject gameObj, bool enabled) where T : MonoBehaviour
	{
		T component = gameObj.GetComponent<T>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.enabled = enabled;
		}
	}

	public static void EnableComponentInChildrenIfExist<T>(this GameObject gameObj, bool enabled) where T : MonoBehaviour
	{
		T componentInChildren = gameObj.GetComponentInChildren<T>();
		if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
		{
			componentInChildren.enabled = enabled;
		}
	}

	public static void EnableColliders(this GameObject gameObj, bool enabled)
	{
		Collider[] componentsInChildren = gameObj.GetComponentsInChildren<Collider>();
		Array.ForEach(componentsInChildren, delegate(Collider component)
		{
			component.enabled = enabled;
		});
	}

	public static void EnableRenderers(this GameObject gameObj, bool enabled)
	{
		Renderer[] componentsInChildren = gameObj.GetComponentsInChildren<Renderer>();
		Array.ForEach(componentsInChildren, delegate(Renderer renderer)
		{
			renderer.enabled = enabled;
		});
	}
}
