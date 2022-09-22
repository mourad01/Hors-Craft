// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.ITapObject
using UnityEngine;

namespace Cooking
{
	public interface ITapObject
	{
		GameObject GetGameObject();

		Vector3 GetWorkerPlace();

		void OnInteraction(Worker worker);

		bool CanTap(Worker worker);
	}
}
