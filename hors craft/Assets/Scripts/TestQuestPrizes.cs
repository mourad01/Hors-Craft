// DecompilerFi decompiler from Assembly-CSharp.dll class: TestQuestPrizes
using Common.Managers;
using UnityEngine;

public class TestQuestPrizes : MonoBehaviour
{
	public Quest[] quests;

	[Range(1f, 10000f)]
	public int expToGrant = 300;

	[ContextMenu("test granting quests ")]
	public void TestGrantingPrizes()
	{
		quests[0].specialPrizes.Grant(delegate
		{
			UnityEngine.Debug.Log("Test ended. Granted.");
		}, 1);
	}

	[ContextMenu("Text quest incrementation")]
	public void TestQuestIncrementation()
	{
		Manager.Get<QuestManager>().IncreaseQuestOfType(QuestType.WinCatWalkFlawlessy);
	}

	[ContextMenu("Text quest incrementation (expirience)")]
	public void TestQuestIncrementation2()
	{
		Manager.Get<ProgressManager>().IncreaseExperience(expToGrant);
	}
}
