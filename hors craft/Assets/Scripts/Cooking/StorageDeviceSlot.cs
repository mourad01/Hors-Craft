// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.StorageDeviceSlot
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class StorageDeviceSlot : MonoBehaviour, ITapObject
	{
		public Transform pivot;

		public Transform workerPlace;

		protected WorkController workController;

		public IPickable placedItem
		{
			get;
			protected set;
		}

		public StorageDevice baseDevice
		{
			get;
			protected set;
		}

		private void Awake()
		{
			baseDevice = GetComponentInParent<StorageDevice>();
			workController = GetComponentInParent<WorkController>();
		}

		public GameObject GetGameObject()
		{
			return base.gameObject;
		}

		public Vector3 GetWorkerPlace()
		{
			return (!(workerPlace == null)) ? workerPlace.position : baseDevice.workerPlace.position;
		}

		public virtual void Reset()
		{
			if (placedItem != null)
			{
				UnityEngine.Object.Destroy(placedItem.GetGameObject());
			}
			placedItem = null;
		}

		public virtual void OnInteraction(Worker worker)
		{
			if (CanPlaceSomething(worker))
			{
				placedItem = worker.heldItems.First();
				placedItem.GetGameObject().transform.SetParent(pivot);
				placedItem.GetGameObject().transform.localPosition = Vector3.zero;
				worker.DisposeHeldItem(placedItem);
			}
			else if (CanUseOnSomething(worker))
			{
				IUsable usableProduct = placedItem as IUsable;
				Product product = worker.heldProducts.First((Product p) => workController.recipesList.CanUse(p, usableProduct));
				worker.ReplaceProduct(baseDevice, product, usableProduct);
				UnityEngine.Object.Destroy(placedItem.GetGameObject());
				placedItem = null;
			}
			else if (CanPickSomething(worker))
			{
				worker.PickUpProduct(placedItem);
				placedItem = null;
			}
		}

		public bool CanTap(Worker worker)
		{
			if (!base.gameObject.activeSelf)
			{
				return false;
			}
			return CanPlaceSomething(worker) || CanPickSomething(worker) || CanUseOnSomething(worker);
		}

		protected virtual bool CanPlaceSomething(Worker worker)
		{
			return placedItem == null && worker.heldItems.Count > 0;
		}

		protected virtual bool CanPickSomething(Worker worker)
		{
			return placedItem != null && worker.CanPickSomething();
		}

		protected virtual bool CanUseOnSomething(Worker worker)
		{
			IUsable usableProduct = placedItem as IUsable;
			return placedItem != null && usableProduct != null && worker.heldProducts.Any((Product p) => workController.recipesList.CanUse(p, usableProduct));
		}
	}
}
