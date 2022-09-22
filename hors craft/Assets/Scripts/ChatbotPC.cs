// DecompilerFi decompiler from Assembly-CSharp.dll class: ChatbotPC
using AIMLbot;
using System;
using UnityEngine;

public class ChatbotPC
{
	private const string UserId = "consoleUser";

	public Bot AimlBot;

	public User myUser;

	public string pathToUserSettings;

	public ChatbotPC()
	{
		AimlBot = new Bot();
		myUser = new User("consoleUser", AimlBot);
		Initialize();
		if (!Application.isEditor)
		{
			pathToUserSettings = Application.streamingAssetsPath + "\\Brain-Graphmaster.xml";
		}
		else
		{
			pathToUserSettings = Application.persistentDataPath + "\\Brain-Graphmaster.xml";
		}
	}

	public void Initialize()
	{
		AimlBot.ChangeMyPath = Application.streamingAssetsPath;
		AimlBot.loadSettings();
		AimlBot.isAcceptingUserInput = false;
		AimlBot.isAcceptingUserInput = true;
	}

	public string getOutput(string input)
	{
		Request request = new Request(input, myUser, AimlBot);
		Result result = AimlBot.Chat(request);
		return result.Output;
	}

	public void SaveBrain()
	{
		try
		{
			myUser.Predicates.DictionaryAsXML.Save(pathToUserSettings);
			UnityEngine.Debug.Log("Brain saved");
		}
		catch (Exception message)
		{
			UnityEngine.Debug.Log("Brain not saved");
			UnityEngine.Debug.Log(message);
		}
	}

	public void LoadBrain()
	{
		try
		{
			myUser.Predicates.loadSettings(pathToUserSettings);
			UnityEngine.Debug.Log("Brain loaded");
		}
		catch (Exception message)
		{
			UnityEngine.Debug.Log("Brain not loaded");
			UnityEngine.Debug.Log(message);
		}
	}
}
