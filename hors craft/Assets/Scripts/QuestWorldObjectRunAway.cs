// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestWorldObjectRunAway
using QuestSystems.Adventure;
using UnityEngine;

public class QuestWorldObjectRunAway : QuestWorldObjectBase
{
	public MobNavigator mobNavigator;

	public override void OnQuestUpdate(EQuestState currentState)
	{
		switch (currentState)
		{
		case EQuestState.disabled:
			ChangeMarkerState(state: false);
			break;
		case EQuestState.done:
		case EQuestState.afterDone:
			ChangeMarkerState(state: false);
			mobNavigator.SetDestination(base.transform.position + Vector3.forward * 10f);
			break;
		}
	}
}
