// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.AbstractUpgradeBehaviour
using UnityEngine;

namespace ItemVInventory
{
	public abstract class AbstractUpgradeBehaviour : MonoBehaviour
	{
		public abstract bool CanUpgrade(int level);

		public abstract UpgradeRequirements UpgradeRequirements(int level, ItemDefinition itemDefinition);

		public abstract void OnUpgrade();
	}
}
