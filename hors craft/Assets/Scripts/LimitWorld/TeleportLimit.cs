// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.TeleportLimit
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/Limit/TeleportLimit")]
	public class TeleportLimit : Limit
	{
		public override EventTypeLW eventType
		{
			[CompilerGenerated]
			get
			{
				return EventTypeLW.PlayerMoved;
			}
		}

		public override bool ProcessEvent(DataLW data)
		{
			PlayerMovement playerMovement = (PlayerMovement)data.target;
			if (!HasToTeleport(playerMovement.transform.position))
			{
				return false;
			}
			playerMovement.TeleportToStartPosition();
			return true;
		}

		private bool HasToTeleport(Vector3 playerPosition)
		{
			return limitShape.IsBoundaryChunk(ChunkData.WorldPositionToIndex(playerPosition), doChunkCorrection);
		}
	}
}
