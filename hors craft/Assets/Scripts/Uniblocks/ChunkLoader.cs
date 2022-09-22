// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ChunkLoader
using UnityEngine;

namespace Uniblocks
{
	public class ChunkLoader : MonoBehaviour
	{
		private Index LastPos;

		private Index currentPos;

		public void Update()
		{
			if (Engine.Initialized && ChunkManager.Initialized)
			{
				currentPos = Engine.PositionToIndex(base.transform.position);
				if (!currentPos.IsEqual(LastPos))
				{
					ChunkManager.SpawnChunks(currentPos.x, currentPos.y, currentPos.z);
				}
				LastPos = currentPos;
			}
		}
	}
}
