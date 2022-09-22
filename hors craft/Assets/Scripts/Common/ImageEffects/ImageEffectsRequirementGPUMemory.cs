// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsRequirementGPUMemory
using UnityEngine;

namespace Common.ImageEffects
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ImageEffectsRequirement/ImageEffectsRequirementGPUMemory")]
	public class ImageEffectsRequirementGPUMemory : ImageEffectsRequirement
	{
		public override bool MeetRequirement()
		{
			return requirement <= (float)SystemInfo.graphicsMemorySize;
		}
	}
}
