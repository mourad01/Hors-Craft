// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectPoolItemBase
using UnityEngine;

public class ObjectPoolItemBase : ScriptableObject
{
	[SerializeField]
	protected string itemName;

	[SerializeField]
	protected int itemId;

	[SerializeField]
	protected GameObject prefab;

	public string ItemName
	{
		get
		{
			return itemName;
		}
		set
		{
			itemName = value;
		}
	}

	public int ItemId
	{
		get
		{
			return itemId;
		}
		set
		{
			itemId = value;
		}
	}

	public GameObject Prefab
	{
		get
		{
			return prefab;
		}
		set
		{
			prefab = value;
		}
	}
}
