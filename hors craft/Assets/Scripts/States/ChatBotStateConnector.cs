// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChatBotStateConnector
using Common.Managers.States.UI;
using GameUI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ChatBotStateConnector : UIConnector
	{
		public delegate void OnClick();

		public GameObject loadingStateGO;

		public GameObject chatStateGO;

		public RectTransform loadingBarRect;

		public Vector3 mobOffset = new Vector3(-1f, -0.1f, -10f);

		public Button returnButton;

		public Button sendButton;

		public GameObject messagePrefab;

		public GameObject waitMessagePrefab;

		public InputField inputField;

		public Scrollbar scrollbar;

		public ScrollRect scrollrect;

		public Camera cameraPlayerPreview;

		public TamePanelController tamePanel;

		public OnClick onReturnButton;

		public OnClick onSubmit;

		private float loadingPercentage;

		private void Awake()
		{
			messagePrefab.SetActive(value: false);
			waitMessagePrefab.SetActive(value: false);
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButton != null)
				{
					onReturnButton();
				}
			});
			sendButton.onClick.AddListener(delegate
			{
				if (onSubmit != null)
				{
					onSubmit();
				}
			});
			inputField.onEndEdit.AddListener(delegate
			{
				if (onSubmit != null)
				{
					onSubmit();
				}
			});
			loadingStateGO.SetActive(value: true);
			chatStateGO.SetActive(value: false);
			SetLoadingPercentage(0f);
		}

		public void SetMobRepsentationPlace(GameObject mobGO)
		{
			mobGO.transform.position = cameraPlayerPreview.transform.position + mobOffset;
			mobGO.transform.localRotation = Quaternion.Euler(0f, 146f, 0f);
		}

		public void SetLoadingPercentage(float percentage)
		{
			loadingPercentage = percentage;
			loadingBarRect.gameObject.GetComponent<Image>().fillAmount = percentage;
			if (loadingPercentage >= 1f)
			{
				loadingStateGO.SetActive(value: false);
				chatStateGO.SetActive(value: true);
			}
		}

		public GameObject AddWaitMessage(string name)
		{
			if (waitMessagePrefab == null)
			{
				return null;
			}
			Transform transform = Object.Instantiate(waitMessagePrefab).transform;
			transform.gameObject.SetActive(value: true);
			transform.FindChildRecursively("Name").GetComponent<Text>().text = name;
			transform.transform.SetParent(messagePrefab.transform.parent, worldPositionStays: false);
			transform.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
			transform.GetComponentInChildren<Animator>().SetTrigger("DotsAnimate");
			ColorController[] componentsInChildren = transform.GetComponentsInChildren<ColorController>();
			ColorController[] array = componentsInChildren;
			foreach (ColorController colorController in array)
			{
				if (colorController.gameObject.GetComponent<Image>() != null)
				{
					colorController.category = ColorManager.ColorCategory.THIRD_COLOR;
					colorController.UpdateColor();
				}
			}
			transform.FindChildRecursively("BubbleLeft").gameObject.SetActive(value: true);
			transform.FindChildRecursively("BubbleRight").gameObject.SetActive(value: false);
			StartCoroutine(ScrollToBottom());
			return transform.gameObject;
		}

		public GameObject AddNewMessage(string message, string name, bool right)
		{
			Transform transform = Object.Instantiate(messagePrefab).transform;
			transform.gameObject.SetActive(value: true);
			transform.SetParent(messagePrefab.transform.parent, worldPositionStays: false);
			ColorController[] componentsInChildren = transform.GetComponentsInChildren<ColorController>();
			ColorController[] array = componentsInChildren;
			foreach (ColorController colorController in array)
			{
				if (colorController.gameObject.GetComponent<Image>() != null)
				{
					colorController.category = ((!right) ? ColorManager.ColorCategory.THIRD_COLOR : ColorManager.ColorCategory.MAIN_COLOR);
					colorController.UpdateColor();
				}
			}
			transform.FindChildRecursively("Text").GetComponent<Text>().text = message;
			transform.FindChildRecursively("Name").GetComponent<Text>().text = name;
			transform.FindChildRecursively("BubbleLeft").gameObject.SetActive(!right);
			transform.FindChildRecursively("BubbleRight").gameObject.SetActive(right);
			transform.GetComponent<VerticalLayoutGroup>().childAlignment = ((!right) ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight);
			transform.GetComponent<RectTransform>().pivot = ((!right) ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f));
			transform.gameObject.SetActive(value: false);
			transform.gameObject.SetActive(value: true);
			StartCoroutine(ScrollToBottom());
			return transform.gameObject;
		}

		private IEnumerator ScrollToBottom()
		{
			yield return new WaitForEndOfFrame();
			scrollrect.normalizedPosition = Vector2.zero;
		}
	}
}
