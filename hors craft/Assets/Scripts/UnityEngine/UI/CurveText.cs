// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityEngine.UI.CurveText
using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Curve Text", 15)]
	public class CurveText : BaseMeshEffect
	{
		[SerializeField]
		public float m_eulerRadius = 0.5f;

		[SerializeField]
		public float m_wrapAngle = 360f;

		[SerializeField]
		public float m_scaleFactor = 100f;

		private float _radius = -1f;

		private float _scaleFactor = -1f;

		private float _circumference = -1f;

		public float eulerRadius
		{
			get
			{
				return m_eulerRadius;
			}
			set
			{
				if (m_eulerRadius != value)
				{
					m_eulerRadius = value;
					if (base.graphic != null)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		public float wrapAngle
		{
			get
			{
				return m_wrapAngle;
			}
			set
			{
				if (m_wrapAngle != value)
				{
					m_wrapAngle = value;
					if (base.graphic != null)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		public float scaleFactor
		{
			get
			{
				return m_scaleFactor;
			}
			set
			{
				if (m_scaleFactor != value)
				{
					m_scaleFactor = value;
					if (base.graphic != null)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		private float radius => eulerRadius * ((float)Math.PI / 180f);

		private float circumference
		{
			get
			{
				if (_radius != radius || _scaleFactor != scaleFactor)
				{
					_circumference = (float)Math.PI * 2f * radius * scaleFactor;
					_radius = radius;
					_scaleFactor = scaleFactor;
				}
				return _circumference;
			}
		}

		protected CurveText()
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
				UnityEngine.Debug.LogWarning("CurveText: Missing Text component");
				return;
			}
			for (int i = 0; i < verts.Count; i++)
			{
				UIVertex value = verts[i];
				float num = value.position.x / circumference;
				Vector3 a = Quaternion.Euler(0f, 0f, (0f - num) * 360f) * Vector3.up;
				value.position = a * radius * scaleFactor + a * value.position.y;
				value.position += Vector3.down * radius * scaleFactor;
				verts[i] = value;
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
