// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.EquipShieldMesh
namespace ItemVInventory
{
	public class EquipShieldMesh : EquipMesh
	{
		public override bool CanEquip()
		{
			return true;
		}

		public override void OnLevelChanged(int newLevel, int oldLevel)
		{
			SetPrefab(FindMeshIndex(newLevel));
		}

		protected override void TryGetPrefabHolder(int level)
		{
			prefabHolder = PlayerGraphic.GetControlledPlayerInstance().leftArmLower.transform;
		}
	}
}
