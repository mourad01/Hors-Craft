// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoveButtonModule
using System.Collections.Generic;
using UnityEngine.UI;

namespace States
{
	public class LoveButtonModule : GameplayModule
	{
		public Button loveButton;

		public Button giftButton;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.IN_FRONT_OF_LOVABLE,
			Fact.IN_FRONT_OF_LOVED_ONE
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			InteractiveObjectContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<InteractiveObjectContext>(Fact.IN_FRONT_OF_LOVABLE);
			if (factContext != null)
			{
				SetListenerToButton(loveButton, factContext.useAction);
			}
			loveButton.gameObject.SetActive(factContext != null);
			InteractiveObjectContext factContext2 = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<InteractiveObjectContext>(Fact.IN_FRONT_OF_LOVED_ONE);
			if (factContext2 != null)
			{
				SetListenerToButton(giftButton, factContext2.useAction);
			}
			giftButton.gameObject.SetActive(factContext2 != null);
		}
	}
}
