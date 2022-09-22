// DecompilerFi decompiler from Assembly-CSharp.dll class: States.OfferStartTimerModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class OfferStartTimerModule : GameplayModule
	{
		public GameObject starterTimer;

		public Button button;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.OFFERPACK_ENABLED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			OfferpackStarterTimerContext offerpackStarterTimerContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<OfferpackStarterTimerContext>(Fact.OFFERPACK_ENABLED).FirstOrDefault();
			if (offerpackStarterTimerContext != null)
			{
				UpdateStarterTimer(offerpackStarterTimerContext.time, offerpackStarterTimerContext.color);
				SetListenerToButton(button, offerpackStarterTimerContext.onClick);
			}
			starterTimer.SetActive(offerpackStarterTimerContext != null);
		}

		public void UpdateStarterTimer(string timer, Color color)
		{
			starterTimer.GetComponentInChildren<Text>().text = timer;
			starterTimer.GetComponentInChildren<Image>().color = color;
		}
	}
}
