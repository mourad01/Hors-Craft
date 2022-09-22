// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingEnemyConfig
using UnityEngine;

namespace DragMinigame
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Drag Game/Drag Racing/Enemy Config")]
	public class DragRacingEnemyConfig : ScriptableObject
	{
		public DragRacingCarConfig carConfig;

		[Range(0f, 10f)]
		public float enemySkill;
	}
}
