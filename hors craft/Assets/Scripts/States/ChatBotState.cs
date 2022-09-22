// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChatBotState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace States
{
	public class ChatBotState : XCraftUIState<ChatBotStateConnector>
	{
		[Serializable]
		public class CustomAnswers
		{
			[Serializable]
			public class Answer
			{
				public string[] Answers;

				public string this[int i] => Answers[i];

				public int Length => Answers.Length;

				public override string ToString()
				{
					return this[UnityEngine.Random.Range(0, Length)];
				}
			}

			[SerializeField]
			private Answer[] answers;

			public string Get()
			{
				return answers[UnityEngine.Random.Range(0, answers.Length)].ToString();
			}

			public bool IsNullOrEmpty()
			{
				if (answers == null || answers.Length <= 0)
				{
					return true;
				}
				return false;
			}
		}

		public bool useOnlyCustomAnswers;

		[SerializeField]
		private CustomAnswers customAnswers;

		private CustomAnswers customAnswersFromStartParameter;

		private const int MAX_MESSAGES_REMEMBERED = 10;

		private int strikes;

		private HumanRepresentation humanRep;

		private int currentBotId;

		private string playerName;

		private string botName;

		private ChatbotMobileWeb bot;

		private ChatBotWebData.Bot webBot;

		private GameObject waitMessage;

		private int seed;

		private bool male;

		private Dictionary<int, List<string>> chatbotSeedToMessagesHistory;

		private bool loadBot;

		private bool frontedChatIsLoaded;

		private Action OnChatClosed;

		private bool historyLoaded;

		private bool onlyCustomAnswers => customAnswersFromStartParameter != null || useOnlyCustomAnswers;

		private CustomAnswers currentCustomAnswers => (customAnswersFromStartParameter == null) ? customAnswers : customAnswersFromStartParameter;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => false;

		//protected override bool canShowBanner => false;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			ChatBotStateStartParameter chatBotStateStartParameter = parameter as ChatBotStateStartParameter;
			Manager.Get<StatsManager>().ChatbotLaunched();
			playerName = Manager.Get<TranslationsManager>().GetText("player.you", "You");
			base.connector.onReturnButton = OnReturn;
			base.connector.onSubmit = OnSend;
			seed = chatBotStateStartParameter.chatBootSeed;
			int botId = 2;
			if (chatBotStateStartParameter.mobGraphic != null && chatBotStateStartParameter.pettable != null)
			{
				humanRep = new HumanRepresentation(chatBotStateStartParameter.mobGraphic, chatBotStateStartParameter.pettable);
				if (humanRep.graphic.animator != null)
				{
					humanRep.graphic.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
					humanRep.graphic.animator.SetBool("walking", value: false);
				}
				humanRep.UIModeOn(base.connector.SetMobRepsentationPlace);
				setGender(ref botId);
				base.connector.tamePanel.gameObject.SetActive(chatBotStateStartParameter.pettable.GetComponent<LovedOne>() == null);
			}
			else
			{
				male = chatBotStateStartParameter.isMale;
			}
			if (chatBotStateStartParameter.botName != null)
			{
				botName = chatBotStateStartParameter.botName;
			}
			currentBotId = botId;
			getBots(botId);
			initiCorrectBot(botId);
			if (chatBotStateStartParameter.hideTamepanel)
			{
				base.connector.tamePanel.gameObject.SetActive(value: false);
			}
			if (chatBotStateStartParameter.hideBackground)
			{
				base.connector.cameraPlayerPreview.gameObject.SetActive(value: false);
			}
			if (chatBotStateStartParameter.customAnswers != null)
			{
				customAnswersFromStartParameter = chatBotStateStartParameter.customAnswers;
			}
			else
			{
				customAnswersFromStartParameter = null;
			}
			OnChatClosed = chatBotStateStartParameter.OnChatClosed;
		}

		private void setGender(ref int botId)
		{
			male = true;
			HumanMob component = humanRep.pettable.GetComponent<HumanMob>();
			if (component != null)
			{
				botId = component.skinIndex;
				if (component.currentGender != 0)
				{
					male = false;
				}
			}
		}

		private void getBots(int botId)
		{
			webBot = MonoBehaviourSingleton<ChatBotWebData>.get.GetBotOfSomeIndex(male, botId);
			bot = Singleton<ChatBotLoader>.get.CreateAndGetBot(male, GetSeed());
		}

		private int GetSeed()
		{
			if (humanRep != null)
			{
				return humanRep.pettable.chatbotSeed;
			}
			return seed;
		}

		private void initiCorrectBot(int botId)
		{
			if (!Manager.Get<ModelManager>().chatbotSettings.IsWebChatBotEnabled())
			{
				StartCoroutine(LoadBot());
			}
			else if (webBot == null)
			{
				StartCoroutine(fakeLoadingForTimeout());
				MonoBehaviourSingleton<ChatBotWebData>.get.Initialize(delegate
				{
					webBot = MonoBehaviourSingleton<ChatBotWebData>.get.GetBotOfSomeIndex(male, botId);
					ShowInitial();
				}, delegate
				{
					base.connector.SetLoadingPercentage(0f);
					StopCoroutine("fakeLoadingForTimeout");
					StartCoroutine(LoadBot());
				});
			}
			else
			{
				ShowInitial();
			}
		}

		public void ShowInitial()
		{
			if (!MonoBehaviourSingleton<ChatBotWebData>.get.canSend)
			{
				StartCoroutine(LoadBot());
				return;
			}
			if (botName.IsNullOrEmpty())
			{
				botName = webBot.name;
			}
			LoadHistory();
			base.connector.SetLoadingPercentage(0f);
			StartCoroutine(fakeLoadingForTimeout());
			MonoBehaviourSingleton<ChatBotWebData>.get.SendRequest((!onlyCustomAnswers) ? "Hi There!" : currentCustomAnswers.Get(), webBot.id, onFirstWebResponse, OnMessageSendError);
		}

		private IEnumerator fakeLoadingForTimeout()
		{
			float timeOfTimeout = Manager.Get<ModelManager>().chatbotSettings.TimeoutForWeb();
			float step = 0.01f;
			float stepTime = step * timeOfTimeout;
			float current = 0f;
			while (current < 1f)
			{
				base.connector.SetLoadingPercentage(current);
				current += step;
				yield return new WaitForSecondsRealtime(stepTime);
			}
		}

		private void onFirstWebResponse(string message)
		{
			base.connector.SetLoadingPercentage(1f);
			StopCoroutine("fakeLoadingForTimeout");
			DestroyWaitMessage();
			AddWebResponse(message);
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (humanRep != null && humanRep.pettable != null)
			{
				base.connector.tamePanel.Refresh(humanRep.pettable);
			}
		}

		public override void FinishState()
		{
			base.FinishState();
			if (humanRep != null)
			{
				humanRep.UIModeOff();
			}
			StopAllCoroutines();
			humanRep = null;
			if (Singleton<ChatBotLoader>.get.loadingPercentage >= 1f && loadBot)
			{
				bot.SaveBrain();
			}
			bot = null;
			botName = null;
			historyLoaded = false;
			customAnswersFromStartParameter = null;
			Singleton<ChatBotLoader>.get.ClearBot();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			if (OnChatClosed != null)
			{
				OnChatClosed();
				OnChatClosed = null;
			}
			if (Manager.Contains<AbstractAchievementManager>())
			{
				Manager.Get<AbstractAchievementManager>().RegisterEvent("npc.talk");
			}
		}

		public void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		public void OnSend()
		{
			string text = base.connector.inputField.text;
			if (!string.IsNullOrEmpty(text))
			{
				base.connector.inputField.text = string.Empty;
				base.connector.AddNewMessage(text, playerName, right: true);
				if (humanRep != null)
				{
					humanRep.pettable.chatbotHistory.Add(new RememberedChatMessage
					{
						player = true,
						message = text
					});
				}
				if (MonoBehaviourSingleton<ChatBotWebData>.get.canSend)
				{
					DestroyWaitMessage();
					waitMessage = base.connector.AddWaitMessage(webBot.name);
					Manager.Get<QuestManager>().OnChat(webBot.id);
					Manager.Get<StatsManager>().ChatbotMessageSend();
					MonoBehaviourSingleton<ChatBotWebData>.get.SendRequest(text, webBot.id, AddWebResponse, OnMessageSendError);
				}
				else if (frontedChatIsLoaded)
				{
					Manager.Get<QuestManager>().OnChat(currentBotId);
					StartCoroutine(WaitAndReply(text));
				}
				else
				{
					StartCoroutine(LoadBot());
				}
			}
		}

		private void DestroyWaitMessage()
		{
			if (waitMessage != null)
			{
				UnityEngine.Object.Destroy(waitMessage);
			}
		}

		private void OnMessageSendError(string strikeText, int strikeNumber)
		{
			DestroyWaitMessage();
			StopCoroutine("fakeLoadingForTimeout");
			base.connector.SetLoadingPercentage(1f);
			base.connector.AddNewMessage(strikeText, botName, right: false);
			humanRep.pettable.chatbotHistory.Add(new RememberedChatMessage
			{
				player = false,
				message = strikeText
			});
		}

		private void AddWebResponse(string response)
		{
			DestroyWaitMessage();
			ProcessResponseMessage(response);
			if (humanRep != null && humanRep.pettable != null)
			{
				humanRep.pettable.ChatTame();
			}
		}

		private IEnumerator WaitAndReply(string messageIn)
		{
			yield return new WaitForSecondsRealtime(0.1f);
			string output = bot.getOutput(messageIn);
			bool timeout = output == "...";
			float waitDuration = (!timeout) ? UnityEngine.Random.Range(0f, 1.5f) : 0f;
			yield return new WaitForSecondsRealtime(waitDuration);
			ProcessResponseMessage(output);
			if (!timeout && humanRep != null)
			{
				humanRep.pettable.ChatTame();
			}
		}

		private void ProcessResponseMessage(string message)
		{
			if (onlyCustomAnswers)
			{
				message = currentCustomAnswers.Get();
			}
			base.connector.AddNewMessage(message, botName, right: false);
			if (humanRep != null)
			{
				humanRep.pettable.chatbotHistory.Add(new RememberedChatMessage
				{
					player = false,
					message = message
				});
				humanRep.graphic.animator.SetTrigger("make_random_gesture");
				humanRep.graphic.animator.SetInteger("random_gesture", UnityEngine.Random.Range(0, 3));
			}
		}

		private void LoadHistory()
		{
			if (!historyLoaded)
			{
				historyLoaded = true;
				if (humanRep != null)
				{
					foreach (RememberedChatMessage item in humanRep.pettable.chatbotHistory)
					{
						string name = (!item.player) ? botName : playerName;
						base.connector.AddNewMessage(item.message, name, item.player);
					}
				}
			}
		}

		private void TrimHistory()
		{
			while (humanRep.pettable.chatbotHistory.Count > 10)
			{
				humanRep.pettable.chatbotHistory.RemoveAt(0);
			}
		}

		private IEnumerator LoadBot()
		{
			UnityEngine.Debug.LogError("DO NOT USE LOCAL CHATBOT ONLY SERVER");
			yield break;
		}
	}
}
