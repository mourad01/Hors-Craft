// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.ManagersContainer
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Managers
{
	public class ManagersContainer : MonoBehaviourSingleton<ManagersContainer>
	{
		private Dictionary<Type, Manager> managersDictionary;

		public bool debugInEditor;

		public bool debugAfterBuild;

		private Dictionary<Type, Manager> abstractManagersCache = new Dictionary<Type, Manager>();

		public override void Init()
		{
			if (!Application.isPlaying)
			{
				throw new Exception("Don't try to Manager.get or ManagersContainer.get in Editor Mode!");
			}
			managersDictionary = new Dictionary<Type, Manager>();
			List<Manager> list = new List<Manager>();
			GetInSceneManagers(base.transform, managersDictionary, list);
			List<Type> list2 = new List<Type>();
			foreach (Manager item in list)
			{
				Type[] dependencies = item.GetDependencies();
				if (dependencies != null && dependencies.Length > 0)
				{
					Type[] array = dependencies;
					foreach (Type type in array)
					{
						bool flag = false;
						if (list2.Contains(type))
						{
							flag = true;
						}
						else
						{
							foreach (Type item2 in list2)
							{
								if (type.IsAssignableFrom(item2))
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							throw new Exception("Manager type " + item.GetType() + " needs manager " + type + " to be initialized first!\nChange gameObjects tree order to fulfill the dependencies.");
						}
					}
				}
				if (Application.isEditor && debugInEditor)
				{
					try
					{
						UnityEngine.Debug.Log("Initializing " + item.GetType().Name + " " + Time.realtimeSinceStartup);
						item.Init();
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogError("Error while initializing " + item.GetType().Name + ":\n" + ex);
					}
				}
				else
				{
					if (debugAfterBuild)
					{
						UnityEngine.Debug.LogWarning("Initializing " + item.GetType().Name + " " + Time.realtimeSinceStartup);
					}
					item.Init();
				}
				list2.Add(item.GetType());
			}
			PlayerSession.IncrementSessionNo();
		}

		public Manager[] GetManagers()
		{
			return managersDictionary.Values.ToArray();
		}

		public void GetInSceneManagers(Transform t, Dictionary<Type, Manager> typeDictionary, List<Manager> managersList, bool selfCheck = false)
		{
			if (selfCheck)
			{
				Manager[] components = t.gameObject.GetComponents<Manager>();
				Manager[] array = components;
				foreach (Manager manager in array)
				{
					if (manager != null)
					{
						if (typeDictionary.ContainsKey(manager.GetType()))
						{
							throw new Exception("Duplicated manager type in managers tree! --> " + manager.GetType());
						}
						managersList.Add(manager);
						typeDictionary.Add(manager.GetType(), manager);
					}
				}
			}
			int childCount = t.childCount;
			for (int j = 0; j < childCount; j++)
			{
				GetInSceneManagers(t.GetChild(j), typeDictionary, managersList, selfCheck: true);
			}
		}

		protected virtual List<Manager> AppendDefaultManagers()
		{
			return new List<Manager>();
		}

		protected T AppendIfNotPresent<T>(List<Manager> added, int siblingIndex) where T : Manager
		{
			if (managersDictionary.ContainsKey(typeof(T)))
			{
				return (T)null;
			}
			GameObject gameObject = new GameObject(typeof(T).Name);
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			gameObject.transform.SetSiblingIndex(siblingIndex);
			T val = gameObject.AddComponent<T>();
			added.Add(val);
			managersDictionary[typeof(T)] = val;
			return val;
		}

		public T GetManager<T>() where T : Manager
		{
			if (managersDictionary.TryGetValue(typeof(T), out Manager value))
			{
				return value as T;
			}
			if (abstractManagersCache.TryGetValue(typeof(T), out value))
			{
				return value as T;
			}
			foreach (Manager value2 in managersDictionary.Values)
			{
				if (value2 is T)
				{
					abstractManagersCache[typeof(T)] = value2;
					return value2 as T;
				}
			}
			return (T)null;
		}

		public Manager GetManager(Type managerType)
		{
			if (managersDictionary.TryGetValue(managerType, out Manager value))
			{
				return value;
			}
			if (abstractManagersCache.TryGetValue(managerType, out value))
			{
				return value;
			}
			foreach (Manager value2 in managersDictionary.Values)
			{
				if (managerType.IsAssignableFrom(value2.GetType()))
				{
					abstractManagersCache[managerType] = value2;
					return value2;
				}
			}
			return null;
		}

		public bool ContainsManager<T>() where T : Manager
		{
			if (managersDictionary.ContainsKey(typeof(T)))
			{
				return true;
			}
			if (abstractManagersCache.ContainsKey(typeof(T)))
			{
				return true;
			}
			foreach (Manager value in managersDictionary.Values)
			{
				if (value is T)
				{
					abstractManagersCache[typeof(T)] = value;
					return true;
				}
			}
			return false;
		}
	}
}
