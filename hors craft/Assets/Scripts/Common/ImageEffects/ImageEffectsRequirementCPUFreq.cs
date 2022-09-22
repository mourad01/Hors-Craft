// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsRequirementCPUFreq
using UnityEngine;

namespace Common.ImageEffects
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ImageEffectsRequirement/ImageEffectsRequirementCPUFreq")]
	public class ImageEffectsRequirementCPUFreq : ImageEffectsRequirement
	{
		public override bool MeetRequirement()
		{
			return requirement <= (float)SystemInfo.processorFrequency;
		}
	}
}
