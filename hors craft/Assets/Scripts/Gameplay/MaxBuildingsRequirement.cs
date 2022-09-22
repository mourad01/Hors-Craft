// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.MaxBuildingsRequirement
using Common.Managers;
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "Max Buildings", menuName = "ScriptableObjects/Requirements/MaxBuildings", order = 1)]
	public class MaxBuildingsRequirement : Requirement
	{
		public override bool CheckIfMet(float requiredAmount = 0f, string id = "")
		{
			return (float)Manager.Get<BlueprintManager>().builtBlueprints.Count < requiredAmount;
		}
	}
}
