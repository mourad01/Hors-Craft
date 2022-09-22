// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.JSONHelper
using Common.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Common.Utils
{
	public static class JSONHelper
	{
		[Serializable]
		private class Wrapper<T>
		{
			public T[] Items;
		}

		public static T Deserialize<T>(string d)
		{
			return Deserialize<T>(Json.Deserialize(d));
		}

		public static T Deserialize<T>(object d)
		{
			return (T)Deserialize(typeof(T), d);
		}

		public static object Deserialize(Type t, object d)
		{
			if (t == typeof(object))
			{
				return d;
			}
			if (t == typeof(double))
			{
				if (d is double || d is long)
				{
					return d;
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(float))
			{
				if (d is double || d is long)
				{
					return Convert.ToSingle(d);
				}
				if (d is string)
				{
					return Convert.ToSingle((string)d);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(byte))
			{
				if (d is long)
				{
					return Convert.ToByte((long)d);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(sbyte))
			{
				if (d is long)
				{
					return Convert.ToSByte((long)d);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(short))
			{
				if (d is long)
				{
					return Convert.ToInt16((long)d);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(ushort))
			{
				if (d is long)
				{
					return Convert.ToUInt16((long)d);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(int))
			{
				if (d is long)
				{
					return Convert.ToInt32((long)d);
				}
				int result = 0;
				if (int.TryParse(d.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(uint))
			{
				if (d is long)
				{
					return Convert.ToUInt32((long)d);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(long))
			{
				if (d is long)
				{
					return (long)d;
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(ulong))
			{
				if (d is long)
				{
					return Convert.ToUInt64((long)d);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(string))
			{
				if (d is string)
				{
					return d;
				}
				if (d != null)
				{
					return d.ToString();
				}
				return string.Empty;
			}
			if (t == typeof(bool))
			{
				if (d is bool)
				{
					return d;
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (d == null)
			{
				if (t == typeof(bool))
				{
					return false;
				}
				return null;
			}
			if (t == typeof(Array))
			{
				if (d is IList)
				{
					IList list = (IList)d;
					Type elementType = t.GetElementType();
					Array array = Array.CreateInstance(elementType, list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						object value = Deserialize(elementType, list[i]);
						array.SetValue(value, i);
					}
					return array;
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(ArrayList))
			{
				if (d is IList)
				{
					IList list2 = (IList)d;
					ArrayList arrayList = new ArrayList();
					for (int j = 0; j < list2.Count; j++)
					{
						arrayList.Add(Deserialize(typeof(object), list2[j]));
					}
					return arrayList;
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
			{
				if (d is IList)
				{
					IList list3 = (IList)d;
					Type t2 = t.GetGenericArguments()[0];
					IList list4 = (IList)Activator.CreateInstance(t);
					for (int k = 0; k < list3.Count; k++)
					{
						object value2 = Deserialize(t2, list3[k]);
						list4.Add(value2);
					}
					return list4;
				}
				if (d is IDictionary && (d as IDictionary).Count == 0)
				{
					return (IList)Activator.CreateInstance(t);
				}
				throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t);
			}
			if (t == typeof(Hashtable))
			{
				Hashtable hashtable = new Hashtable();
				IList<PropertyInfo> list5 = new List<PropertyInfo>(t.GetProperties());
				{
					foreach (PropertyInfo item in list5)
					{
						object value3 = item.GetValue(d, null);
						hashtable.Add(value3.ToString(), value3);
					}
					return hashtable;
				}
			}
			if (d is IDictionary)
			{
				IDictionary dictionary = (IDictionary)d;
				if (!t.IsGenericType)
				{
					object obj = Activator.CreateInstance(t);
					FieldInfo[] fields = t.GetFields();
					for (int l = 0; l < fields.Length; l++)
					{
						if (dictionary.Contains(fields[l].Name))
						{
							object value4 = Deserialize(fields[l].FieldType, dictionary[fields[l].Name]);
							fields[l].SetValue(obj, value4);
						}
						else if (Attribute.GetCustomAttribute(fields[l], typeof(Required)) != null)
						{
							throw new DeserializationConstraintException("Class " + t + " required field " + fields[l].Name + " cannot be defined.");
						}
					}
					return obj;
				}
				Type genericTypeDefinition = t.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(Dictionary<, >))
				{
					Type left = t.GetGenericArguments()[0];
					if (left == typeof(string))
					{
						Type t3 = t.GetGenericArguments()[1];
						IDictionary dictionary2 = (IDictionary)Activator.CreateInstance(t);
						IEnumerator enumerator2 = dictionary.Keys.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								string key = (string)enumerator2.Current;
								dictionary2.Add(key, Deserialize(t3, dictionary[key]));
							}
							return dictionary2;
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = (enumerator2 as IDisposable)) != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
			else if (d is IList && (d as IList).Count == 0 && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				return (IDictionary)Activator.CreateInstance(t);
			}
			throw new DeserializationException("Cannot convert " + d.GetType() + " to " + t + " " + d);
		}

		public static string ToJSON(string fieldName, object ob, int intendation = 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Intend(stringBuilder, intendation);
			stringBuilder.Append("\"");
			stringBuilder.Append(fieldName);
			stringBuilder.Append("\"");
			stringBuilder.Append(" : ");
			ToJSON(ob, stringBuilder, intendation, intend: false);
			return stringBuilder.ToString();
		}

		public static string ToJSON(object ob, int intendation = 0, bool intend = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (intend)
			{
				Intend(stringBuilder, intendation);
			}
			if (ob == null)
			{
				return "null";
			}
			if (ob is string)
			{
				string text = ob.ToString();
				text = text.Replace("\\\"", "\\\\\"").Replace("\"", "\\\"");
				return stringBuilder.Append("\"").Append(text).Append("\"")
					.ToString();
			}
			if (ob is byte || ob is sbyte || ob is int || ob is uint || ob is short || ob is ushort || ob is long || ob is ulong || ob is float || ob is double || ob is char || ob is decimal)
			{
				return stringBuilder.Append(ob.ToString()).ToString();
			}
			if (ob is bool)
			{
				return stringBuilder.Append(ob.ToString().ToLower()).ToString();
			}
			if (ob is IDictionary)
			{
				stringBuilder.Append("{\n");
				IDictionary dictionary = ob as IDictionary;
				int num = 0;
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						ToJSON(dictionaryEntry.Key.ToString(), dictionaryEntry.Value, stringBuilder, intendation + 1);
						if (num++ < dictionary.Count - 1)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append("\n");
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
				Intend(stringBuilder, intendation);
				return stringBuilder.Append("}").ToString();
			}
			if (ob is IEnumerable)
			{
				stringBuilder.Append("[");
				IEnumerable enumerable = ob as IEnumerable;
				bool flag = true;
				IEnumerator enumerator2 = enumerable.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object current = enumerator2.Current;
						if (!flag)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append("\n");
						ToJSON(current, stringBuilder, intendation + 1);
						flag = false;
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = (enumerator2 as IDisposable)) != null)
					{
						disposable2.Dispose();
					}
				}
				stringBuilder.Append('\n');
				Intend(stringBuilder, intendation);
				return stringBuilder.Append("]").ToString();
			}
			FieldInfo[] fields = ob.GetType().GetFields();
			if (fields != null && fields.Length > 0)
			{
				stringBuilder.Append("{");
				bool flag2 = true;
				FieldInfo[] array = fields;
				foreach (FieldInfo fieldInfo in array)
				{
					if (!fieldInfo.IsLiteral)
					{
						if (!flag2)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append("\n");
						object value = fieldInfo.GetValue(ob);
						ToJSON(fieldInfo.Name, value, stringBuilder, intendation + 1);
						flag2 = false;
					}
				}
				stringBuilder.Append('\n');
				Intend(stringBuilder, intendation);
				return stringBuilder.Append("}").ToString();
			}
			return stringBuilder.Append(ob.ToString()).ToString();
		}

		private static void ToJSON(string fieldName, object ob, StringBuilder result, int intendation = 0)
		{
			Intend(result, intendation);
			result.Append("\"");
			result.Append(fieldName);
			result.Append("\"");
			result.Append(" : ");
			ToJSON(ob, result, intendation, intend: false);
		}

		private static void ToJSON(object ob, StringBuilder result, int intendation = 0, bool intend = true)
		{
			if (intend)
			{
				Intend(result, intendation);
			}
			if (ob == null)
			{
				result.Append("null");
				return;
			}
			if (ob is string)
			{
				string text = ob.ToString();
				text = text.Replace("\\\"", "\\\\\"").Replace("\"", "\\\"");
				result.Append("\"").Append(text).Append("\"");
				return;
			}
			if (ob is byte || ob is sbyte || ob is int || ob is uint || ob is short || ob is ushort || ob is long || ob is ulong || ob is float || ob is double || ob is char || ob is decimal)
			{
				result.Append(ob.ToString());
				return;
			}
			if (ob is bool)
			{
				result.Append(ob.ToString().ToLower());
				return;
			}
			if (ob is IDictionary)
			{
				result.Append("{\n");
				IDictionary dictionary = ob as IDictionary;
				int num = 0;
				IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
						ToJSON(dictionaryEntry.Key.ToString(), dictionaryEntry.Value, result, intendation + 1);
						if (num++ < dictionary.Count - 1)
						{
							result.Append(",");
						}
						result.Append("\n");
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
				Intend(result, intendation);
				result.Append("}");
				return;
			}
			if (ob is IEnumerable)
			{
				result.Append("[");
				IEnumerable enumerable = ob as IEnumerable;
				bool flag = true;
				IEnumerator enumerator2 = enumerable.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object current = enumerator2.Current;
						if (!flag)
						{
							result.Append(",");
						}
						result.Append("\n");
						ToJSON(current, result, intendation + 1);
						flag = false;
					}
				}
				finally
				{
					IDisposable disposable2;
					if ((disposable2 = (enumerator2 as IDisposable)) != null)
					{
						disposable2.Dispose();
					}
				}
				result.Append('\n');
				Intend(result, intendation);
				result.Append("]");
				return;
			}
			FieldInfo[] fields = ob.GetType().GetFields();
			if (fields != null && fields.Length > 0)
			{
				result.Append("{");
				bool flag2 = true;
				FieldInfo[] array = fields;
				foreach (FieldInfo fieldInfo in array)
				{
					if (!fieldInfo.IsLiteral)
					{
						if (!flag2)
						{
							result.Append(",");
						}
						result.Append("\n");
						object value = fieldInfo.GetValue(ob);
						ToJSON(fieldInfo.Name, value, result, intendation + 1);
						flag2 = false;
					}
				}
				result.Append('\n');
				Intend(result, intendation);
				result.Append("}");
			}
			else
			{
				result.Append(ob.ToString());
			}
		}

		private static void Intend(StringBuilder sb, int intendation)
		{
			for (int i = 0; i < intendation; i++)
			{
				sb.Append('\t');
			}
		}

		public static T[] FromJson<T>(string json)
		{
			Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
			return wrapper.Items;
		}

		public static string ToJson<T>(T[] array)
		{
			Wrapper<T> wrapper = new Wrapper<T>();
			wrapper.Items = array;
			return JsonUtility.ToJson(wrapper);
		}

		public static string ToJson<T>(T[] array, bool prettyPrint)
		{
			Wrapper<T> wrapper = new Wrapper<T>();
			wrapper.Items = array;
			return JsonUtility.ToJson(wrapper, prettyPrint);
		}
	}
}
