// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityGoExtensions
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UnityGoExtensions
{
	[CompilerGenerated]
	private static Func<IEnumerable<Transform>, Transform[]> _003C_003Ef__mg_0024cache0;

	public static T GetComponentOnObject<T>(this GameObject go, bool showErrorInConsole) where T : Component
	{
		T component = go.GetComponent<T>();
		if (showErrorInConsole && (UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			UnityEngine.Debug.LogError($"Unable to find component '{typeof(T).Name}' on object '{go.name}'");
		}
		return component;
	}

	public static T GetComponentOnObject<T>(this Transform trans, bool showErrorInConsole) where T : Component
	{
		T component = trans.GetComponent<T>();
		if (showErrorInConsole && (UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			UnityEngine.Debug.LogError($"Unable to find component '{typeof(T).Name}' on object '{trans.name}'");
		}
		return component;
	}

	public static T GetComponentOnObjectOrParent<T>(this GameObject go, bool showErrorInConsole) where T : Component
	{
		T componentInParent = go.GetComponentInParent<T>();
		if (showErrorInConsole && (UnityEngine.Object)componentInParent == (UnityEngine.Object)null)
		{
			UnityEngine.Debug.LogError($"Unable to find component '{typeof(T).Name}' on object (or parent) '{go.name}'");
		}
		return componentInParent;
	}

	public static bool IsNullOrInactive(this GameObject go)
	{
		return go == null || !go.activeSelf;
	}

	public static bool IsActive(this GameObject go)
	{
		return go != null && go.activeSelf;
	}

	public static void ActivateAndParent(this GameObject go)
	{
		if (!(go == null))
		{
			if (go.transform.parent != null)
			{
				go.transform.parent.gameObject.ActivateAndParent();
			}
			go.SetActive(value: true);
		}
	}

	public static bool HasRigidbody(this GameObject go)
	{
		return go.GetComponent<Rigidbody>() != null;
	}

	public static bool HasCharacterController(this GameObject go)
	{
		return go.GetComponent<CharacterController>() != null;
	}

	public static bool HasAnimation(this GameObject go)
	{
		return go.GetComponent<Animation>() != null;
	}

	public static bool HasComponent<T>(this GameObject go) where T : Component
	{
		return (UnityEngine.Object)go.GetComponent<T>() != (UnityEngine.Object)null;
	}

	public static void SetLayerRecursively(this GameObject go, int layer)
	{
		go.layer = layer;
		IEnumerator enumerator = go.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.gameObject.SetLayerRecursively(layer);
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

	public static void SetCollisionRecursively(this GameObject go, bool enabled)
	{
		Collider component = go.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = enabled;
		}
		IEnumerator enumerator = go.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.gameObject.SetCollisionRecursively(enabled);
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

	public static List<T> GetComponentsInChildrenWithTag<T>(this GameObject go, string tag) where T : Component
	{
		List<T> list = new List<T>();
		if (go.CompareTag(tag))
		{
			list.Add(go.GetComponent<T>());
		}
		IEnumerator enumerator = go.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				list.AddRange(transform.gameObject.GetComponentsInChildrenWithTag<T>(tag));
			}
			return list;
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

	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return go.AddComponent<T>();
		}
		return component;
	}

	public static int GetCollisionMask(this GameObject go, int layer = -1)
	{
		if (layer == -1)
		{
			layer = go.layer;
		}
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			num |= ((!Physics.GetIgnoreLayerCollision(layer, i)) ? 1 : 0) << i;
		}
		return num;
	}

	public static Transform[] GetChildrenWithName(this GameObject go, string name)
	{
		return (from Transform w in go.transform
			where w.name.Equals(name, StringComparison.CurrentCultureIgnoreCase)
			select w).Return((Func<IEnumerable<Transform>, Transform[]>)Enumerable.ToArray<Transform>, (Transform[])null);
	}

	public static T[] GetChildrenComponent<T>(this GameObject go) where T : Component
	{
		return (from Transform w in go.transform
			where w.gameObject.HasComponent<T>()
			select w).Return((IEnumerable<Transform> r) => r.ToList().UnityConvertAll((Transform x) => x.gameObject.GetComponent<T>()).ToArray(), new List<T>().ToArray());
	}

	public static T GetInterface<T>(this GameObject go) where T : class
	{
		if (!typeof(T).IsInterface)
		{
			UnityEngine.Debug.LogError(typeof(T).ToString() + " is not an interface");
			return (T)null;
		}
		return go.GetComponents<Component>().OfType<T>().FirstOrDefault();
	}

	public static T GetInterfaceInChildren<T>(this GameObject go) where T : class
	{
		if (!typeof(T).IsInterface)
		{
			UnityEngine.Debug.LogError(typeof(T).ToString() + " is not an interface");
			return (T)null;
		}
		return go.GetComponentsInChildren<Component>().OfType<T>().FirstOrDefault();
	}

	public static IEnumerable<T> GetInterfaces<T>(this GameObject go) where T : class
	{
		if (!typeof(T).IsInterface)
		{
			UnityEngine.Debug.LogError(typeof(T).ToString() + " is not an interface");
			return Enumerable.Empty<T>();
		}
		return go.GetComponents<Component>().OfType<T>();
	}

	public static IEnumerable<T> GetInterfacesInChildren<T>(this GameObject go) where T : class
	{
		if (!typeof(T).IsInterface)
		{
			UnityEngine.Debug.LogError(typeof(T).ToString() + " is not an interface");
			return Enumerable.Empty<T>();
		}
		return go.GetComponentsInChildren<Component>(includeInactive: true).OfType<T>();
	}
}
