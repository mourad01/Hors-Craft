// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PotionsModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class PotionsModule : GameplayModule
	{
		public Text potionsText;

		public Button potionsButton;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.POTIONS
		};

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			PotionsContext potionsContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<PotionsContext>(Fact.POTIONS).FirstOrDefault();
			if (potionsContext != null)
			{
				SetListenerToButton(potionsButton, potionsContext.onButtonClicked);
				potionsText.text = potionsContext.leftPotions.ToString();
			}
			potionsButton.gameObject.SetActive(potionsContext != null);
		}
	}
}
