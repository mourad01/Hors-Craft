// DecompilerFi decompiler from Assembly-CSharp.dll class: SpawnHospitalTutorial
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Initial Popups/Spawn Hospital ")]
public class SpawnHospitalTutorial : SpawnGameObjectExe
{
	public float npcOffset = 2.5f;

	public Vector3 cameraOffset = Vector3.zero;

	public override void Show()
	{
		HospitalTutorial componentInChildren = go.GetComponentInChildren<HospitalTutorial>();
		componentInChildren.npcOffset = npcOffset;
		componentInChildren.cameraOffset = cameraOffset;
		base.Show();
	}
}
