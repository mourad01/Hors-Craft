// DecompilerFi decompiler from Assembly-CSharp.dll class: TutorialManager
using Common.Managers;
using System.Collections.Generic;
using System.Linq;

public class TutorialManager : Manager
{
	public List<Tutorial> tutorialsToShow;

	public List<TutorialRequirement> commonTutorialRequirements;

	public override void Init()
	{
		foreach (Tutorial item in tutorialsToShow)
		{
			item.Init();
		}
		foreach (TutorialRequirement commonTutorialRequirement in commonTutorialRequirements)
		{
			commonTutorialRequirement.Init();
		}
	}

	public void TryToShowTutorial(Tutorial tutorialToShow)
	{
		if (AreCommonRequirementsFulfilled())
		{
			tutorialToShow.ShowTutorial();
		}
	}

	public void ForceShowTutorial(string tutorialName)
	{
		Tutorial tutorial = tutorialsToShow.FirstOrDefault((Tutorial t) => t.name.Equals(tutorialName));
		if (tutorial != null)
		{
			tutorial.ShowTutorial();
		}
	}

	private bool AreCommonRequirementsFulfilled()
	{
		if (commonTutorialRequirements == null)
		{
			return true;
		}
		return commonTutorialRequirements.All((TutorialRequirement r) => r.IsFulfilled());
	}
}
