// DecompilerFi decompiler from Assembly-CSharp.dll class: Faction
using com.ootii.Cameras;
using Common.Managers;
using System;
using Uniblocks;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "State/Faction")]
public class Faction : ScriptableObject
{
	[SerializeField]
	private Vector3 playerPosition;

	[SerializeField]
	private CraftableList craftableList;

	[SerializeField]
	private CraftableList oppositeSideList;

	[SerializeField]
	private VehicleController vehicleController;

	public CraftableList GetCraftableList()
	{
		return craftableList;
	}

	public void SetFaction()
	{
		SetPlayerPosition();
		SetCraftableList();
		ReinitTankAndPlayer();
	}

	private void SetPlayerPosition()
	{
		if ((bool)CameraController.instance.MainCamera)
		{
			PlayerMovement component = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
			component.transform.position = playerPosition;
			PlayerPrefs.SetString("player.position", playerPosition.x + " " + playerPosition.y + " " + playerPosition.z);
		}
	}

	private void ReinitTankAndPlayer()
	{
		if ((bool)CameraController.instance.MainCamera)
		{
			PlayerMovement component = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
			component.SpawnAndMountVehicle(vehicleController);
		}
	}

	public void SetCraftableList()
	{
		CraftingManager craftingManager = Manager.Get<CraftingManager>();
		string name = Manager.Get<SavedWorldManager>().GetCurrentWorld().name;
		PlayerPrefs.SetString(name + ".faction", base.name);
		craftingManager.LoadFactionAdditionalCraftableList();
		craftingManager.CreateList();
	}

	public void LoadCraftableList(CraftableList craftableList)
	{
		if (craftableList != null)
		{
			AddCraftableList(craftableList);
		}
	}

	private void AddCraftableList(CraftableList craftableList)
	{
		CraftingManager craftingManager = Manager.Get<CraftingManager>();
		if (Array.Exists(craftingManager.craftableLists, (CraftableList a) => a == oppositeSideList))
		{
			craftableList = oppositeSideList;
			UnityEngine.Debug.LogError(craftableList);
		}
		if (craftingManager.craftableLists == null || craftingManager.craftableLists.Length == 0)
		{
			craftingManager.craftableLists = new CraftableList[1];
			craftingManager.craftableLists[0] = craftableList;
			return;
		}
		CraftableList[] array = new CraftableList[craftingManager.craftableLists.Length + 1];
		for (int i = 0; i < array.Length - 1; i++)
		{
			array[i] = craftingManager.craftableLists[i];
		}
		array[array.Length - 1] = craftableList;
		craftingManager.craftableLists = array;
	}
}
