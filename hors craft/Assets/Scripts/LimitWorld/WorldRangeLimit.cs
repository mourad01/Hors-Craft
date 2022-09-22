// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.WorldRangeLimit
using System;
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/Limit/WorldRangeLimit")]
	public class WorldRangeLimit : Limit
	{
		[SerializeField]
		private float maxNoDespawnArea = 100f;

		public override EventTypeLW eventType
		{
			[CompilerGenerated]
			get
			{
				return EventTypeLW.SpawningChunk;
			}
		}

		protected override bool doChunkCorrection
		{
			[CompilerGenerated]
			get
			{
				return true;
			}
		}

		protected override Action initialAction => delegate
		{
			ChunkManager.DespawnActive = (maxNoDespawnArea <= limitShape.GetAreaSize());
		};

		public override void ResetLimit()
		{
			base.ResetLimit();
			Index index = Engine.PositionToIndex(PlayerGraphic.GetControlledPlayerInstance().transform.position);
			ChunkManager.SpawnChunks(index.x, index.y, index.z);
		}

		public override void ReSetup()
		{
			limitShape.NewSize();
			Index index = Engine.PositionToIndex(PlayerGraphic.GetControlledPlayerInstance().transform.position);
			ChunkManager.SpawnChunks(index.x, index.y, index.z);
		}

		public override bool ProcessEvent(DataLW data)
		{
			return !base.inited || limitShape.Contains((Index)data.target, doChunkCorrection);
		}
	}
}
