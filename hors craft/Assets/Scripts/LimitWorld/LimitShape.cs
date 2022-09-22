// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.LimitShape
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	public abstract class LimitShape : ScriptableObject
	{
		protected Vector3 _center;

		protected virtual string SerializationKey
		{
			[CompilerGenerated]
			get
			{
				return $"limitShape.{base.name}";
			}
		}

		public virtual Vector3 center
		{
			get
			{
				return _center;
			}
			set
			{
				_center = value;
			}
		}

		public virtual void Init()
		{
			InitSizes();
		}

		public virtual void NewSize()
		{
			center = center;
			SaveParams();
		}

		public abstract void Expand(Vector3 size, bool additional);

		public abstract float GetAreaSize();

		public abstract bool Contains(Index position, bool chunkCorrection);

		public abstract bool Contains(Vector3 worldPosition, bool chunkCorrection);

		public abstract bool IsBoundary(Index position, bool chunkCorrection);

		public abstract bool OutOf(Index position, bool chunkCorrection);

		public abstract void DrawGizos(Color color, bool chunkCorrection, bool selected);

		public abstract Vector3 GetRadius(bool chunkCorrection);

		public abstract float GetDistanceToLimit(Vector3 playerPosition, bool doChunkCorrection);

		public bool IsBoundaryChunk(Index position, bool chunkCorrection)
		{
			return IsBoundary(position, chunkCorrection);
		}

		protected abstract void InitSizes();

		protected abstract void SaveParams();
	}
}
