// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.LimitSphere3D
using System;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/LimitShape/LimitSphere3D")]
	public class LimitSphere3D : LimitShape
	{
		[SerializeField]
		protected float radius;

		public override Vector3 center
		{
			get
			{
				return _center;
			}
			set
			{
				_center = value;
				_center.x += 7.5f;
				_center.z += 7.5f;
				_center.y = 0f;
			}
		}

		public override void Expand(Vector3 size, bool additional)
		{
			if (additional)
			{
				radius += size.x;
			}
			else
			{
				radius = size.x;
			}
		}

		public override float GetAreaSize()
		{
			return 4.18878937f * radius * radius * radius;
		}

		public override bool Contains(Index position, bool chunkCorrection)
		{
			return Vector3.Distance(center, position.ToVector3()) <= radius;
		}

		public override bool Contains(Vector3 worldPosition, bool chunkCorrection)
		{
			return Vector3.Distance(worldPosition, center) < radius * (float)ChunkData.SideLength;
		}

		public override bool IsBoundary(Index position, bool chunkCorrection)
		{
			return Math.Abs(Vector3.Distance(center, position.ToVector3()) - radius) < 0.2f;
		}

		public override bool OutOf(Index position, bool chunkCorrection)
		{
			return Vector3.Distance(center, position.ToVector3()) > radius;
		}

		public override void DrawGizos(Color color, bool chunkCorrection, bool selected)
		{
			Gizmos.color = color;
			if (selected)
			{
				Gizmos.DrawSphere(Vector3.zero, 0.5f);
			}
			if (radius != 0f)
			{
				Gizmos.DrawWireSphere(Vector3.zero, radius);
			}
		}

		public override Vector3 GetRadius(bool chunkCorrection)
		{
			return new Vector3(radius * (float)ChunkData.SideLength, radius * (float)ChunkData.SideLength, radius * (float)ChunkData.SideLength);
		}

		public override float GetDistanceToLimit(Vector3 playerPosition, bool doChunkCorrection)
		{
			float num = Vector3.Distance(playerPosition, center);
			return (!(num < (radius - 1f) * (float)ChunkData.SideLength)) ? ((num - (radius - 1f) * (float)ChunkData.SideLength) / (float)ChunkData.SideLength) : (-1f);
		}

		protected override void InitSizes()
		{
			radius = PlayerPrefs.GetFloat(SerializationKey, radius);
		}

		protected override void SaveParams()
		{
			PlayerPrefs.SetFloat(SerializationKey, radius);
			PlayerPrefs.Save();
		}
	}
}
