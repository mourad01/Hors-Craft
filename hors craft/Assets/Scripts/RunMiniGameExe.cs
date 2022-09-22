// DecompilerFi decompiler from Assembly-CSharp.dll class: RunMiniGameExe
using Common.Managers;
using Gameplay.Minigames;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Run minigame Exec")]
public class RunMiniGameExe : InitialPopupExecution
{
	public StatsManager.MinigameType minigameType = StatsManager.MinigameType.WALKWAY;

	public GameObject walkWayPrefab;

	public override void Show()
	{
		StatsManager.MinigameType minigameType = this.minigameType;
		if (minigameType == StatsManager.MinigameType.WALKWAY)
		{
			RunWalkWay();
		}
		else
		{
			UnityEngine.Debug.Log(" Minigame Not suported");
		}
	}

	private void RunWalkWay()
	{
		GameObject gameObject = Object.Instantiate(walkWayPrefab);
		Walkway component = gameObject.GetComponent<Walkway>();
		if (component == null)
		{
			UnityEngine.Debug.LogError("No Walkway script added to spawned walkway prefab. Add it to the prefab.");
		}
		else
		{
			component.StartCoroutine(WaitAndRun(component));
		}
	}

	private IEnumerator WaitAndRun(Walkway walkway)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		walkway.OnUse();
	}
}
