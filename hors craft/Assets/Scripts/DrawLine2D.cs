// DecompilerFi decompiler from Assembly-CSharp.dll class: DrawLine2D
using System.Collections.Generic;
using UnityEngine;

public class DrawLine2D : MonoBehaviour
{
	public class Line
	{
		public Vector3 start;

		public Vector3 end;

		public Color color;

		public Line(Vector3 start, Vector3 end, Color color)
		{
			this.start = start;
			this.end = end;
			this.color = color;
		}
	}

	public Material redMat;

	public List<Line> lines;

	private void Awake()
	{
		redMat = Resources.Load<Material>("prefabs/materials/Red");
		lines = new List<Line>();
	}

	private void OnPostRender()
	{
		if (lines.Count > 0)
		{
			GL.PushMatrix();
			redMat.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(1);
			foreach (Line line in lines)
			{
				GL.Color(line.color);
				GL.Vertex(line.start);
				GL.Vertex(line.end);
			}
			GL.End();
			GL.PopMatrix();
		}
	}
}
