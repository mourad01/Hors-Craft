// DecompilerFi decompiler from Assembly-CSharp.dll class: AnotherTutorialFinishedReq
using UnityEngine;

public class AnotherTutorialFinishedReq : TutorialRequirement
{
	public Tutorial tutorialToFinish;

	private string checkedKey;

	public override void Init()
	{
		checkedKey = tutorialToFinish.GetTutorialFinishedKey();
	}

	public override bool IsFulfilled()
	{
		return PlayerPrefs.GetInt(checkedKey, 0) == 1;
	}
}
