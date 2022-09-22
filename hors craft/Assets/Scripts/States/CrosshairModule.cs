// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CrosshairModule
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace States
{
	public class CrosshairModule : GameplayModule
	{
		protected override Fact[] listenedFacts
		{
			[CompilerGenerated]
			get
			{
				return new Fact[1]
				{
					Fact.MCPE_STEERING
				};
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			bool flag = !MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING);
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
		}
	}
}
