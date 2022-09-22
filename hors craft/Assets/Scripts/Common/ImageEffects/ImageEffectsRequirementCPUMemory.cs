// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsRequirementCPUMemory
using UnityEngine;

namespace Common.ImageEffects
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ImageEffectsRequirement/ImageEffectsRequirementCPUMemory")]
	public class ImageEffectsRequirementCPUMemory : ImageEffectsRequirement
	{
		public override bool MeetRequirement()
		{
			return requirement <= (float)SystemInfo.systemMemorySize;
		}
	}
}
