// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectPoolItem
using UnityEngine;

public class ObjectPoolItem : MonoBehaviour
{
	protected int id;

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public virtual void Init(int id)
	{
		base.gameObject.SetActive(value: true);
		this.id = id;
	}

	public virtual void End()
	{
		if (!(ObjectPoolManager.instance == null))
		{
			((IObjectPool)ObjectPoolManager.instance).ReturnObject(id, this);
			base.gameObject.SetActive(value: false);
		}
	}
}
