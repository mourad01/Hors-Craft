// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.GameUI.ModifiedShadow
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Common.GameUI
{
	public class ModifiedShadow : Shadow
	{
		protected new void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
		{
			int num = verts.Count + (end - start);
			if (verts.Capacity < num)
			{
				verts.Capacity = num;
			}
			for (int i = start; i < end; i++)
			{
				UIVertex uIVertex = verts[i];
				verts.Add(uIVertex);
				Vector3 position = uIVertex.position;
				position.x += x;
				position.y += y;
				uIVertex.position = position;
				Color32 color2 = color;
				if (base.useGraphicAlpha)
				{
					byte a = color2.a;
					UIVertex uIVertex2 = verts[i];
					color2.a = (byte)(a * uIVertex2.color.a / 255);
				}
				uIVertex.color = color2;
				verts[i] = uIVertex;
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = new List<UIVertex>();
				vh.GetUIVertexStream(list);
				ModifyVertices(list);
				vh.AddUIVertexTriangleStream(list);
			}
		}

		public virtual void ModifyVertices(List<UIVertex> verts)
		{
		}
	}
}
