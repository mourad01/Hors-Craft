// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityEngine.UI.LetterSpacing
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Letter Spacing", 14)]
	public class LetterSpacing : BaseMeshEffect
	{
		[SerializeField]
		private float m_spacing;

		public float spacing
		{
			get
			{
				return m_spacing;
			}
			set
			{
				if (m_spacing != value)
				{
					m_spacing = value;
					if (base.graphic != null)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		protected LetterSpacing()
		{
		}

		public void ModifyVertices(List<UIVertex> verts)
		{
			if (!IsActive())
			{
				return;
			}
			Text component = GetComponent<Text>();
			if (component == null)
			{
				UnityEngine.Debug.LogWarning("LetterSpacing: Missing Text component");
				return;
			}
			string[] array = component.text.Split('\n');
			float num = spacing * (float)component.fontSize / 100f;
			float num2 = 0f;
			int num3 = 0;
			switch (component.alignment)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				num2 = 0f;
				break;
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				num2 = 0.5f;
				break;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				num2 = 1f;
				break;
			}
			foreach (string text in array)
			{
				float num4 = (float)(text.Length - 1) * num * num2;
				for (int j = 0; j < text.Length; j++)
				{
					int index = num3 * 6;
					int index2 = num3 * 6 + 1;
					int index3 = num3 * 6 + 2;
					int num5 = num3 * 6 + 3;
					int index4 = num3 * 6 + 4;
					int index5 = num3 * 6 + 5;
					if (num5 > verts.Count - 1)
					{
						return;
					}
					UIVertex value = verts[index];
					UIVertex value2 = verts[index2];
					UIVertex value3 = verts[index3];
					UIVertex value4 = verts[num5];
					UIVertex value5 = verts[index4];
					UIVertex value6 = verts[index5];
					Vector3 vector = Vector3.right * (num * (float)j - num4);
					value.position += vector;
					value2.position += vector;
					value3.position += vector;
					value4.position += vector;
					value5.position += vector;
					value6.position += vector;
					verts[index] = value;
					verts[index2] = value2;
					verts[index3] = value3;
					verts[num5] = value4;
					verts[index4] = value5;
					verts[index5] = value6;
					num3++;
				}
				num3++;
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = new List<UIVertex>();
				vh.GetUIVertexStream(list);
				ModifyVertices(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
			}
		}
	}
}
