// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.LimitRect3D
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/LimitShape/LimitRect3D")]
	public class LimitRect3D : LimitShape
	{
		[SerializeField]
		protected Vector3 size;

		protected MinMax x;

		protected MinMax y;

		protected MinMax z;

		[SerializeField]
		protected bool limitY;

		private static IndexPair[] PAIRS = new IndexPair[6]
		{
			new IndexPair
			{
				axis = new Index(0, 1, 1),
				startPoint = new Index(0, 0, 0)
			},
			new IndexPair
			{
				axis = new Index(0, 1, 1),
				startPoint = new Index(15, 0, 0)
			},
			new IndexPair
			{
				axis = new Index(1, 1, 0),
				startPoint = new Index(0, 0, 0)
			},
			new IndexPair
			{
				axis = new Index(1, 1, 0),
				startPoint = new Index(0, 0, 15)
			},
			new IndexPair
			{
				axis = new Index(1, 0, 1),
				startPoint = new Index(0, 0, 0)
			},
			new IndexPair
			{
				axis = new Index(1, 0, 1),
				startPoint = new Index(0, 15, 0)
			}
		};

		public override Vector3 center
		{
			set
			{
				x = new MinMax(value.x - size.x / 2f, value.x + size.x / 2f);
				y = new MinMax(value.y - size.y / 2f, value.y + size.y / 2f);
				z = new MinMax(value.z - size.z / 2f, value.z + size.z / 2f);
				base.center = value;
			}
		}

		public override void Init()
		{
			base.Init();
			int num = ChunkData.SideLength - 1;
			PAIRS = new IndexPair[6]
			{
				new IndexPair
				{
					axis = new Index(0, 1, 1),
					startPoint = new Index(0, 0, 0)
				},
				new IndexPair
				{
					axis = new Index(0, 1, 1),
					startPoint = new Index(num, 0, 0)
				},
				new IndexPair
				{
					axis = new Index(1, 1, 0),
					startPoint = new Index(0, 0, 0)
				},
				new IndexPair
				{
					axis = new Index(1, 1, 0),
					startPoint = new Index(0, 0, num)
				},
				new IndexPair
				{
					axis = new Index(1, 0, 1),
					startPoint = new Index(0, 0, 0)
				},
				new IndexPair
				{
					axis = new Index(1, 0, 1),
					startPoint = new Index(0, num, 0)
				}
			};
		}

		public override void Expand(Vector3 size, bool additional)
		{
			if (additional)
			{
				this.size += size;
			}
			else
			{
				this.size = size;
			}
		}

		public override float GetAreaSize()
		{
			bool flag = !limitY;
			return size.x * size.z * ((!flag) ? size.y : 1f);
		}

		public override bool Contains(Index position, bool chunkCorrection)
		{
			bool flag = !limitY;
			return x.InBounds(position.x, chunkCorrection) && z.InBounds(position.z, chunkCorrection) && (flag || y.InBounds(position.y, chunkCorrection));
		}

		public override bool Contains(Vector3 worldPosition, bool chunkCorrection)
		{
			bool flag = !limitY;
			return x.InBoundsWorlds(worldPosition.x, chunkCorrection) && z.InBoundsWorlds(worldPosition.z, chunkCorrection) && (flag || y.InBoundsWorlds(worldPosition.y, chunkCorrection));
		}

		public override bool IsBoundary(Index position, bool chunkCorrection)
		{
			bool flag = !limitY;
			return (x.IsBound(position.x, chunkCorrection) || z.IsBound(position.z, chunkCorrection) || (!flag && y.IsBound(position.y, chunkCorrection))) && Contains(position, chunkCorrection);
		}

		public List<IndexPair> GetBoundaryAxis(Index position, bool chunkCorrection)
		{
			bool flag = !limitY;
			List<IndexPair> list = new List<IndexPair>();
			if (x.IsMin(position.x, chunkCorrection))
			{
				list.Add(PAIRS[0]);
			}
			if (x.IsMax(position.x, chunkCorrection))
			{
				list.Add(PAIRS[1]);
			}
			if (z.IsMin(position.z, chunkCorrection))
			{
				list.Add(PAIRS[2]);
			}
			if (z.IsMax(position.z, chunkCorrection))
			{
				list.Add(PAIRS[3]);
			}
			if (!flag && y.IsMin(position.y, chunkCorrection))
			{
				list.Add(PAIRS[4]);
			}
			if (!flag && y.IsMax(position.y, chunkCorrection))
			{
				list.Add(PAIRS[5]);
			}
			return list;
		}

		public override bool OutOf(Index position, bool chunkCorrection)
		{
			bool flag = !limitY;
			return !x.InBounds(position.x, chunkCorrection) || !z.InBounds(position.z, chunkCorrection) || (!flag && !y.InBounds(position.y, chunkCorrection));
		}

		public Vector3[,] GetWorldBoundsPositions(float ySize, bool chunkCorrection)
		{
			Vector2 bound = x.GetBound(chunkCorrection);
			Vector2 bound2 = z.GetBound(chunkCorrection);
			Vector3[,] array = new Vector3[4, 2];
			float num = bound.x;
			Vector3 center = this.center;
			array[0, 0] = new Vector3(num, 0f, center.z);
			array[0, 1] = new Vector3(1f, ySize, size.z * (float)ChunkData.SideLength);
			float num2 = bound.y;
			Vector3 center2 = this.center;
			array[1, 0] = new Vector3(num2, 0f, center2.z);
			array[1, 1] = new Vector3(1f, ySize, size.z * (float)ChunkData.SideLength);
			Vector3 center3 = this.center;
			array[2, 0] = new Vector3(center3.x, 0f, bound2.x);
			array[2, 1] = new Vector3(size.x * (float)ChunkData.SideLength, ySize, 1f);
			Vector3 center4 = this.center;
			array[3, 0] = new Vector3(center4.x, 0f, bound2.y);
			array[3, 1] = new Vector3(size.x * (float)ChunkData.SideLength, ySize, 1f);
			return array;
		}

		public override void DrawGizos(Color color, bool chunkCorrection, bool selected)
		{
			Gizmos.color = color;
			if (selected)
			{
				Gizmos.DrawSphere(Vector3.zero, 0.5f);
			}
			if (!size.Equals(Vector3.zero))
			{
				Vector3 setIndex = size;
				if (chunkCorrection)
				{
					Index left = new Index(setIndex);
					left /= 2;
					setIndex = (left * 2).ToVector3();
				}
				Gizmos.DrawWireCube(Vector3.zero, setIndex);
			}
		}

		public override Vector3 GetRadius(bool chunkCorrection)
		{
			Vector3 result = default(Vector3);
			Vector2 bound = x.GetBound(chunkCorrection);
			float num = bound.x;
			Vector2 bound2 = x.GetBound(chunkCorrection);
			result.x = Mathf.Abs(num - bound2.y) / 2f;
			Vector2 bound3 = y.GetBound(chunkCorrection);
			float num2 = bound3.x;
			Vector2 bound4 = y.GetBound(chunkCorrection);
			result.y = Mathf.Abs(num2 - bound4.y) / 2f;
			Vector2 bound5 = z.GetBound(chunkCorrection);
			float num3 = bound5.x;
			Vector2 bound6 = z.GetBound(chunkCorrection);
			result.z = Mathf.Abs(num3 - bound6.y) / 2f;
			return result;
		}

		public override float GetDistanceToLimit(Vector3 playerPosition, bool doChunkCorrection)
		{
			VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(playerPosition);
			if (voxelInfo?.chunk == null)
			{
				return -1f;
			}
			if (OutOfBoundry(voxelInfo.chunk.ChunkIndex, doChunkCorrection))
			{
				return 1f;
			}
			if (!IsBoundaryChunk(voxelInfo.chunk.ChunkIndex, chunkCorrection: false))
			{
				return -1f;
			}
			IndexPair[] boundaryAxisNoY = GetBoundaryAxisNoY(voxelInfo.chunk.ChunkIndex, doChunkCorrection);
			float num = 0f;
			IndexPair[] array = boundaryAxisNoY;
			for (int i = 0; i < array.Length; i++)
			{
				IndexPair indexPair = array[i];
				float num2 = 0f;
				Index left = (indexPair.axis - 1) * -1;
				Vector3 a = left * (indexPair.startPoint + ChunkData.IndexToPosition(voxelInfo.chunk.ChunkIndex) + new Vector3(0.5f, 0.5f, 0.5f));
				Vector3 b = left * playerPosition;
				num2 = ((float)(ChunkData.SideLength - 1) - Vector3.Distance(a, b)) / (float)(ChunkData.SideLength - 1);
				num = Mathf.Max(num, num2);
			}
			return num;
		}

		private IndexPair[] GetBoundaryAxisNoY(Index position, bool doChunkCorrection)
		{
			return GetBoundaryAxis(position, doChunkCorrection).ToArray();
		}

		private bool OutOfBoundry(Index position, bool doChunkCorrection)
		{
			return OutOf(position, doChunkCorrection);
		}

		protected override void InitSizes()
		{
			string @string = PlayerPrefs.GetString(SerializationKey, null);
			if (!string.IsNullOrEmpty(@string))
			{
				size = JsonUtility.FromJson<Vector3>(@string);
			}
		}

		protected override void SaveParams()
		{
			PlayerPrefs.SetString(SerializationKey, JsonUtility.ToJson(size));
			PlayerPrefs.Save();
		}
	}
}
