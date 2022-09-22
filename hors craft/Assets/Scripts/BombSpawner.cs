// DecompilerFi decompiler from Assembly-CSharp.dll class: BombSpawner
using Common.Managers;
using Gameplay;
using System.Collections;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject bombPrefab;

	[SerializeField]
	private float timeToStartSpawning = 20f;

	[SerializeField]
	private float timeBetweenSpawns = 5f;

	[SerializeField]
	private float relativeSpawnHeight = 20f;

	private SurvivalManager survivalManager;

	private PlayerController playerController;

	private float currentTimeToStartSpawning;

	private bool areBombsBeingSpawned;

	private void Start()
	{
		survivalManager = Manager.Get<SurvivalManager>();
		if (!survivalManager)
		{
			UnityEngine.Object.Destroy(this);
		}
		playerController = GetComponent<PlayerController>();
		if (!bombPrefab || !playerController)
		{
			UnityEngine.Object.Destroy(this);
		}
		currentTimeToStartSpawning = 0f;
	}

	private void Update()
	{
		if (areBombsBeingSpawned)
		{
			if (playerController.isInVehicle)
			{
				areBombsBeingSpawned = false;
				StopAllCoroutines();
			}
			return;
		}
		UpdateTimeToStartSpawning();
		if (currentTimeToStartSpawning > timeToStartSpawning)
		{
			StartCoroutine(SpawnBombs(timeBetweenSpawns));
			areBombsBeingSpawned = true;
		}
	}

	private void UpdateTimeToStartSpawning()
	{
		if (playerController.isInVehicle)
		{
			currentTimeToStartSpawning = 0f;
		}
		else
		{
			currentTimeToStartSpawning += Time.deltaTime;
		}
	}

	private IEnumerator SpawnBombs(float timeBetweenSpawns)
	{
		float currentTimeBetweenSpawns = timeBetweenSpawns;
		while (true)
		{
			currentTimeBetweenSpawns += Time.deltaTime;
			if (!playerController.isInVehicle && currentTimeBetweenSpawns > timeBetweenSpawns && survivalManager.IsCombatTime())
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(bombPrefab);
				Vector3 normalized = base.transform.forward.normalized;
				gameObject.transform.position = base.transform.position + normalized * 3f + Vector3.up * relativeSpawnHeight;
				currentTimeBetweenSpawns = 0f;
			}
			yield return null;
		}
	}
}
