// DecompilerFi decompiler from Assembly-CSharp.dll class: States.HardForAdModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class HardForAdModule : GameplayModule
	{
		public Button HardForAdButton;

		public Image image;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.HARD_FOR_AD_ENABLED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			HardForAdContext hardForAdContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HardForAdContext>(Fact.HARD_FOR_AD_ENABLED).FirstOrDefault();
			if (hardForAdContext != null)
			{
				SetListenerToButton(HardForAdButton, hardForAdContext.onHardForAdButton);
			}
			base.gameObject.SetActive(hardForAdContext != null);
		}
	}
}
