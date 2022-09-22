// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.CopyComponents
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Common.Utils
{
	public static class CopyComponents
	{
		public static void CopyProperties<T, Y>(T copyFrom, Y copyTo)
		{
			PropertyInfo[] properties = copyFrom.GetType().GetProperties();
			PropertyInfo[] properties2 = copyTo.GetType().GetProperties();
			PropertyInfo[] array = properties2;
			foreach (PropertyInfo property in array)
			{
				PropertyInfo propertyInfo = properties.FirstOrDefault((PropertyInfo p) => p.Name.Equals(property.Name));
				if (propertyInfo != null)
				{
					try
					{
						property.SetValue(copyTo, propertyInfo.GetValue(copyFrom, null), null);
					}
					catch
					{
					}
				}
			}
		}

		public static void CopyFields<T, Y>(T copyFrom, Y copyTo)
		{
			FieldInfo[] fields = copyFrom.GetType().GetFields();
			FieldInfo[] fields2 = copyTo.GetType().GetFields();
			FieldInfo[] array = fields2;
			foreach (FieldInfo field in array)
			{
				FieldInfo fieldInfo = fields.FirstOrDefault((FieldInfo p) => p.Name.Equals(field.Name));
				if (fieldInfo != null)
				{
					try
					{
						field.SetValue(copyTo, fieldInfo.GetValue(copyFrom));
					}
					catch
					{
					}
				}
			}
		}

		public static void CopyGameObjectComponents(GameObject copyFrom, GameObject copyTo, Func<Component, bool> filter = null, bool copyProperties = true)
		{
			Component[] components = copyFrom.GetComponents<Component>();
			Component[] array = components;
			foreach (Component component in array)
			{
				if (filter == null || filter(component))
				{
					Component component2 = copyTo.GetComponent(component.GetType());
					if (component2 == null)
					{
						component2 = copyTo.AddComponent(component.GetType());
					}
					if (copyProperties)
					{
						CopyProperties(component, component2);
					}
					CopyFields(component, component2);
				}
			}
		}

		public static T GetCopyOf<T>(this Component comp, T other) where T : Component
		{
			Type type = comp.GetType();
			if (type != other.GetType())
			{
				return (T)null;
			}
			BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			PropertyInfo[] properties = type.GetProperties(bindingAttr);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo.CanWrite)
				{
					try
					{
						propertyInfo.SetValue(comp, propertyInfo.GetValue(other, null), null);
					}
					catch
					{
					}
				}
			}
			FieldInfo[] fields = type.GetFields(bindingAttr);
			FieldInfo[] array2 = fields;
			foreach (FieldInfo fieldInfo in array2)
			{
				fieldInfo.SetValue(comp, fieldInfo.GetValue(other));
			}
			return comp as T;
		}

		public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
		{
			return go.AddComponent<T>().GetCopyOf(toAdd);
		}
	}
}
