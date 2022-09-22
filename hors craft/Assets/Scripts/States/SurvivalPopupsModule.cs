// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SurvivalPopupsModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class SurvivalPopupsModule : GameplayModule
	{
		public GameObject explorePopup;

		public GameObject fightPopup;

		private bool previouslyWasCombat;

		private static bool hasShownExplorePopup;

		private static bool hasShownFightPopup;

		private float popupDisableTime;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.SURVIVAL_DEFAULT_POPUPS_ALLOWED,
			Fact.SURVIVAL_PHASE
		};

		private void Awake()
		{
		}

		public override void Init()
		{
			base.Init();
			SurvivalPhaseContext survivalPhaseContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE).FirstOrDefault();
			if (survivalPhaseContext != null)
			{
				previouslyWasCombat = !survivalPhaseContext.isCombat;
				CheckPopups();
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			CheckPopups();
		}

		private void CheckPopups()
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.SURVIVAL_DEFAULT_POPUPS_ALLOWED))
			{
				bool isCombat = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE).FirstOrDefault().isCombat;
				if (isCombat && !previouslyWasCombat && !hasShownFightPopup)
				{
					ShowFightPopup();
					previouslyWasCombat = true;
				}
				UnityEngine.Debug.LogError(!isCombat + " " + previouslyWasCombat);
				if (!isCombat && previouslyWasCombat && !hasShownExplorePopup)
				{
					ShowExplorePopup();
					previouslyWasCombat = false;
				}
			}
		}

		private void ShowExplorePopup()
		{
			explorePopup.SetActive(value: true);
			popupDisableTime = Time.time + ((!hasShownExplorePopup) ? 3f : 1.5f);
			hasShownExplorePopup = true;
			hasShownFightPopup = false;
		}

		private void ShowFightPopup()
		{
			fightPopup.SetActive(value: true);
			popupDisableTime = Time.time + ((!hasShownFightPopup) ? 3f : 1.5f);
			hasShownFightPopup = true;
			hasShownExplorePopup = false;
		}

		protected override void Update()
		{
			base.Update();
			if (Time.time > popupDisableTime)
			{
				explorePopup.SetActive(value: false);
				fightPopup.SetActive(value: false);
			}
		}
	}
}
