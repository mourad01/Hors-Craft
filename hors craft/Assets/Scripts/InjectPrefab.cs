// DecompilerFi decompiler from Assembly-CSharp.dll class: InjectPrefab
using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("UI/Inject Prefab", 1000)]
[RequireComponent(typeof(RectTransform))]
public class InjectPrefab : MonoBehaviour
{
	public GameObject injectPrefab;

	private GameObject m_injectedTemplate;

	[SerializeField]
	[HideInInspector]
	private GameObject m_injectedInstance;

	public GameObject injectedInstance
	{
		get
		{
			refreshInjected();
			return m_injectedInstance;
		}
	}

	private void injectedDestroy()
	{
		if (m_injectedInstance != null)
		{
			destroySmart(ref m_injectedInstance);
		}
		m_injectedTemplate = null;
	}

	private static void destroySmart<T>(ref T obj) where T : UnityEngine.Object
	{
		if ((UnityEngine.Object)obj != (UnityEngine.Object)null)
		{
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			else
			{
				UnityEngine.Object.DestroyObject(obj);
			}
			obj = (T)null;
		}
	}

	private static void recursiveSetHideFlags(GameObject go, HideFlags flags)
	{
		go.hideFlags = flags;
		IEnumerator enumerator = go.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				recursiveSetHideFlags(transform.gameObject, flags);
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

	private void injectedCreate()
	{
		if (injectPrefab != null)
		{
			m_injectedTemplate = injectPrefab;
			m_injectedInstance = UnityEngine.Object.Instantiate(m_injectedTemplate);
			m_injectedInstance.transform.SetParent(base.transform, worldPositionStays: false);
			recursiveSetHideFlags(m_injectedInstance, HideFlags.DontSave);
		}
	}

	private void Start()
	{
		refreshInjected();
	}

	private void refreshInjected()
	{
		if (m_injectedTemplate != injectPrefab)
		{
			injectedDestroy();
			injectedCreate();
		}
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		injectedDestroy();
	}
}
