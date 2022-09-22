// DecompilerFi decompiler from Assembly-CSharp.dll class: BumperTicketOffice
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class BumperTicketOffice : InteractiveObject, IGameCallbacksListener
{
	[Serializable]
	public class SpawnPointsContainer
	{
		public Vector3 playerSpawnPoint;

		public List<Vector3> resourcesSpawnPoints;

		public List<Vector3> enemiesSpawnPoint;
	}

	protected class BumpersData
	{
		public HoverCar bumperCar;

		public EnemyBumperCar[] enemies;
	}

	private const float MAX_BUMPER_TIME = 50f;

	private const float SPAWN_RESOURCES_TIME = 1f;

	private const string KEY = "ticketOffice.";

	public Action<float> onBumperTime;

	public Action<bool> onBumperUseChange;

	private float usingTimer;

	private float resourcesTimer;

	private bool isInUse;

	[SerializeField]
	private GameObject playerPrefab;

	[SerializeField]
	private GameObject enemyPrefab;

	private static SpawnPointsContainer spawnPoints;

	private Index index;

	private static Dictionary<Index, BumpersData> office2Data = new Dictionary<Index, BumpersData>();

	private HoverCar bumperCar;

	private EnemyBumperCar[] enemies;

	private List<Vector3> resourcesPoints
	{
		get
		{
			if (spawnPoints == null)
			{
				return null;
			}
			return spawnPoints.resourcesSpawnPoints;
		}
		set
		{
			if (spawnPoints != null)
			{
				spawnPoints.resourcesSpawnPoints = value;
			}
		}
	}

	private List<Vector3> enemiesPoints
	{
		get
		{
			if (spawnPoints == null)
			{
				List<Vector3> list = new List<Vector3>();
				list.Add(base.transform.position);
				return list;
			}
			return spawnPoints.enemiesSpawnPoint;
		}
		set
		{
			if (spawnPoints != null)
			{
				spawnPoints.enemiesSpawnPoint = value;
			}
		}
	}

	private Vector3 playerPoint
	{
		get
		{
			if (spawnPoints == null)
			{
				return Vector3.zero;
			}
			return spawnPoints.playerSpawnPoint;
		}
		set
		{
			if (spawnPoints != null)
			{
				spawnPoints.playerSpawnPoint = value;
			}
		}
	}

	private string prefsKey
	{
		get
		{
			Vector3 position = base.transform.position;
			string str = position.x.ToString("0.00");
			Vector3 position2 = base.transform.position;
			string str2 = position2.z.ToString("0.00");
			Vector3 position3 = base.transform.position;
			string str3 = position3.z.ToString("0.00");
			return "ticketOffice." + str + str2 + str3;
		}
	}

	protected override void Awake()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		index = new Index(base.transform.position);
		if (WorldPlayerPrefs.get.HasString(prefsKey))
		{
			string @string = WorldPlayerPrefs.get.GetString(prefsKey);
			spawnPoints = JSONHelper.Deserialize<SpawnPointsContainer>(@string);
			if (!office2Data.ContainsKey(index))
			{
				SpawnBumpers();
			}
			else
			{
				bumperCar = office2Data[index].bumperCar;
				enemies = office2Data[index].enemies;
			}
		}
		if (spawnPoints == null)
		{
			spawnPoints = new SpawnPointsContainer();
			spawnPoints.playerSpawnPoint = base.transform.TransformPoint(1f, 0f, 0f);
			spawnPoints.resourcesSpawnPoints = new List<Vector3>
			{
				base.transform.TransformPoint(0f, 0f, 1f)
			};
			spawnPoints.enemiesSpawnPoint = new List<Vector3>
			{
				base.transform.TransformPoint(-1f, 0f, 0f)
			};
		}
	}

	public override void OnUse()
	{
		base.OnUse();
		if (!(bumperCar == null))
		{
			TicketsManager.TakeEntranceFeeIfPossible(StartMinigame);
		}
	}

	private void StartMinigame()
	{
		VehicleHoverAction hoverAction = PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<CameraEventsSender>().GetHoverAction<VehicleHoverAction>();
		if (hoverAction != null)
		{
			EnableCars(enable: true);
			hoverAction.ForceEnterVehicle(bumperCar.gameObject);
			isInUse = true;
			usingTimer = 50f;
			resourcesTimer = 1f;
		}
	}

	private void OnDestroy()
	{
		if (MonoBehaviourSingleton<ManagersContainer>.get != null && Manager.Contains<GameCallbacksManager>())
		{
			Manager.Get<GameCallbacksManager>().UnregisterObject(this);
		}
	}

	private void Update()
	{
		if (isInUse)
		{
			usingTimer -= Time.deltaTime;
			resourcesTimer -= Time.deltaTime;
			if (usingTimer <= 0f)
			{
				UnityEngine.Object.FindObjectOfType<CameraEventsSender>().GetHoverAction<VehicleHoverAction>()?.OnVehicleUse();
				EnableCars(enable: false);
			}
			if (resourcesTimer <= 0f)
			{
				SpawnResource();
				resourcesTimer = 1f;
			}
			if (!bumperCar.IsInUse)
			{
				isInUse = false;
				BumperUseChange();
				EnableCars(enable: false);
			}
			else if (onBumperTime != null)
			{
				onBumperTime(usingTimer);
			}
		}
	}

	private void BumperUseChange()
	{
		if (onBumperUseChange != null)
		{
			onBumperUseChange(isInUse);
		}
	}

	private void SpawnResource()
	{
		Vector3 vector = bumperCar.transform.position;
		if (resourcesPoints.Count > 0)
		{
			vector = resourcesPoints[UnityEngine.Random.Range(0, resourcesPoints.Count)];
		}
		Manager.Get<CraftingManager>().SpawnRandomResource(vector);
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].target = vector;
		}
	}

	private void SpawnBumpers()
	{
		StartCoroutine(SpawnWithDelay());
	}

	private IEnumerator SpawnWithDelay()
	{
		bool isBlockBelowCar;
		do
		{
			isBlockBelowCar = (Engine.VoxelRaycast(enemiesPoints[0], Vector3.down, 16f, ignoreTransparent: true) != null);
			yield return null;
		}
		while (!isBlockBelowCar);
		GameObject playerBumper = UnityEngine.Object.Instantiate(playerPrefab);
		playerBumper.tag = "Untagged";
		playerBumper.transform.position = playerPoint;
		bumperCar = playerBumper.GetComponentInChildren<HoverCar>();
		playerBumper.GetComponentInChildren<BumperCar>().isSpawningResources = false;
		enemies = new EnemyBumperCar[enemiesPoints.Count];
		for (int i = 0; i < enemiesPoints.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(enemyPrefab);
			playerBumper.tag = "Untagged";
			gameObject.transform.position = enemiesPoints[i];
			enemies[i] = gameObject.GetComponentInChildren<EnemyBumperCar>();
		}
		office2Data.Add(index, new BumpersData
		{
			bumperCar = bumperCar,
			enemies = enemies
		});
		EnableCars(enable: false);
	}

	private void EnableCars(bool enable)
	{
		bumperCar.enabled = enable;
		bumperCar.GetComponentInParent<Rigidbody>().isKinematic = !enable;
		EnemyBumperCar[] array = enemies;
		foreach (EnemyBumperCar enemyBumperCar in array)
		{
			enemyBumperCar.GetComponent<Rigidbody>().isKinematic = !enable;
			enemyBumperCar.enabled = enable;
		}
	}

	private void DespawnBumpers()
	{
		EnemyBumperCar[] array = enemies;
		foreach (EnemyBumperCar enemyBumperCar in array)
		{
			if ((bool)enemyBumperCar)
			{
				UnityEngine.Object.Destroy(enemyBumperCar.gameObject);
			}
		}
		UnityEngine.Object.Destroy(bumperCar.gameObject);
	}

	private string GetJSON()
	{
		UnityEngine.Debug.Log("Geting bumpers json");
		return JSONHelper.ToJSON(spawnPoints);
	}

	public void OnGameSavedFrequent()
	{
		if (Application.isEditor)
		{
			WorldPlayerPrefs.get.SetString(prefsKey, GetJSON());
		}
	}

	public void OnGameSavedInfrequent()
	{
		if (Application.isEditor)
		{
			WorldPlayerPrefs.get.SetString(prefsKey, GetJSON());
		}
	}

	public void OnGameplayStarted()
	{
	}

	public void OnGameplayRestarted()
	{
	}
}
