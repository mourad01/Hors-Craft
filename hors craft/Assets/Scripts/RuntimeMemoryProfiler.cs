// DecompilerFi decompiler from Assembly-CSharp.dll class: RuntimeMemoryProfiler
using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class RuntimeMemoryProfiler : MonoBehaviour
{
	public class MemorySnapObject
	{
		public int size;

		public UnityEngine.Object unityObject;

		public Type type;

		public string name
		{
			get
			{
				if (unityObject != null)
				{
					return unityObject.name;
				}
				return "unknown";
			}
		}

		public bool isValid => unityObject != null;

		public string ToString()
		{
			if (!isValid)
			{
				return "No valid snap element";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Element named: ");
			stringBuilder.Append(name);
			stringBuilder.Append(" of type: ");
			stringBuilder.Append(type.FullName);
			stringBuilder.Append(" has size: ");
			stringBuilder.Append(GetFormatedSize(size));
			return stringBuilder.ToString();
		}
	}

	public static int GetAllActiveObjectsSize(Type type)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			return -1;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(type);
		int num = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object o in array2)
		{
			num += Profiler.GetRuntimeMemorySize(o);
		}
		return num;
	}

	public static int GetAllObjectsSize(Type type)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			return -1;
		}
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
		int num = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object o in array2)
		{
			num += Profiler.GetRuntimeMemorySize(o);
		}
		return num;
	}

	public static int GetAllActiveObjectsSize(Type type, Func<UnityEngine.Object, bool> filter)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			return -1;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(type);
		int num = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			if (filter(@object))
			{
				num += Profiler.GetRuntimeMemorySize(@object);
			}
		}
		return num;
	}

	public static int GetAllObjectsSize(Type type, Func<UnityEngine.Object, bool> filter)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			return -1;
		}
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
		int num = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			if (filter(@object))
			{
				num += Profiler.GetRuntimeMemorySize(@object);
			}
		}
		return num;
	}

	public static int GetAllActiveObjectsSize(Type type, out string log, bool useFormatedLog = false)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			log = string.Empty;
			return -1;
		}
		StringBuilder stringBuilder = new StringBuilder();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(type);
		stringBuilder.Append("Type: ");
		stringBuilder.Append(type.FullName);
		stringBuilder.Append(" has ");
		stringBuilder.Append(array.Length);
		stringBuilder.Append(" elements");
		stringBuilder.AppendLine();
		int num = 0;
		int num2 = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			num2 = Profiler.GetRuntimeMemorySize(@object);
			stringBuilder.Append("Element: ");
			stringBuilder.Append(@object.name);
			stringBuilder.Append(" ");
			if (useFormatedLog)
			{
				stringBuilder.Append(GetFormatedSize(num2));
			}
			else
			{
				stringBuilder.Append(num2);
				stringBuilder.Append("Bytes");
			}
			stringBuilder.AppendLine();
			num += num2;
		}
		log = stringBuilder.ToString();
		return num;
	}

	public static int GetAllObjectsSize(Type type, out string log, bool useFormatedLog = false)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			log = string.Empty;
			return -1;
		}
		StringBuilder stringBuilder = new StringBuilder();
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
		stringBuilder.Append("Type: ");
		stringBuilder.Append(type.FullName);
		stringBuilder.Append(" has ");
		stringBuilder.Append(array.Length);
		stringBuilder.Append(" elements");
		stringBuilder.AppendLine();
		int num = 0;
		int num2 = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			num2 = Profiler.GetRuntimeMemorySize(@object);
			stringBuilder.Append("Element: ");
			stringBuilder.Append(@object.name);
			stringBuilder.Append(" ");
			if (useFormatedLog)
			{
				stringBuilder.Append(GetFormatedSize(num2));
			}
			else
			{
				stringBuilder.Append(num2);
				stringBuilder.Append("Bytes");
			}
			stringBuilder.AppendLine();
			num += num2;
		}
		log = stringBuilder.ToString();
		return num;
	}

	public static int GetAllActiveObjectsSize(Type type, Func<UnityEngine.Object, bool> filter, out string log, bool useFormatedLog = false)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			log = string.Empty;
			return -1;
		}
		StringBuilder stringBuilder = new StringBuilder();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(type);
		stringBuilder.Append("Type: ");
		stringBuilder.Append(type.FullName);
		stringBuilder.AppendLine();
		int num = 0;
		int num2 = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			if (filter(@object))
			{
				num2 = Profiler.GetRuntimeMemorySize(@object);
				stringBuilder.Append("Element: ");
				stringBuilder.Append(@object.name);
				stringBuilder.Append(" ");
				if (useFormatedLog)
				{
					stringBuilder.Append(GetFormatedSize(num2));
				}
				else
				{
					stringBuilder.Append(num2);
					stringBuilder.Append("Bytes");
				}
				stringBuilder.AppendLine();
				num += num2;
			}
		}
		log = stringBuilder.ToString();
		return num;
	}

	public static int GetAllObjectsSize(Type type, Func<UnityEngine.Object, bool> filter, out string log, bool useFormatedLog = false)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			log = string.Empty;
			return -1;
		}
		StringBuilder stringBuilder = new StringBuilder();
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
		stringBuilder.Append("Type: ");
		stringBuilder.Append(type.FullName);
		stringBuilder.AppendLine();
		int num = 0;
		int num2 = 0;
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			if (filter(@object))
			{
				num2 = Profiler.GetRuntimeMemorySize(@object);
				stringBuilder.Append("Element: ");
				stringBuilder.Append(@object.name);
				stringBuilder.Append(" ");
				if (useFormatedLog)
				{
					stringBuilder.Append(GetFormatedSize(num2));
				}
				else
				{
					stringBuilder.Append(num2);
					stringBuilder.Append("Bytes");
				}
				stringBuilder.AppendLine();
				num += num2;
			}
		}
		log = stringBuilder.ToString();
		return num;
	}

	public static MemorySnapObject[] GetAllActiveObjectsSnap(Type type)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			return null;
		}
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(type);
		MemorySnapObject[] array2 = new MemorySnapObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = new MemorySnapObject
			{
				type = type,
				size = Profiler.GetRuntimeMemorySize(array[i]),
				unityObject = array[i]
			};
		}
		return array2;
	}

	public static MemorySnapObject[] GetAllObjectsSnap(Type type)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			return null;
		}
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
		MemorySnapObject[] array2 = new MemorySnapObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = new MemorySnapObject
			{
				type = type,
				size = Profiler.GetRuntimeMemorySize(array[i]),
				unityObject = array[i]
			};
		}
		return array2;
	}

	public static string GetFormatedSize(int size, bool useShort = true)
	{
		int num = size;
		float num2 = (float)size / 1024f;
		float num3 = num2 / 1024f;
		float num4 = num3 / 1024f;
		if (useShort)
		{
			if (num4 > 0.5f)
			{
				return $"{num4:#,###.###} GB";
			}
			if (num3 > 0.5f)
			{
				return $"{num3:#,###.###} MB";
			}
			if (num2 > 0.5f)
			{
				return $"{num2:#,###.###} KB";
			}
			return $"{num.ToString(CultureInfo.InvariantCulture)} B";
		}
		return $"{num4:#,###.###} GB, {num3:#,###.###} MB, {num2:#,###.###} KB, {num.ToString(CultureInfo.InvariantCulture)} B";
	}

	public static string GetFormatedSize(long size, bool useShort = true)
	{
		long num = size;
		float num2 = (float)size / 1024f;
		float num3 = num2 / 1024f;
		float num4 = num3 / 1024f;
		if (useShort)
		{
			if (num4 > 0.5f)
			{
				return $"{num4:#,###.###} GB";
			}
			if (num3 > 0.5f)
			{
				return $"{num3:#,###.###} MB";
			}
			if (num2 > 0.5f)
			{
				return $"{num2:#,###.###} KB";
			}
			return $"{num.ToString(CultureInfo.InvariantCulture)} B";
		}
		return $"{num4:#,###.###} GB, {num3:#,###.###} MB, {num2:#,###.###} KB, {num.ToString(CultureInfo.InvariantCulture)} B";
	}

	public static string GetFormatedSize(uint size, bool useShort = true)
	{
		uint num = size;
		float num2 = (float)(double)size / 1024f;
		float num3 = num2 / 1024f;
		float num4 = num3 / 1024f;
		if (useShort)
		{
			if (num4 > 0.5f)
			{
				return $"{num4:#,###.###} GB";
			}
			if (num3 > 0.5f)
			{
				return $"{num3:#,###.###} MB";
			}
			if (num2 > 0.5f)
			{
				return $"{num2:#,###.###} KB";
			}
			return $"{num.ToString(CultureInfo.InvariantCulture)} B";
		}
		return $"{num4:#,###.###} GB, {num3:#,###.###} MB, {num2:#,###.###} KB, {num.ToString(CultureInfo.InvariantCulture)} B";
	}

	public static string GetFullLogObject(Type type)
	{
		if (!Debug.isDebugBuild && !Application.isEditor)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(type);
		UnityEngine.Object[] array2 = Resources.FindObjectsOfTypeAll(type);
		stringBuilder.Append("Type: ");
		stringBuilder.Append(type.FullName);
		stringBuilder.Append(" has ");
		stringBuilder.Append(array.Length);
		stringBuilder.Append("/");
		stringBuilder.Append(array2.Length);
		stringBuilder.Append(" elements ");
		int num = 0;
		UnityEngine.Object[] array3 = array;
		foreach (UnityEngine.Object o in array3)
		{
			num += Profiler.GetRuntimeMemorySize(o);
		}
		stringBuilder.Append(GetFormatedSize(num));
		stringBuilder.Append("/");
		num = 0;
		UnityEngine.Object[] array4 = array2;
		foreach (UnityEngine.Object o2 in array4)
		{
			num += Profiler.GetRuntimeMemorySize(o2);
		}
		stringBuilder.Append(GetFormatedSize(num));
		return stringBuilder.ToString();
	}
}
