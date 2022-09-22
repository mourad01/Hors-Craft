// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.StartGame

using States;
using Common.Managers;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class StartGame : MonoBehaviour
	{
		[SerializeField]
		private Canvas splashCanvas;

		private void Awake()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			LoadMainSceneAdditively loadMainSceneAdditively = base.gameObject.AddComponent<LoadMainSceneAdditively>();
			loadMainSceneAdditively.onLoaded = OnMainSceneLoaded;
			if (splashCanvas == null)
			{
				splashCanvas = UnityEngine.Object.FindObjectOfType<Canvas>();
			}
			Image component = splashCanvas.transform.Find("Logo").GetComponent<Image>();
			component.sprite = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Logo");
			component.SetNativeSize();
		}

		private void OnMainSceneLoaded()
		{
			Manager.Get<StateMachineManager>().SetState<LoadingState>();
			
			StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.MAIN_SCENE_LOADED);
			if (splashCanvas != null)
			{
				UnityEngine.Object.Destroy(splashCanvas.gameObject);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
