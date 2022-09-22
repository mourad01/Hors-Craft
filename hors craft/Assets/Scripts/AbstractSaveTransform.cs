// DecompilerFi decompiler from Assembly-CSharp.dll class: AbstractSaveTransform
using Common.Model;
using UnityEngine;

public abstract class AbstractSaveTransform : MonoBehaviour
{
	[SerializeField]
	protected string prefabName;

	public AbstractSTModule module;

	public Settings settings = new Settings();

	public string PrefabName
	{
		get
		{
			return prefabName;
		}
		set
		{
			prefabName = value;
		}
	}

	public abstract IVector3Index chunkIndex
	{
		get;
	}

	public virtual void Start()
	{
		if (module == null)
		{
			throw new UnassignedReferenceException($"Assaign module to prefab: {base.name}");
		}
		module.Register(this);
	}

	public void RegisterToFreezed()
	{
		if (module == null)
		{
			throw new UnassignedReferenceException($"Assaign module to prefab: {base.name}");
		}
		module.RegisterToFreezed(this);
	}

	public void Despawn()
	{
		UnityEngine.Debug.LogError("Implement this action");
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
