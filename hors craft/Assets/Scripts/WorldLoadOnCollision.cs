// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldLoadOnCollision
using Common.Managers;
using UnityEngine;

public class WorldLoadOnCollision : MonoBehaviour
{
	public int loadLevel;

	private void OnTriggerEnter(Collider other)
	{
		TestWorldChange();
	}

	[ContextMenu("test world loading")]
	protected void TestWorldChange()
	{
		Manager.Get<StateMachineManager>().PushState<ChooseWorldState>();
	}
}
