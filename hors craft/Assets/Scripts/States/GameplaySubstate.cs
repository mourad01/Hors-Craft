// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GameplaySubstate
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class GameplaySubstate : MonoBehaviour
	{
		public List<string> modules;

		public GameplayState.Substates substate;

		private List<ModuleLoader> activeModuleLoaders = new List<ModuleLoader>();

		private Action<ModuleLoader, GameObject> onSpawnActions;

		private void Awake()
		{
			IGameplaySubstateAction[] components = GetComponents<IGameplaySubstateAction>();
			for (int i = 0; i < components.Length; i++)
			{
				onSpawnActions = (Action<ModuleLoader, GameObject>)Delegate.Combine(onSpawnActions, components[i].GetAction());
			}
		}

		public void Activate(List<ModuleLoader> moduleLoaders)
		{
			moduleLoaders.ForEach(delegate(ModuleLoader ml)
			{
				if (IsModuleAllowed(ml))
				{
					Action<ModuleLoader, GameObject> onSpawnAction = onSpawnActions;
					ml.Spawn(init: true, onSpawnAction);
					activeModuleLoaders.Add(ml);
				}
				else
				{
					ml.Despawn();
				}
			});
		}

		private bool IsModuleAllowed(ModuleLoader loader)
		{
			return modules.Contains(loader.path);
		}

		public void Show()
		{
			(from ml in activeModuleLoaders
				where ml != null
				select ml).ToList().ForEach(delegate(ModuleLoader ml)
			{
				Action<ModuleLoader, GameObject> onSpawnAction = onSpawnActions;
				ml.Spawn(init: true, onSpawnAction);
			});
		}

		public void Hide()
		{
			(from ml in activeModuleLoaders
				where ml != null
				select ml).ToList().ForEach(delegate(ModuleLoader ml)
			{
				ml.Despawn();
			});
		}
	}
}
