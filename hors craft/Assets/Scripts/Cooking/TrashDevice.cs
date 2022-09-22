// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.TrashDevice
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class TrashDevice : TappableDevice
	{
		public override bool CanTap(Worker worker)
		{
			return worker.heldItems.Count > 0;
		}

		public override void OnInteraction(Worker worker)
		{
			IPickable pickable = null;
			if (base.workController.recipesList.burntDish != null)
			{
				pickable = worker.heldProducts.FirstOrDefault((Product p) => p.GetKey() == base.workController.recipesList.burntDish.GetComponent<Product>().GetKey());
			}
			if (pickable == null)
			{
				pickable = worker.heldItems[0];
			}
			worker.DisposeHeldItem(pickable);
			UnityEngine.Object.Destroy(pickable.GetGameObject());
		}

		protected override void SetUpgradeValues(UpgradeEffect effect, float value)
		{
		}

		public override bool Unlocked()
		{
			return true;
		}
	}
}
