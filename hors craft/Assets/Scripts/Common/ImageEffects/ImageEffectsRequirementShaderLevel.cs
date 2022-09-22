// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsRequirementShaderLevel
using UnityEngine;

namespace Common.ImageEffects
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ImageEffectsRequirement/ImageEffectsRequirementShaderLevel")]
	public class ImageEffectsRequirementShaderLevel : ImageEffectsRequirement
	{
		public override bool MeetRequirement()
		{
			return requirement <= (float)SystemInfo.graphicsShaderLevel;
		}
	}
}
