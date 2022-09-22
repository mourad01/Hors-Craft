// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.DragRacingRoad
using System;
using UnityEngine;

namespace DragMinigame
{
	public class DragRacingRoad : MonoBehaviour
	{
		[Serializable]
		public class Sector
		{
			public int starrindex;

			public int endIndex;
		}

		[SerializeField]
		private Transform[] rows;

		[SerializeField]
		private Sector[] sectors;

		[SerializeField]
		private DragRacingMob[] mobs;

		private void OnEnable()
		{
		}
	}
}
