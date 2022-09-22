// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingGameConfig
using UnityEngine;

namespace DragMinigame
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Drag Game/Drag Racing/Game Config")]
	public class DragRacingGameConfig : DragGameConfig
	{
		public float raceLength;

		public DragRacingEnemyConfig enemyConfig;
	}
}
