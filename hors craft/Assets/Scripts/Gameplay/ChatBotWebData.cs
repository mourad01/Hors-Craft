// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.ChatBotWebData
using Common.Managers;
using Common.Utils;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Gameplay
{
	public class ChatBotWebData : MonoBehaviourSingleton<ChatBotWebData>
	{
		public class Bot
		{
			public string gender;

			public string name;

			public int id;

			public Bot(string gender, string name, int id)
			{
				this.gender = gender;
				this.name = name;
				this.id = id;
			}
		}

		private List<Bot> bots;

		private int strikes;

		public string initializeUrl => $"{Manager.Get<ConnectionInfoManager>().homeURL}chatbotAlfa/chat/bots";

		public bool initialized => bots != null && bots.Count > 0;

		public bool canSend => strikes < 3 && initialized;

		public void Initialize(Action onSuccess, Action onError)
		{
			if (Manager.Get<ModelManager>().chatbotSettings.IsWebChatBotEnabled())
			{
				bots = new List<Bot>();
				TryLoadOnTimeOut(onSuccess, onError);
			}
		}

		public void TryLoadOnTimeOut(Action onSuccess, Action onError)
		{
			WWWTimeOut.CreateWWW(Manager.Get<ModelManager>().chatbotSettings.TimeoutForWeb(), delegate(string text)
			{
				OnLoadSuccess(text, onSuccess);
			}, delegate(string text)
			{
				OnInitError(text, onError);
			}, initializeUrl);
		}

		private void OnInitError(string text, Action onFinish)
		{
			UnityEngine.Debug.LogError(text);
			onFinish?.Invoke();
		}

		private void OnLoadSuccess(string text, Action onFinish)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
			dictionary = (dictionary["bots"] as Dictionary<string, object>);
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				Dictionary<string, object> dictionary2 = item.Value as Dictionary<string, object>;
				int id = int.Parse(item.Key);
				bots.Add(new Bot(dictionary2["gender"].ToString(), dictionary2["name"].ToString(), id));
			}
			UnityEngine.Debug.Log("Bots downloaded");
			onFinish?.Invoke();
		}

		public string GetStrikeText()
		{
			switch (strikes)
			{
			case 0:
				return "Could you repeat, please?";
			case 1:
				return "Sorry, didn't hear that..";
			default:
				return "Something is wrong, please could you wait a minute?";
			}
		}

		public void SendRequest(string thingToSay, int webBotId, Action<string> onResponse, Action<string, int> onError)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Content-Type"] = "application/json";
			dictionary["X-Device-Type"] = SystemInfo.deviceModel;
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2["say"] = thingToSay;
			string s = Json.Serialize(dictionary2);
			WWWTimeOut.CreateWWW(Manager.Get<ModelManager>().chatbotSettings.TimeoutForWeb(), delegate(string text)
			{
				Dictionary<string, object> dictionary3 = Json.Deserialize(text) as Dictionary<string, object>;
				strikes = 0;
				if (dictionary3 == null)
				{
					UnityEngine.Debug.LogError(text);
					onError(GetStrikeText(), strikes);
					strikes++;
				}
				if (dictionary3.ContainsKey("botsay"))
				{
					onResponse(dictionary3["botsay"].ToString());
				}
			}, delegate(string error)
			{
				UnityEngine.Debug.LogError(error);
				onError(GetStrikeText(), strikes);
				strikes++;
			}, $"{Manager.Get<ConnectionInfoManager>().homeURL}chatbotAlfa/chat/talk?bot_id={webBotId}&convo_id={Hash.CalculateMD5Hash(PlayerId.GetId())}", Encoding.UTF8.GetBytes(s), dictionary);
		}

		public Bot GetBotOfSomeIndex(bool isMale, int index)
		{
			if (!initialized)
			{
				return null;
			}
			List<Bot> list = (!isMale) ? getFemaleBots() : getMaleBots();
			return list[index % list.Count];
		}

		private Bot getBotById(int id)
		{
			return bots.Find((Bot bot) => bot.id == id);
		}

		private List<Bot> getFemaleBots()
		{
			return getBotsByGender("woman");
		}

		private List<Bot> getMaleBots()
		{
			return getBotsByGender("man");
		}

		private List<Bot> getBotsByGender(string gender)
		{
			return bots.FindAll((Bot bot) => bot.gender == gender);
		}
	}
}
