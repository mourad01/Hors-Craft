// DecompilerFi decompiler from Assembly-CSharp.dll class: MonoBehaviourSingleton
using Common.Utils;
using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
	private const string STR_NoInstanceOfATemporaryOneIsCreated = "No instance of {0}, a temporary one is created.";

	private const string STR_TempInstanceOf = "Temp Instance of ";

	private const string STR_ProblemDuringTheCreationOf = "Problem during the creation of {0}";

	private static T m_Instance;

	private static bool IsShuttingDown;

	public static T get
	{
		get
		{
			if (IsShuttingDown)
			{
				return (T)null;
			}
			if ((Object)m_Instance == (Object)null)
			{
				m_Instance = (UnityEngine.Object.FindObjectOfType(typeof(T)) as T);
				if ((Object)m_Instance == (Object)null)
				{
					UnityEngine.Debug.Log($"No instance of {typeof(T).ToString()}, a temporary one is created.");
					m_Instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
					if ((Object)m_Instance == (Object)null)
					{
						UnityEngine.Debug.Log($"Problem during the creation of {typeof(T).ToString()}");
					}
				}
				m_Instance.gameObject.AddComponent<DontDestroyOnLoadBehaviour>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	private void Awake()
	{
		if ((Object)m_Instance == (Object)null)
		{
			m_Instance = (this as T);
			m_Instance.Init();
		}
	}

	public virtual void Init()
	{
	}

	internal virtual void OnApplicationQuit()
	{
		IsShuttingDown = true;
		m_Instance = (T)null;
	}
}
