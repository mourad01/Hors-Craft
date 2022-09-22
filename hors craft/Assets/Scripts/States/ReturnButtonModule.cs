// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ReturnButtonModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class ReturnButtonModule : GameplayModule
	{
		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.RETURN_BUTTON_ACTIVATED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.RETURN_BUTTON_ACTIVATED))
			{
				base.gameObject.SetActive(value: true);
				ReturnButtonContext returnButtonContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<ReturnButtonContext>(Fact.RETURN_BUTTON_ACTIVATED).FirstOrDefault();
				SetListenerToButton(GetComponentInChildren<Button>(), returnButtonContext.onReturnButton);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
