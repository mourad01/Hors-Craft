// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlueprintWithChance
using System;
using UnityEngine;

namespace Uniblocks
{
	[Serializable]
	public class BlueprintWithChance
	{
		[SerializeField]
		public BlueprintCraftableObject blueprintCraftableObject;

		public float chanceToSpawn;

		[HideInInspector]
		public int size;
	}
}
