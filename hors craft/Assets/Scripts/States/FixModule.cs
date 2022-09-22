// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FixModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class FixModule : GameplayModule
	{
		public Button fixButton;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.IN_FRONT_OF_FIXABLE
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			InteractiveObjectContext interactiveObjectContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<InteractiveObjectContext>(Fact.IN_FRONT_OF_FIXABLE).FirstOrDefault();
			if (interactiveObjectContext != null)
			{
				SetListenerToButton(fixButton, interactiveObjectContext.useAction);
			}
			fixButton.gameObject.SetActive(interactiveObjectContext != null);
		}
	}
}
