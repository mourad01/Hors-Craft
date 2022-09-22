// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectExtensions
public static class ObjectExtensions
{
	public static bool IsA<T>(this object obj)
	{
		return obj is T;
	}

	public static bool IsNotA<T>(this object obj)
	{
		return obj.IsA<T>().Toggle();
	}

	public static T As<T>(this object obj) where T : class
	{
		return obj as T;
	}

	public static bool HasValue(this object obj)
	{
		return obj != null;
	}

	public static T Clone<T>(this object item) where T : class
	{
		return (item == null) ? ((T)null) : item.XMLSerialize_ToString().XMLDeserialize_ToObject<T>();
	}
}
