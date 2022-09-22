// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.LevelRequirement
using Common.Managers;
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "LevelRequirement", menuName = "ScriptableObjects/Requirements/Level", order = 1)]
	public class LevelRequirement : Requirement
	{
		public override bool CheckIfMet(float requiredAmount = 0f, string id = "")
		{
			return requiredAmount <= (float)Manager.Get<ProgressManager>().level;
		}
	}
}
