// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FishingTutorialModule
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class FishingTutorialModule : GameplayModule
	{
		public GameObject tutorialContent;

		public GameObject tutorialState1;

		public GameObject tutorialState2;

		public Button moveNext1;

		public Button moveNext2;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.FISHING_ENABLED,
			Fact.TUTORIALS_N_POPUPS_DISABLED
		};

		public override void Init()
		{
			base.Init();
			moveNext1.onClick.AddListener(delegate
			{
				OnTutorialState1();
			});
			moveNext2.onClick.AddListener(delegate
			{
				OnTutorialState2();
			});
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TUTORIALS_N_POPUPS_DISABLED))
			{
				tutorialContent.SetActive(value: false);
			}
			else if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.FISHING_ENABLED))
			{
				if (!Manager.Get<FishingManager>().isTutorialFinished)
				{
					tutorialContent.SetActive(value: true);
					tutorialState1.SetActive(value: true);
					MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_TUTORIAL);
					return;
				}
				if (!Manager.Get<FishingManager>().isTutorialPopupFinished)
				{
					Manager.Get<StateMachineManager>().PushState<AfterTutorialPopUpState>();
				}
				tutorialContent.SetActive(value: false);
			}
			else
			{
				tutorialContent.SetActive(value: false);
			}
		}

		private void OnTutorialState1()
		{
			tutorialState1.SetActive(value: false);
			tutorialState2.SetActive(value: true);
		}

		private void OnTutorialState2()
		{
			tutorialState2.SetActive(value: false);
			Manager.Get<StateMachineManager>().PushState<FishingState>();
			tutorialContent.GetComponent<Animator>().SetTrigger("TurnOff");
			tutorialContent.SetActive(value: false);
		}
	}
}
