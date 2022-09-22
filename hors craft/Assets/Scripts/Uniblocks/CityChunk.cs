// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CityChunk
using System;
using System.Collections.Generic;

namespace Uniblocks
{
	[Serializable]
	public class CityChunk
	{
		public int x;

		public int z;

		public List<BlueprintWithPosition> blueprintsWithPosition;

		public CityChunk(int x, int z, List<BlueprintWithPosition> blueprintsWithPosition)
		{
			this.x = x;
			this.z = z;
			this.blueprintsWithPosition = blueprintsWithPosition;
		}
	}
}
