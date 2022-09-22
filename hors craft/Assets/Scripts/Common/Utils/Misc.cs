// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.Misc
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Utils
{
	public static class Misc
	{
		public static void ContextIndependendDestroy(UnityEngine.Object ob)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(ob);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(ob);
			}
		}

		public static double GetTimeStampDouble()
		{
			return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static DateTime UnixTimeStampToDateTimeDouble(double timestamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
		}

		public static int GetTimeStamp()
		{
			return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static DateTime UnixTimeStampToDateTime(int timestamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
		}

		public static T ConvertDictionaryTo<T>(IDictionary<string, object> dictionary) where T : new()
		{
			Type typeFromHandle = typeof(T);
			T val = new T();
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				typeFromHandle.GetProperty(item.Key).SetValue(val, item.Value, null);
			}
			return val;
		}

		public static Texture2D LoadPNG(string filePath)
		{
			Texture2D texture2D = null;
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(data);
			}
			else
			{
				UnityEngine.Debug.LogWarning("File don't exist : " + filePath);
			}
			return texture2D;
		}

		public static T Random<T>(this IEnumerable<T> array)
		{
			if (array == null || array.Count() == 0)
			{
				UnityEngine.Debug.LogWarning("Array is null or empty");
				return default(T);
			}
			return array.ElementAt(UnityEngine.Random.Range(0, array.Count()));
		}

		public static T IncrementEnum<T>(T en) where T : struct, IConvertible
		{
			int length = Enum.GetValues(typeof(T)).Length;
			int num = Convert.ToInt32(en);
			int num2 = (num + 1) % length;
			return (T)(object)num2;
		}

		public static T ToEnum<T>(this float value) where T : struct, IConvertible
		{
			value = Mathf.Clamp(value, 0f, 1f);
			int length = Enum.GetValues(typeof(T)).Length;
			int a = (int)(value * (float)length);
			a = Mathf.Min(a, length - 1);
			return (T)(object)a;
		}

		public static string CustomNameToKey(string name)
		{
			string input = name.ToLower();
			return Regex.Replace(input, "\\s+", ".");
		}

		public static Color HexToColor(string hex)
		{
			if (hex.Length < 6)
			{
				return Color.magenta;
			}
			hex = hex.Replace("#", string.Empty);
			float r = (float)(int)byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) / 255f;
			float g = (float)(int)byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber) / 255f;
			float b = (float)(int)byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber) / 255f;
			float a = 1f;
			if (hex.Length == 8)
			{
				a = (float)(int)byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber) / 255f;
			}
			return new Color(r, g, b, a);
		}

		public static List<T> GetObjectsOfType<T>(this Scene scene) where T : MonoBehaviour
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			return GetObjectsOfType<T>(rootGameObjects);
		}

		private static List<T> GetObjectsOfType<T>(GameObject[] rootObjects) where T : MonoBehaviour
		{
			List<T> list = new List<T>();
			foreach (GameObject gameObject in rootObjects)
			{
				list.AddRange(gameObject.GetComponentsInChildren<T>());
			}
			return list;
		}

		public static T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			Type type = original.GetType();
			T val = destination.GetComponent(type) as T;
			if (!(UnityEngine.Object)val)
			{
				val = (destination.AddComponent(type) as T);
			}
			FieldInfo[] fields = type.GetFields();
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (!fieldInfo.IsStatic)
				{
					fieldInfo.SetValue(val, fieldInfo.GetValue(original));
				}
			}
			PropertyInfo[] properties = type.GetProperties();
			PropertyInfo[] array2 = properties;
			foreach (PropertyInfo propertyInfo in array2)
			{
				if (propertyInfo.CanWrite && propertyInfo.CanWrite && !(propertyInfo.Name == "name"))
				{
					propertyInfo.SetValue(val, propertyInfo.GetValue(original, null), null);
				}
			}
			return val;
		}
	}
}
