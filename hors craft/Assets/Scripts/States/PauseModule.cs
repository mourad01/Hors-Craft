// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PauseModule
using Common.Managers;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class PauseModule : GameplayModule
	{
		public Button pauseButton;

		public GameObject adTimer;

		public GameObject chestNotification;

		public TranslateText adTimerText;

		public TranslateText adNowText;

		public Image timeTV;

		private ModelManager modelManager;

		private bool useAdTimer;

		private float lastPauseAdTimerUpdate;

		protected override Fact[] listenedFacts => new Fact[0];

		public override void Init()
		{
			base.Init();
			pauseButton.onClick.AddListener(delegate
			{
				OnPause();
			});
			adTimerText = adTimer.GetComponentInChildren<TranslateText>();
			if (Application.isPlaying)
			{
				modelManager = Manager.Get<ModelManager>();
				useAdTimer = (modelManager.pauseAdTimer.GetAdTimerEnabled() && !modelManager.modulesContext.isAdsFree);
				adTimer.SetActive(useAdTimer);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (UnityEngine.Input.GetKeyDown(KeyCode.E))
			{
				OnPause();
			}
			if (useAdTimer && !modelManager.modulesContext.isAdsFree)
			{
				UpdatePauseAdTimer();
			}
			else if (adTimer.activeSelf)
			{
				adTimer.SetActive(value: false);
			}
			if (modelManager.lootSettings.IsShopEnabled() && AutoRefreshingStock.GetStockCount("free.chest") >= 1)
			{
				chestNotification.SetActive(value: true);
			}
			else
			{
				chestNotification.SetActive(value: false);
			}
		}

		private void OnPause()
		{
			StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.MENU_OPENED);
			(Manager.Get<StateMachineManager>().currentState as GameplayState).OnPause();
		}

		private void UpdatePauseAdTimer()
		{
			if (Time.realtimeSinceStartup > lastPauseAdTimerUpdate + 1f)
			{
				lastPauseAdTimerUpdate = Time.realtimeSinceStartup;
				int @int = PlayerPrefs.GetInt("timeSinceStartup");
				int secondsToNewAd = modelManager.timeBasedAdRequirements.TimeRemainingForContext("pause", @int);
				UnityEngine.Debug.Log("Updated Pause Ad Timer, seconds to new ad: " + secondsToNewAd);
				if (secondsToNewAd <= 0)
				{
					adTimerText.gameObject.SetActive(value: false);
					adNowText.gameObject.SetActive(value: true);
					timeTV.gameObject.SetActive(value: false);
				}
				else
				{
					timeTV.gameObject.SetActive(value: true);
					adTimerText.gameObject.SetActive(value: true);
					adNowText.gameObject.SetActive(value: false);
					adTimerText.ClearVisitors();
					adTimerText.AddVisitor((string text) => text.Replace("{s}", Mathf.Max(0, secondsToNewAd).ToString()));
				}
			}
		}
	}
}
