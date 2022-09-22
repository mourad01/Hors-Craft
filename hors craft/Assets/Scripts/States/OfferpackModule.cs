// DecompilerFi decompiler from Assembly-CSharp.dll class: States.OfferpackModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class OfferpackModule : GameplayModule
	{
		public Button offerPackButton;

		public Image image;

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
			OfferPackContext offerPackContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<OfferPackContext>(Fact.OFFERPACK_ENABLED).FirstOrDefault();
			if (offerPackContext != null)
			{
				SetListenerToButton(offerPackButton, offerPackContext.onOfferpackButton);
				image.sprite = offerPackContext.sprite;
			}
			base.gameObject.SetActive(offerPackContext != null);
		}
	}
}
