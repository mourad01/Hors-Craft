// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityEngine.UI.AutoExpandGridLayoutGroup
namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/Auto Expand Grid Layout Group", 152)]
	public class AutoExpandGridLayoutGroup : LayoutGroup
	{
		public enum Corner
		{
			UpperLeft,
			UpperRight,
			LowerLeft,
			LowerRight
		}

		public enum Axis
		{
			Horizontal,
			Vertical
		}

		public enum Constraint
		{
			Flexible,
			FixedColumnCount,
			FixedRowCount
		}

		[SerializeField]
		protected Corner m_StartCorner;

		[SerializeField]
		protected Axis m_StartAxis;

		[SerializeField]
		protected Vector2 m_CellSize = new Vector2(100f, 100f);

		[SerializeField]
		protected Vector2 m_Spacing = Vector2.zero;

		[SerializeField]
		protected Constraint m_Constraint;

		[SerializeField]
		protected int m_ConstraintCount = 2;

		public bool specialOneChildBehaviour;

		public Corner startCorner
		{
			get
			{
				return m_StartCorner;
			}
			set
			{
				SetProperty(ref m_StartCorner, value);
			}
		}

		public Axis startAxis
		{
			get
			{
				return m_StartAxis;
			}
			set
			{
				SetProperty(ref m_StartAxis, value);
			}
		}

		public Vector2 cellSize
		{
			get
			{
				return m_CellSize;
			}
			set
			{
				SetProperty(ref m_CellSize, value);
			}
		}

		public Vector2 spacing
		{
			get
			{
				return m_Spacing;
			}
			set
			{
				SetProperty(ref m_Spacing, value);
			}
		}

		public Constraint constraint
		{
			get
			{
				return m_Constraint;
			}
			set
			{
				SetProperty(ref m_Constraint, value);
			}
		}

		public int constraintCount
		{
			get
			{
				return m_ConstraintCount;
			}
			set
			{
				SetProperty(ref m_ConstraintCount, Mathf.Max(1, value));
			}
		}

		protected AutoExpandGridLayoutGroup()
		{
		}

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			int num = 0;
			int num2 = 0;
			if (m_Constraint == Constraint.FixedColumnCount)
			{
				num = (num2 = m_ConstraintCount);
			}
			else if (m_Constraint == Constraint.FixedRowCount)
			{
				num = (num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)m_ConstraintCount - 0.001f));
			}
			else
			{
				num = 1;
				num2 = Mathf.CeilToInt(Mathf.Sqrt(base.rectChildren.Count));
			}
			float num3 = base.padding.horizontal;
			Vector2 cellSize = this.cellSize;
			float x = cellSize.x;
			Vector2 spacing = this.spacing;
			float num4 = num3 + (x + spacing.x) * (float)num;
			Vector2 spacing2 = this.spacing;
			float totalMin = num4 - spacing2.x;
			float num5 = base.padding.horizontal;
			Vector2 cellSize2 = this.cellSize;
			float x2 = cellSize2.x;
			Vector2 spacing3 = this.spacing;
			float num6 = num5 + (x2 + spacing3.x) * (float)num2;
			Vector2 spacing4 = this.spacing;
			SetLayoutInputForAxis(totalMin, num6 - spacing4.x, -1f, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			int num = 0;
			if (m_Constraint == Constraint.FixedColumnCount)
			{
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)m_ConstraintCount - 0.001f);
			}
			else if (m_Constraint == Constraint.FixedRowCount)
			{
				num = m_ConstraintCount;
			}
			else
			{
				Vector2 size = base.rectTransform.rect.size;
				float x = size.x;
				float num2 = x - (float)base.padding.horizontal;
				Vector2 spacing = this.spacing;
				float num3 = num2 + spacing.x + 0.001f;
				Vector2 cellSize = this.cellSize;
				float x2 = cellSize.x;
				Vector2 spacing2 = this.spacing;
				int num4 = Mathf.Max(1, Mathf.FloorToInt(num3 / (x2 + spacing2.x)));
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num4);
			}
			float num5 = base.padding.vertical;
			Vector2 cellSize2 = this.cellSize;
			float y = cellSize2.y;
			Vector2 spacing3 = this.spacing;
			float num6 = num5 + (y + spacing3.y) * (float)num;
			Vector2 spacing4 = this.spacing;
			float num7 = num6 - spacing4.y;
			SetLayoutInputForAxis(num7, num7, -1f, 1);
		}

		public override void SetLayoutHorizontal()
		{
			SetCellsAlongAxis(0);
		}

		public override void SetLayoutVertical()
		{
			SetCellsAlongAxis(1);
		}

		private void SetCellsAlongAxis(int axis)
		{
			if (axis == 0)
			{
				for (int i = 0; i < base.rectChildren.Count; i++)
				{
					RectTransform rectTransform = base.rectChildren[i];
					m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
					rectTransform.anchorMin = Vector2.up;
					rectTransform.anchorMax = Vector2.up;
					rectTransform.sizeDelta = this.cellSize;
				}
				return;
			}
			Vector2 size = base.rectTransform.rect.size;
			float num = size.x;
			Vector2 size2 = base.rectTransform.rect.size;
			float num2 = size2.y - (float)base.padding.bottom;
			if (specialOneChildBehaviour && base.rectChildren.Count == 1)
			{
				num = num2;
			}
			int num3 = 1;
			int num4 = 1;
			if (m_Constraint == Constraint.FixedColumnCount)
			{
				num3 = m_ConstraintCount;
				num4 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num3 - 0.001f);
			}
			else if (m_Constraint == Constraint.FixedRowCount)
			{
				num4 = m_ConstraintCount;
				num3 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num4 - 0.001f);
			}
			else
			{
				Vector2 cellSize = this.cellSize;
				float x = cellSize.x;
				Vector2 spacing = this.spacing;
				if (x + spacing.x <= 0f)
				{
					num3 = int.MaxValue;
				}
				else
				{
					float num5 = num - (float)base.padding.horizontal;
					Vector2 spacing2 = this.spacing;
					float num6 = num5 + spacing2.x + 0.001f;
					Vector2 cellSize2 = this.cellSize;
					float x2 = cellSize2.x;
					Vector2 spacing3 = this.spacing;
					num3 = Mathf.Max(1, Mathf.FloorToInt(num6 / (x2 + spacing3.x)));
				}
				Vector2 cellSize3 = this.cellSize;
				float y = cellSize3.y;
				Vector2 spacing4 = this.spacing;
				if (y + spacing4.y <= 0f)
				{
					num4 = int.MaxValue;
				}
				else
				{
					float num7 = num2 - (float)base.padding.vertical;
					Vector2 spacing5 = this.spacing;
					float num8 = num7 + spacing5.y + 0.001f;
					Vector2 cellSize4 = this.cellSize;
					float y2 = cellSize4.y;
					Vector2 spacing6 = this.spacing;
					num4 = Mathf.Max(1, Mathf.FloorToInt(num8 / (y2 + spacing6.y)));
				}
			}
			int num9 = (int)startCorner % 2;
			int num10 = (int)startCorner / 2;
			int num11;
			int num12;
			int num13;
			if (startAxis == Axis.Horizontal)
			{
				num11 = num3;
				num12 = Mathf.Clamp(num3, 1, base.rectChildren.Count);
				num13 = Mathf.Clamp(num4, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num11));
			}
			else
			{
				num11 = num4;
				num13 = Mathf.Clamp(num4, 1, base.rectChildren.Count);
				num12 = Mathf.Clamp(num3, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num11));
			}
			float num14 = num12;
			Vector2 cellSize5 = this.cellSize;
			float num15 = num14 * cellSize5.x;
			float num16 = num12 - 1;
			Vector2 spacing7 = this.spacing;
			float x3 = num15 + num16 * spacing7.x;
			float num17 = num13;
			Vector2 cellSize6 = this.cellSize;
			float num18 = num17 * cellSize6.y;
			float num19 = num13 - 1;
			Vector2 spacing8 = this.spacing;
			Vector2 vector = new Vector2(x3, num18 + num19 * spacing8.y);
			Vector2 vector2 = new Vector2(GetStartOffset(0, vector.x), GetStartOffset(1, vector.y));
			for (int j = 0; j < base.rectChildren.Count; j++)
			{
				int num20;
				int num21;
				if (startAxis == Axis.Horizontal)
				{
					num20 = j % num11;
					num21 = j / num11;
				}
				else
				{
					num20 = j / num11;
					num21 = j % num11;
				}
				if (num9 == 1)
				{
					num20 = num12 - 1 - num20;
				}
				if (num10 == 1)
				{
					num21 = num13 - 1 - num21;
				}
				float num22 = (num - this.spacing[0] * (float)(num12 - 1)) / (float)num12;
				float num23 = (num2 - this.spacing[1] * (float)(num13 - 1)) / (float)num13;
				if (specialOneChildBehaviour && base.rectChildren.Count == 1)
				{
					RectTransform rect = base.rectChildren[j];
					Vector2 size3 = base.rectTransform.rect.size;
					SetChildAlongAxis(rect, 0, size3.x / 2f - num / 2f, num22);
				}
				else
				{
					SetChildAlongAxis(base.rectChildren[j], 0, vector2.x + (num22 + this.spacing[0]) * (float)num20, num22);
				}
				SetChildAlongAxis(base.rectChildren[j], 1, vector2.y + (num23 + this.spacing[1]) * (float)num21, num23);
			}
		}
	}
}
