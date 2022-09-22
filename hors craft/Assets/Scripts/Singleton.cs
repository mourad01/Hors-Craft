// DecompilerFi decompiler from Assembly-CSharp.dll class: Singleton
using System;

public class Singleton<T> : IDisposable where T : class, IDisposable, new()
{
	protected static T instance;

	public static T get
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}
			return instance;
		}
	}

	public static T Get
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}
			return instance;
		}
	}

	public virtual void Dispose()
	{
		instance = (T)null;
	}
}
