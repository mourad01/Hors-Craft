// DecompilerFi decompiler from Assembly-CSharp.dll class: SaveTransform
using Uniblocks;
using UnityEngine;

public class SaveTransform : AbstractSaveTransform
{
	public override IVector3Index chunkIndex => Engine.PositionToIndex(base.transform.position);

	public override void Start()
	{
	}

	public void OnDestroy()
	{
		module.Unregister(this);
	}

	public void ForceInit()
	{
		if (module == null)
		{
			UnityEngine.Debug.LogErrorFormat("Assaign module to prefab: {0}", base.name);
		}
		else
		{
			module.Register(this);
		}
	}

	public void OnEnable()
	{
		if (module == null)
		{
			UnityEngine.Debug.LogErrorFormat("Assaign module to prefab: {0}", base.name);
		}
		else
		{
			module.Register(this);
		}
	}

	public void OnDisable()
	{
		if (module == null)
		{
			UnityEngine.Debug.LogErrorFormat("Assaign module to prefab: {0}", base.name);
		}
		else
		{
			module.Unregister(this);
		}
	}
}
