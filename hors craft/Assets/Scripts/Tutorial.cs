// DecompilerFi decompiler from Assembly-CSharp.dll class: Tutorial
using Common.Gameplay;
using Common.Managers;
using States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Tutorial")]
public class Tutorial : ScriptableObject, IFactChangedListener
{
	private const string TUTORIAL_FINISHED_KEY = "TutorialManager.WasFinished.";

	[HideInInspector]
	public bool wasShown;

	public List<TutorialRequirement> tutorialRequirements;

	public List<Reward> tutorialRewards;

	public GameObject tutorialPrefab;

	private Fact[] listenedFacts;

	public bool wasFinished
	{
		get
		{
			return PlayerPrefs.GetInt("TutorialManager.WasFinished." + base.name, 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("TutorialManager.WasFinished." + base.name, value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public string GetTutorialFinishedKey()
	{
		return "TutorialManager.WasFinished." + base.name;
	}

	public void Init()
	{
		wasShown = false;
		if (!wasFinished)
		{
			List<Fact> list = new List<Fact>();
			foreach (TutorialRequirement tutorialRequirement in tutorialRequirements)
			{
				tutorialRequirement.Init();
				list.AddRange(tutorialRequirement.factsToSubscribeTo);
			}
			listenedFacts = list.ToArray();
			MonoBehaviourSingleton<GameplayFacts>.get.RegisterFactChangedListener(this, listenedFacts);
		}
	}

	public void CheckAndTryToShow()
	{
		if (!wasFinished && !wasShown && AreRequirementsFulfilled())
		{
			Manager.Get<TutorialManager>().TryToShowTutorial(this);
		}
	}

	private bool AreRequirementsFulfilled()
	{
		if (tutorialRequirements == null)
		{
			return true;
		}
		return tutorialRequirements.All((TutorialRequirement t) => t.IsFulfilled());
	}

	public void ShowTutorial()
	{
		wasShown = true;
		GameObject gameObject = UnityEngine.Object.Instantiate(tutorialPrefab);
		TutorialGenerator component = gameObject.GetComponent<TutorialGenerator>();
		component.onTutorialFinished.AddListener(CompleteTutorial);
		MonoBehaviourSingleton<GameplayFacts>.get.UnregisterFactChangedListener(this, listenedFacts);
	}

	public void CompleteTutorial()
	{
		wasFinished = true;
		if (tutorialRewards != null)
		{
			for (int i = 0; i < tutorialRewards.Count; i++)
			{
				tutorialRewards[i].ClaimReward();
			}
		}
	}

	public void OnFactsChanged(HashSet<Fact> facts)
	{
		CheckAndTryToShow();
	}
}
