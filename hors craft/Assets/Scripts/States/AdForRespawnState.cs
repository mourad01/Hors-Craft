// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AdForRespawnState
using Common.Managers;
using Common.Managers.States;
using System;

namespace States
{
	public class AdForRespawnState : XCraftUIState<AdForRespawnStateConnector>
	{
		private AdForRespawnStartParameter parameters;

		protected override AutoAdsConfig autoAdsConfig
		{
			get
			{
				AutoAdsConfig autoAdsConfig = new AutoAdsConfig();
				autoAdsConfig.autoShowOnStart = true;
				autoAdsConfig.properAdReason = StatsManager.AdReason.XCRAFT_RESPAWN;
				return autoAdsConfig;
			}
		}

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			if (!(startParameter is AdForRespawnStartParameter))
			{
				throw new InvalidOperationException("Wrong start parameter used!");
			}
			parameters = (startParameter as AdForRespawnStartParameter);
			SetCustomButtons();
			base.connector.SetCustomTitle(parameters.titleTextKey);
			base.connector.SetSecondText(parameters.aditionalTexts);
			base.connector.secondText.AddVisitor(delegate(string text)
			{
				SurvivalRankManager survivalRankManager = Manager.Get<SurvivalRankManager>();
				if (survivalRankManager == null)
				{
					return string.Empty;
				}
				return (!text.Contains("{0}")) ? $"{text}{survivalRankManager.leaderboardPoints.ToString()}" : text.Replace("{0}", survivalRankManager.leaderboardPoints.ToString());
			});
			base.connector.secondText.ForceRefresh();
		}

		protected void SetCustomButtons()
		{
			base.connector.SetOkButton(parameters.buttonOkTextKey, parameters.onAccept);
			base.connector.SetCancelButton(parameters.buttonCancelTextKey, parameters.onRefuse);
		}
	}
}
