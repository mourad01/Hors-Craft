// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Helpers.ReflectionHelper
using System;
using System.Reflection;
using UnityEngine;

namespace com.ootii.Helpers
{
	public class ReflectionHelper
	{
		public static bool IsSubclassOf(Type rType, Type rBaseType)
		{
			if (rType == rBaseType || rType.IsSubclassOf(rBaseType))
			{
				return true;
			}
			return false;
		}

		public static bool IsAssignableFrom(Type rType, Type rDerivedType)
		{
			if (rType == rDerivedType || rType.IsAssignableFrom(rDerivedType))
			{
				return true;
			}
			return false;
		}

		public static T GetAttribute<T>(Type rObjectType)
		{
			object[] customAttributes = rObjectType.GetCustomAttributes(typeof(T), inherit: true);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				return default(T);
			}
			return (T)customAttributes[0];
		}

		public static bool IsDefined(Type rObjectType, Type rType)
		{
			object[] customAttributes = rObjectType.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsDefined(FieldInfo rFieldInfo, Type rType)
		{
			object[] customAttributes = rFieldInfo.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsDefined(MemberInfo rMemberInfo, Type rType)
		{
			object[] customAttributes = rMemberInfo.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsDefined(PropertyInfo rPropertyInfo, Type rType)
		{
			object[] customAttributes = rPropertyInfo.GetCustomAttributes(rType, inherit: true);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				return true;
			}
			return false;
		}

		public static void SetProperty(object rObject, string rName, object rValue)
		{
			Type type = rObject.GetType();
			PropertyInfo[] properties = type.GetProperties();
			if (properties == null || properties.Length <= 0)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < properties.Length)
				{
					if (properties[num].Name == rName && properties[num].CanWrite)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			properties[num].SetValue(rObject, rValue, null);
		}

		public static bool IsTypeValid(string rType)
		{
			try
			{
				Type type = Type.GetType(rType);
				return type != null;
			}
			catch
			{
				return false;
			}
		}

		public static bool IsPrimitive(Type rType)
		{
			return rType.IsPrimitive;
		}

		public static bool IsValueType(Type rType)
		{
			return rType.IsValueType;
		}

		public static bool IsGenericType(Type rType)
		{
			return rType.IsGenericType;
		}

		public static object GetDefaultValue(Type rType)
		{
			bool flag = false;
			if (rType.IsValueType)
			{
				return Activator.CreateInstance(rType);
			}
			Vector3 vector = default(Vector3);
			return vector.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(rType)
				.Invoke(vector, null);
		}
	}
}
