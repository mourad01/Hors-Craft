// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlueprintsShopState
using Common.Managers;
using Common.Managers.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace States
{
	public class BlueprintsShopState : XCraftUIState<BlueprintsShopStateConnector>
	{
		[Serializable]
		public class ScrollableContentInfo
		{
			public string name;

			public ScrollableContent content;
		}

		public List<ScrollableContentInfo> contents;

		private BlueprintsShopStateConnector.Tab currentTab;

		protected override AutoAdsConfig autoAdsConfig
		{
			[CompilerGenerated]
			get
			{
				return (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL)) ? base.autoAdsConfig : new AutoAdsConfig
				{
					autoShowOnStart = false
				};
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			int i;
			for (i = 0; i < contents.Count; i++)
			{
				BlueprintsShopStateConnector.Tab tab = base.connector.tabs.FirstOrDefault((BlueprintsShopStateConnector.Tab t) => t.name == contents[i].name);
				if (tab != null)
				{
					tab.mediator.SetAdapter(contents[i].content.adapter);
					contents[i].content.InjectScrollableContent(tab.mediator);
					tab.mediator.CreateContent();
					tab.button.onClick.AddListener(delegate
					{
						ChooseTab(tab);
					});
				}
			}
			ChooseTab(base.connector.tabs[0]);
			base.connector.returnButton.onClick.AddListener(delegate
			{
				Manager.Get<StateMachineManager>().PopState();
			});
		}

		private void ChooseTab(BlueprintsShopStateConnector.Tab tab)
		{
			(from t2 in base.connector.tabs
				where t2 != tab
				select t2).ToList().ForEach(delegate(BlueprintsShopStateConnector.Tab t2)
			{
				t2.mediator.Hide();
				t2.button.interactable = true;
				t2.topImage.gameObject.SetActive(value: true);
				t2.selectedImage.gameObject.SetActive(value: false);
			});
			tab.mediator.Show();
			tab.button.interactable = false;
			tab.topImage.gameObject.SetActive(value: false);
			tab.selectedImage.gameObject.SetActive(value: true);
			currentTab = tab;
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			ChooseTab(currentTab);
			currentTab.mediator.Show();
		}

		public override void FreezeState()
		{
			base.FreezeState();
			currentTab.mediator.Hide();
		}
	}
}
