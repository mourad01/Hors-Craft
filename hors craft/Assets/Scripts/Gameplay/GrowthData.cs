// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GrowthData
using System;

namespace Gameplay
{
	[Serializable]
	public class GrowthData
	{
		public string id;

		public int currentStage;

		public bool boosted;

		public GrowthData()
		{
			id = string.Empty;
			currentStage = 0;
			boosted = false;
		}
	}
}
