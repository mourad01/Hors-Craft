// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GrenadeIndicatorModule
using GameUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class GrenadeIndicatorModule : GameplayModule
	{
		public GameObject grenadeIndicator;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.GRENADE_INCOMING
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> facts)
		{
			GrenadeIncomingContext grenadeIncomingContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<GrenadeIncomingContext>(Fact.GRENADE_INCOMING).FirstOrDefault();
			if (grenadeIncomingContext != null)
			{
				SpawnIndicator(grenadeIncomingContext);
			}
		}

		private void SpawnIndicator(GrenadeIncomingContext context)
		{
			GameObject gameObject = Object.Instantiate(grenadeIndicator);
			gameObject.transform.SetParent(base.transform, worldPositionStays: true);
			GrenadeIndicatorController component = gameObject.GetComponent<GrenadeIndicatorController>();
			component.Initialize(context.pivot, context.grenade, context.grenade.GetComponent<Grenade>().radius);
		}
	}
}
