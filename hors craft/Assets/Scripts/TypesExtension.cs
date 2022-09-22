// DecompilerFi decompiler from Assembly-CSharp.dll class: TypesExtension
using System;
using System.Collections.Generic;

public static class TypesExtension
{
	public static List<Type> TypeHierarchy(this Type type)
	{
		List<Type> list = new List<Type>();
		do
		{
			list.Add(type);
			type = type.BaseType;
		}
		while (type != null);
		return list;
	}

	public static int GetDepthOfInheritance(this Type type)
	{
		return type.TypeHierarchy().Count;
	}
}
