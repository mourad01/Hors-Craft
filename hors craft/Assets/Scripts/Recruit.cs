// DecompilerFi decompiler from Assembly-CSharp.dll class: Recruit
using Common.Utils;
using UnityEngine;

public class Recruit : MonoBehaviour
{
	private void Awake()
	{
		GameObject gameObject = base.gameObject;
		GameObject original = SurvivalContextsBroadcaster.instance.GetContext<SoldiersRecruiterContext>().friendlySoldiersPrefabs.Random();
		GameObject gameObject2 = UnityEngine.Object.Instantiate(original);
		ShootingEnemy component = gameObject2.GetComponent<ShootingEnemy>();
		Soldier soldier = gameObject2.AddComponent<Soldier>();
		CopyComponents.CopyFields(component, soldier);
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.rotation = gameObject.transform.rotation;
		UnityEngine.Object.Destroy(gameObject2.GetComponent<CrosshairTarget>());
		UnityEngine.Object.DestroyImmediate(component);
		UnityEngine.Object.DestroyImmediate(gameObject);
		UnityEngine.Debug.Log("Soldier " + soldier.gameObject.name + " recruited. Fight well and be brave.");
	}
}
