// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GameCallbacksManager
using Common.Managers;
using System;
using System.Collections.Generic;

namespace Gameplay
{
	public class GameCallbacksManager : Manager
	{
		private List<IGameCallbacksListener> createdListeners;

		private List<IGameCallbacksListener> listeners
		{
			get
			{
				if (createdListeners == null)
				{
					createdListeners = new List<IGameCallbacksListener>();
				}
				return createdListeners;
			}
		}

		public override void Init()
		{
		}

		public void RegisterListener(IGameCallbacksListener obj)
		{
			listeners.Add(obj);
		}

		public void UnregisterObject(IGameCallbacksListener obj)
		{
			if (listeners.Contains(obj))
			{
				listeners.Remove(obj);
			}
		}

		public void FrequentSave()
		{
			DoForEveryObject(listeners, delegate(IGameCallbacksListener obj)
			{
				obj.OnGameSavedFrequent();
			});
			WorldPlayerPrefs.get.Save();
		}

		public void InFrequentSave()
		{
			DoForEveryObject(listeners, delegate(IGameCallbacksListener obj)
			{
				obj.OnGameSavedInfrequent();
			});
			WorldPlayerPrefs.get.Save();
		}

		public void GameplayStarted()
		{
			DoForEveryObject(listeners, delegate(IGameCallbacksListener obj)
			{
				obj.OnGameplayStarted();
			});
		}

		public void Restart()
		{
			DoForEveryObject(listeners, delegate(IGameCallbacksListener obj)
			{
				obj.OnGameplayRestarted();
			});
		}

		private void DoForEveryObject(List<IGameCallbacksListener> objList, Action<IGameCallbacksListener> action)
		{
			if (objList != null)
			{
				for (int i = 0; i < objList.Count; i++)
				{
					action(objList[i]);
				}
			}
		}
	}
}
