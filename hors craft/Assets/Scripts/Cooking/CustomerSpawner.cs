// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.CustomerSpawner
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cooking
{
	public class CustomerSpawner : MonoBehaviour
	{
		public GameObject[] customerPrefabs;

		public Transform spawnPoint;

		private WorkController workController;

		private WorkPlace workPlace;

		private Wave wave;

		private bool isSpawning;

		private int customersSpawned;

		private float waveTimer;

		public void Init(WorkController workController)
		{
			this.workController = workController;
			workPlace = workController.workPlace;
		}

		public void InitWave(Wave wave)
		{
			this.wave = wave;
			isSpawning = true;
			waveTimer = 0f;
			customersSpawned = 0;
		}

		private void Update()
		{
			if (workController.cookingGameplay == null || workController.cookingGameplay.isTutorialOrMinigame || !isSpawning || !workPlace.AnySpotFree())
			{
				return;
			}
			waveTimer += Time.deltaTime;
			if (customersSpawned < wave.customers.Count)
			{
				if (wave.positiveServedCustomers == wave.customers.Count - 1)
				{
					StartCoroutine(SpawnCustomer(special: true));
					isSpawning = false;
				}
				else if ((customersSpawned != wave.customers.Count - 1 || wave.negativeServedCustomers != 0) && waveTimer > wave.customers[customersSpawned].spawnTime)
				{
					StartCoroutine(SpawnCustomer(special: false));
				}
			}
			else
			{
				EndWave();
			}
		}

		private IEnumerator SpawnCustomer(bool special)
		{
			CustomerConfig customerConfig = wave.customers[customersSpawned++];
			if (special)
			{
				yield return new WaitForSeconds(1f);
			}
			WorkPlace.CustomerPlace customerTargetPlace = workPlace.GetFreeCustomerPlace();
			GameObject spawned = UnityEngine.Object.Instantiate(customerPrefabs[Random.Range(0, customerPrefabs.Length)]);
			spawned.transform.position = spawnPoint.position;
			spawned.transform.rotation = spawnPoint.rotation;
			Customer customer = spawned.GetComponent<Customer>();
			customerTargetPlace.isFree = false;
			customerTargetPlace.customer = customer;
			customer.specialCustomer = special;
			customer.Init(customerTargetPlace, wave, customerConfig.products, customerConfig.waitTime);
			SceneManager.MoveGameObjectToScene(scene: workController.gameObject.scene, go: customer.gameObject);
		}

		public void EndWave()
		{
			isSpawning = false;
			wave = null;
		}
	}
}
