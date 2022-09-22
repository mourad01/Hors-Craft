// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SkipTimeModule
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class SkipTimeModule : GameplayModule
	{
		public Button skipButton;

		protected override Fact[] listenedFacts => new Fact[0];

		public override void Init()
		{
			base.Init();
			skipButton.onClick.AddListener(delegate
			{
				OnSkip();
			});
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
		}

		private void OnSkip()
		{
			float time = Manager.Get<ModelManager>().growthSettings.SkipTimeAmount();
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL))
			{
				OnSkipSuccess(time);
				return;
			}
			int num = (int)(time / 60f / 60f);
			string text = Manager.Get<TranslationsManager>().GetText("watch.ad.get.growth", "Watch ad to skip {0} hours!");
			text = text.Replace("{0}", num.ToString());
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				numberOfAdsNeeded = 1,
				translationKey = string.Empty,
				description = text,
				reason = StatsManager.AdReason.SKIP_TIME,
				immediatelyAd = false,
				type = AdsCounters.None,
				onSuccess = delegate(bool b)
				{
					if (b)
					{
						OnSkipSuccess(time);
					}
				},
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.translationKey = "menu.watch";
					componentInChildren.defaultText = "Watch";
					componentInChildren.ForceRefresh();
				}
			});
		}

		private void OnSkipSuccess(float time)
		{
			Manager.Get<GrowthManager>().FastForward(time);
		}
	}
}
