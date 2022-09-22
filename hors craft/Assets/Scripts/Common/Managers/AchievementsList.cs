// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AchievementsList
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	[CreateAssetMenu(fileName = "AchievementsList", menuName = "ScriptableObjects/Achievements/AchievementsList", order = 2)]
	public class AchievementsList : ScriptableObject
	{
		public List<Achievement> availableAchievements;
	}
}
