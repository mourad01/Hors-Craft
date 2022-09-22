// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.Pass
using System;
using UnityEngine;

namespace Uniblocks
{
	[Serializable]
	public class Pass
	{
		public float chance;

		[SerializeField]
		public BlueprintWithChance[] blueprintsWithChance;
	}
}
