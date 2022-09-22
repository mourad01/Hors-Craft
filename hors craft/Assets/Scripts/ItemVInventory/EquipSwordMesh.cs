// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.EquipSwordMesh
using com.ootii.Cameras;
using UnityEngine;

namespace ItemVInventory
{
	public class EquipSwordMesh : EquipMesh
	{
		protected override void Start()
		{
			SurvivalContextsBroadcaster.instance.Unregister<NewWeaponEquipedContext>(WeaponSwordEquiped);
			SurvivalContextsBroadcaster.instance.Register<NewWeaponEquipedContext>(WeaponSwordEquiped);
		}

		protected void OnDestroy()
		{
			SurvivalContextsBroadcaster.instance.Unregister<NewWeaponEquipedContext>(WeaponSwordEquiped);
		}

		public override bool CanEquip()
		{
			return true;
		}

		public override void OnLevelChanged(int newLevel, int oldLevel)
		{
			Equip(newLevel);
		}

		protected override void TryGetPrefabHolder(int level)
		{
			if (PlayerGraphic.GetControlledPlayerInstance().rightArm.GetComponent<Renderer>().enabled)
			{
				prefabHolder = PlayerGraphic.GetControlledPlayerInstance().rightArmLower.transform;
				SetPrefab(FindMeshIndex(level));
			}
			GameObject gameObject = CameraController.instance.gameObject;
			SwordHolder componentInChildren = gameObject.GetComponentInChildren<SwordHolder>();
			prefabHolder = ((!(componentInChildren != null)) ? null : componentInChildren.transform);
		}

		protected override void SetPrefab(int index)
		{
			base.SetPrefab(index);
			prefabHolder = null;
		}

		public void WeaponSwordEquiped()
		{
			if (!(base.gameObject == null))
			{
				ItemDefinition componentInParent = GetComponentInParent<ItemDefinition>();
				if (!(componentInParent == null))
				{
					int level = componentInParent.level;
					Equip(level);
				}
			}
		}
	}
}
