// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestWorldObjectChangePosition
using QuestSystems.Adventure;
using UnityEngine;

public class QuestWorldObjectChangePosition : QuestWorldObjectBase
{
	public bool changePosition;

	public Transform characterRoot;

	public Vector3 newPosition;

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
			if (changePosition)
			{
				characterRoot.localPosition = newPosition;
			}
			break;
		}
	}
}
