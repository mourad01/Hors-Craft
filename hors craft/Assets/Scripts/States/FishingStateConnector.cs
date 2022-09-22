// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FishingStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using GameUI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class FishingStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Button returnButton;

		public Button actionFishingButton;

		public SimpleRepeatButton fightFishingButton;

		public TranslateText fishingButtonText;

		public TranslateText fishingAreaButtonText;

		public TranslateText fishingStateText;

		public Slider miniGameSlider;

		public Animator miniGamePanel;

		public ResourceGatheredAnimator resourceGatherSpot;

		public RectTransform miniGameRodArea;

		public RectTransform miniGameRodHandle;

		public RectTransform miniGameFishArea;

		public RectTransform miniGameFishHandle;

		public FishCatchedStats fishCatchedStats;

		public UIPulsingEffect pullPulse;

		private Image miniGameRodHandleImage;

		[Header("Tutorial Objects")]
		public GameObject tutorialContent;

		public Image tutorialOverlay;

		public GameObject tutorialPopup2;

		public GameObject tutorialPopup3;

		public GameObject tutorialPopup4;

		public GameObject tutorialPopup5;

		public GameObject tutorialPopup6;

		public GameObject tutorialPopup7;

		public Button tutorialMoveNextButton2;

		public Button tutorialMoveNextButton3;

		public SimpleRepeatButton tutorialMoveNextButton4;

		public Button tutorialMoveNextButton5;

		public Button tutorialMoveNextButton6;

		public Button tutorialMoveNextButton7;

		public Slider tutorialSlider5;

		public RectTransform turorialRodAreaSlider5;

		public RectTransform tutorialRodHandleSlider5;

		public RectTransform tutorialFishAreaSlider5;

		public RectTransform tutorialFishHandleSlider5;

		public Animator tutorialAnimator5;

		public Color RodNearColor = Color.black;

		public Color RodFarColor = Color.black;

		public Image[] fisherMasterTutorImages;

		[Header("Debug Objects")]
		public GameObject debugObjectsController;

		public Button prevChangeFish;

		public Button nextChangeFish;

		public Button prevChangeRod;

		public Button nextChangeRod;

		public Text fishOnHookCodeName;

		public Text fishOnHookName;

		public Text fishOnHookRarity;

		public Text fishOnHookWeight;

		public Text fishOnHookPoints;

		public Text leftToSuccess;

		public Text choosedFishName;

		public Text choosedFishRarity;

		public Text choosedRodName;

		public OnClick onReturnButtonClicked;

		public OnClick onFishingButtonClicked;

		public OnClick onPrevChangeFishClicked;

		public OnClick onNextChangeFishClicked;

		public OnClick onPrevChangeRodClicked;

		public OnClick onNextChangeRodClicked;

		public OnClick onTutorialMoveNextButton2Clicked;

		public OnClick onTutorialMoveNextButton3Clicked;

		public OnClick onTutorialMoveNextButton4Clicked;

		public OnClick onTutorialMoveNextButton5Clicked;

		public OnClick onTutorialMoveNextButton6Clicked;

		public OnClick onTutorialMoveNextButton7Clicked;

		private bool tutorial5Showed;

		private FishingManager fishingManager;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
			actionFishingButton.onClick.AddListener(delegate
			{
				if (onFishingButtonClicked != null)
				{
					onFishingButtonClicked();
				}
			});
			prevChangeFish.onClick.AddListener(delegate
			{
				if (onPrevChangeFishClicked != null)
				{
					onPrevChangeFishClicked();
				}
			});
			nextChangeFish.onClick.AddListener(delegate
			{
				if (onNextChangeFishClicked != null)
				{
					onNextChangeFishClicked();
				}
			});
			prevChangeRod.onClick.AddListener(delegate
			{
				if (onPrevChangeRodClicked != null)
				{
					onPrevChangeRodClicked();
				}
			});
			nextChangeRod.onClick.AddListener(delegate
			{
				if (onNextChangeRodClicked != null)
				{
					onNextChangeRodClicked();
				}
			});
			tutorialMoveNextButton2.onClick.AddListener(delegate
			{
				if (onTutorialMoveNextButton2Clicked != null)
				{
					onTutorialMoveNextButton2Clicked();
				}
			});
			tutorialMoveNextButton3.onClick.AddListener(delegate
			{
				if (onTutorialMoveNextButton3Clicked != null)
				{
					onTutorialMoveNextButton3Clicked();
				}
			});
			tutorialMoveNextButton5.onClick.AddListener(delegate
			{
				if (onTutorialMoveNextButton5Clicked != null)
				{
					onTutorialMoveNextButton5Clicked();
				}
			});
			tutorialMoveNextButton6.onClick.AddListener(delegate
			{
				if (onTutorialMoveNextButton6Clicked != null)
				{
					onTutorialMoveNextButton6Clicked();
				}
			});
			tutorialMoveNextButton7.onClick.AddListener(delegate
			{
				if (onTutorialMoveNextButton7Clicked != null)
				{
					onTutorialMoveNextButton7Clicked();
				}
			});
			fishingStateText.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
			miniGameRodHandleImage = miniGameRodHandle.GetComponent<Image>();
			fishingManager = Manager.Get<FishingManager>();
		}

		public void SetRodAreaColor()
		{
			Vector3 localPosition = miniGameRodHandle.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = miniGameFishHandle.localPosition;
			float num = Mathf.Abs(x - localPosition2.x);
			Vector2 sizeDelta = miniGameFishHandle.sizeDelta;
			float num2 = sizeDelta.x / 2f;
			Vector2 sizeDelta2 = miniGameRodHandle.sizeDelta;
			float num3 = num2 + sizeDelta2.x / 2f;
			if (num < num3)
			{
				if (fishingManager.reelUpgrade)
				{
					fishingManager.catchingFishProgress += 0.4f / (float)fishingManager.fishConfigOnHook.rarity / fishingManager.reelUpgradeValue;
				}
				else
				{
					fishingManager.catchingFishProgress += 0.4f / (float)fishingManager.fishConfigOnHook.rarity;
				}
				RodNearFishColor();
				if (!fishingManager.isTutorialFinished && !tutorial5Showed)
				{
					fishingManager.tutorialPause = true;
					tutorial5Showed = true;
					tutorialPopup4.SetActive(value: false);
					tutorialPopup5.SetActive(value: true);
					SetTutorialSlider();
				}
			}
			else
			{
				fishingManager.catchingFishProgress -= 0.3f / (float)fishingManager.fishConfigOnHook.rarity;
				RodFarFishColor();
			}
		}

		public void SetFishWidth(float width)
		{
			RectTransform rectTransform = miniGameFishArea;
			float x = width / 2f;
			Vector2 offsetMin = miniGameFishArea.offsetMin;
			rectTransform.offsetMin = new Vector2(x, offsetMin.y);
			RectTransform rectTransform2 = miniGameFishArea;
			float x2 = (0f - width) / 2f;
			Vector2 offsetMax = miniGameFishArea.offsetMax;
			rectTransform2.offsetMax = new Vector2(x2, offsetMax.y);
			RectTransform rectTransform3 = miniGameFishHandle;
			Vector2 sizeDelta = miniGameFishHandle.sizeDelta;
			rectTransform3.sizeDelta = new Vector2(width, sizeDelta.y);
		}

		public void SetRodWidth(float rodWidth, float fishRarity)
		{
			float num = rodWidth - (fishRarity - 1f) * 20f;
			RectTransform rectTransform = miniGameRodArea;
			float x = num / 2f;
			Vector2 offsetMin = miniGameRodArea.offsetMin;
			rectTransform.offsetMin = new Vector2(x, offsetMin.y);
			RectTransform rectTransform2 = miniGameRodArea;
			float x2 = (0f - num) / 2f;
			Vector2 offsetMax = miniGameRodHandle.offsetMax;
			rectTransform2.offsetMax = new Vector2(x2, offsetMax.y);
			RectTransform rectTransform3 = miniGameRodHandle;
			float x3 = num;
			Vector2 sizeDelta = miniGameRodHandle.sizeDelta;
			rectTransform3.sizeDelta = new Vector2(x3, sizeDelta.y);
		}

		public void RodNearFishColor()
		{
			Color rodNearColor = RodNearColor;
			rodNearColor.a = 1f;
			miniGameRodHandleImage.color = rodNearColor;
		}

		public void RodFarFishColor()
		{
			Color rodFarColor = RodFarColor;
			rodFarColor.a = 1f;
			miniGameRodHandleImage.color = rodFarColor;
		}

		public void SetFishPivotPosition(float x)
		{
			RectTransform rectTransform = miniGameFishHandle;
			Vector2 anchorMin = miniGameFishHandle.anchorMin;
			rectTransform.anchorMin = new Vector2(x, anchorMin.y);
			RectTransform rectTransform2 = miniGameFishHandle;
			Vector2 anchorMax = miniGameFishHandle.anchorMax;
			rectTransform2.anchorMax = new Vector2(x, anchorMax.y);
		}

		public void SetTutorialSlider()
		{
			tutorialFishAreaSlider5.offsetMin = miniGameFishArea.offsetMin;
			tutorialFishAreaSlider5.offsetMax = miniGameFishArea.offsetMax;
			tutorialFishHandleSlider5.anchorMin = miniGameFishHandle.anchorMin;
			tutorialFishHandleSlider5.anchorMax = miniGameFishHandle.anchorMax;
			tutorialFishHandleSlider5.sizeDelta = miniGameFishHandle.sizeDelta;
			turorialRodAreaSlider5.offsetMin = miniGameRodArea.offsetMin;
			turorialRodAreaSlider5.offsetMax = miniGameRodArea.offsetMax;
			tutorialSlider5.value = miniGameSlider.value;
			tutorialRodHandleSlider5.sizeDelta = miniGameRodHandle.sizeDelta;
			tutorialRodHandleSlider5.GetComponent<Image>().color = Color.green;
		}

		public void SetTutorialFisherTutorSprite(Sprite image)
		{
			if (!(image == null))
			{
				Image[] array = fisherMasterTutorImages;
				foreach (Image image2 in array)
				{
					image2.sprite = image;
				}
			}
		}
	}
}
