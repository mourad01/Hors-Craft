// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.MonoBehaviourSingleton`1
using Common.Utils;
using UnityEngine;

namespace TsgCommon
{
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
	{
		private static T instance;

		private static bool isShuttingDown;

		public static T get
		{
			get
			{
				if (isShuttingDown)
				{
					return (T)null;
				}
				if ((Object)instance == (Object)null)
				{
					instance = (UnityEngine.Object.FindObjectOfType(typeof(T)) as T);
					if ((Object)instance == (Object)null)
					{
						instance = new GameObject("Autocreated " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
					}
					instance.gameObject.AddComponent<DontDestroyOnLoadBehaviour>();
					instance.Init();
				}
				return instance;
			}
		}

		private void Awake()
		{
			if ((Object)instance == (Object)null)
			{
				instance = (this as T);
				instance.Init();
			}
		}

		public virtual void Init()
		{
		}

		internal virtual void OnApplicationQuit()
		{
			isShuttingDown = true;
			instance = (T)null;
		}
	}
}
