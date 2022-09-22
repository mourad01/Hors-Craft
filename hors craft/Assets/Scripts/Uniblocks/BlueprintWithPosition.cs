// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlueprintWithPosition
using System;

namespace Uniblocks
{
	[Serializable]
	public class BlueprintWithPosition
	{
		public int x;

		public int z;

		public string blueprintResourceName;

		public BlueprintWithPosition(int x, int z, string blueprintResourceName)
		{
			this.x = x;
			this.z = z;
			this.blueprintResourceName = blueprintResourceName;
		}
	}
}
