// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.IMeshCreator
using UnityEngine;

namespace Uniblocks
{
	public interface IMeshCreator
	{
		void Init();

		void InitSubmeshArrays(Material[] sharedMaterials, Chunk chunk);

		void Generate(ChunkData chunk, bool triggerVoxelEvents = true);

		void LogSizes();

		Vector2[] GetTopUVs(float pad, float tUnit, Vector2 tOffset, int rotation);
	}
}
