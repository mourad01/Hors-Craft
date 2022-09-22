// DecompilerFi decompiler from Assembly-CSharp.dll class: Crosshair
using com.ootii.Cameras;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
	public enum HitState
	{
		IN_CAMERA,
		IN_AREA,
		HITTED,
		OUT_OF_RANGE
	}

	private class Hull
	{
		public List<Vector3> points;

		public GameObject target;

		public float distance;

		public HitState state;
	}

	public static Crosshair instance;

	public float distance = 20f;

	public float radius = 0.1f;

	public int maxRaycasts = 30;

	public float LODsize = 20f;

	private HashSet<GameObject> visibleEnemies = new HashSet<GameObject>();

	private GameObject autoaim;

	private Rect aimRect;

	private List<Hull> hulls = new List<Hull>();

	private int hullCount;

	private Vector3[] boundPoints = new Vector3[8];

	private List<Vector3> screenPoints = new List<Vector3>(8);

	private Camera cam;

	private Vector3 p0;

	private void Start()
	{
		instance = this;
		cam = CameraController.instance.MainCamera;
	}

	public void SetParameters(WeaponHandler weapon)
	{
		distance = weapon.distance;
		radius = weapon.range * (float)cam.pixelHeight;
	}

	public void GetAllTargets(List<GameObject> result)
	{
		result.Clear();
		foreach (Hull hull in hulls)
		{
			if (hull.state == HitState.HITTED)
			{
				result.Add(hull.target);
			}
		}
	}

	public bool IsOutOfRange()
	{
		if (hulls.Count > 0)
		{
			for (int i = 0; i < hulls.Count; i++)
			{
				if (hulls[i].state != 0)
				{
					if (hulls[i].state == HitState.HITTED)
					{
						return false;
					}
					if (hulls[i].state == HitState.OUT_OF_RANGE)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public GameObject GetClosestTarget()
	{
		foreach (Hull hull in hulls)
		{
			if (hull.state == HitState.HITTED)
			{
				return hull.target;
			}
		}
		return null;
	}

	public void AddEnemy(GameObject enemy)
	{
		visibleEnemies.Add(enemy);
	}

	public void RemoveEnemy(GameObject enemy)
	{
		visibleEnemies.Remove(enemy);
	}

	private void LateUpdate()
	{
	}

	public void FindTargets()
	{
		foreach (Hull hull in hulls)
		{
			hull.points.Clear();
			hull.distance = float.PositiveInfinity;
			hull.state = HitState.IN_CAMERA;
		}
		FindConvexHulls();
		FindIntersections();
		DoRaycasts();
	}

	private void DoRaycasts()
	{
		int num = 0;
		for (int i = 0; i < hullCount; i++)
		{
			Hull hull = hulls[i];
			bool flag = false;
			float num2 = 0f;
			if (hull.state == HitState.IN_AREA)
			{
				Vector3 pos = new Vector3(0f, 0f, 0f);
				foreach (Vector3 point in hull.points)
				{
					Vector3 current = point;
					pos.x += current.x;
					pos.y += current.y;
				}
				pos.x /= hull.points.Count;
				pos.y /= hull.points.Count;
				if (pos.x >= 0f && pos.y >= 0f && pos.x < (float)cam.pixelWidth && pos.y < (float)cam.pixelHeight)
				{
					Ray ray = cam.ScreenPointToRay(pos);
					if (Physics.Raycast(ray, out RaycastHit hitInfo, 3f * distance))
					{
						CrosshairTarget componentInParent = hitInfo.collider.gameObject.GetComponentInParent<CrosshairTarget>();
						if ((bool)componentInParent)
						{
							flag = (hull.target == componentInParent.gameObject);
							num2 = hitInfo.distance;
						}
					}
					num++;
				}
			}
			if (flag)
			{
				if (num2 <= distance)
				{
					hull.state = HitState.HITTED;
				}
				else if (hull.state == HitState.IN_AREA)
				{
					hull.state = HitState.OUT_OF_RANGE;
				}
			}
			if (num >= maxRaycasts)
			{
				break;
			}
		}
	}

	private void FindIntersections()
	{
		Vector3 vector = new Vector3(0.5f * (float)cam.pixelWidth, 0.5f * (float)cam.pixelHeight, cam.nearClipPlane);
		for (int i = 0; i < hullCount; i++)
		{
			Hull hull = hulls[i];
			for (int j = 0; j < hull.points.Count; j++)
			{
				Vector3 b = hull.points[j];
				Vector3 a = hull.points[(j + 1) % hull.points.Count];
				Vector3 vector2 = vector - b;
				Vector3 a2 = a - b;
				float d = Mathf.Clamp01((vector2.x * a2.x + vector2.y * a2.y) / (a2.x * a2.x + a2.y * a2.y));
				Vector3 a3 = d * a2 + b;
				if ((a3 - vector).magnitude < radius)
				{
					hull.state = HitState.IN_AREA;
					break;
				}
			}
			if (hull.state == HitState.IN_CAMERA && Inside(vector, hull.points))
			{
				hull.state = HitState.IN_AREA;
			}
		}
	}

	private void FindConvexHulls()
	{
		hullCount = 0;
		foreach (GameObject visibleEnemy in visibleEnemies)
		{
			Vector3 vector = cam.WorldToScreenPoint(visibleEnemy.transform.position);
			if (vector.z <= distance)
			{
				MeshRenderer[] componentsInChildren = visibleEnemy.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					Bounds bounds = meshRenderer.bounds;
					GetPoints(bounds, boundPoints);
					screenPoints.Clear();
					Vector3[] array = boundPoints;
					foreach (Vector3 position in array)
					{
						screenPoints.Add(cam.WorldToScreenPoint(position));
					}
					if (hullCount >= hulls.Count)
					{
						AddHull();
					}
					hulls[hullCount].target = visibleEnemy;
					hulls[hullCount].distance = vector.z;
					hulls[hullCount].state = HitState.IN_CAMERA;
					GrahamScan(screenPoints, hulls[hullCount].points);
					hullCount++;
				}
			}
			else if (vector.z <= 2f * distance)
			{
				MeshRenderer[] componentsInChildren2 = visibleEnemy.GetComponentsInChildren<MeshRenderer>();
				screenPoints.Clear();
				MeshRenderer[] array2 = componentsInChildren2;
				foreach (MeshRenderer meshRenderer2 in array2)
				{
					Bounds bounds2 = meshRenderer2.bounds;
					GetPoints(bounds2, boundPoints);
					Vector3[] array3 = boundPoints;
					foreach (Vector3 position2 in array3)
					{
						screenPoints.Add(cam.WorldToScreenPoint(position2));
					}
				}
				if (hullCount >= hulls.Count)
				{
					AddHull();
				}
				hulls[hullCount].target = visibleEnemy;
				hulls[hullCount].distance = vector.z;
				hulls[hullCount].state = HitState.IN_CAMERA;
				GrahamScan(screenPoints, hulls[hullCount].points);
				hullCount++;
			}
		}
		hulls.Sort(HullComparer);
	}

	private void AddHull()
	{
		hulls.Add(new Hull
		{
			points = new List<Vector3>()
		});
	}

	private int HullComparer(Hull left, Hull right)
	{
		return (!(left.distance < right.distance)) ? 1 : (-1);
	}

	private void GiftWrapping(List<Vector3> points, List<Vector3> hull)
	{
		Vector3 vector = points[0];
		foreach (Vector3 point in points)
		{
			Vector3 current = point;
			if (current.x < vector.x)
			{
				vector = current;
			}
		}
		hull.Clear();
		int num = 0;
		Vector3 vector2 = vector;
		Vector3 vector3 = points[0];
		do
		{
			hull.Add(vector2);
			vector3 = points[0];
			for (int i = 0; i < points.Count; i++)
			{
				if (vector3 == vector2 || ccw(hull[num], vector3, points[i]) > 0f)
				{
					vector3 = points[i];
				}
			}
			num++;
			vector2 = vector3;
		}
		while (vector3 != hull[0]);
	}

	private float ccw(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		return (p2.x - p1.x) * (p3.y - p1.y) - (p2.y - p1.y) * (p3.x - p1.x);
	}

	private void InsertionSort(List<Vector3> points)
	{
		for (int i = 1; i < points.Count; i++)
		{
			int num = i;
			while (num > 0 && ComparePoints(points[num - 1], points[num]) > 0)
			{
				Vector3 value = points[num];
				points[num] = points[num - 1];
				points[num - 1] = value;
				num--;
			}
		}
	}

	private void GrahamScan(List<Vector3> points, List<Vector3> hull)
	{
		p0 = points[0];
		int index = 0;
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 vector = points[i];
			if (vector.y < p0.y || (vector.y == p0.y && vector.x < p0.y))
			{
				p0 = vector;
				index = i;
			}
		}
		Vector3 value = points[0];
		points[0] = points[index];
		points[index] = value;
		InsertionSort(points);
		hull.Add(points[0]);
		hull.Add(points[1]);
		hull.Add(points[2]);
		for (int j = 3; j < points.Count; j++)
		{
			while (hull.Count >= 2 && ccw(hull[hull.Count - 2], hull[hull.Count - 1], points[j]) >= 0f)
			{
				hull.RemoveAt(hull.Count - 1);
			}
			hull.Add(points[j]);
		}
	}

	private int ComparePoints(Vector3 q, Vector3 r)
	{
		float num = ccw(p0, q, r);
		if (num == 0f)
		{
			return (int)((q.x - p0.x) * (q.x - p0.x) + (q.y - p0.y) * (q.y - p0.y) - ((r.x - p0.x) * (r.x - p0.x) + (r.y - p0.y) * (r.y - p0.y)));
		}
		return (num > 0f) ? 1 : (-1);
	}

	private void GetPoints(Bounds bounds, Vector3[] points)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		Vector3 vector = default(Vector3);
		vector.x = center.x - extents.x;
		vector.y = center.y - extents.y;
		vector.z = center.z - extents.z;
		points[0] = vector;
		vector.x = center.x + extents.x;
		vector.y = center.y - extents.y;
		vector.z = center.z - extents.z;
		points[1] = vector;
		vector.x = center.x - extents.x;
		vector.y = center.y + extents.y;
		vector.z = center.z - extents.z;
		points[2] = vector;
		vector.x = center.x - extents.x;
		vector.y = center.y - extents.y;
		vector.z = center.z + extents.z;
		points[3] = vector;
		vector.x = center.x - extents.x;
		vector.y = center.y + extents.y;
		vector.z = center.z + extents.z;
		points[4] = vector;
		vector.x = center.x + extents.x;
		vector.y = center.y - extents.y;
		vector.z = center.z + extents.z;
		points[5] = vector;
		vector.x = center.x + extents.x;
		vector.y = center.y + extents.y;
		vector.z = center.z - extents.z;
		points[6] = vector;
		vector.x = center.x + extents.x;
		vector.y = center.y + extents.y;
		vector.z = center.z + extents.z;
		points[7] = vector;
	}

	private bool Inside(Vector3 p, List<Vector3> convex)
	{
		for (int i = 0; i < convex.Count; i++)
		{
			Vector3 p2 = convex[i];
			Vector3 p3 = convex[(i + 1) % convex.Count];
			if (ccw(p2, p, p3) < 0f)
			{
				return false;
			}
		}
		return true;
	}

	private void OnDrawGizmos()
	{
		if (!(cam != null))
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < hullCount; i++)
		{
			Hull hull = hulls[i];
			if (hull.state == HitState.HITTED)
			{
				Gizmos.color = Color.red;
			}
			else if (hull.state == HitState.OUT_OF_RANGE)
			{
				Gizmos.color = Color.magenta;
			}
			else if (hull.state == HitState.IN_AREA)
			{
				Gizmos.color = Color.green;
			}
			else
			{
				Gizmos.color = Color.blue;
			}
			flag = (flag || hull.state == HitState.HITTED);
			for (int j = 0; j < hull.points.Count; j++)
			{
				Camera camera = cam;
				Vector3 vector = hull.points[j];
				float x = vector.x;
				Vector3 vector2 = hull.points[j];
				Vector3 from = camera.ScreenToWorldPoint(new Vector3(x, vector2.y, cam.nearClipPlane + 0.001f));
				Camera camera2 = cam;
				Vector3 vector3 = hull.points[(j + 1) % hull.points.Count];
				float x2 = vector3.x;
				Vector3 vector4 = hull.points[(j + 1) % hull.points.Count];
				Vector3 to = camera2.ScreenToWorldPoint(new Vector3(x2, vector4.y, cam.nearClipPlane + 0.001f));
				Gizmos.DrawLine(from, to);
			}
		}
		Gizmos.color = ((!flag) ? Color.green : Color.red);
		int num = 40;
		Vector3 vector5 = new Vector3(0.5f * (float)cam.pixelWidth, 0.5f * (float)cam.pixelHeight, cam.nearClipPlane);
		for (int k = 0; k < num; k++)
		{
			float f = (float)Math.PI * 2f * (float)k / (float)num;
			float f2 = (float)Math.PI * 2f * (float)(k + 1) / (float)num;
			float x3 = vector5.x + Mathf.Cos(f) * radius;
			float y = vector5.y + Mathf.Sin(f) * radius;
			float x4 = vector5.x + Mathf.Cos(f2) * radius;
			float y2 = vector5.y + Mathf.Sin(f2) * radius;
			Gizmos.DrawLine(cam.ScreenToWorldPoint(new Vector3(x3, y, cam.nearClipPlane + 0.1f)), cam.ScreenToWorldPoint(new Vector3(x4, y2, cam.nearClipPlane + 0.1f)));
		}
	}
}
