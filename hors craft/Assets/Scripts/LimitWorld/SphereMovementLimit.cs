// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.SphereMovementLimit
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/Limit/SphereMovementLimit")]
	public class SphereMovementLimit : Limit
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
			float num = Vector3.Distance(playerMovement.transform.position, limitShape.center);
			float num2 = num;
			Vector3 radius = limitShape.GetRadius(chunkCorrection: true);
			if (num2 > radius.x - playerMovement.controller.height)
			{
				Transform transform = playerMovement.transform;
				Vector3 normalized = (playerMovement.transform.position - limitShape.center).normalized;
				Vector3 radius2 = limitShape.GetRadius(chunkCorrection: true);
				transform.position = normalized * (radius2.x - playerMovement.controller.height) + limitShape.center;
			}
			return true;
		}
	}
}
