// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FarmButtonModule
using System.Collections.Generic;
using UnityEngine.UI;

namespace States
{
	public class FarmButtonModule : GameplayModule
	{
		public Button sowButton;

		public Button waterButton;

		public Button harvestButton;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.IN_FRONT_OF_GROWABLE
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			HarvestContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<HarvestContext>(Fact.IN_FRONT_OF_GROWABLE);
			if (factContext != null)
			{
				SetListenerToButton(harvestButton, factContext.useAction);
			}
			harvestButton.gameObject.SetActive(factContext != null);
			InteractiveVoxelContext factContext2 = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<InteractiveVoxelContext>(Fact.IN_FRONT_OF_GROWABLE);
			if (factContext2 != null)
			{
				SetListenerToButton(sowButton, factContext2.useAction);
			}
			sowButton.gameObject.SetActive(factContext2 != null);
			WateringContext factContext3 = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<WateringContext>(Fact.IN_FRONT_OF_GROWABLE);
			if (factContext3 != null)
			{
				SetListenerToButton(waterButton, factContext3.useAction);
			}
			waterButton.gameObject.SetActive(factContext3 != null);
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL))
			{
				GetComponent<LayoutElement>().ignoreLayout = false;
			}
			else if (!waterButton.gameObject.activeSelf && !sowButton.gameObject.activeSelf && !harvestButton.gameObject.activeSelf)
			{
				GetComponent<LayoutElement>().ignoreLayout = true;
			}
			else
			{
				GetComponent<LayoutElement>().ignoreLayout = false;
			}
		}
	}
}
