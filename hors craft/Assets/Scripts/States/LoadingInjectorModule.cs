// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadingInjectorModule
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class LoadingInjectorModule : GameplayModule
	{
		public GameObject loadingInjector;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.LOADING
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			loadingInjector.SetActive(MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.LOADING));
		}
	}
}
