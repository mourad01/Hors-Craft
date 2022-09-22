// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectPoolInventory
using UnityEngine;

public class ObjectPoolInventory : MonoBehaviour
{
	public ObjectPoolItemList inventoryList;

	[Header("Add item fields")]
	[SerializeField]
	protected string objectName;

	[SerializeField]
	protected int id;

	[SerializeField]
	protected GameObject prefabToAdd;

	[ContextMenu("Add Item to inventory")]
	public void AddItemToInventory()
	{
		ObjectPoolItemBase objectPoolItemBase = ScriptableObject.CreateInstance<ObjectPoolItemBase>();
		objectPoolItemBase.ItemName = objectName;
		objectPoolItemBase.ItemId = id;
		objectPoolItemBase.Prefab = prefabToAdd;
		AddAsset(objectPoolItemBase);
	}

	private void CreateOrFindInventory()
	{
	}

	public void AddAsset<T>(T newAsset = null) where T : ScriptableObject
	{
	}

	public T GetItemFromInventory<T>(int id) where T : ScriptableObject
	{
		if (inventoryList == null)
		{
			CreateOrFindInventory();
		}
		return inventoryList.GetItem<T>(id);
	}
}
