// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FPSModule
using System.Collections.Generic;

namespace States
{
	public class FPSModule : GameplayModule
	{
		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.FPS_ENABLED,
			Fact.DEV_ENABLED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			base.gameObject.SetActive(MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.DEV_ENABLED) && MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.FPS_ENABLED));
		}
	}
}
