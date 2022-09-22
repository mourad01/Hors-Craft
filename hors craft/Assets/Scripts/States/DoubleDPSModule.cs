// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DoubleDPSModule
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class DoubleDPSModule : GameplayModule
	{
		public Text dpsText;

		public Image tintImage;

		public Button doubleDPSButton;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.DOUBLE_DPS
		};

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			DoubleDPSContext doubleDPSContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<DoubleDPSContext>(Fact.DOUBLE_DPS).FirstOrDefault();
			if (doubleDPSContext != null)
			{
				SetListenerToButton(doubleDPSButton, doubleDPSContext.onButtonClicked);
				dpsText.gameObject.SetActive(doubleDPSContext.active);
				tintImage.gameObject.SetActive(doubleDPSContext.active);
				doubleDPSButton.interactable = !doubleDPSContext.active;
				if (doubleDPSContext.active)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(doubleDPSContext.leftTime);
					int hours = timeSpan.Hours;
					int minutes = timeSpan.Minutes;
					int seconds = timeSpan.Seconds;
					dpsText.text = ((hours <= 0) ? string.Empty : (hours.ToString("00") + ":")) + ((minutes <= 0 && hours <= 0) ? string.Empty : (minutes.ToString("00") + ":")) + seconds.ToString("00");
					tintImage.fillAmount = doubleDPSContext.proggress;
				}
			}
			doubleDPSButton.gameObject.SetActive(doubleDPSContext != null);
		}
	}
}
