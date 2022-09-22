// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.FlatTerrainGenerator
namespace Uniblocks
{
	public class FlatTerrainGenerator : CommonTerrainGenerator
	{
		protected override void GenerateVoxelData(int seed, ChunkData chunk)
		{
			int y = chunk.ChunkIndex.y;
			int sideLength = ChunkData.SideLength;
			for (int i = 0; i < sideLength; i++)
			{
				for (int j = 0; j < sideLength; j++)
				{
					for (int k = 0; k < sideLength; k++)
					{
						int num = j + sideLength * y;
						if (num < 8)
						{
							chunk.SetVoxelSimple(i, j, k, otherIDs.dirtBlockID);
						}
					}
				}
			}
		}
	}
}
