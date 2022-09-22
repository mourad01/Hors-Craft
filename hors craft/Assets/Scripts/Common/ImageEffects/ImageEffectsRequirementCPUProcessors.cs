// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsRequirementCPUProcessors
using UnityEngine;

namespace Common.ImageEffects
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ImageEffectsRequirement/ImageEffectsRequirementCPUProcessors")]
	public class ImageEffectsRequirementCPUProcessors : ImageEffectsRequirement
	{
		public override bool MeetRequirement()
		{
			return requirement <= (float)SystemInfo.processorCount;
		}
	}
}
