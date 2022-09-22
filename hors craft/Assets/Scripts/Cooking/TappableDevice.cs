// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.TappableDevice
using UnityEngine;

namespace Cooking
{
	public abstract class TappableDevice : Device, ITapObject
	{
		public Transform deviceWorkerPlace;

		public abstract void OnInteraction(Worker worker);

		public abstract bool CanTap(Worker worker);

		public GameObject GetGameObject()
		{
			return base.gameObject;
		}

		public virtual Vector3 GetWorkerPlace()
		{
			return deviceWorkerPlace.position;
		}
	}
}
