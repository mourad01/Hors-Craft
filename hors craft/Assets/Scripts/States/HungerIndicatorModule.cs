// DecompilerFi decompiler from Assembly-CSharp.dll class: States.HungerIndicatorModule
using System.Collections.Generic;
using UnityEngine.UI;

namespace States
{
	public class HungerIndicatorModule : GameplayModule
	{
		public Image filledImage;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.HUNGER
		};

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			base.Update();
			if (changedFacts.Contains(Fact.HUNGER))
			{
				HungerContext lastItem = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HungerContext>(Fact.HUNGER).GetLastItem();
				if (lastItem != null)
				{
					SetHungerProgress(lastItem.hunger);
				}
				filledImage.transform.parent.parent.gameObject.SetActive(lastItem != null);
			}
		}

		private void SetHungerProgress(float progress)
		{
			filledImage.fillAmount = progress;
		}
	}
}
