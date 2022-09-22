// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LevelUpWithStatsPopupState
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using UnityEngine;

namespace States
{
	public class LevelUpWithStatsPopupState : XCraftUIState<LevelUpWithStatsPopupStateConnector>
	{
		[Space(2f)]
		public GameObject contentElement;

		protected override bool hasBackgroundOverlay => false;

		protected override bool hasBackground => false;

		//protected override bool canShowBanner => false;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = false;
				return autoAdsConfig;
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			LevelUpWithStatsPopupStartParameter levelUpWithStatsPopupStartParameter = (LevelUpWithStatsPopupStartParameter)parameter;
			TimeScaleHelper.value = 0f;
			base.connector.okButton.onClick.RemoveAllListeners();
			base.connector.okButton.onClick.AddListener(OnOkButton);
			if (levelUpWithStatsPopupStartParameter.topDecor != null)
			{
				base.connector.topDecor.sprite = levelUpWithStatsPopupStartParameter.topDecor;
			}
			for (int i = 0; i < levelUpWithStatsPopupStartParameter.statsConfigs.Count; i++)
			{
				LevelUpStatElement component = Object.Instantiate(contentElement, base.connector.contentParent).GetComponent<LevelUpStatElement>();
				component.ico.sprite = levelUpWithStatsPopupStartParameter.statsConfigs[i].statIco;
				component.text.text = levelUpWithStatsPopupStartParameter.statsConfigs[i].statText;
			}
			base.connector.levelupText.text = string.Format(Manager.Get<TranslationsManager>().GetText("level.up.state.text", "You levelup to {0}"), levelUpWithStatsPopupStartParameter.level);
		}

		private void OnOkButton()
		{
			for (int num = base.connector.contentParent.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(base.connector.contentParent.GetChild(num).gameObject);
			}
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		}
	}
}
