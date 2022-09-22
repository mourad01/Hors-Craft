// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.CanvasManager
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.Managers
{
	public class CanvasManager : Manager
	{
		[Header("If not set and found, it will be created automatically.")]
		[SerializeField]
		private Canvas mainCanvas;

		public Canvas canvas
		{
			get
			{
				if (mainCanvas == null)
				{
					FindMainCanvas();
					if (mainCanvas == null)
					{
						CreateDefaultCanvas();
						CreateEventSystem();
					}
				}
				return mainCanvas;
			}
		}

		public override void Init()
		{
		}

		private void FindMainCanvas()
		{
			mainCanvas = Object.FindObjectOfType<Canvas>();
		}

		private void CreateDefaultCanvas()
		{
			GameObject gameObject = new GameObject("Canvas");
			mainCanvas = gameObject.AddComponent<Canvas>();
			mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = new Vector2(1280f, 1024f);
			canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
			canvasScaler.referencePixelsPerUnit = 100f;
			gameObject.AddComponent<GraphicRaycaster>();
		}

		private void CreateEventSystem()
		{
			GameObject gameObject = new GameObject("EventSystem");
			gameObject.AddComponent<EventSystem>();
			gameObject.AddComponent<StandaloneInputModule>();
		}
	}
}
