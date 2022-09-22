// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityEngine.UI.ScaleToPhysicalSize
using Common.Managers;
using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("Layout/Scale To Physical Size")]
	[RequireComponent(typeof(RectTransform))]
	public class ScaleToPhysicalSize : UIBehaviour, ILayoutSelfController, ILayoutController
	{
		[Tooltip("The physical unit to specify sizes in.")]
		public CanvasScaler.Unit physicalUnit;

		public float horizontalPhysicalSize = 1f;

		public float verticalPhysicalSize = 1f;

		public float physicalSizeScale = 1f;

		public float horizontalPixelSizeMin = 10f;

		public float horizontalPixelSizeMax = 1000f;

		public float verticalPixelSizeMin = 10f;

		public float verticalPixelSizeMax = 1000f;

		[NonSerialized]
		private RectTransform m_Rect;

		private DrivenRectTransformTracker m_Tracker;

		private RectTransform rectTransform
		{
			get
			{
				if (m_Rect == null)
				{
					m_Rect = GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}

		protected ScaleToPhysicalSize()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			SetDirty();
		}

		private void HandleSelfFittingAlongAxis(int axis)
		{
			m_Tracker.Add(this, rectTransform, (axis != 0) ? DrivenTransformProperties.SizeDeltaY : DrivenTransformProperties.SizeDeltaX);
			float dpi = Screen.dpi;
			float num = 1f;
			switch (physicalUnit)
			{
			case CanvasScaler.Unit.Centimeters:
				num = 2.54f;
				break;
			case CanvasScaler.Unit.Millimeters:
				num = 25.4f;
				break;
			case CanvasScaler.Unit.Inches:
				num = 1f;
				break;
			case CanvasScaler.Unit.Points:
				num = 72f;
				break;
			case CanvasScaler.Unit.Picas:
				num = 6f;
				break;
			}
			float num2 = (axis != 0) ? verticalPhysicalSize : horizontalPhysicalSize;
			Canvas canvas = (!Application.isPlaying) ? (Object.FindObjectOfType(typeof(Canvas)) as Canvas) : Manager.Get<CanvasManager>().canvas;
			num2 *= physicalSizeScale * (dpi / num) / canvas.scaleFactor;
			float min = (axis != 0) ? verticalPixelSizeMin : horizontalPixelSizeMin;
			float max = (axis != 0) ? verticalPixelSizeMax : horizontalPixelSizeMax;
			num2 = Mathf.Clamp(num2, min, max);
			rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, num2);
		}

		public virtual void SetLayoutHorizontal()
		{
			m_Tracker.Clear();
			HandleSelfFittingAlongAxis(0);
		}

		public virtual void SetLayoutVertical()
		{
			HandleSelfFittingAlongAxis(1);
		}

		protected void SetDirty()
		{
			if (IsActive())
			{
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			}
		}
	}
}
