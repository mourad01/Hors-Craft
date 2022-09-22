// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.MovementLimit
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/Limit/MovementLimit")]
	public class MovementLimit : Limit
	{
		[SerializeField]
		public AnimationCurve maxDistanceToGround;

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
			float distanceToLimit = limitShape.GetDistanceToLimit(playerMovement.transform.position, doChunkCorrection);
			if (distanceToLimit < -0.8f || distanceToLimit > 5f)
			{
				return false;
			}
			float num = maxDistanceToGround.Evaluate(distanceToLimit);
			VoxelInfo voxelInfo = Engine.VoxelGridRaycast(playerMovement.transform.position, Vector3.down, 642f);
			Vector3 vector = voxelInfo.index + ChunkData.IndexToPosition(voxelInfo.chunk.ChunkIndex) + new Vector3(0.5f, 0.5f, 0.5f);
			vector.y += num;
			float y = vector.y;
			Vector3 position = playerMovement.transform.position;
			if (y < position.y)
			{
				Vector3 position2 = playerMovement.transform.position;
				position2.y = vector.y - 0.5f;
				playerMovement.transform.position = position2;
			}
			return true;
		}
	}
}
