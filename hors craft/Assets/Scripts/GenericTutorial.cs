// DecompilerFi decompiler from Assembly-CSharp.dll class: GenericTutorial
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GenericTutorial : MonoBehaviour
{
	public enum TutorialStartMethod
	{
		ON_START,
		ON_BUTTON,
		CALLBACK
	}

	public enum Shape
	{
		CUSTOM,
		CIRCLE,
		RECT,
		WHOLE_SCREEN_DARK,
		WHOLE_SCREEN_BRIGHT
	}

	public enum ArrowMode
	{
		MAX,
		MIN
	}

	protected class MaskOperator : IDisposable
	{
		protected static MaskOperator _instance;

		private GameObject _rootMask;

		[CompilerGenerated]
		private static Action<GameObject> _003C_003Ef__mg_0024cache0;

		public static MaskOperator Instance => _instance ?? (_instance = CrateMask());

		public GameObject rootMask
		{
			get
			{
				if (_rootMask == null)
				{
					return null;
				}
				_rootMask.transform.SetAsLastSibling();
				return _rootMask;
			}
			private set
			{
				_rootMask = value;
			}
		}

		public GameObject[] maskImages
		{
			get;
			private set;
		}

		private MaskOperator()
		{
		}

		private static MaskOperator CrateMask()
		{
			return new MaskOperator();
		}

		public void Init()
		{
			InstantiateRootMask();
			InstantiateMasks();
		}

		public void ClearMask()
		{
			if (!(rootMask == null))
			{
				GetMaskChildrenToDestroy().ForEach(UnityEngine.Object.DestroyImmediate);
			}
		}

		public void EnableInput(bool enable)
		{
			if (!(rootMask == null))
			{
				IEnumerable<Button> enumerable = FilterOutMainChildren(rootMask.GetComponentsInChildren<Button>(), onlyCustom: true);
				foreach (Button item in enumerable)
				{
					item.interactable = enable;
				}
			}
		}

		public void EnableInputThroughMask(bool enable)
		{
			if (!(rootMask == null))
			{
				IEnumerable<Image> enumerable = FilterOutMainChildren(rootMask.GetComponentsInChildren<Image>(), onlyCustom: true);
				foreach (Image item in enumerable)
				{
					item.raycastTarget = enable;
				}
			}
		}

		public void SetMasks(Rect targetRect, Shape shape, Color darkColor, Sprite customSprite = null)
		{
			List<Pair<Sprite, Rect>> list = GenericTutorialMaskGenerator.CreateMask(targetRect, shape, darkColor, customSprite);
			if (list.Count == 1)
			{
				SetMainMask(list[0]);
			}
			else
			{
				SetSubMask(list);
			}
		}

		private void InstantiateRootMask()
		{
			rootMask = CreateCanvasObject("Mask", Manager.Get<CanvasManager>().canvas.transform, addImage: true);
		}

		private void InstantiateMasks()
		{
			maskImages = new GameObject[5];
			for (int i = 0; i < maskImages.Length; i++)
			{
				maskImages[i] = CreateCanvasObject("Mask_" + i, rootMask.transform, addImage: true, addButton: true, SetAlpha);
			}
		}

		private void SetAlpha()
		{
			for (int i = 0; i < maskImages.Length; i++)
			{
				Color color = maskImages[i].GetComponent<Image>().color;
				color.a = 1f;
				maskImages[i].GetComponent<Image>().color = color;
			}
		}

		private List<GameObject> GetMaskChildrenToDestroy()
		{
			return (from t in FilterOutMainChildren(rootMask.transform.Cast<Transform>())
				select t.gameObject).ToList();
		}

		private void SetMainMask(Pair<Sprite, Rect> sprite)
		{
			Image component = rootMask.GetComponent<Image>();
			component.sprite = sprite.First;
			component.raycastTarget = true;
			Color color = component.color;
			color.a = 0.6f;
			component.color = color;
			for (int i = 0; i < maskImages.Length; i++)
			{
				Image component2 = maskImages[i].GetComponent<Image>();
				color = component2.color;
				color.a = 0f;
				component2.color = color;
				component2.raycastTarget = false;
				maskImages[i].GetComponent<Button>().interactable = false;
			}
		}

		private void SetSubMask(List<Pair<Sprite, Rect>> sprites)
		{
			Image component = rootMask.GetComponent<Image>();
			Color color = component.color;
			color.a = 0f;
			component.color = color;
			component.raycastTarget = false;
			for (int i = 0; i < maskImages.Length; i++)
			{
				Image component2 = maskImages[i].GetComponent<Image>();
				color = component2.color;
				color.a = 0.6f;
				component2.color = color;
				component2.raycastTarget = true;
				maskImages[i].GetComponent<Button>().interactable = true;
				component2.sprite = sprites[i].First;
				RectTransform component3 = maskImages[i].GetComponent<RectTransform>();
				component3.anchorMax = Vector3.zero;
				component3.anchorMin = Vector3.zero;
				component3.sizeDelta = sprites[i].Second.size;
				component3.anchoredPosition = sprites[i].Second.center;
			}
		}

		private IEnumerable<T> FilterOutMainChildren<T>(IEnumerable<T> enumerable, bool onlyCustom = false) where T : Component
		{
			return onlyCustom ? (from e in enumerable
				where e.gameObject == maskImages[0] || maskImages.Count((GameObject mI) => mI == e.gameObject) == 0
				select e) : (from e in enumerable
				where maskImages.Count((GameObject mI) => mI == e.gameObject) == 0
				select e);
		}

		private static GameObject CreateCanvasObject(string name, Transform parent, bool addImage = false, bool addButton = false, Action callBack = null)
		{
			GameObject gameObject = new GameObject(name);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			gameObject.transform.SetParent(parent);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			if (addImage)
			{
				Image image = gameObject.AddComponent<Image>();
				image.raycastTarget = true;
				image.preserveAspect = false;
			}
			if (!addButton)
			{
				return gameObject;
			}
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			if (callBack != null)
			{
				button.onClick.AddListener(delegate
				{
					callBack();
				});
			}
			return gameObject;
		}

		public void Dispose()
		{
			UnityEngine.Object.DestroyImmediate(_rootMask);
			_instance = null;
		}
	}

	[Serializable]
	public class TutorialStep
	{
		[Header("MUST HAVE")]
		public GameObject element;

		public Shape shape;

		[Header("OPTIONAL")]
		public string infoKey;

		public string infoDefaultText;

		public float delayBefore;

		public float delayAfter;

		public List<TutorialCallback> tutorialCallbacks = new List<TutorialCallback>();

		public TranslateText.OnVisitText translationVisitor;

		public Action<GameObject> configInfoPanelAfterSpawn;

		public bool forceShowArrow;
	}

	public enum CallbackType
	{
		ON_ELEMENT_CLICK,
		ON_STEP_INIT,
		ON_STEP_SPAWN,
		ON_STEP_CHANGE
	}

	[Serializable]
	public class TutorialCallback
	{
		public CallbackType type;

		public List<Button> buttonsToActivate = new List<Button>();

		public UnityEvent eventsToActivate;

		public Action action;

		public void DoIt()
		{
			if (buttonsToActivate != null)
			{
				buttonsToActivate.ForEach(delegate(Button b)
				{
					b.onClick.Invoke();
				});
			}
			if (eventsToActivate != null)
			{
				eventsToActivate.Invoke();
			}
			if (action != null)
			{
				action();
			}
		}
	}

	public const float RECT_MULTIPLIER = 1.3f;

	public const float firstAlpha = 0.6f;

	public Color darkColor = new Color(0f, 0f, 0f, 0.7f);

	public Button triggeringButton;

	public TutorialStartMethod startMethod;

	public bool pauseGameOnTutorial;

	public bool autoChangeSteps = true;

	public bool blockInputBetweenSteps;

	public bool allowInputThroughMask;

	public GameObject infoPanelPrefab;

	public GameObject arrowPrefab;

	public ArrowMode arrowMode;

	public Vector2 arrowOffset;

	public Sprite customShapeSprite;

	public TutorialStep[] tutorialSteps;

	public Action<GameObject> defaultConfigInfoPaneAfterSpawn;

	private GameObject targetObject;

	private Camera _cam;

	public int currentStep
	{
		get;
		protected set;
	}

	private Camera cam => _cam ?? (_cam = Camera.main);

	private void Start()
	{
		PrepareCustomSprite();
		if (startMethod == TutorialStartMethod.ON_START)
		{
			StartTutorial();
		}
		else if (startMethod == TutorialStartMethod.ON_BUTTON)
		{
			triggeringButton.onClick.AddListener(StartTutorial);
		}
	}

	private void PrepareCustomSprite()
	{
		Texture2D texture = customShapeSprite.texture;
		Color[] pixels = texture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].r < 0.2f && pixels[i].g < 0.2f && pixels[i].b < 0.2f)
			{
				pixels[i] = darkColor;
			}
		}
		texture.SetPixels(pixels);
		texture.Apply();
	}

	public void StartTutorial()
	{
		if (pauseGameOnTutorial)
		{
			TimeScaleHelper.value = 0f;
		}
		StopAllCoroutines();
		MaskOperator.Instance.Init();
		currentStep = 0;
		SetTutorialStep();
	}

	public void EndTutorial()
	{
		StopAllCoroutines();
		if (pauseGameOnTutorial)
		{
			TimeScaleHelper.value = 1f;
		}
		MaskOperator.Instance.Dispose();
	}

	protected void OnTutorialButtonClicked()
	{
		TutorialStep tutorialStep = tutorialSteps[currentStep];
		RunCallback(tutorialStep, CallbackType.ON_ELEMENT_CLICK);
		if (autoChangeSteps)
		{
			NextStep(tutorialStep.delayAfter);
		}
		if (blockInputBetweenSteps)
		{
			MaskOperator.Instance.EnableInput(enable: false);
		}
		if (allowInputThroughMask)
		{
			MaskOperator.Instance.EnableInputThroughMask(enable: false);
		}
	}

	public virtual void NextStep(float delay = 0f)
	{
		StopAllCoroutines();
		StartCoroutine(DoAfter(delay, delegate
		{
			RunCallback(tutorialSteps[currentStep], CallbackType.ON_STEP_CHANGE);
			currentStep++;
			if (currentStep < tutorialSteps.Length)
			{
				SetTutorialStep();
			}
			else
			{
				EndTutorial();
			}
		}));
	}

	protected void SetTutorialStep()
	{
		TutorialStep step = tutorialSteps[currentStep];
		RunCallback(step, CallbackType.ON_STEP_INIT);
		StopAllCoroutines();
		StartCoroutine(DoAfter(step.delayBefore, delegate
		{
			RunCallback(step, CallbackType.ON_STEP_SPAWN);
			SpawnTutorialStepElements(step);
			if (allowInputThroughMask)
			{
				MaskOperator.Instance.EnableInputThroughMask(enable: false);
			}
		}));
	}

	protected virtual void SpawnTutorialStepElements(TutorialStep step)
	{
		MaskOperator.Instance.ClearMask();
		Rect rect = CalculateTargetRect(step.element);
		MaskOperator.Instance.SetMasks(rect, step.shape, darkColor, customShapeSprite);
		InstantiateButtonOnMask(rect);
		if (infoPanelPrefab != null)
		{
			SpawnInfoPanel(step, rect);
		}
		if (step.forceShowArrow || (step.shape != Shape.WHOLE_SCREEN_BRIGHT && step.shape != Shape.WHOLE_SCREEN_DARK))
		{
			SpawnArrow(rect);
		}
	}

	protected Rect CalculateTargetRect(GameObject element)
	{
		if (!(element.transform is RectTransform))
		{
			return Calculate3DElementRect(element);
		}
		if ((bool)element.GetComponent<Canvas>() && element.GetComponent<Canvas>().renderMode == RenderMode.WorldSpace)
		{
			return CalculateWorldUIElementRect(element);
		}
		return CalculateUIElementRect(element);
	}

	private Rect Calculate3DElementRect(GameObject element)
	{
		return RenderersBounds.Calculate3DElementRectOnUI(element, cam, 1.3f);
	}

	private Rect CalculateUIElementRect(GameObject element)
	{
		Rect result = default(Rect);
		RectTransform rectTransform = element.transform as RectTransform;
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		float x = (array[0].x + array[2].x) / 2f;
		float y = (array[0].y + array[2].y) / 2f;
		result.position = new Vector2(x, y) - rectTransform.rect.size / 2f * 1.3f;
		result.width = rectTransform.rect.width * 1.3f;
		result.height = rectTransform.rect.height * 1.3f;
		return result;
	}

	private Rect CalculateWorldUIElementRect(GameObject element)
	{
		Rect result = default(Rect);
		RectTransform rectTransform = element.transform as RectTransform;
		Vector2 a = rectTransform.rect.size / 2f;
		Vector3 vector = Camera.main.WorldToScreenPoint(element.transform.position);
		float x = vector.x;
		float y = vector.y;
		result.position = new Vector2(x, y) - a / 2f * 1.3f;
		result.width = a.x * 1.3f;
		result.height = a.y * 1.3f;
		return result;
	}

	protected GameObject InstantiateButtonOnMask(Rect rect)
	{
		GameObject gameObject = new GameObject("Button");
		gameObject.transform.SetParent(MaskOperator.Instance.rootMask.transform);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.position = rect.center;
		switch (arrowMode)
		{
		case ArrowMode.MAX:
		{
			float num = Mathf.Max(rect.width, rect.height);
			rectTransform.sizeDelta = new Vector2(num, num);
			break;
		}
		case ArrowMode.MIN:
			rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
			break;
		}
		gameObject.transform.localScale = Vector3.one;
		Image image = gameObject.AddComponent<Image>();
		image.raycastTarget = true;
		image.sprite = null;
		image.color = new Color(0f, 0f, 0f, 0f);
		Button button = gameObject.AddComponent<Button>();
		button.transition = Selectable.Transition.None;
		button.interactable = true;
		button.onClick.AddListener(OnTutorialButtonClicked);
		return gameObject;
	}

	protected virtual GameObject SpawnInfoPanel(TutorialStep step, Rect rect)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(infoPanelPrefab, MaskOperator.Instance.rootMask.transform, worldPositionStays: false);
		gameObject.transform.localScale = Vector3.one;
		RectTransform rectTransform = gameObject.transform as RectTransform;
		Vector2 center = rect.center;
		bool flag = center.y > (float)(Screen.height / 2);
		Vector2 sizeDelta = rectTransform.sizeDelta;
		float x = sizeDelta.x;
		Vector2 sizeDelta2 = rectTransform.sizeDelta;
		float y = sizeDelta2.y;
		if (!flag)
		{
			RectTransform rectTransform2 = rectTransform;
			Vector2 anchorMin = rectTransform.anchorMin;
			rectTransform2.anchorMin = new Vector2(anchorMin.x, 1f);
			RectTransform rectTransform3 = rectTransform;
			Vector2 anchorMax = rectTransform.anchorMax;
			rectTransform3.anchorMax = new Vector3(anchorMax.x, 1f);
			rectTransform.pivot = new Vector2(0.5f, 1f);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.sizeDelta = new Vector2(x, y);
			/*if (gameObject.GetComponent<MoveIfBannerEnabled>() == null)
			{
				gameObject.AddComponent<MoveIfBannerEnabled>();
			}*/
		}
		else
		{
			/*if (gameObject.GetComponent<MoveIfBannerEnabled>() != null)
			{
				UnityEngine.Object.Destroy(gameObject.GetComponent<MoveIfBannerEnabled>());
			}*/
			RectTransform rectTransform4 = rectTransform;
			Vector2 anchorMin2 = rectTransform.anchorMin;
			rectTransform4.anchorMin = new Vector2(anchorMin2.x, 0f);
			RectTransform rectTransform5 = rectTransform;
			Vector2 anchorMax2 = rectTransform.anchorMax;
			rectTransform5.anchorMax = new Vector3(anchorMax2.x, 0f);
			rectTransform.pivot = new Vector2(0.5f, 0f);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.sizeDelta = new Vector2(x, y);
		}
		TranslateText componentInChildren = gameObject.GetComponentInChildren<TranslateText>();
		if (componentInChildren != null)
		{
			componentInChildren.defaultText = step.infoDefaultText;
			componentInChildren.translationKey = step.infoKey;
			if (step.translationVisitor != null)
			{
				componentInChildren.AddVisitor(step.translationVisitor);
			}
			componentInChildren.ForceRefresh();
		}
		if (defaultConfigInfoPaneAfterSpawn != null)
		{
			defaultConfigInfoPaneAfterSpawn(gameObject);
		}
		if (step.configInfoPanelAfterSpawn != null)
		{
			step.configInfoPanelAfterSpawn(gameObject);
		}
		return gameObject;
	}

	protected void SpawnArrow(Rect rect)
	{
		if (!(arrowPrefab == null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(arrowPrefab, MaskOperator.Instance.rootMask.transform, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			float num = 0f;
			switch (arrowMode)
			{
			case ArrowMode.MAX:
			{
				Vector2 size3 = rect.size;
				float x2 = size3.x;
				Vector2 size4 = rect.size;
				num = Mathf.Max(x2, size4.y) * 0.5f;
				break;
			}
			case ArrowMode.MIN:
			{
				Vector2 size = rect.size;
				float x = size.x;
				Vector2 size2 = rect.size;
				num = Mathf.Min(x, size2.y) * 0.5f;
				break;
			}
			}
			Vector2 normalized = (rect.center + arrowOffset - new Vector2(Screen.width, Screen.height) / 2f).normalized;
			float num2 = Mathf.Atan2(1f, 0f) - Mathf.Atan2(normalized.x, normalized.y);
			num2 *= 57.29578f;
			num2 += 180f;
			Vector2 center = rect.center;
			center.x += Mathf.Cos((float)Math.PI / 180f * num2) * num;
			center.y += Mathf.Sin((float)Math.PI / 180f * num2) * num;
			gameObject.transform.position = center;
			gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f + num2);
		}
	}

	protected static IEnumerator DoAfter(float timer, Action action)
	{
		yield return new WaitForSecondsRealtime(timer);
		action();
	}

	protected static void RunCallback(TutorialStep step, CallbackType type)
	{
		if (step.tutorialCallbacks != null)
		{
			(from callback in step.tutorialCallbacks
				where callback.type == type
				select callback).ToList().ForEach(delegate(TutorialCallback callback)
			{
				callback.DoIt();
			});
		}
	}
}
