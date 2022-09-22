// DecompilerFi decompiler from Assembly-CSharp.dll class: SpawnGameObjectExe
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Initial Popups/Spawn Game Object")]
public class SpawnGameObjectExe : InitialPopupExecution
{
	public GameObject go;

	public override void Show()
	{
		Object.Instantiate(go);
	}
}
