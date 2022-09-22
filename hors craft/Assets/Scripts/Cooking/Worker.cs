// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.Worker
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Cooking
{
	public class Worker : MonoBehaviour
	{
		public class QueueItem
		{
			public ITapObject tapObject;

			public GameObject spawnedMark;
		}

		private const int TAP_QUEUE_CAPACITY = 3;

		public GameObject queueMarkPrefab;

		public List<Transform> productParents;

		public Action<GameObject> onInteraction;

		public float baseSpeed = 4.5f;

		private WorkController _workController;

		private NavMeshAgent navigator;

		private Queue<QueueItem> tappedQueue = new Queue<QueueItem>();

		private QueueItem targetItem;

		private Animator animator;

		private bool visible = true;

		public List<IPickable> heldItems
		{
			get;
			private set;
		}

		public List<Product> heldProducts => (from p in heldItems
			where p is Product
			select p as Product).ToList();

		public bool busy => targetItem != null;

		private WorkController workController
		{
			get
			{
				if (_workController == null)
				{
					_workController = GetComponentInParent<WorkController>();
				}
				return _workController;
			}
		}

		private void Awake()
		{
			navigator = GetComponent<NavMeshAgent>();
			heldItems = new List<IPickable>();
			animator = GetComponentInChildren<Animator>();
		}

		public void Init()
		{
			float percentUpgradeEffectSummarized = workController.workPlace.GetPercentUpgradeEffectSummarized(Device.UpgradeEffect.WALKING_SPEED);
			navigator.speed = baseSpeed * percentUpgradeEffectSummarized;
		}

		public bool CanPickSomething()
		{
			return heldItems.Count < productParents.Count;
		}

		public void GiveProductToCustomer(Customer customer, Product product)
		{
			if (heldItems.Contains(product))
			{
				heldItems.Remove(product);
			}
		}

		public void PickUpProduct(IPickable pickable)
		{
			if (heldItems.Count < productParents.Count && pickable != null)
			{
				heldItems.Add(pickable);
				Transform parent = productParents.First((Transform p) => p.childCount == 0);
				pickable.GetGameObject().transform.SetParent(parent);
				pickable.GetGameObject().transform.localPosition = Vector3.zero;
				pickable.GetGameObject().transform.localRotation = Quaternion.identity;
				pickable.GetGameObject().SetActive(value: true);
			}
		}

		public void ReplaceProduct(Device device, Product product, IUsable usable = null)
		{
			if (usable == null)
			{
				usable = device;
			}
			Product product2 = device.SpawnNewProduct(product, usable);
			if (product2 != null)
			{
				int index = heldItems.IndexOf(product);
				heldItems[index] = product2;
				product2.transform.SetParent(product.transform.parent);
				product2.transform.localPosition = Vector3.zero;
				if (product2.usePrefabPositionAndRotation)
				{
					product2.transform.localRotation = Quaternion.identity;
				}
				UnityEngine.Object.Destroy(product.gameObject);
			}
		}

		public IPickable DisposeHeldItem(IPickable pickable)
		{
			int index = heldItems.IndexOf(pickable);
			heldItems.RemoveAt(index);
			return pickable;
		}

		public void ResetQueue()
		{
			foreach (QueueItem item in tappedQueue)
			{
				UnityEngine.Object.Destroy(item.spawnedMark);
			}
			if (targetItem != null && targetItem.spawnedMark != null)
			{
				UnityEngine.Object.Destroy(targetItem.spawnedMark);
			}
			tappedQueue = new Queue<QueueItem>();
			targetItem = null;
			navigator.ResetPath();
		}

		private void Update()
		{
			UpdateMovement();
			UpdateTapQueue();
			UpdateAnimator();
		}

		private void UpdateMovement()
		{
			if (busy)
			{
				UpdateMovingToTarget();
			}
			else
			{
				FindNewTarget();
			}
		}

		private void UpdateMovingToTarget()
		{
			if (HasArrivedAtDestination())
			{
				UseObject();
			}
		}

		private void UpdateAnimator()
		{
			if (visible)
			{
				animator.SetBool("walking", !HasArrivedAtDestination());
				animator.SetBool("left_hand_up", productParents[0].childCount > 0);
				animator.SetBool("right_hand_up", productParents[1].childCount > 0);
			}
		}

		private void FindNewTarget()
		{
			if (tappedQueue.Count > 0)
			{
				if (tappedQueue.Peek().tapObject.CanTap(this))
				{
					GoToObject(tappedQueue.Dequeue());
					return;
				}
				QueueItem queueItem = tappedQueue.Dequeue();
				UnityEngine.Object.Destroy(queueItem.spawnedMark);
			}
			else
			{
				GoToFloorTarget();
			}
		}

		private void UseObject()
		{
			targetItem.tapObject.OnInteraction(this);
			if (onInteraction != null)
			{
				onInteraction(targetItem.tapObject.GetGameObject());
			}
			onInteraction = null;
			UnityEngine.Object.Destroy(targetItem.spawnedMark);
			targetItem = null;
		}

		private void GoToObject(QueueItem queueItem)
		{
			targetItem = queueItem;
			navigator.SetDestination(targetItem.tapObject.GetWorkerPlace());
		}

		private void UpdateTapQueue()
		{
			if (tappedQueue.Count < 3)
			{
				CheckInput();
			}
		}

		private void CheckInput()
		{
			CurrentInputInfo inputInfo = PlayerInputInfo.inputInfo;
			if (inputInfo.phase == TouchPhase.Began)
			{
				TapPosition(inputInfo.position);
			}
		}

		private void TapPosition(Vector3 position)
		{
			ITapObject tapObject = DoRaycast(position);
			if (tapObject != null)
			{
				AddToQueue(tapObject);
			}
		}

		public void AddToQueue(ITapObject tapObject)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(queueMarkPrefab, Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
			workController.cookingGameplay.MoveToConnector(gameObject);
			gameObject.transform.SetAsLastSibling();
			gameObject.transform.position = new Vector3(99999f, 99999f, 0f);
			MonoBehaviour monoBehaviour = tapObject as MonoBehaviour;
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<QueueMark>().Init(monoBehaviour.gameObject, workController.mainCam);
			tappedQueue.Enqueue(new QueueItem
			{
				tapObject = tapObject,
				spawnedMark = gameObject
			});
		}

		private ITapObject DoRaycast(Vector3 position)
		{
			ITapObject result = null;
			Ray ray = workController.mainCam.ScreenPointToRay(position);
			if (Physics.Raycast(ray, out RaycastHit hitInfo))
			{
				result = hitInfo.collider.gameObject.GetComponentInParent<ITapObject>();
			}
			return result;
		}

		private bool HasArrivedAtDestination()
		{
			if (!navigator.pathPending && navigator.remainingDistance <= navigator.stoppingDistance && (!navigator.hasPath || navigator.velocity.sqrMagnitude == 0f))
			{
				return true;
			}
			return false;
		}

		private void GoToFloorTarget()
		{
			CurrentInputInfo inputInfo = PlayerInputInfo.inputInfo;
			if (inputInfo.phase == TouchPhase.Began)
			{
				Ray ray = workController.mainCam.ScreenPointToRay(inputInfo.position);
				if (Physics.Raycast(ray, out RaycastHit hitInfo))
				{
					navigator.SetDestination(hitInfo.point);
				}
			}
		}

		public void ShowGraphics(bool show)
		{
			visible = show;
			base.transform.Find("human").gameObject.SetActive(show);
		}
	}
}
