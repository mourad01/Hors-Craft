// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RelationshipFragment
using Common.Managers;
using GameUI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class RelationshipFragment : Fragment
	{
		public GameObject noRelationshipContent;

		public GameObject relationshipContent;

		public Button sendSmsButton;

		public Button giftButton;

		public Button dateButton;

		public Button breakUpButton;

		public Button leaderboardsButton;

		public Image favouriteGiftIcon;

		public Image relationshipProgress;

		public Image relationshipStageImage;

		public Text relationshipStageText;

		public Text relationshipDurationText;

		public Text dateButtonText;

		public Text smsButtonText;

		public Camera cameraPlayerPreview;

		public ModelDragRotator rotator;

		public Vector3 graphicOffset = new Vector3(0f, -0.25f, 3.4f);

		private LoveManager loveManager;

		private HumanRepresentation humanRep;

		private LovedOne lovedOne;

		private int previousCooldown = -1;

		private int previousSmsCooldown = -1;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			InitListeners();
			InitLove();
		}

		private void InitListeners()
		{
			leaderboardsButton.onClick.AddListener(delegate
			{
				OnLeaderboards();
			});
			giftButton.onClick.AddListener(delegate
			{
				OnGift();
			});
			dateButton.onClick.AddListener(delegate
			{
				OnDate();
			});
			breakUpButton.onClick.AddListener(delegate
			{
				OnBreakUp();
			});
			if (sendSmsButton != null)
			{
				sendSmsButton.onClick.AddListener(delegate
				{
					OnSendSms();
				});
			}
		}

		private void InitLove()
		{
			loveManager = Manager.Get<LoveManager>();
			lovedOne = loveManager.lovedOne;
			relationshipContent.SetActive(lovedOne != null);
			noRelationshipContent.SetActive(lovedOne == null);
			if (lovedOne != null)
			{
				InitHumanRepresentation();
				UpdateRelationshipStage();
				if (!loveManager.allowBreakUp)
				{
					breakUpButton.gameObject.SetActive(value: false);
				}
			}
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			if (lovedOne != null)
			{
				UpdateRelationshipStage();
				if (humanRep == null)
				{
					InitHumanRepresentation();
				}
			}
		}

		private void UpdateRelationshipStage()
		{
			LoveManager.RelationshipStage relationshipStage = lovedOne.relationshipStage;
			relationshipStageImage.sprite = relationshipStage.tamePanelSprite;
			relationshipStageText.text = Manager.Get<TranslationsManager>().GetText(relationshipStage.stageNameKey, relationshipStage.stageNameDefault).ToUpper();
			favouriteGiftIcon.sprite = loveManager.gifts[lovedOne.info.favouriteGiftIndex].sprite;
			LoveContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<LoveContext>(Fact.IN_LOVE);
			if (factContext != null)
			{
				relationshipProgress.fillAmount = factContext.loveValue / factContext.maxLoveValue;
			}
			float minutesSpentInRelationship = lovedOne.minutesSpentInRelationship;
			int num = Mathf.FloorToInt(minutesSpentInRelationship / 1440f);
			minutesSpentInRelationship -= (float)num * 1440f;
			int num2 = Mathf.FloorToInt(minutesSpentInRelationship / 60f);
			minutesSpentInRelationship -= (float)num2 * 60f;
			relationshipDurationText.text = $"{num.ToString()}d {num2.ToString()}h {((int)minutesSpentInRelationship).ToString()}m";
		}

		private void InitHumanRepresentation()
		{
			humanRep = new HumanRepresentation(lovedOne.GetComponentInChildren<PlayerGraphic>());
			humanRep.UIModeOn(SetLoverRepresentationPlace);
			humanRep.graphic.ShowBodyAndLegs();
			humanRep.graphic.ShowHands();
		}

		private void SetLoverRepresentationPlace(GameObject go)
		{
			SetOffsetToAspect();
			go.transform.SetParent(cameraPlayerPreview.transform, worldPositionStays: false);
			go.transform.position = cameraPlayerPreview.transform.position + graphicOffset;
			go.transform.localRotation = Quaternion.Euler(0f, 146f, 0f);
			rotator.modelToRotate = go;
		}

		private void SetOffsetToAspect()
		{
			if (Screen.width <= 800)
			{
				graphicOffset = new Vector3(0f, -0.25f, 2.4f);
			}
			else if (Screen.width > 800 && Screen.width <= 1300)
			{
				graphicOffset = new Vector3(0f, -0.6f, 3f);
			}
			else if (Screen.width >= 1300 && Screen.width < 2000)
			{
				graphicOffset = new Vector3(0f, -0.5f, 4.4f);
			}
			else if (Screen.width >= 2000)
			{
				graphicOffset = new Vector3(0f, -0.7f, 5.4f);
			}
		}

		private void Update()
		{
			UpdateDateButton();
			if (smsButtonText != null)
			{
				UpdateSmsButton();
			}
		}

		private void UpdateDateButton()
		{
			float dateCooldown = loveManager.GetDateCooldown();
			if (previousCooldown != (int)dateCooldown)
			{
				if (dateCooldown == 0f)
				{
					dateButtonText.text = Manager.Get<TranslationsManager>().GetText("love.date", "Go on a date!");
				}
				else
				{
					string text = Manager.Get<TranslationsManager>().GetText("love.next.date.in", "Next date in {0}");
					TimeSpan timeSpan = TimeSpan.FromSeconds(dateCooldown);
					string newValue = string.Format("{0}:{1}", timeSpan.Minutes.ToString("00"), timeSpan.Seconds.ToString("00"));
					text = text.Replace("{0}", newValue);
					dateButtonText.text = text;
				}
				previousCooldown = (int)dateCooldown;
			}
		}

		private void UpdateSmsButton()
		{
			float smsCooldown = loveManager.GetSmsCooldown();
			if (previousSmsCooldown != (int)smsCooldown)
			{
				if (smsCooldown == 0f)
				{
					smsButtonText.text = Manager.Get<TranslationsManager>().GetText("love.send.sms", "Send a message!");
				}
				else
				{
					string text = Manager.Get<TranslationsManager>().GetText("love.next.sms.in", "Next message in {0}");
					TimeSpan timeSpan = TimeSpan.FromSeconds(smsCooldown);
					string newValue = string.Format("{0}:{1}", timeSpan.Minutes.ToString("00"), timeSpan.Seconds.ToString("00"));
					text = text.Replace("{0}", newValue);
					smsButtonText.text = text;
				}
				previousSmsCooldown = (int)smsCooldown;
			}
		}

		private void OnGift()
		{
			TurnHumaRepOff();
			loveManager.OpenGifts();
		}

		private void OnDate()
		{
			TurnHumaRepOff();
			loveManager.TryToDate(lovedOne.gameObject);
		}

		private void OnBreakUp()
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "love.sure.break.up";
					t.defaultText = "Are you sure you want to end this relationship?";
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
					t.translationKey = "menu.cancel";
					t.defaultText = "cancel";
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
						loveManager.BreakUp();
					});
					t.translationKey = "menu.ok";
					t.defaultText = "ok";
				}
			});
		}

		private void OnSendSms()
		{
			Manager.Get<LoveManager>().TrySendSms();
		}

		private void OnLeaderboards()
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(RankingsState)))
			{
				Manager.Get<StateMachineManager>().PushState<RankingsState>();
			}
			else if (Manager.Contains<RankingManager>() && Manager.Get<AbstractModelManager>().rankingsSettings.RankingsEnabled())
			{
				startParameter.pauseState.ChangeFor("Ranking");
				Manager.Get<SocialPlatformManager>().social.ShowRankings();
			}
			else
			{
				UnityEngine.Debug.LogError("Couldn't find RankinsgState or RankingFragment in Pause");
			}
		}

		private void TurnHumaRepOff()
		{
			if (humanRep != null)
			{
				PlayerGraphic graphic = humanRep.graphic;
				humanRep.UIModeOff();
				humanRep = null;
				graphic.graphicRepresentation.transform.localScale = new Vector3(graphic.scaleFactor, graphic.scaleFactor, graphic.scaleFactor);
			}
		}

		public void OnDisable()
		{
			TurnHumaRepOff();
		}
	}
}
