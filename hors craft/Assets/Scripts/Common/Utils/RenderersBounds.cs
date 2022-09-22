// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.RenderersBounds
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Utils
{
	public static class RenderersBounds
	{
		public static Bounds MaximumBounds(GameObject gameObject)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MaxValue;
			float num4 = float.MinValue;
			float num5 = float.MinValue;
			float num6 = float.MinValue;
			Renderer[] componentsInChildren = gameObject.transform.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (!(renderer is ParticleSystemRenderer) && !(renderer is TrailRenderer))
				{
					Vector3 vector = renderer.bounds.min - gameObject.transform.position;
					Vector3 vector2 = renderer.bounds.max - gameObject.transform.position;
					num = Mathf.Min(num, vector.x);
					num2 = Mathf.Min(num2, vector.y);
					num3 = Mathf.Min(num3, vector.z);
					num4 = Mathf.Max(num4, vector2.x);
					num5 = Mathf.Max(num5, vector2.y);
					num6 = Mathf.Max(num6, vector2.z);
				}
			}
			Vector3 vector3 = new Vector3(num, num2, num3);
			Vector3 vector4 = new Vector3(num4, num5, num6);
			Vector3 center = (vector3 + vector4) / 2f;
			Vector3 size = vector4 - vector3;
			return new Bounds(center, size);
		}

		public static Vector3 MiddlePoint(GameObject gameObject)
		{
			return MaximumBounds(gameObject).center;
		}

		public static Rect Calculate3DElementRectOnUI(GameObject element, Camera cam, float rectMultiplier = 1f)
		{
			Bounds bounds = MaximumBounds(element);
			bounds.center += element.transform.position;
			List<Vector3> extentsOnCanvas = new List<Vector3>();
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = list;
			Vector3 center = bounds.center;
			Vector3 extents = bounds.extents;
			float x = 0f - extents.x;
			Vector3 extents2 = bounds.extents;
			float y = 0f - extents2.y;
			Vector3 extents3 = bounds.extents;
			list2.Add(center + new Vector3(x, y, 0f - extents3.z));
			List<Vector3> list3 = list;
			Vector3 center2 = bounds.center;
			Vector3 extents4 = bounds.extents;
			float x2 = 0f - extents4.x;
			Vector3 extents5 = bounds.extents;
			float y2 = 0f - extents5.y;
			Vector3 extents6 = bounds.extents;
			list3.Add(center2 + new Vector3(x2, y2, extents6.z));
			List<Vector3> list4 = list;
			Vector3 center3 = bounds.center;
			Vector3 extents7 = bounds.extents;
			float x3 = 0f - extents7.x;
			Vector3 extents8 = bounds.extents;
			float y3 = extents8.y;
			Vector3 extents9 = bounds.extents;
			list4.Add(center3 + new Vector3(x3, y3, 0f - extents9.z));
			List<Vector3> list5 = list;
			Vector3 center4 = bounds.center;
			Vector3 extents10 = bounds.extents;
			float x4 = 0f - extents10.x;
			Vector3 extents11 = bounds.extents;
			float y4 = extents11.y;
			Vector3 extents12 = bounds.extents;
			list5.Add(center4 + new Vector3(x4, y4, extents12.z));
			List<Vector3> list6 = list;
			Vector3 center5 = bounds.center;
			Vector3 extents13 = bounds.extents;
			float x5 = extents13.x;
			Vector3 extents14 = bounds.extents;
			float y5 = 0f - extents14.y;
			Vector3 extents15 = bounds.extents;
			list6.Add(center5 + new Vector3(x5, y5, 0f - extents15.z));
			List<Vector3> list7 = list;
			Vector3 center6 = bounds.center;
			Vector3 extents16 = bounds.extents;
			float x6 = extents16.x;
			Vector3 extents17 = bounds.extents;
			float y6 = 0f - extents17.y;
			Vector3 extents18 = bounds.extents;
			list7.Add(center6 + new Vector3(x6, y6, extents18.z));
			List<Vector3> list8 = list;
			Vector3 center7 = bounds.center;
			Vector3 extents19 = bounds.extents;
			float x7 = extents19.x;
			Vector3 extents20 = bounds.extents;
			float y7 = extents20.y;
			Vector3 extents21 = bounds.extents;
			list8.Add(center7 + new Vector3(x7, y7, 0f - extents21.z));
			List<Vector3> list9 = list;
			Vector3 center8 = bounds.center;
			Vector3 extents22 = bounds.extents;
			float x8 = extents22.x;
			Vector3 extents23 = bounds.extents;
			float y8 = extents23.y;
			Vector3 extents24 = bounds.extents;
			list9.Add(center8 + new Vector3(x8, y8, extents24.z));
			list.ForEach(delegate(Vector3 point)
			{
				extentsOnCanvas.Add(cam.WorldToScreenPoint(point));
			});
			float num = extentsOnCanvas.Min((Vector3 point) => point.x);
			float num2 = extentsOnCanvas.Min((Vector3 point) => point.y);
			float num3 = extentsOnCanvas.Max((Vector3 point) => point.x);
			float num4 = extentsOnCanvas.Max((Vector3 point) => point.y);
			float num5 = num3 - num;
			float num6 = num4 - num2;
			Rect result = default(Rect);
			Vector2 a = new Vector2(num5 * rectMultiplier - num5, num6 * rectMultiplier - num6);
			result.position = new Vector2(num, num2) - a / 2f;
			result.width = num5 * rectMultiplier;
			result.height = num6 * rectMultiplier;
			return result;
		}
	}
}
