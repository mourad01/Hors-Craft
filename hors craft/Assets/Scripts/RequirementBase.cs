// DecompilerFi decompiler from Assembly-CSharp.dll class: RequirementBase
using System;
using UnityEngine;

[Serializable]
public class RequirementBase
{
	public enum ECheckState
	{
		hasNone,
		hasSome,
		hasAll
	}

	[SerializeField]
	protected ERequirementType requirementType;

	[SerializeField]
	protected int requirementItemId;

	[SerializeField]
	protected int requirementQty;

	public int RequirementItemId => requirementItemId;

	public int RequirementQty => requirementQty;

	public RequirementBase()
	{
	}

	public RequirementBase(ERequirementType requirementType, int requirementItemId, int requirementQty)
	{
		this.requirementType = requirementType;
		this.requirementQty = requirementQty;
		this.requirementItemId = requirementItemId;
	}

	public ECheckState CheckProgress()
	{
		int resourcesCount = Singleton<PlayerData>.get.playerItems.GetResourcesCount(requirementItemId);
		if (requirementType == ERequirementType.gatherItems)
		{
			if (resourcesCount == 0)
			{
				return ECheckState.hasNone;
			}
			if (resourcesCount > 0 && resourcesCount < requirementQty)
			{
				return ECheckState.hasSome;
			}
			return ECheckState.hasAll;
		}
		return ECheckState.hasNone;
	}

	public override string ToString()
	{
		return $" [type:{requirementType}, id:{requirementItemId}, qty:{requirementQty} ]";
	}

	public void GrantItems()
	{
		if (requirementType == ERequirementType.gatherItems)
		{
			Singleton<PlayerData>.get.playerItems.AddResource(requirementItemId, requirementQty);
		}
	}

	public virtual RequirementBase DeepCopy()
	{
		RequirementBase requirementBase = new RequirementBase();
		requirementBase.requirementType = requirementType;
		requirementBase.requirementQty = requirementQty;
		requirementBase.requirementItemId = requirementItemId;
		return requirementBase;
	}
}
