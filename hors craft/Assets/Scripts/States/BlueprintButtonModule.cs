// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlueprintButtonModule
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class BlueprintButtonModule : GameplayModule
	{
		public Button blueprintButton;

		public GameObject mark;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.LEVEL_UP
		};

		public override void Init()
		{
			base.Init();
			if (Application.isPlaying)
			{
				EnableBlueprintsButton();
				CraftingSettings craftingSettings = Manager.Get<ModelManager>().craftingSettings;
				craftingSettings.OnModelDownload = (Action)Delegate.Combine(craftingSettings.OnModelDownload, new Action(EnableBlueprintsButton));
				if (mark != null)
				{
					mark.SetActive(value: false);
				}
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (mark != null)
			{
				mark.SetActive(value: true);
			}
		}

		private void EnableBlueprintsButton()
		{
			if (Manager.Get<ModelManager>().craftingSettings.IsBlueprintsButtonEnabled())
			{
				blueprintButton.gameObject.SetActive(value: true);
				blueprintButton.onClick.RemoveAllListeners();
				blueprintButton.onClick.AddListener(OpenBlueprints);
			}
			else
			{
				blueprintButton.gameObject.SetActive(value: false);
			}
		}

		private void OpenBlueprints()
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(BlueprintsShopState)))
			{
				Manager.Get<StateMachineManager>().PushState<BlueprintsShopState>();
			}
			else
			{
				Manager.Get<StateMachineManager>().PushState<CraftingPopupState>(new CraftingPopupStateStartParameter
				{
					type = typeof(BlueprintCraftableObject)
				});
			}
			if (mark != null)
			{
				mark.SetActive(value: false);
			}
		}
	}
}
