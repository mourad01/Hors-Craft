// DecompilerFi decompiler from Assembly-CSharp.dll class: RelationshipStageSetup
using Common.Managers;
using Gameplay;
using UnityEngine;

[CreateAssetMenu(fileName = "RelationshipStage", menuName = "ScriptableObjects/RelationshipStage")]
public class RelationshipStageSetup : ScriptableObject
{
	public string cutsceneToShow;

	public void StartRelationshipStage()
	{
		Manager.Get<SimpleCutsceneManager>().ShowCutscene(cutsceneToShow);
	}
}
