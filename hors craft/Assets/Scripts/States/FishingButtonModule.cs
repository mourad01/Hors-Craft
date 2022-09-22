// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FishingButtonModule
using Common.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class FishingButtonModule : GameplayModule
	{
		public Button fishingButton;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.NEARBY_WATER
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			NearbyWaterContext nearbyWaterContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<NearbyWaterContext>(Fact.NEARBY_WATER).FirstOrDefault();
			if (nearbyWaterContext != null)
			{
				SetListenerToButton(fishingButton, nearbyWaterContext.OnFishingAction);
			}
			bool flag = true;
			if (Manager.Contains<FishingManager>())
			{
				flag = Manager.Get<FishingManager>().isTutorialFinished;
			}
			fishingButton.gameObject.SetActive(nearbyWaterContext != null && flag);
		}
	}
}
