// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TamePanelModule
using GameUI;
using System.Collections.Generic;
using System.Linq;

namespace States
{
	public class TamePanelModule : GameplayModule
	{
		public TamePanelController panelController;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.TAME_PANEL_CONFIG,
			Fact.IN_FRONT_OF_TAMEABLE
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (changedFacts.Contains(Fact.TAME_PANEL_CONFIG))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<TamePanelContext>(Fact.TAME_PANEL_CONFIG).FirstOrDefault()?.setPanel(panelController);
			}
			if (changedFacts.Contains(Fact.IN_FRONT_OF_TAMEABLE))
			{
				if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_FRONT_OF_LOVED_ONE) || MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_FRONT_OF_LOVABLE))
				{
					panelController.gameObject.SetActive(value: false);
				}
				else
				{
					panelController.gameObject.SetActive(MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_FRONT_OF_TAMEABLE));
				}
			}
		}
	}
}
