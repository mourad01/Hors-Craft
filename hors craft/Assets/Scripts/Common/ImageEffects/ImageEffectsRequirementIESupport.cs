// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.ImageEffects.ImageEffectsRequirementIESupport
using UnityEngine;

namespace Common.ImageEffects
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ImageEffectsRequirement/ImageEffectsRequirementIESupport")]
	public class ImageEffectsRequirementIESupport : ImageEffectsRequirement
	{
		public override bool MeetRequirement()
		{
			return SystemInfo.supportsImageEffects;
		}
	}
}
