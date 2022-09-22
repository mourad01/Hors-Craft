// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CoinsDrawerModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class CoinsDrawerModule : GameplayModule
	{
		public GameObject currencyDrawerInjector;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.SOFT_CURRENCY_CHANGED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			CurrencyChangedContext currencyChangedContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<CurrencyChangedContext>(Fact.SOFT_CURRENCY_CHANGED).FirstOrDefault();
			if (currencyChangedContext != null)
			{
				CoinsDrawer componentInChildren = currencyDrawerInjector.GetComponentInChildren<CoinsDrawer>();
				componentInChildren.OnCurrencyChange(currencyChangedContext.valueChanged);
			}
		}
	}
}
