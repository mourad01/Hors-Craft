// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.OnlyTerrainPlacing
using Uniblocks;

namespace SurvivalMobSpawning
{
	public class OnlyTerrainPlacing : AbstractSpawnPlacingBehaviour
	{
		protected override ushort placingBlockId => Engine.usefulIDs.grassBlockID;
	}
}
