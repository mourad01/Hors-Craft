// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Graphics.GraphicsManager
using com.ootii.Cameras;
using com.ootii.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace com.ootii.Graphics
{
	[ExecuteInEditMode]
	public class GraphicsManager : MonoBehaviour
	{
		public class Octahedron
		{
			public Vector3[] Vertices;

			public int[] Triangles;

			public Octahedron()
			{
				Vertices = CreateVertices();
				Triangles = CreateTriangles();
				Vector3[] array = new Vector3[Triangles.Length];
				for (int i = 0; i < Triangles.Length; i++)
				{
					array[i] = Vertices[Triangles[i]];
					Triangles[i] = i;
				}
				Vertices = array;
			}

			private Vector3[] CreateVertices()
			{
				int num = 3;
				float[] array = new float[18]
				{
					0f,
					0.5f,
					0f,
					0.5f,
					0f,
					0f,
					0f,
					0f,
					-0.5f,
					-0.5f,
					0f,
					0f,
					0f,
					-0f,
					0.5f,
					0f,
					-0.5f,
					-0f
				};
				Vector3[] array2 = new Vector3[array.Length / num];
				for (int i = 0; i < array.Length; i += num)
				{
					array2[i / num] = new Vector3(array[i], array[i + 1], array[i + 2]);
				}
				return array2;
			}

			private int[] CreateTriangles()
			{
				return new int[24]
				{
					1,
					2,
					0,
					2,
					3,
					0,
					3,
					4,
					0,
					0,
					4,
					1,
					5,
					2,
					1,
					5,
					3,
					2,
					5,
					4,
					3,
					5,
					1,
					4
				};
			}
		}

		private class Icosahedron
		{
			public Vector3[] Vertices;

			public int[] Triangles;

			public Icosahedron()
			{
				Vertices = CreateVertices();
				Triangles = CreateTriangles();
			}

			private Vector3[] CreateVertices()
			{
				Vector3[] array = new Vector3[12];
				float num = 0.5f;
				float num2 = (num + Mathf.Sqrt(5f)) / 2f;
				array[0] = new Vector3(num2, 0f, num);
				array[9] = new Vector3(0f - num2, 0f, num);
				array[11] = new Vector3(0f - num2, 0f, 0f - num);
				array[1] = new Vector3(num2, 0f, 0f - num);
				array[2] = new Vector3(num, num2, 0f);
				array[5] = new Vector3(num, 0f - num2, 0f);
				array[10] = new Vector3(0f - num, 0f - num2, 0f);
				array[8] = new Vector3(0f - num, num2, 0f);
				array[3] = new Vector3(0f, num, num2);
				array[7] = new Vector3(0f, num, 0f - num2);
				array[6] = new Vector3(0f, 0f - num, 0f - num2);
				array[4] = new Vector3(0f, 0f - num, num2);
				for (int i = 0; i < 12; i++)
				{
					array[i].Normalize();
				}
				return array;
			}

			private int[] CreateTriangles()
			{
				return new int[60]
				{
					1,
					2,
					0,
					2,
					3,
					0,
					3,
					4,
					0,
					4,
					5,
					0,
					5,
					1,
					0,
					6,
					7,
					1,
					2,
					1,
					7,
					7,
					8,
					2,
					2,
					8,
					3,
					8,
					9,
					3,
					3,
					9,
					4,
					9,
					10,
					4,
					10,
					5,
					4,
					10,
					6,
					5,
					6,
					1,
					5,
					6,
					11,
					7,
					7,
					11,
					8,
					8,
					11,
					9,
					9,
					11,
					10,
					10,
					11,
					6
				};
			}
		}

		private static Material mSimpleMaterial = null;

		private static List<Vector3> mVectors1 = new List<Vector3>();

		private static List<Vector3> mVectors2 = new List<Vector3>();

		private static Stopwatch mInternalTimer = new Stopwatch();

		private static List<Line> mLines = new List<Line>();

		private static List<Triangle> mTriangles = new List<Triangle>();

		private static List<Text> mText = new List<Text>();

		private static string mShader = "Hidden/GraphicsManagerUI";

		private static Font mFont = null;

		private static Dictionary<Font, TextFont> mFonts = new Dictionary<Font, TextFont>();

		private static Octahedron mOctahedron = null;

		public string _DefaultShader = "Hidden/GraphicsManagerUI";

		public Font _DefaultFont;

		public bool _DrawToSceneView = true;

		public bool _DrawToGameView = true;

		protected bool mIsSceneActive;

		private static float InternalTime
		{
			get
			{
				if (Application.isPlaying)
				{
					return Time.time;
				}
				return (float)mInternalTimer.ElapsedMilliseconds / 1000f;
			}
		}

		public string DefaultShader
		{
			get
			{
				return _DefaultShader;
			}
			set
			{
				_DefaultShader = value;
			}
		}

		public Font DefaultFont
		{
			get
			{
				return _DefaultFont;
			}
			set
			{
				_DefaultFont = value;
			}
		}

		public bool DrawToSceneView
		{
			get
			{
				return _DrawToSceneView;
			}
			set
			{
				_DrawToSceneView = value;
			}
		}

		public bool DrawToGameView
		{
			get
			{
				return _DrawToGameView;
			}
			set
			{
				_DrawToGameView = value;
			}
		}

		public int LineCount => mLines.Count;

		public int TriangleCount => mTriangles.Count;

		public int TextCount => mText.Count;

		private GraphicsManager()
		{
			mInternalTimer.Start();
		}

		public IEnumerator Start()
		{
			CreateMaterials();
			mShader = _DefaultShader;
			mFont = _DefaultFont;
			AddFont(mFont);
			WaitForEndOfFrame lWait = new WaitForEndOfFrame();
			while (true)
			{
				yield return lWait;
				if (_DrawToGameView)
				{
					RenderText();
				}
				ClearGraphics();
				ClearText();
			}
		}

		protected void OnPostRender()
		{
			if (_DrawToGameView)
			{
				RenderLines();
				RenderTriangles();
				if (!mIsSceneActive)
				{
					ClearGraphics();
				}
			}
			mIsSceneActive = false;
		}

		public static void ClearGraphics()
		{
			float internalTime = InternalTime;
			for (int num = mLines.Count - 1; num >= 0; num--)
			{
				if (mLines[num].ExpirationTime < internalTime)
				{
					Line.Release(mLines[num]);
					mLines.RemoveAt(num);
				}
			}
			for (int num2 = mTriangles.Count - 1; num2 >= 0; num2--)
			{
				if (mTriangles[num2].ExpirationTime < internalTime)
				{
					Triangle.Release(mTriangles[num2]);
					mTriangles.RemoveAt(num2);
				}
			}
		}

		public static void ClearText()
		{
			float internalTime = InternalTime;
			for (int num = mText.Count - 1; num >= 0; num--)
			{
				if (mText[num].ExpirationTime < internalTime)
				{
					Text.Release(mText[num]);
					mText.RemoveAt(num);
				}
			}
		}

		public static void DrawLine(Vector3 rStart, Vector3 rEnd, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			Line line = Line.Allocate();
			line.Transform = rTransform;
			line.Start = rStart;
			line.End = rEnd;
			line.Color = rColor;
			line.ExpirationTime = InternalTime + rDuration;
			mLines.Add(line);
		}

		public static void DrawLines(List<Vector3> rLines, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			for (int i = 1; i < rLines.Count; i++)
			{
				Line line = Line.Allocate();
				line.Transform = rTransform;
				line.Start = rLines[i - 1];
				line.End = rLines[i];
				line.Color = rColor;
				line.ExpirationTime = InternalTime + rDuration;
				mLines.Add(line);
			}
		}

		public static void DrawTriangle(Vector3 rPoint1, Vector3 rPoint2, Vector3 rPoint3, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			Triangle triangle = Triangle.Allocate();
			triangle.Transform = rTransform;
			triangle.Point1 = rPoint1;
			triangle.Point2 = rPoint2;
			triangle.Point3 = rPoint3;
			triangle.Color = rColor;
			triangle.ExpirationTime = InternalTime + rDuration;
			mTriangles.Add(triangle);
		}

		public static void DrawBox(Vector3 rCenter, float rWidth, float rHeight, float rDepth, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			float num = rWidth * 0.5f;
			float num2 = rHeight * 0.5f;
			float num3 = rDepth * 0.5f;
			DrawLine(rCenter + new Vector3(num, num2, num3), rCenter + new Vector3(0f - num, num2, num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(num, num2, num3), rCenter + new Vector3(num, 0f - num2, num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(num, num2, num3), rCenter + new Vector3(num, num2, 0f - num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(0f - num, num2, num3), rCenter + new Vector3(0f - num, 0f - num2, num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(0f - num, num2, num3), rCenter + new Vector3(0f - num, num2, 0f - num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(0f - num, num2, 0f - num3), rCenter + new Vector3(num, num2, 0f - num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(0f - num, num2, 0f - num3), rCenter + new Vector3(0f - num, 0f - num2, 0f - num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(0f - num, 0f - num2, 0f - num3), rCenter + new Vector3(0f - num, 0f - num2, num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(0f - num, 0f - num2, 0f - num3), rCenter + new Vector3(num, 0f - num2, 0f - num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(num, 0f - num2, 0f - num3), rCenter + new Vector3(num, 0f - num2, num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(num, 0f - num2, 0f - num3), rCenter + new Vector3(num, num2, 0f - num3), rColor, rTransform, rDuration);
			DrawLine(rCenter + new Vector3(num, 0f - num2, num3), rCenter + new Vector3(0f - num, 0f - num2, num3), rColor, rTransform, rDuration);
		}

		public static void DrawBox(Bounds rBounds, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			Vector3 center = rBounds.center;
			Vector3 size = rBounds.size;
			float x = size.x;
			Vector3 size2 = rBounds.size;
			float y = size2.y;
			Vector3 size3 = rBounds.size;
			DrawBox(center, x, y, size3.z, rColor, rTransform, rDuration);
		}

		public static void DrawCollider(BoxCollider rColldier, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			if (!(rColldier == null))
			{
				mVectors1.Clear();
				List<Vector3> list = mVectors1;
				Vector3 size = rColldier.size;
				float x = 0.5f * size.x;
				Vector3 size2 = rColldier.size;
				float y = 0.5f * size2.y;
				Vector3 size3 = rColldier.size;
				list.Add(new Vector3(x, y, 0.5f * size3.z) + rColldier.center);
				List<Vector3> list2 = mVectors1;
				Vector3 size4 = rColldier.size;
				float x2 = -0.5f * size4.x;
				Vector3 size5 = rColldier.size;
				float y2 = 0.5f * size5.y;
				Vector3 size6 = rColldier.size;
				list2.Add(new Vector3(x2, y2, 0.5f * size6.z) + rColldier.center);
				List<Vector3> list3 = mVectors1;
				Vector3 size7 = rColldier.size;
				float x3 = -0.5f * size7.x;
				Vector3 size8 = rColldier.size;
				float y3 = 0.5f * size8.y;
				Vector3 size9 = rColldier.size;
				list3.Add(new Vector3(x3, y3, -0.5f * size9.z) + rColldier.center);
				List<Vector3> list4 = mVectors1;
				Vector3 size10 = rColldier.size;
				float x4 = 0.5f * size10.x;
				Vector3 size11 = rColldier.size;
				float y4 = 0.5f * size11.y;
				Vector3 size12 = rColldier.size;
				list4.Add(new Vector3(x4, y4, -0.5f * size12.z) + rColldier.center);
				List<Vector3> list5 = mVectors1;
				Vector3 size13 = rColldier.size;
				float x5 = 0.5f * size13.x;
				Vector3 size14 = rColldier.size;
				float y5 = -0.5f * size14.y;
				Vector3 size15 = rColldier.size;
				list5.Add(new Vector3(x5, y5, 0.5f * size15.z) + rColldier.center);
				List<Vector3> list6 = mVectors1;
				Vector3 size16 = rColldier.size;
				float x6 = -0.5f * size16.x;
				Vector3 size17 = rColldier.size;
				float y6 = -0.5f * size17.y;
				Vector3 size18 = rColldier.size;
				list6.Add(new Vector3(x6, y6, 0.5f * size18.z) + rColldier.center);
				List<Vector3> list7 = mVectors1;
				Vector3 size19 = rColldier.size;
				float x7 = -0.5f * size19.x;
				Vector3 size20 = rColldier.size;
				float y7 = -0.5f * size20.y;
				Vector3 size21 = rColldier.size;
				list7.Add(new Vector3(x7, y7, -0.5f * size21.z) + rColldier.center);
				List<Vector3> list8 = mVectors1;
				Vector3 size22 = rColldier.size;
				float x8 = 0.5f * size22.x;
				Vector3 size23 = rColldier.size;
				float y8 = -0.5f * size23.y;
				Vector3 size24 = rColldier.size;
				list8.Add(new Vector3(x8, y8, -0.5f * size24.z) + rColldier.center);
				for (int i = 0; i < mVectors1.Count; i++)
				{
					mVectors1[i] = rColldier.transform.TransformPoint(mVectors1[i]);
				}
				DrawLine(mVectors1[0], mVectors1[1], rColor, rTransform, rDuration);
				DrawLine(mVectors1[1], mVectors1[2], rColor, rTransform, rDuration);
				DrawLine(mVectors1[2], mVectors1[3], rColor, rTransform, rDuration);
				DrawLine(mVectors1[3], mVectors1[0], rColor, rTransform, rDuration);
				DrawLine(mVectors1[4], mVectors1[5], rColor, rTransform, rDuration);
				DrawLine(mVectors1[5], mVectors1[6], rColor, rTransform, rDuration);
				DrawLine(mVectors1[6], mVectors1[7], rColor, rTransform, rDuration);
				DrawLine(mVectors1[7], mVectors1[4], rColor, rTransform, rDuration);
				DrawLine(mVectors1[0], mVectors1[4], rColor, rTransform, rDuration);
				DrawLine(mVectors1[1], mVectors1[5], rColor, rTransform, rDuration);
				DrawLine(mVectors1[2], mVectors1[6], rColor, rTransform, rDuration);
				DrawLine(mVectors1[3], mVectors1[7], rColor, rTransform, rDuration);
			}
		}

		public static void DrawSolidCollider(BoxCollider rColldier, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			if (!(rColldier == null))
			{
				mVectors1.Clear();
				List<Vector3> list = mVectors1;
				Vector3 size = rColldier.size;
				float x = 0.5f * size.x;
				Vector3 size2 = rColldier.size;
				float y = 0.5f * size2.y;
				Vector3 size3 = rColldier.size;
				list.Add(new Vector3(x, y, 0.5f * size3.z) + rColldier.center);
				List<Vector3> list2 = mVectors1;
				Vector3 size4 = rColldier.size;
				float x2 = -0.5f * size4.x;
				Vector3 size5 = rColldier.size;
				float y2 = 0.5f * size5.y;
				Vector3 size6 = rColldier.size;
				list2.Add(new Vector3(x2, y2, 0.5f * size6.z) + rColldier.center);
				List<Vector3> list3 = mVectors1;
				Vector3 size7 = rColldier.size;
				float x3 = -0.5f * size7.x;
				Vector3 size8 = rColldier.size;
				float y3 = 0.5f * size8.y;
				Vector3 size9 = rColldier.size;
				list3.Add(new Vector3(x3, y3, -0.5f * size9.z) + rColldier.center);
				List<Vector3> list4 = mVectors1;
				Vector3 size10 = rColldier.size;
				float x4 = 0.5f * size10.x;
				Vector3 size11 = rColldier.size;
				float y4 = 0.5f * size11.y;
				Vector3 size12 = rColldier.size;
				list4.Add(new Vector3(x4, y4, -0.5f * size12.z) + rColldier.center);
				List<Vector3> list5 = mVectors1;
				Vector3 size13 = rColldier.size;
				float x5 = 0.5f * size13.x;
				Vector3 size14 = rColldier.size;
				float y5 = -0.5f * size14.y;
				Vector3 size15 = rColldier.size;
				list5.Add(new Vector3(x5, y5, 0.5f * size15.z) + rColldier.center);
				List<Vector3> list6 = mVectors1;
				Vector3 size16 = rColldier.size;
				float x6 = -0.5f * size16.x;
				Vector3 size17 = rColldier.size;
				float y6 = -0.5f * size17.y;
				Vector3 size18 = rColldier.size;
				list6.Add(new Vector3(x6, y6, 0.5f * size18.z) + rColldier.center);
				List<Vector3> list7 = mVectors1;
				Vector3 size19 = rColldier.size;
				float x7 = -0.5f * size19.x;
				Vector3 size20 = rColldier.size;
				float y7 = -0.5f * size20.y;
				Vector3 size21 = rColldier.size;
				list7.Add(new Vector3(x7, y7, -0.5f * size21.z) + rColldier.center);
				List<Vector3> list8 = mVectors1;
				Vector3 size22 = rColldier.size;
				float x8 = 0.5f * size22.x;
				Vector3 size23 = rColldier.size;
				float y8 = -0.5f * size23.y;
				Vector3 size24 = rColldier.size;
				list8.Add(new Vector3(x8, y8, -0.5f * size24.z) + rColldier.center);
				for (int i = 0; i < mVectors1.Count; i++)
				{
					mVectors1[i] = rColldier.transform.TransformPoint(mVectors1[i]);
				}
				Color rColor2 = new Color(rColor.r, rColor.g, rColor.b, 0.1f);
				DrawTriangle(mVectors1[0], mVectors1[1], mVectors1[2], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[0], mVectors1[2], mVectors1[3], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[0], mVectors1[1], mVectors1[5], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[0], mVectors1[5], mVectors1[4], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[0], mVectors1[3], mVectors1[7], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[0], mVectors1[7], mVectors1[4], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[6], mVectors1[5], mVectors1[1], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[6], mVectors1[1], mVectors1[2], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[6], mVectors1[7], mVectors1[4], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[6], mVectors1[4], mVectors1[5], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[6], mVectors1[2], mVectors1[3], rColor2, rTransform, rDuration);
				DrawTriangle(mVectors1[6], mVectors1[3], mVectors1[7], rColor2, rTransform, rDuration);
				DrawLine(mVectors1[0], mVectors1[1], rColor, rTransform, rDuration);
				DrawLine(mVectors1[1], mVectors1[2], rColor, rTransform, rDuration);
				DrawLine(mVectors1[2], mVectors1[3], rColor, rTransform, rDuration);
				DrawLine(mVectors1[3], mVectors1[0], rColor, rTransform, rDuration);
				DrawLine(mVectors1[4], mVectors1[5], rColor, rTransform, rDuration);
				DrawLine(mVectors1[5], mVectors1[6], rColor, rTransform, rDuration);
				DrawLine(mVectors1[6], mVectors1[7], rColor, rTransform, rDuration);
				DrawLine(mVectors1[7], mVectors1[4], rColor, rTransform, rDuration);
				DrawLine(mVectors1[0], mVectors1[4], rColor, rTransform, rDuration);
				DrawLine(mVectors1[1], mVectors1[5], rColor, rTransform, rDuration);
				DrawLine(mVectors1[2], mVectors1[6], rColor, rTransform, rDuration);
				DrawLine(mVectors1[3], mVectors1[7], rColor, rTransform, rDuration);
			}
		}

		public static void DrawCircle(Vector3 rCenter, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			DrawCircle(rCenter, rRadius, rColor, Vector3.up, rTransform, rDuration);
		}

		public static void DrawCircle(Vector3 rCenter, float rRadius, Color rColor, Vector3 rNormal, Transform rTransform = null, float rDuration = 0f)
		{
			int num = 36;
			Vector3[] array = new Vector3[num];
			Quaternion rotation = Quaternion.AngleAxis(360f / (float)(num - 1), rNormal);
			Quaternion rotation2 = Quaternion.FromToRotation(Vector3.up, rNormal);
			Vector3 vector = rotation2 * Vector3.forward * rRadius;
			for (int i = 0; i < num; i++)
			{
				array[i] = rCenter + vector;
				vector = rotation * vector;
			}
			for (int j = 1; j < num; j++)
			{
				DrawLine(array[j - 1], array[j], rColor, rTransform, rDuration);
			}
		}

		public static void DrawSolidCircle(Vector3 rCenter, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			DrawSolidCircle(rCenter, rRadius, rColor, Vector3.up, rTransform, rDuration);
		}

		public static void DrawSolidCircle(Vector3 rCenter, float rRadius, Color rColor, Vector3 rNormal, Transform rTransform = null, float rDuration = 0f)
		{
			int num = 36;
			mVectors1.Clear();
			Quaternion rotation = Quaternion.AngleAxis(360f / (float)(num - 1), rNormal);
			Vector3 vector = Vector3.forward * rRadius;
			for (int i = 0; i < num; i++)
			{
				mVectors1.Add(rCenter + vector);
				vector = rotation * vector;
			}
			Color rColor2 = new Color(rColor.r, rColor.g, rColor.b, 0.1f);
			for (int j = 1; j < num; j++)
			{
				DrawLine(mVectors1[j - 1], mVectors1[j], rColor, rTransform, rDuration);
				DrawTriangle(rCenter, mVectors1[j - 1], mVectors1[j], rColor2, rTransform, rDuration);
			}
		}

		public static void DrawSolidCone(Vector3 rPosition, Vector3 rDirection, float rDistance, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			Vector3 a = Vector3.Cross(rDirection, Vector3.up);
			Vector3 vector = rPosition + rDirection * rDistance;
			int num = 36;
			mVectors1.Clear();
			Quaternion rotation = Quaternion.AngleAxis(360f / (float)(num - 1), rDirection);
			Vector3 vector2 = a * rRadius;
			for (int i = 0; i < num; i++)
			{
				mVectors1.Add(vector + vector2);
				vector2 = rotation * vector2;
			}
			Color rColor2 = new Color(rColor.r, rColor.g, rColor.b, 0.1f);
			for (int j = 1; j < num; j++)
			{
				DrawLine(mVectors1[j - 1], mVectors1[j], rColor, rTransform, rDuration);
				DrawTriangle(vector, mVectors1[j - 1], mVectors1[j], rColor2, rTransform, rDuration);
				DrawTriangle(rPosition, mVectors1[j - 1], mVectors1[j], rColor2, rTransform, rDuration);
			}
		}

		public static void DrawArc(Vector3 rCenter, Vector3 rFrom, float rAngle, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			DrawArc(rCenter, Vector3.up, rFrom, rAngle, rRadius, rColor, rTransform, rDuration);
		}

		public static void DrawArc(Vector3 rCenter, Vector3 rNormal, Vector3 rFrom, float rAngle, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			int num = 36;
			Vector3[] array = new Vector3[num];
			Quaternion rotation = Quaternion.AngleAxis(rAngle / (float)(num - 1), rNormal);
			Vector3 vector = rFrom.normalized * rRadius;
			for (int i = 0; i < num; i++)
			{
				array[i] = rCenter + vector;
				vector = rotation * vector;
			}
			for (int j = 1; j < num; j++)
			{
				DrawLine(array[j - 1], array[j], rColor, rTransform, rDuration);
			}
		}

		public static void DrawSolidArc(Vector3 rCenter, Vector3 rFrom, float rAngle, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			DrawSolidArc(rCenter, Vector3.up, rFrom, rAngle, rRadius, rColor, rTransform, rDuration);
		}

		public static void DrawSolidArc(Vector3 rCenter, Vector3 rNormal, Vector3 rFrom, float rAngle, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			int num = 36;
			Vector3[] array = new Vector3[num];
			Quaternion rotation = Quaternion.AngleAxis(rAngle / (float)(num - 1), rNormal);
			Vector3 vector = rFrom.normalized * rRadius;
			for (int i = 0; i < num; i++)
			{
				array[i] = rCenter + vector;
				vector = rotation * vector;
			}
			for (int j = 1; j < num; j++)
			{
				DrawTriangle(rCenter, array[j - 1], array[j], rColor, rTransform, rDuration);
			}
		}

		public static void DrawSolidCenteredArc(Vector3 rCenter, Vector3 rFrom, float rAngle, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			DrawSolidCenteredArc(rCenter, Vector3.up, rFrom, rAngle, rRadius, rColor, rTransform, rDuration);
		}

		public static void DrawSolidCenteredArc(Vector3 rCenter, Vector3 rNormal, Vector3 rFrom, float rAngle, float rRadius, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			int num = 36;
			Vector3[] array = new Vector3[num];
			Quaternion rotation = Quaternion.AngleAxis(rAngle / (float)(num - 1), rNormal);
			Vector3 vector = Quaternion.AngleAxis((0f - rAngle) * 0.5f, rNormal) * (rFrom.normalized * rRadius);
			for (int i = 0; i < num; i++)
			{
				array[i] = rCenter + vector;
				vector = rotation * vector;
			}
			for (int j = 1; j < num; j++)
			{
				DrawTriangle(rCenter, array[j - 1], array[j], rColor, rTransform, rDuration);
			}
		}

		public static void DrawArrow(Vector3 rStart, Vector3 rEnd, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			Line line = Line.Allocate();
			line.Transform = rTransform;
			line.Start = rStart;
			line.End = rEnd;
			line.Color = rColor;
			line.ExpirationTime = InternalTime + rDuration;
			mLines.Add(line);
			DrawPoint(rEnd, rColor, rTransform, rDuration);
		}

		public static void DrawFrustum(Vector3 rPosition, Quaternion rRotation, float rHAngle, float rVAngle, float rMinDistance, float rMaxDistance, Color rColor, bool rIsSpherical = true)
		{
			if (rHAngle == 0f || rVAngle == 0f || rMaxDistance == 0f)
			{
				return;
			}
			int num = 10;
			int num2 = num + 1;
			float num3 = rHAngle * 0.5f;
			float num4 = rVAngle * 0.5f;
			Vector3 zero = Vector3.zero;
			mVectors1.Clear();
			mVectors2.Clear();
			for (float num5 = 0f - num4; num5 <= num4; num5 += rVAngle / (float)num)
			{
				float f = 0f - num5 * ((float)Math.PI / 180f);
				float num6 = Mathf.Sin(f);
				for (float num7 = 0f - num3; num7 <= num3; num7 += rHAngle / (float)num)
				{
					float f2 = 0f - num7 * ((float)Math.PI / 180f) + 1.57079f;
					float num8 = Mathf.Cos(f2) * ((!rIsSpherical) ? 1f : Mathf.Cos(f));
					float num9 = Mathf.Sin(f2) * ((!rIsSpherical) ? 1f : Mathf.Cos(f));
					zero.x = rMinDistance * num8;
					zero.y = rMinDistance * num6;
					zero.z = rMinDistance * num9;
					mVectors1.Add(rPosition + rRotation * zero);
					zero.x = rMaxDistance * num8;
					zero.y = rMaxDistance * num6;
					zero.z = rMaxDistance * num9;
					mVectors2.Add(rPosition + rRotation * zero);
				}
			}
			if (rVAngle < 360f)
			{
				for (int i = 0; i < num; i++)
				{
					DrawLine(mVectors2[i], mVectors2[i + 1], rColor);
					DrawLine(mVectors2[num * num2 + i], mVectors2[num * num2 + i + 1], rColor);
					DrawLine(mVectors1[i], mVectors1[i + 1], rColor);
					DrawLine(mVectors1[num * num2 + i], mVectors1[num * num2 + i + 1], rColor);
				}
			}
			if (rHAngle < 360f)
			{
				for (int j = 0; j < num; j++)
				{
					DrawLine(mVectors2[j * num2], mVectors2[(j + 1) * num2], rColor);
					DrawLine(mVectors2[j * num2 + num], mVectors2[(j + 1) * num2 + num], rColor);
					DrawLine(mVectors1[j * num2], mVectors1[(j + 1) * num2], rColor);
					DrawLine(mVectors1[j * num2 + num], mVectors1[(j + 1) * num2 + num], rColor);
					DrawLine(mVectors1[j * num2 + num / 2], mVectors1[(j + 1) * num2 + num / 2], rColor);
					DrawLine(mVectors2[j * num2 + num / 2], mVectors2[(j + 1) * num2 + num / 2], rColor);
				}
			}
			if (rHAngle < 360f && rVAngle < 360f)
			{
				DrawLine(mVectors1[0], mVectors2[0], rColor);
				DrawLine(mVectors1[num], mVectors2[num], rColor);
				DrawLine(mVectors1[num * num2], mVectors2[num * num2], rColor);
				DrawLine(mVectors1[num * num2 + num], mVectors2[num * num2 + num], rColor);
			}
		}

		public static void DrawSolidFrustum(Vector3 rPosition, Quaternion rRotation, float rHAngle, float rVAngle, float rMinDistance, float rMaxDistance, Color rColor, bool rIsSpherical = true)
		{
			if (rHAngle == 0f || rVAngle == 0f || rMaxDistance == 0f)
			{
				return;
			}
			int num = 10;
			int num2 = num + 1;
			float num3 = rHAngle * 0.5f;
			float num4 = rVAngle * 0.5f;
			Vector3 zero = Vector3.zero;
			mVectors1.Clear();
			mVectors2.Clear();
			for (float num5 = 0f - num4; num5 <= num4; num5 += rVAngle / (float)num)
			{
				float f = 0f - num5 * ((float)Math.PI / 180f);
				float num6 = Mathf.Sin(f);
				for (float num7 = 0f - num3; num7 <= num3; num7 += rHAngle / (float)num)
				{
					float f2 = 0f - num7 * ((float)Math.PI / 180f) + 1.57079f;
					float num8 = Mathf.Cos(f2) * ((!rIsSpherical) ? 1f : Mathf.Cos(f));
					float num9 = Mathf.Sin(f2) * ((!rIsSpherical) ? 1f : Mathf.Cos(f));
					zero.x = rMinDistance * num8;
					zero.y = rMinDistance * num6;
					zero.z = rMinDistance * num9;
					mVectors1.Add(rPosition + rRotation * zero);
					zero.x = rMaxDistance * num8;
					zero.y = rMaxDistance * num6;
					zero.z = rMaxDistance * num9;
					mVectors2.Add(rPosition + rRotation * zero);
				}
			}
			Color rColor2 = new Color(rColor.r, rColor.g, rColor.b, 0.1f);
			if (rMinDistance > 0f)
			{
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num; j++)
					{
						DrawTriangle(mVectors1[(i + 1) * num2 + j], mVectors1[i * num2 + j], mVectors1[(i + 1) * num2 + j + 1], rColor2);
						DrawTriangle(mVectors1[i * num2 + j], mVectors1[i * num2 + j + 1], mVectors1[(i + 1) * num2 + j + 1], rColor2);
					}
				}
			}
			for (int k = 0; k < num; k++)
			{
				for (int l = 0; l < num; l++)
				{
					DrawTriangle(mVectors2[(k + 1) * num2 + l], mVectors2[k * num2 + l], mVectors2[(k + 1) * num2 + l + 1], rColor2);
					DrawTriangle(mVectors2[k * num2 + l], mVectors2[k * num2 + l + 1], mVectors2[(k + 1) * num2 + l + 1], rColor2);
				}
			}
			if (rVAngle < 360f)
			{
				for (int m = 0; m < num; m++)
				{
					DrawTriangle(mVectors2[m], mVectors1[m], mVectors2[m + 1], rColor2);
					DrawTriangle(mVectors1[m], mVectors1[m + 1], mVectors2[m + 1], rColor2);
				}
				for (int n = 0; n < num; n++)
				{
					DrawTriangle(mVectors2[num * num2 + n], mVectors1[num * num2 + n], mVectors2[num * num2 + n + 1], rColor2);
					DrawTriangle(mVectors1[num * num2 + n], mVectors1[num * num2 + n + 1], mVectors2[num * num2 + n + 1], rColor2);
				}
			}
			if (rHAngle < 360f)
			{
				for (int num10 = 0; num10 < num; num10++)
				{
					DrawTriangle(mVectors2[(num10 + 1) * num2], mVectors2[num10 * num2], mVectors1[(num10 + 1) * num2], rColor2);
					DrawTriangle(mVectors2[num10 * num2], mVectors1[num10 * num2], mVectors1[(num10 + 1) * num2], rColor2);
				}
				for (int num11 = 0; num11 < num; num11++)
				{
					DrawTriangle(mVectors2[(num11 + 1) * num2 + num], mVectors2[num11 * num2 + num], mVectors1[(num11 + 1) * num2 + num], rColor2);
					DrawTriangle(mVectors2[num11 * num2 + num], mVectors1[num11 * num2 + num], mVectors1[(num11 + 1) * num2 + num], rColor2);
				}
			}
			if (rVAngle < 360f)
			{
				for (int num12 = 0; num12 < num; num12++)
				{
					DrawLine(mVectors2[num12], mVectors2[num12 + 1], rColor);
					DrawLine(mVectors2[num * num2 + num12], mVectors2[num * num2 + num12 + 1], rColor);
					DrawLine(mVectors1[num12], mVectors1[num12 + 1], rColor);
					DrawLine(mVectors1[num * num2 + num12], mVectors1[num * num2 + num12 + 1], rColor);
				}
			}
			if (rHAngle < 360f)
			{
				for (int num13 = 0; num13 < num; num13++)
				{
					DrawLine(mVectors2[num13 * num2], mVectors2[(num13 + 1) * num2], rColor);
					DrawLine(mVectors2[num13 * num2 + num], mVectors2[(num13 + 1) * num2 + num], rColor);
					DrawLine(mVectors1[num13 * num2], mVectors1[(num13 + 1) * num2], rColor);
					DrawLine(mVectors1[num13 * num2 + num], mVectors1[(num13 + 1) * num2 + num], rColor);
				}
			}
			if (rHAngle < 360f && rVAngle < 360f)
			{
				DrawLine(mVectors1[0], mVectors2[0], rColor);
				DrawLine(mVectors1[num], mVectors2[num], rColor);
				DrawLine(mVectors1[num * num2], mVectors2[num * num2], rColor);
				DrawLine(mVectors1[num * num2 + num], mVectors2[num * num2 + num], rColor);
			}
		}

		public static void DrawPoint(Vector3 rCenter, Color rColor, Transform rTransform = null, float rDuration = 0f)
		{
			if (mOctahedron == null)
			{
				mOctahedron = new Octahedron();
			}
			float d = 0.075f;
			for (int i = 0; i < mOctahedron.Triangles.Length; i += 3)
			{
				DrawTriangle(rCenter + mOctahedron.Vertices[mOctahedron.Triangles[i]] * d, rCenter + mOctahedron.Vertices[mOctahedron.Triangles[i + 1]] * d, rCenter + mOctahedron.Vertices[mOctahedron.Triangles[i + 2]] * d, rColor, rTransform, rDuration);
			}
		}

		public static void DrawQuaternion(Vector3 rCenter, Quaternion rRotation, float rScale = 1f, float rDuration = 0f)
		{
			DrawLine(rCenter, rCenter + rRotation.Forward() * rScale, Color.blue, null, rDuration);
			DrawLine(rCenter, rCenter + rRotation.Right() * rScale, Color.red, null, rDuration);
			DrawLine(rCenter, rCenter + rRotation.Up() * rScale, Color.green, null, rDuration);
		}

		public static void DrawCapsule(Vector3 rStart, Vector3 rEnd, float rRadius, Color rColor, float rDuration = 0f)
		{
			Vector3 normalized = (rEnd - rStart).normalized;
			Quaternion rotation = (normalized.sqrMagnitude != 0f) ? Quaternion.LookRotation(normalized, Vector3.up) : Quaternion.identity;
			Vector3 rNormal = rotation * Vector3.forward;
			Vector3 vector = rotation * Vector3.right;
			Vector3 vector2 = rotation * Vector3.up;
			DrawArc(rStart, rNormal, vector2, 360f, rRadius, rColor, null, rDuration);
			DrawArc(rStart, vector2, vector, 180f, rRadius, rColor, null, rDuration);
			DrawArc(rStart, vector, -vector2, 180f, rRadius, rColor, null, rDuration);
			DrawArc(rEnd, rNormal, vector2, 360f, rRadius, rColor, null, rDuration);
			DrawArc(rEnd, vector2, -vector, 180f, rRadius, rColor, null, rDuration);
			DrawArc(rEnd, vector, vector2, 180f, rRadius, rColor, null, rDuration);
			DrawLine(rStart + vector * rRadius, rEnd + vector * rRadius, rColor, null, rDuration);
			DrawLine(rStart + -vector * rRadius, rEnd + -vector * rRadius, rColor, null, rDuration);
			DrawLine(rStart + vector2 * rRadius, rEnd + vector2 * rRadius, rColor, null, rDuration);
			DrawLine(rStart + -vector2 * rRadius, rEnd + -vector2 * rRadius, rColor, null, rDuration);
		}

		public static void DrawSphere(Vector3 rCenter, float rRadius, Color rColor, float rDuration = 0f)
		{
			Vector3 forward = Vector3.forward;
			Vector3 right = Vector3.right;
			Vector3 up = Vector3.up;
			DrawArc(rCenter, forward, up, 360f, rRadius, rColor, null, rDuration);
			DrawArc(rCenter, up, right, 360f, rRadius, rColor, null, rDuration);
			DrawArc(rCenter, right, -up, 360f, rRadius, rColor, null, rDuration);
		}

		public static void DrawTexture(Texture rTexture, Vector3 rPosition, float rWidth, float rHeight)
		{
			Vector2 vector = CameraController.instance.MainCamera.WorldToScreenPoint(rPosition);
			vector.x = Mathf.Floor(vector.x);
			vector.y = Mathf.Floor(vector.y);
			Rect position = new Rect(vector.x - rWidth * 0.5f, (float)Screen.height - vector.y - rHeight * 0.5f, rWidth, rHeight);
			GUI.DrawTexture(position, rTexture);
		}

		public static void DrawTexture(Texture rTexture, Vector2 rPosition, float rWidth, float rHeight)
		{
			rPosition.x *= Screen.width;
			rPosition.y *= Screen.height;
			Rect position = new Rect(rPosition.x - rWidth * 0.5f, (float)Screen.height - rPosition.y - rHeight * 0.5f, rWidth, rHeight);
			GUI.DrawTexture(position, rTexture);
		}

		public static void DrawText(string rText, Vector3 rPosition, Color rColor, float rDuration = 0f)
		{
			DrawText(rText, rPosition, rColor, mFont, rDuration);
		}

		public static void DrawText(string rText, Vector3 rPosition, Color rColor, Font rFont, float rDuration = 0f)
		{
			if (!mFonts.ContainsKey(rFont) && !AddFont(rFont))
			{
				return;
			}
			TextFont textFont = mFonts[rFont];
			int num = Mathf.Abs(textFont.MinX);
			char[] array = rText.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				rFont.GetCharacterInfo(array[i], out CharacterInfo info);
				num += Mathf.Max(info.advance, info.glyphWidth);
			}
			int num2 = textFont.MaxY - textFont.MinY;
			Texture2D texture2D = new Texture2D(num, num2, TextureFormat.ARGB32, mipChain: false, linear: true);
			Color32[] array2 = new Color32[num * num2];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = new Color32(0, 0, 0, 0);
			}
			texture2D.SetPixels32(array2);
			int num3 = Mathf.Abs(textFont.MinX);
			int num4 = Mathf.Abs(textFont.MinY);
			for (int k = 0; k < array.Length; k++)
			{
				TextCharacter characterPixels = GetCharacterPixels(rFont, array[k]);
				if (characterPixels.Pixels != null)
				{
					for (int l = 0; l < characterPixels.Pixels.Length; l++)
					{
						rColor.a = characterPixels.Pixels[l].a;
						characterPixels.Pixels[l] = rColor;
					}
					texture2D.SetPixels(num3 + characterPixels.MinX, num4 + characterPixels.MinY, characterPixels.Width, characterPixels.Height, characterPixels.Pixels);
				}
				num3 += characterPixels.Advance;
			}
			texture2D.Apply();
			Text text = Text.Allocate();
			text.Position = rPosition;
			text.Texture = texture2D;
			text.ExpirationTime = InternalTime + rDuration;
			mText.Add(text);
		}

		public static bool AddFont(Font rFont)
		{
			if (rFont == null)
			{
				return false;
			}
			if (mFonts.ContainsKey(rFont))
			{
				return true;
			}
			Texture2D texture2D = (Texture2D)rFont.material.mainTexture;
			byte[] rawTextureData = texture2D.GetRawTextureData();
			Texture2D texture2D2 = new Texture2D(texture2D.width, texture2D.height, texture2D.format, mipChain: false);
			texture2D2.LoadRawTextureData(rawTextureData);
			texture2D2.Apply();
			TextFont textFont = TextFont.Allocate();
			textFont.Font = rFont;
			textFont.Texture = texture2D2;
			char[] array = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.,?:;~!@#$%^&*()_+-=".ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				rFont.GetCharacterInfo(array[i], out CharacterInfo info);
				if (info.minX < textFont.MinX)
				{
					textFont.MinX = info.minX;
				}
				if (info.maxX > textFont.MaxX)
				{
					textFont.MaxX = info.maxX;
				}
				if (info.minY < textFont.MinY)
				{
					textFont.MinY = info.minY;
				}
				if (info.maxY > textFont.MaxY)
				{
					textFont.MaxY = info.maxY;
				}
			}
			mFonts.Add(rFont, textFont);
			return true;
		}

		private static void RenderLines()
		{
			if (mSimpleMaterial == null)
			{
				CreateMaterials();
			}
			mSimpleMaterial.SetPass(0);
			for (int i = 0; i < mLines.Count; i++)
			{
				Line line = mLines[i];
				GL.PushMatrix();
				if (line.Transform == null)
				{
					GL.MultMatrix(Matrix4x4.identity);
				}
				else
				{
					GL.MultMatrix(line.Transform.localToWorldMatrix);
				}
				GL.Begin(1);
				GL.Color(line.Color);
				GL.Vertex3(line.Start.x, line.Start.y, line.Start.z);
				GL.Vertex3(line.End.x, line.End.y, line.End.z);
				GL.End();
				GL.PopMatrix();
			}
		}

		private static void RenderTriangles()
		{
			if (mSimpleMaterial == null)
			{
				CreateMaterials();
			}
			mSimpleMaterial.SetPass(0);
			for (int i = 0; i < mTriangles.Count; i++)
			{
				Triangle triangle = mTriangles[i];
				GL.PushMatrix();
				if (triangle.Transform == null)
				{
					GL.MultMatrix(Matrix4x4.identity);
				}
				else
				{
					GL.MultMatrix(triangle.Transform.localToWorldMatrix);
				}
				GL.Begin(4);
				GL.Color(triangle.Color);
				GL.Vertex3(triangle.Point1.x, triangle.Point1.y, triangle.Point1.z);
				GL.Vertex3(triangle.Point2.x, triangle.Point2.y, triangle.Point2.z);
				GL.Vertex3(triangle.Point3.x, triangle.Point3.y, triangle.Point3.z);
				GL.End();
				GL.PopMatrix();
			}
		}

		private static void RenderText()
		{
			for (int i = 0; i < mText.Count; i++)
			{
				Text text = mText[i];
				if (!(text.Texture == null) && !object.ReferenceEquals(text.Texture, null))
				{
					int width = text.Texture.width;
					int height = text.Texture.height;
					Vector2 vector = CameraController.instance.MainCamera.WorldToScreenPoint(text.Position);
					Rect position = new Rect(vector.x - (float)width * 0.5f, (float)Screen.height - vector.y - (float)height * 0.5f, width, height);
					GUI.DrawTexture(position, text.Texture);
				}
			}
		}

		private static void CreateMaterials()
		{
			if (!(mSimpleMaterial != null))
			{
				Shader shader = Shader.Find(mShader);
				if (shader == null)
				{
					shader = Shader.Find("Hidden/GraphicsManagerUI");
				}
				mSimpleMaterial = new Material(shader);
				mSimpleMaterial.hideFlags = HideFlags.HideAndDontSave;
				mSimpleMaterial.SetInt("_SrcBlend", 5);
				mSimpleMaterial.SetInt("_DstBlend", 10);
				mSimpleMaterial.SetInt("_Cull", 0);
				mSimpleMaterial.SetInt("_ZWrite", 0);
			}
		}

		private static TextCharacter GetCharacterPixels(Font rFont, char rCharacter)
		{
			if (!mFonts.ContainsKey(rFont))
			{
				return null;
			}
			if (mFonts[rFont].Characters.ContainsKey(rCharacter))
			{
				return mFonts[rFont].Characters[rCharacter];
			}
			Texture2D texture = mFonts[rFont].Texture;
			Vector2 zero = Vector2.zero;
			Color[] pixels = null;
			rFont.GetCharacterInfo(rCharacter, out CharacterInfo info);
			Vector2 uvBottomLeft = info.uvBottomLeft;
			float x = uvBottomLeft.x;
			Vector2 uvBottomRight = info.uvBottomRight;
			if (x == uvBottomRight.x)
			{
				Vector2 uvBottomLeft2 = info.uvBottomLeft;
				float y = uvBottomLeft2.y;
				Vector2 uvBottomRight2 = info.uvBottomRight;
				zero = ((!(y > uvBottomRight2.y)) ? info.uvBottomLeft : info.uvBottomRight);
			}
			else
			{
				Vector2 uvBottomLeft3 = info.uvBottomLeft;
				float y2 = uvBottomLeft3.y;
				Vector2 uvTopLeft = info.uvTopLeft;
				zero = ((!(y2 > uvTopLeft.y)) ? info.uvBottomLeft : info.uvTopLeft);
			}
			int x2 = (int)((float)texture.width * zero.x);
			int y3 = (int)((float)texture.height * zero.y);
			int glyphWidth = info.glyphWidth;
			int glyphHeight = info.glyphHeight;
			Vector2 uvBottomLeft4 = info.uvBottomLeft;
			float x3 = uvBottomLeft4.x;
			Vector2 uvBottomRight3 = info.uvBottomRight;
			if (x3 == uvBottomRight3.x)
			{
				Vector2 uvBottomLeft5 = info.uvBottomLeft;
				float y4 = uvBottomLeft5.y;
				Vector2 uvBottomRight4 = info.uvBottomRight;
				if (y4 > uvBottomRight4.y)
				{
					pixels = texture.GetPixels(x2, y3, glyphHeight, glyphWidth);
					pixels = RotatePixelsLeft(pixels, glyphHeight, glyphWidth);
				}
			}
			Vector2 uvBottomLeft6 = info.uvBottomLeft;
			float y5 = uvBottomLeft6.y;
			Vector2 uvTopLeft2 = info.uvTopLeft;
			if (y5 > uvTopLeft2.y)
			{
				pixels = texture.GetPixels(x2, y3, glyphWidth, glyphHeight);
				pixels = FlipPixelsVertically(pixels, glyphWidth, glyphHeight);
			}
			Vector2 uvTopLeft3 = info.uvTopLeft;
			float x4 = uvTopLeft3.x;
			Vector2 uvTopRight = info.uvTopRight;
			if (x4 > uvTopRight.x)
			{
				pixels = texture.GetPixels(x2, y3, glyphWidth, glyphHeight);
				pixels = FlipPixelsHorizontally(pixels, glyphWidth, glyphHeight);
			}
			TextCharacter textCharacter = TextCharacter.Allocate();
			textCharacter.Character = rCharacter;
			textCharacter.Pixels = pixels;
			textCharacter.MinX = info.minX;
			textCharacter.MinY = info.minY;
			textCharacter.Width = glyphWidth;
			textCharacter.Height = glyphHeight;
			textCharacter.Advance = info.advance;
			mFonts[rFont].Characters.Add(rCharacter, textCharacter);
			return textCharacter;
		}

		private static Color[] RotatePixelsLeft(Color[] rArray, int rWidth, int rHeight)
		{
			Color[] array = new Color[rArray.Length];
			for (int i = 0; i < rArray.Length; i++)
			{
				int num = i / rWidth;
				int num2 = i % rWidth;
				int num3 = num2;
				int num4 = rHeight - num - 1;
				int num5 = num3 * rHeight + num4;
				array[num5] = rArray[i];
			}
			return array;
		}

		private static Color[] FlipPixelsHorizontally(Color[] rArray, int rWidth, int rHeight)
		{
			Color[] array = new Color[rArray.Length];
			for (int i = 0; i < rHeight; i++)
			{
				for (int j = 0; j < rWidth; j++)
				{
					Color color = rArray[i * rWidth + j];
					array[(rWidth - 1 - j) * rHeight + i] = color;
				}
			}
			return array;
		}

		private static Color[] FlipPixelsVertically(Color[] rArray, int rWidth, int rHeight)
		{
			Color[] array = new Color[rArray.Length];
			int num = 0;
			while (num < rArray.Length)
			{
				int num2 = num / rWidth;
				int num3 = rHeight - num2;
				int num4 = (num3 - 1) * rWidth;
				int num5 = num4;
				while (num5 < num4 + rWidth)
				{
					array[num5] = rArray[num];
					num5++;
					num++;
				}
			}
			return array;
		}
	}
}
