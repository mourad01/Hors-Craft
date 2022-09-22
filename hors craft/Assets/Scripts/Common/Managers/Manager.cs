// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.Manager
using System;
using UnityEngine;

namespace Common.Managers
{
	public abstract class Manager : MonoBehaviour
	{
		public static T Get<T>() where T : Manager
		{
			return MonoBehaviourSingleton<ManagersContainer>.get.GetManager<T>();
		}

		public static Manager Get(Type managerType)
		{
			return MonoBehaviourSingleton<ManagersContainer>.get.GetManager(managerType);
		}

		public static bool Contains<T>() where T : Manager
		{
			return MonoBehaviourSingleton<ManagersContainer>.get.ContainsManager<T>();
		}

		public virtual Type[] GetDependencies()
		{
			return null;
		}

		public abstract void Init();

		public virtual void OnConsentSpecified(bool consentAquired)
		{
		}
	}
}
