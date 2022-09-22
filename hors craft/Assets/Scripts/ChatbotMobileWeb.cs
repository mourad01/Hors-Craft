// DecompilerFi decompiler from Assembly-CSharp.dll class: ChatbotMobileWeb
using AIMLbot;
using System;
using System.Xml;
using UnityEngine;

public class ChatbotMobileWeb
{
	private static int idGenerator;

	public int id;

	private const string UserId = "console.user";

	public Bot AimlBot;

	public User myUser;

	private string keyUserSettings = "aiml.brain";

	private XmlDocument xml;

	public ChatbotMobileWeb()
	{
		AimlBot = new Bot();
		myUser = new User("console.user", AimlBot);
		xml = new XmlDocument();
		id = idGenerator++;
	}

	public void loadAIMLFromXML(XmlReader aiml, string aimlFileName)
	{
		throw new NotImplementedException("U-u");
	}

	public void LoadGlobalSettings(string GlobalSettings)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(GlobalSettings);
		AimlBot.GlobalSettings.loadSettings(xmlDocument);
	}

	public void LoadGenderSubstitutions(string GenderSubstitutions)
	{
		xml.LoadXml(GenderSubstitutions);
		AimlBot.GenderSubstitutions.loadSettings(xml);
	}

	public void LoadPerson2Substitutions(string Person2Substitutions)
	{
		xml.LoadXml(Person2Substitutions);
		AimlBot.Person2Substitutions.loadSettings(xml);
	}

	public void LoadPersonSubstitutions(string PersonSubstitutions)
	{
		xml.LoadXml(PersonSubstitutions);
		AimlBot.PersonSubstitutions.loadSettings(xml);
	}

	public void LoadSubstitutions(string Substitutions)
	{
		xml.LoadXml(Substitutions);
		AimlBot.Substitutions.loadSettings(xml);
	}

	public void LoadDefaultPredicates(string DefaultPredicates)
	{
		xml.LoadXml(DefaultPredicates);
		AimlBot.DefaultPredicates.loadSettings(xml);
	}

	public void LoadSplitters(string Splitters)
	{
		xml.LoadXml(Splitters);
		AimlBot.loadSplittersXml(xml);
	}

	public void LoadUserDefaultPredicates()
	{
		myUser.LoadDefaultPredicates();
	}

	public string getOutput(string input)
	{
		Request request = new Request(input, myUser, AimlBot);
		Result result = AimlBot.Chat(request);
		return result.Output;
	}

	public void SaveBrain()
	{
		string outerXml = myUser.Predicates.DictionaryAsXML.OuterXml;
		PlayerPrefs.SetString(keyUserSettings, outerXml);
		UnityEngine.Debug.Log("Brain saved");
	}

	public void ClearBrain()
	{
		PlayerPrefs.DeleteKey(keyUserSettings);
	}

	public void LoadBrain()
	{
		try
		{
			string @string = PlayerPrefs.GetString(keyUserSettings);
			xml.LoadXml(@string);
			myUser.Predicates.loadSettings(xml);
			UnityEngine.Debug.Log("Brain loaded");
		}
		catch (Exception message)
		{
			UnityEngine.Debug.Log("Brain not loaded");
			UnityEngine.Debug.Log(message);
		}
	}
}
