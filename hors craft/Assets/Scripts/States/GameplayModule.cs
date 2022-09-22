// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GameplayModule
using Common.Managers;
using GameUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class GameplayModule : MonoBehaviour, IFactChangedListener
	{
		protected virtual Fact[] listenedFacts => new Fact[0];

		public virtual void Init()
		{
			if (listenedFacts.Length > 0)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RegisterFactChangedListener(this, listenedFacts);
			}
			OnFactsChanged(new HashSet<Fact>(listenedFacts));
		}

		public virtual void OnFactsChanged(HashSet<Fact> changedFacts)
		{
		}

		private void Start()
		{
			GameplayState stateInstance = Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>();
			if (!stateInstance.useAlphaButtons)
			{
				return;
			}
			ColorController[] componentsInChildren = GetComponentsInChildren<ColorController>(includeInactive: true);
			foreach (ColorController colorController in componentsInChildren)
			{
				Image component = colorController.gameObject.GetComponent<Image>();
				if (component != null && colorController.patternRepeater == stateInstance.alphaCondition)
				{
					component.color = component.color.WithAlpha(stateInstance.alphaValue);
				}
			}
		}

		protected virtual void Update()
		{
		}

		public virtual void Hide()
		{
			base.gameObject.SetActive(value: false);
		}

		public virtual void Show()
		{
			base.gameObject.SetActive(value: true);
		}

		protected void SetListenerToButton(Button button, Action action)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate
			{
				action();
			});
		}

		private void OnDestroy()
		{
			if (listenedFacts.Length > 0 && MonoBehaviourSingleton<GameplayFacts>.get != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.UnregisterFactChangedListener(this, listenedFacts);
			}
		}
	}
}
