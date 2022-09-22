// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.IsometricPlaceableBlueprint
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using LimitWorld;
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	public class IsometricPlaceableBlueprint : IsometricPlaceableObject
	{
		private struct RevertInfo
		{
			public ushort id;

			public byte rotation;
		}

		private readonly HashSet<ChunkData> chunksToUpdate = new HashSet<ChunkData>();

		private Dictionary<Vector3, RevertInfo> voxelsToRevert;

		private BlueprintData blueprint;

		private BlueprintManager blueprintManager;

		private string dataName;

		private Vector3 previousPosition = Vector3.zero;

		private Vector3 min;

		private Vector3 max;

		private Vector3 center;

		private int voxelsToFill;

		private float yPosition;

		private bool savedYPosition;

		private Vector3[] localPositions;

		private Vector3[] localBoxPositions;

		private readonly HashSet<Vector3> boundsVoxelsPositions = new HashSet<Vector3>();

		private Dictionary<Vector2, int> front = new Dictionary<Vector2, int>();

		private Dictionary<Vector2, int> back = new Dictionary<Vector2, int>();

		private Dictionary<Vector2, int> left = new Dictionary<Vector2, int>();

		private Dictionary<Vector2, int> right = new Dictionary<Vector2, int>();

		private Dictionary<Vector2, int> up = new Dictionary<Vector2, int>();

		private Dictionary<Vector2, int> down = new Dictionary<Vector2, int>();

		private Vector3[] positionsFindHighest = new Vector3[9];

		private List<ChunkData> chunkAndNeighbours = new List<ChunkData>(5);

		protected override string GetErrorKey()
		{
			return "blueprint.cant.place.2";
		}

		protected override string GetErrorDefaultText()
		{
			return "you can't place blueprint here";
		}

		public void SetBlueprint(BlueprintData data, string name)
		{
			blueprint = data;
			dataName = name;
			blueprintManager = Manager.Get<BlueprintManager>();
			CalculateSize();
		}

		private void CalculateSize()
		{
			min.x = blueprint.width - 1;
			min.y = blueprint.height - 1;
			min.z = blueprint.depth - 1;
			max.x = 0f;
			max.y = 0f;
			max.z = 0f;
			for (int i = 0; i <= blueprint.width - 1; i++)
			{
				for (int j = 0; j <= blueprint.height - 1; j++)
				{
					for (int k = 0; k <= blueprint.depth - 1; k++)
					{
						Voxel voxelType = Engine.GetVoxelType(blueprint.voxelsArray[i][j][k]);
						ushort uniqueID = voxelType.GetUniqueID();
						if (uniqueID != 0 && uniqueID != 12 && !voxelType.isThatFlower && !voxelType.isItSomeKindOfGrass)
						{
							min.x = Mathf.Min(min.x, i);
							min.y = Mathf.Min(min.y, j);
							min.z = Mathf.Min(min.z, k);
							max.x = Mathf.Max(max.x, i);
							max.y = Mathf.Max(max.y, j);
							max.z = Mathf.Max(max.z, k);
						}
					}
				}
			}
			for (int l = (int)min.x; l <= (int)max.x; l++)
			{
				for (int m = (int)min.y; m <= (int)max.y; m++)
				{
					for (int n = (int)min.z; n <= (int)max.z; n++)
					{
						ushort uniqueID2 = Engine.GetVoxelType(blueprint.voxelsArray[l][m][n]).GetUniqueID();
						if (uniqueID2 != 0 && uniqueID2 != 12)
						{
							if (!front.ContainsKey(new Vector2(l, m)))
							{
								front.Add(new Vector2(l, m), n);
							}
							back.AddOrReplace(new Vector2(l, m), n);
						}
					}
				}
			}
			for (int num = (int)min.z; num <= (int)max.z; num++)
			{
				for (int num2 = (int)min.y; num2 <= (int)max.y; num2++)
				{
					for (int num3 = (int)min.x; num3 <= (int)max.x; num3++)
					{
						ushort uniqueID3 = Engine.GetVoxelType(blueprint.voxelsArray[num3][num2][num]).GetUniqueID();
						if (uniqueID3 != 0 && uniqueID3 != 12)
						{
							if (!left.ContainsKey(new Vector2(num, num2)))
							{
								left.Add(new Vector2(num, num2), num3);
							}
							right.AddOrReplace(new Vector2(num, num2), num3);
						}
					}
				}
			}
			for (int num4 = (int)min.z; num4 <= (int)max.z; num4++)
			{
				for (int num5 = (int)min.x; num5 <= (int)max.x; num5++)
				{
					for (int num6 = (int)min.y; num6 <= (int)max.y; num6++)
					{
						ushort uniqueID4 = Engine.GetVoxelType(blueprint.voxelsArray[num5][num6][num4]).GetUniqueID();
						if (uniqueID4 != 0 && uniqueID4 != 12)
						{
							if (!down.ContainsKey(new Vector2(num4, num5)))
							{
								down.Add(new Vector2(num4, num5), num6);
							}
							up.AddOrReplace(new Vector2(num4, num5), num6);
						}
					}
				}
			}
			for (int num7 = (int)min.x; num7 <= (int)max.x; num7++)
			{
				for (int num8 = (int)min.y; num8 <= (int)max.y; num8++)
				{
					for (int num9 = (int)min.z; num9 <= (int)max.z; num9++)
					{
						ushort uniqueID5 = Engine.GetVoxelType(blueprint.voxelsArray[num7][num8][num9]).GetUniqueID();
						if (uniqueID5 != 0 && uniqueID5 != 12)
						{
							voxelsToFill++;
						}
					}
				}
			}
			foreach (KeyValuePair<Vector2, int> item in front)
			{
				HashSet<Vector3> hashSet = boundsVoxelsPositions;
				Vector2 key = item.Key;
				float x = key.x;
				Vector2 key2 = item.Key;
				hashSet.Add(new Vector3(x, key2.y, item.Value));
			}
			front = null;
			foreach (KeyValuePair<Vector2, int> item2 in back)
			{
				HashSet<Vector3> hashSet2 = boundsVoxelsPositions;
				Vector2 key3 = item2.Key;
				float x2 = key3.x;
				Vector2 key4 = item2.Key;
				hashSet2.Add(new Vector3(x2, key4.y, item2.Value));
			}
			back = null;
			foreach (KeyValuePair<Vector2, int> item3 in left)
			{
				HashSet<Vector3> hashSet3 = boundsVoxelsPositions;
				float x3 = item3.Value;
				Vector2 key5 = item3.Key;
				float y = key5.y;
				Vector2 key6 = item3.Key;
				hashSet3.Add(new Vector3(x3, y, key6.x));
			}
			left = null;
			foreach (KeyValuePair<Vector2, int> item4 in right)
			{
				HashSet<Vector3> hashSet4 = boundsVoxelsPositions;
				float x4 = item4.Value;
				Vector2 key7 = item4.Key;
				float y2 = key7.y;
				Vector2 key8 = item4.Key;
				hashSet4.Add(new Vector3(x4, y2, key8.x));
			}
			right = null;
			foreach (KeyValuePair<Vector2, int> item5 in down)
			{
				HashSet<Vector3> hashSet5 = boundsVoxelsPositions;
				Vector2 key9 = item5.Key;
				float y3 = key9.y;
				float y4 = item5.Value;
				Vector2 key10 = item5.Key;
				hashSet5.Add(new Vector3(y3, y4, key10.x));
			}
			down = null;
			foreach (KeyValuePair<Vector2, int> item6 in up)
			{
				HashSet<Vector3> hashSet6 = boundsVoxelsPositions;
				Vector2 key11 = item6.Key;
				float y5 = key11.y;
				float y6 = item6.Value;
				Vector2 key12 = item6.Key;
				hashSet6.Add(new Vector3(y5, y6, key12.x));
			}
			up = null;
			voxelsToRevert = new Dictionary<Vector3, RevertInfo>(voxelsToFill, VectorExtensionMethods.Vector3EqualityComparerInstance);
			center = new Vector3(min.x + (float)Mathf.FloorToInt((max.x - min.x) / 2f), min.y + (float)Mathf.FloorToInt((max.y - min.y) / 2f), min.z + (float)Mathf.FloorToInt((max.z - min.z) / 2f));
			localPositions = new Vector3[8]
			{
				new Vector3(min.x - center.x, 0f, 0f),
				new Vector3(max.x - center.x, 0f, 0f),
				new Vector3(0f, 0f, min.z - center.z),
				new Vector3(0f, 0f, max.z - center.z),
				new Vector3(min.x - center.x, 0f, min.z - center.z),
				new Vector3(max.x - center.x, 0f, min.z - center.z),
				new Vector3(min.x - center.x, 0f, max.z - center.z),
				new Vector3(max.x - center.x, 0f, max.z - center.z)
			};
			localBoxPositions = new Vector3[8]
			{
				new Vector3(min.x - center.x, min.y - center.y, min.z - center.z),
				new Vector3(max.x - center.x, min.y - center.y, min.z - center.z),
				new Vector3(min.x - center.x, min.y - center.y, max.z - center.z),
				new Vector3(max.x - center.x, min.y - center.y, max.z - center.z),
				new Vector3(min.x - center.x, max.y - center.y, min.z - center.z),
				new Vector3(max.x - center.x, max.y - center.y, min.z - center.z),
				new Vector3(min.x - center.x, max.y - center.y, max.z - center.z),
				new Vector3(max.x - center.x, max.y - center.y, max.z - center.z)
			};
		}

		protected override void Update()
		{
			base.Update();
			if (Vector3.Distance(previousPosition, base.transform.position) > 0.5f)
			{
				previousPosition = base.transform.position;
				UpdateGraphics();
			}
		}

		private void UpdateGraphics()
		{
			chunksToUpdate.Clear();
			RevertVoxels();
			SetVoxels();
			EnablePlacement(!CheckBlueprintIntersection() && CheckWorldLimit());
			foreach (ChunkData item in chunksToUpdate)
			{
				item.Empty = false;
				item.Changed = true;
			}
			chunksToUpdate.Clear();
		}

		private bool CheckWorldLimit()
		{
			bool flag = true;
			if (MonoBehaviourSingleton<LimitedWorld>.get.active)
			{
				for (int i = 0; i < localBoxPositions.Length; i++)
				{
					Quaternion q = Quaternion.Euler(0f, rotation * 90, 0f);
					Matrix4x4 matrix4x = Matrix4x4.Rotate(q);
					flag = (flag && MonoBehaviourSingleton<LimitedWorld>.get.IsInBounds(matrix4x.MultiplyPoint3x4(localBoxPositions[i]) + base.transform.position));
				}
			}
			return flag;
		}

		private void RevertVoxels()
		{
			foreach (KeyValuePair<Vector3, RevertInfo> item in voxelsToRevert)
			{
				VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(item.Key);
				ChunkData chunk = voxelInfo.chunk;
				int rawIndex = voxelInfo.rawIndex;
				RevertInfo value = item.Value;
				chunk.SetVoxelSimple(rawIndex, value.id);
				ChunkData chunk2 = voxelInfo.chunk;
				int rawIndex2 = voxelInfo.rawIndex;
				RevertInfo value2 = item.Value;
				chunk2.SetRotation(rawIndex2, value2.rotation);
				chunkAndNeighbours.Clear();
				voxelInfo.GetChunkAndNeighbours(chunkAndNeighbours);
				voxelInfo.chunk.ignoreVoxelEventsOnNextRebuild = true;
				for (int i = 0; i < chunkAndNeighbours.Count; i++)
				{
					chunksToUpdate.Add(chunkAndNeighbours[i]);
				}
			}
			voxelsToRevert.Clear();
		}

		private void SetVoxels()
		{
			Quaternion q = Quaternion.Euler(0f, base.rotation * 90, 0f);
			Matrix4x4 matrix4x = Matrix4x4.Rotate(q);
			foreach (Vector3 boundsVoxelsPosition in boundsVoxelsPositions)
			{
				Vector3 current = boundsVoxelsPosition;
				byte rotation;
				ushort voxel = blueprint.GetVoxel((int)current.x, (int)current.y, (int)current.z, out rotation);
				rotation = (byte)((rotation + base.rotation) % 4);
				Vector3 point = current - center;
				point = matrix4x.MultiplyPoint3x4(point) + base.transform.position;
				if (voxel != 0 && voxel != 12)
				{
					VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(point);
					if (voxelInfo != null)
					{
						voxelsToRevert.Add(point, new RevertInfo
						{
							id = voxelInfo.GetVoxel(),
							rotation = voxelInfo.GetVoxelRotation()
						});
						voxelInfo.chunk.SetVoxelSimple(voxelInfo.rawIndex, voxel);
						voxelInfo.chunk.SetRotation(voxelInfo.rawIndex, rotation);
						chunkAndNeighbours.Clear();
						voxelInfo.GetChunkAndNeighbours(chunkAndNeighbours);
						voxelInfo.chunk.ignoreVoxelEventsOnNextRebuild = true;
						for (int i = 0; i < chunkAndNeighbours.Count; i++)
						{
							chunksToUpdate.Add(chunkAndNeighbours[i]);
						}
					}
				}
			}
		}

		protected override void SnapToGround()
		{
			Vector3 zero = Vector3.zero;
			bool flag = false;
			float num = VoxelFindHighestTerrain(out bool foundTerrain);
			if (!foundTerrain)
			{
				return;
			}
			while (!flag)
			{
				flag = true;
				for (int i = (int)min.x; i <= (int)max.x; i++)
				{
					if (!flag)
					{
						break;
					}
					for (int j = (int)min.z; j <= (int)max.z; j++)
					{
						if (!flag)
						{
							break;
						}
						zero = new Vector3(i, 0f, j);
						Vector3 vector = zero - center;
						for (int num2 = rotation; num2 > 0; num2--)
						{
							vector = vector.Rotate90right();
						}
						vector += base.transform.position;
						vector = new Vector3(vector.x, num, vector.z);
						ushort realVoxel = GetRealVoxel(vector);
						if (realVoxel != 0 && realVoxel != 12)
						{
							Voxel voxelType = Engine.GetVoxelType(realVoxel);
							if (voxelType.VTransparency == Transparency.solid || (voxelType.VTransparency == Transparency.semiTransparent && !voxelType.isThatFlower && !voxelType.isItSomeKindOfGrass && !voxelType.editorOnly) || voxelType.VTransparency == Transparency.overlay)
							{
								flag = false;
							}
						}
					}
				}
				num += 1f;
			}
			num -= 1f;
			Vector3 position = base.transform.position;
			position.y = num + center.y - min.y;
			base.transform.position = position;
			yPosition = position.y;
			savedYPosition = true;
		}

		private float VoxelFindHighestTerrain(out bool foundTerrain)
		{
			Vector3 position = base.transform.position;
			if (savedYPosition)
			{
				position.y = yPosition;
			}
			position.y += max.y - center.y;
			positionsFindHighest[0] = position;
			for (int i = 0; i < localPositions.Length; i++)
			{
				int num = rotation;
				Vector3 vector = localPositions[i];
				while (num > 0)
				{
					vector = vector.Rotate90right();
					num--;
				}
				positionsFindHighest[i + 1] = position + vector;
			}
			for (int num2 = 200; num2 > 0; num2--)
			{
				for (int j = 0; j < positionsFindHighest.Length; j++)
				{
					ushort realVoxel = GetRealVoxel(positionsFindHighest[j]);
					if (realVoxel == 0 || realVoxel == 12)
					{
						positionsFindHighest[j].y -= 1f;
						continue;
					}
					foundTerrain = true;
					return positionsFindHighest[j].y;
				}
			}
			foundTerrain = false;
			return positionsFindHighest[0].y;
		}

		private ushort GetRealVoxel(Vector3 position)
		{
			if (voxelsToRevert.TryGetValue(position, out RevertInfo value))
			{
				return value.id;
			}
			return Engine.PositionToVoxelInfo(position)?.GetVoxel() ?? 0;
		}

		private bool CheckBlueprintIntersection()
		{
			Vector3 a = min;
			blueprint.GetVoxel((int)min.x, (int)min.y, (int)min.z, out byte rotation);
			rotation = (byte)((rotation + base.rotation) % 4);
			a -= center;
			for (int num = base.rotation; num > 0; num--)
			{
				a = a.Rotate90right();
			}
			a += base.transform.position;
			Vector3 a2 = max;
			blueprint.GetVoxel((int)max.x, (int)max.y, (int)max.z, out rotation);
			rotation = (byte)((rotation + base.rotation) % 4);
			a2 -= center;
			for (int num = base.rotation; num > 0; num--)
			{
				a2 = a2.Rotate90right();
			}
			a2 += base.transform.position;
			return blueprintManager.CheckBlueprintIntersection(a, a2);
		}

		public override bool OnPlace()
		{
			Vector3 zero = Vector3.zero;
			ushort blueprintID = Engine.usefulIDs.blueprintID;
			for (int i = (int)min.x; i <= (int)max.x; i++)
			{
				for (int j = (int)min.y; j <= (int)max.y; j++)
				{
					for (int k = (int)min.z; k <= (int)max.z; k++)
					{
						zero = new Vector3(i, j, k);
						byte rotation;
						ushort uniqueID = Engine.GetVoxelType(blueprint.GetVoxel(i, j, k, out rotation)).GetUniqueID();
						rotation = (byte)((rotation + base.rotation) % 4);
						Vector3 vector = zero - center;
						for (int num = base.rotation; num > 0; num--)
						{
							vector = vector.Rotate90right();
						}
						vector += base.transform.position;
						if (uniqueID == 0 || uniqueID == 12)
						{
							continue;
						}
						VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(vector);
						if (voxelInfo == null)
						{
							continue;
						}
						if ((float)j == min.y)
						{
							if (!voxelInfo.chunk.rebuildOnMainThread)
							{
								voxelInfo.chunk.rebuildOnMainThread = true;
							}
							voxelInfo.chunk.SetVoxel(voxelInfo.index, blueprintID, updateMesh: true, 0);
							voxelInfo.chunk.SetRotation(voxelInfo.rawIndex, rotation);
						}
						else if (vector.y < (float)Engine.TerrainGenerator.waterHeight)
						{
							voxelInfo.chunk.SetVoxel(voxelInfo.index, Engine.usefulIDs.waterBlockID, updateMesh: true, 0);
						}
						else
						{
							voxelInfo.chunk.SetVoxel(voxelInfo.index, 0, updateMesh: true, 0);
						}
					}
				}
			}
			TrySetPosition();
			Manager.Get<BlueprintManager>().RegisterBlueprint(new PlacedBlueprintData
			{
				dataName = dataName,
				position = base.transform.position,
				rotation = base.rotation,
				craftableId = craftableId,
				min = min,
				max = max,
				center = center,
				blocksToFillInBlueprint = voxelsToFill
			}, loaded: true);
			base.OnPlace();
			return true;
		}

		private void TrySetPosition()
		{
			for (int i = (int)min.y + 1; (float)i <= max.y; i++)
			{
				Vector3[] array = new Vector3[4]
				{
					new Vector3(center.x, i, min.z - 3f),
					new Vector3(center.x, i, max.z + 3f),
					new Vector3(min.x - 3f, i, center.z),
					new Vector3(max.x + 3f, i, center.z)
				};
				Vector3[] array2 = array;
				foreach (Vector3 vector in array2)
				{
					Vector3 a = vector;
					a -= center;
					for (int num = rotation; num > 0; num--)
					{
						a = a.Rotate90right();
					}
					Vector3 direction = -a;
					direction.y = 0f;
					direction = direction.normalized;
					a += base.transform.position;
					ushort voxel = Engine.VoxelGridRaycastIgnoreWater(a, direction, 3f).GetVoxel();
					if (blueprintManager.neutralIDList.Contains(Engine.PositionToVoxelInfo(a).GetVoxel()) && (blueprintManager.neutralIDList.Contains(voxel) || voxel == Engine.usefulIDs.blueprintID))
					{
						PlayerGraphic.GetControlledPlayerInstance().gameObject.transform.position = a;
						Vector3 eulerAngles = CameraController.instance.Anchor.eulerAngles;
						CameraController.instance.Anchor.LookAt(base.transform.position);
						Vector3 eulerAngles2 = CameraController.instance.Anchor.eulerAngles;
						eulerAngles.y = eulerAngles2.y;
						CameraController.instance.Anchor.eulerAngles = eulerAngles;
						return;
					}
				}
			}
		}

		public override void OnRotate()
		{
			base.OnRotate();
			UpdateGraphics();
		}

		public override void OnDelete()
		{
			chunksToUpdate.Clear();
			RevertVoxels();
			foreach (ChunkData item in chunksToUpdate)
			{
				item.ignoreVoxelEventsOnNextRebuild = true;
				item.Empty = false;
				item.Changed = true;
			}
			chunksToUpdate.Clear();
			base.OnDelete();
		}

		public override Bounds GetBounds()
		{
			Vector3 vector = max - min;
			float num = (!(vector.x > vector.y)) ? vector.y : vector.x;
			Bounds result = default(Bounds);
			result.size = new Vector3(0f, (!(num > vector.z)) ? vector.z : num, 0f);
			return result;
		}

		private void OnDrawGizmosSelected()
		{
			for (int i = 0; i < localBoxPositions.Length; i++)
			{
				Quaternion q = Quaternion.Euler(0f, rotation * 90, 0f);
				Gizmos.DrawSphere(Matrix4x4.Rotate(q).MultiplyPoint3x4(localBoxPositions[i]) + base.transform.position, 0.5f);
			}
		}
	}
}
