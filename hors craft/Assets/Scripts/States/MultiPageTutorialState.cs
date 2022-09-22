// DecompilerFi decompiler from Assembly-CSharp.dll class: States.MultiPageTutorialState
using Common.Managers;
using Common.Managers.States;
using Common.Waterfall;
using System;
using UnityEngine;

namespace States
{
	public class MultiPageTutorialState : XCraftUIState<MultiPageTutorialStateConnector>
	{
		[Serializable]
		public class TutorialPage
		{
			public Sprite image;

			public string defaultText;

			public string translationKey;
		}

		public TutorialPage[] pages;

		private int page;

		private int pageCount;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			base.connector.onLeftButtonClicked = OnPreviousPage;
			base.connector.onRightButtonClicked = OnNextPage;
			page = 0;
			pageCount = pages.Length;
			SetPage();
		}

		private void OnPreviousPage()
		{
			page--;
			SetPage();
		}

		private void OnNextPage()
		{
			page++;
			SetPage();
		}

		private void SetPage()
		{
			TutorialPage tutorialPage = pages[page];
			base.connector.image.sprite = tutorialPage.image;
			base.connector.text.GetComponent<TranslateText>().defaultText = tutorialPage.defaultText;
			base.connector.text.GetComponent<TranslateText>().translationKey = tutorialPage.translationKey;
			base.connector.text.GetComponent<TranslateText>().ForceRefresh();
			if (page >= pageCount - 1)
			{
				base.connector.rightButton.onClick.RemoveAllListeners();
				base.connector.rightButton.onClick.AddListener(OnPlay);
			}
			else
			{
				base.connector.rightButton.onClick.RemoveAllListeners();
				base.connector.rightButton.onClick.AddListener(OnNextPage);
			}
			if (page <= 0)
			{
				base.connector.leftButton.gameObject.SetActive(value: false);
			}
			else if (!base.connector.leftButton.gameObject.activeSelf)
			{
				base.connector.leftButton.gameObject.SetActive(value: true);
			}
		}

		private void OnPlay()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		public override void UpdateState()
		{
			base.UpdateState();
			//AdWaterfall.get.HideBanner();
		}
	}
}
