// DecompilerFi decompiler from Assembly-CSharp.dll class: StorySystem.TouchesVisual
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StorySystem
{
	public class TouchesVisual : MonoBehaviour
	{
		private Dictionary<TouchInfo, LineRenderer> info2Renderer = new Dictionary<TouchInfo, LineRenderer>();

		private Color[] colors = new Color[8]
		{
			Color.white,
			Color.black,
			Color.blue,
			Color.cyan,
			Color.red,
			Color.green,
			Color.magenta,
			Color.yellow
		};

		private void Start()
		{
			TouchController get = MonoBehaviourSingleton<TouchController>.get;
			get.newTouchInfo = (Action<TouchInfo>)Delegate.Remove(get.newTouchInfo, new Action<TouchInfo>(NewTouchAdded));
			TouchController get2 = MonoBehaviourSingleton<TouchController>.get;
			get2.newTouchInfo = (Action<TouchInfo>)Delegate.Combine(get2.newTouchInfo, new Action<TouchInfo>(NewTouchAdded));
		}

		private void NewTouchAdded(TouchInfo touchInfo)
		{
			touchInfo.onMoved = (Action<TouchInfo>)Delegate.Remove(touchInfo.onMoved, new Action<TouchInfo>(UpdateTouchLine));
			touchInfo.onMoved = (Action<TouchInfo>)Delegate.Combine(touchInfo.onMoved, new Action<TouchInfo>(UpdateTouchLine));
			info2Renderer.Add(touchInfo, GetNewLine(touchInfo.fingerId));
		}

		private LineRenderer GetNewLine(int touchId)
		{
			GameObject gameObject = new GameObject("Line");
			gameObject.transform.SetParent(base.transform);
			LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
			lineRenderer.useWorldSpace = true;
			lineRenderer.sortingOrder = 100;
			lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
			float num3 = lineRenderer.endWidth = (lineRenderer.startWidth = 0.1f + (float)touchId * 0.02f);
			lineRenderer.positionCount = 0;
			Color color3 = lineRenderer.endColor = (lineRenderer.startColor = colors[touchId % (int)colors.LongLength]);
			return lineRenderer;
		}

		private void UpdateTouchLine(TouchInfo touchInfo)
		{
			LineRenderer lineRenderer = info2Renderer[touchInfo];
			lineRenderer.positionCount++;
			lineRenderer.SetPosition(lineRenderer.positionCount - 1, touchInfo.LastWorldPoint());
		}
	}
}
