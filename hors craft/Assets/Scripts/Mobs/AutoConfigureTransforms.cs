// DecompilerFi decompiler from Assembly-CSharp.dll class: Mobs.AutoConfigureTransforms
using Common.Utils;
using UnityEngine;

namespace Mobs
{
	public class AutoConfigureTransforms
	{
		private const float GROUND_INDICATOR_LENGTH = 0.3f;

		private const float OBSTACLE_INDICATOR_LENGTH = 0.6f;

		private const float STEP = 0.01f;

		public static void AutoconfigureTransforms(Mob mob)
		{
			TriggerIndicator triggerIndicator = FindOrCreateIndicator(mob, "GroundedIndicator");
			TriggerIndicator triggerIndicator2 = FindOrCreateIndicator(mob, "ObstacleForwardIndicator");
			Bounds bounds = FindMaxBoundsWithoutIndicators(mob);
			Bounds bounds2 = bounds;
			Vector3 center = bounds2.center;
			Vector3 down = Vector3.down;
			Vector3 extents = bounds.extents;
			bounds2.center = center + down * extents.y;
			Vector3 size = bounds2.size;
			float x = size.x;
			Vector3 size2 = bounds2.size;
			bounds2.size = new Vector3(x, 0.3f, size2.z);
			SetBoxColliderSizeWithWorldBounds(triggerIndicator.gameObject, bounds2);
			mob.groundOffset = mob.transform.InverseTransformPoint(bounds2.center);
			Bounds bounds3 = bounds;
			Vector3 center2 = bounds3.center;
			Vector3 forward = Vector3.forward;
			Vector3 extents2 = bounds.extents;
			bounds3.center = center2 + forward * extents2.z;
			bounds3.center += Vector3.up * 0.001f;
			Vector3 size3 = bounds3.size;
			float x2 = size3.x;
			Vector3 size4 = bounds3.size;
			bounds3.size = new Vector3(x2, size4.y, 0.6f);
			SetBoxColliderSizeWithWorldBounds(triggerIndicator2.gameObject, bounds3);
			mob.groundIndicator = triggerIndicator;
			mob.forwardObstacleIndicator = triggerIndicator2;
		}

		private static TriggerIndicator FindOrCreateIndicator(Mob mob, string name)
		{
			Transform transform = mob.transform.Find(name);
			if (transform != null && transform.GetComponent<TriggerIndicator>() != null)
			{
				return transform.GetComponent<TriggerIndicator>();
			}
			transform = new GameObject(name).transform;
			transform.SetParent(mob.transform, worldPositionStays: false);
			return transform.gameObject.AddComponent<TriggerIndicator>();
		}

		private static Bounds FindMaxBoundsWithoutIndicators(Mob mob)
		{
			Collider[] componentsInChildren = mob.GetComponentsInChildren<Collider>();
			bool flag = false;
			Bounds result = default(Bounds);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].isTrigger || componentsInChildren[i].GetComponent<TriggerIndicator>() != null)
				{
					continue;
				}
				if (!flag)
				{
					result = componentsInChildren[i].bounds;
					flag = true;
					continue;
				}
				Bounds bounds = componentsInChildren[i].bounds;
				while (true)
				{
					Vector3 min = bounds.min;
					float x = min.x;
					Vector3 min2 = result.min;
					if (x < min2.x)
					{
						result.min += new Vector3(-0.01f, 0f, 0f);
						continue;
					}
					break;
				}
				while (true)
				{
					Vector3 min3 = bounds.min;
					float y = min3.y;
					Vector3 min4 = result.min;
					if (y < min4.y)
					{
						result.min += new Vector3(0f, -0.01f, 0f);
						continue;
					}
					break;
				}
				while (true)
				{
					Vector3 min5 = bounds.min;
					float z = min5.z;
					Vector3 min6 = result.min;
					if (z < min6.z)
					{
						result.min += new Vector3(0f, 0f, -0.01f);
						continue;
					}
					break;
				}
				while (true)
				{
					Vector3 max = bounds.max;
					float x2 = max.x;
					Vector3 max2 = result.max;
					if (x2 > max2.x)
					{
						result.max += new Vector3(0.01f, 0f, 0f);
						continue;
					}
					break;
				}
				while (true)
				{
					Vector3 max3 = bounds.max;
					float y2 = max3.y;
					Vector3 max4 = result.max;
					if (y2 > max4.y)
					{
						result.max += new Vector3(0f, 0.01f, 0f);
						continue;
					}
					break;
				}
				while (true)
				{
					Vector3 max5 = bounds.max;
					float z2 = max5.z;
					Vector3 max6 = result.max;
					if (!(z2 > max6.z))
					{
						break;
					}
					result.max += new Vector3(0f, 0f, 0.01f);
				}
			}
			return result;
		}

		private static void SetBoxColliderSizeWithWorldBounds(GameObject go, Bounds bounds)
		{
			BoxCollider boxCollider = go.GetComponent<BoxCollider>();
			if (boxCollider == null)
			{
				boxCollider = go.AddComponent<BoxCollider>();
			}
			boxCollider.isTrigger = true;
			boxCollider.center = go.transform.InverseTransformPoint(bounds.center);
			boxCollider.size = go.transform.InverseTransformVector(bounds.size);
		}

		public static void AutoconfigureBoxCollider(Mob mob)
		{
			Bounds bounds = RenderersBounds.MaximumBounds(mob.gameObject);
			BoxCollider boxCollider = mob.gameObject.GetComponent<BoxCollider>();
			if (boxCollider == null)
			{
				boxCollider = mob.gameObject.AddComponent<BoxCollider>();
			}
			boxCollider.center = bounds.center;
			boxCollider.size = bounds.size;
		}
	}
}
