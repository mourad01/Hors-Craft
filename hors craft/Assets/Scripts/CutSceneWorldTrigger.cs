// DecompilerFi decompiler from Assembly-CSharp.dll class: CutSceneWorldTrigger
using UnityEngine;

public class CutSceneWorldTrigger : MonoBehaviour
{
	public CutScenePlayerBase cutScenePlayer;

	public bool called;

	private void OnTriggerEnter(Collider other)
	{
		if (!called && !(other == null) && !(cutScenePlayer == null))
		{
			if (other.tag.Equals("Player"))
			{
				cutScenePlayer.OnUse();
			}
			called = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		called = false;
	}
}
