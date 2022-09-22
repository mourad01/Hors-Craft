// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.Bot
using AIMLbot.AIMLTagHandlers;
using AIMLbot.Normalize;
using AIMLbot.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace AIMLbot
{
	public class Bot
	{
		public delegate void LogMessageDelegate();

		public SettingsDictionary GlobalSettings;

		public SettingsDictionary GenderSubstitutions;

		public SettingsDictionary Person2Substitutions;

		public SettingsDictionary PersonSubstitutions;

		public SettingsDictionary Substitutions;

		public SettingsDictionary DefaultPredicates;

		private Dictionary<string, TagHandler> CustomTags;

		private Dictionary<string, Assembly> LateBindingAssemblies = new Dictionary<string, Assembly>();

		public List<string> Splitters = new List<string>();

		private List<string> LogBuffer = new List<string>();

		public bool isAcceptingUserInput = true;

		public DateTime StartedOn = DateTime.Now;

		public int Size;

		public TemplatesDictionary Graphmaster;

		public bool TrustAIML = true;

		public static int MaxThatSize = 256;

		private string MyPath = Application.streamingAssetsPath;

		public string LastLogMessage = string.Empty;

		private int MaxLogBufferSize => Convert.ToInt32(GlobalSettings.grabSetting("maxlogbuffersize"));

		private string NotAcceptingUserInputMessage => GlobalSettings.grabSetting("notacceptinguserinputmessage");

		public double TimeOut => Convert.ToDouble(GlobalSettings.grabSetting("timeout"));

		public string TimeOutMessage => GlobalSettings.grabSetting("timeoutmessage");

		public CultureInfo Locale => new CultureInfo(GlobalSettings.grabSetting("culture"));

		public Regex Strippers => new Regex(GlobalSettings.grabSetting("stripperregex"), RegexOptions.IgnorePatternWhitespace);

		public string AdminEmail
		{
			get
			{
				return GlobalSettings.grabSetting("adminemail");
			}
			set
			{
				if (value.Length > 0)
				{
					string pattern = "^(([^<>()[\\]\\\\.,;:\\s@\\\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\\\"]+)*)|(\\\".+\\\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$";
					Regex regex = new Regex(pattern);
					if (!regex.IsMatch(value))
					{
						throw new Exception("The AdminEmail is not a valid email address");
					}
					GlobalSettings.addSetting("adminemail", value);
				}
				else
				{
					GlobalSettings.addSetting("adminemail", string.Empty);
				}
			}
		}

		public bool IsLogging
		{
			get
			{
				string text = GlobalSettings.grabSetting("islogging");
				if (text.ToLower() == "true")
				{
					return true;
				}
				return false;
			}
		}

		public bool WillCallHome
		{
			get
			{
				string text = GlobalSettings.grabSetting("willcallhome");
				if (text.ToLower() == "true")
				{
					return true;
				}
				return false;
			}
		}

		public Gender Sex
		{
			get
			{
				switch (Convert.ToInt32(GlobalSettings.grabSetting("gender")))
				{
				case -1:
					return Gender.Unknown;
				case 0:
					return Gender.Female;
				case 1:
					return Gender.Male;
				default:
					return Gender.Unknown;
				}
			}
		}

		public string PathToAIML => Path.Combine(MyPath, GlobalSettings.grabSetting("aimldirectory"));

		public string PathToConfigFiles => Path.Combine(MyPath, GlobalSettings.grabSetting("configdirectory"));

		public string PathToLogs => Path.Combine(MyPath, GlobalSettings.grabSetting("logdirectory"));

		public string ChangeMyPath
		{
			get
			{
				return MyPath;
			}
			set
			{
				MyPath = value;
			}
		}

		public event LogMessageDelegate WrittenToLog;

		public Bot()
		{
			setup();
		}

		private void setup()
		{
			GlobalSettings = new SettingsDictionary(this);
			GenderSubstitutions = new SettingsDictionary(this);
			Person2Substitutions = new SettingsDictionary(this);
			PersonSubstitutions = new SettingsDictionary(this);
			Substitutions = new SettingsDictionary(this);
			DefaultPredicates = new SettingsDictionary(this);
			CustomTags = new Dictionary<string, TagHandler>();
		}

		public void loadSettings()
		{
			string pathToSettings = Path.Combine(MyPath, Path.Combine("config", "Settings.xml"));
			loadSettings(pathToSettings);
		}

		public void loadSettings(string pathToSettings)
		{
			GlobalSettings.loadSettings(pathToSettings);
			if (!GlobalSettings.containsSettingCalled("version"))
			{
				GlobalSettings.addSetting("version", Environment.Version.ToString());
			}
			if (!GlobalSettings.containsSettingCalled("name"))
			{
				GlobalSettings.addSetting("name", "Unknown");
			}
			if (!GlobalSettings.containsSettingCalled("botmaster"))
			{
				GlobalSettings.addSetting("botmaster", "Unknown");
			}
			if (!GlobalSettings.containsSettingCalled("master"))
			{
				GlobalSettings.addSetting("botmaster", "Unknown");
			}
			if (!GlobalSettings.containsSettingCalled("author"))
			{
				GlobalSettings.addSetting("author", "Nicholas H.Tollervey");
			}
			if (!GlobalSettings.containsSettingCalled("location"))
			{
				GlobalSettings.addSetting("location", "Unknown");
			}
			if (!GlobalSettings.containsSettingCalled("gender"))
			{
				GlobalSettings.addSetting("gender", "-1");
			}
			if (!GlobalSettings.containsSettingCalled("birthday"))
			{
				GlobalSettings.addSetting("birthday", "2006/11/08");
			}
			if (!GlobalSettings.containsSettingCalled("birthplace"))
			{
				GlobalSettings.addSetting("birthplace", "Towcester, Northamptonshire, UK");
			}
			if (!GlobalSettings.containsSettingCalled("website"))
			{
				GlobalSettings.addSetting("website", "http://sourceforge.net/projects/aimlbot");
			}
			if (GlobalSettings.containsSettingCalled("adminemail"))
			{
				string text2 = AdminEmail = GlobalSettings.grabSetting("adminemail");
			}
			else
			{
				GlobalSettings.addSetting("adminemail", string.Empty);
			}
			if (!GlobalSettings.containsSettingCalled("islogging"))
			{
				GlobalSettings.addSetting("islogging", "False");
			}
			if (!GlobalSettings.containsSettingCalled("willcallhome"))
			{
				GlobalSettings.addSetting("willcallhome", "False");
			}
			if (!GlobalSettings.containsSettingCalled("timeout"))
			{
				GlobalSettings.addSetting("timeout", "2000");
			}
			if (!GlobalSettings.containsSettingCalled("timeoutmessage"))
			{
				GlobalSettings.addSetting("timeoutmessage", "ERROR: The request has timed out.");
			}
			if (!GlobalSettings.containsSettingCalled("culture"))
			{
				GlobalSettings.addSetting("culture", "en-US");
			}
			if (!GlobalSettings.containsSettingCalled("splittersfile"))
			{
				GlobalSettings.addSetting("splittersfile", "Splitters.xml");
			}
			if (!GlobalSettings.containsSettingCalled("person2substitutionsfile"))
			{
				GlobalSettings.addSetting("person2substitutionsfile", "Person2Substitutions.xml");
			}
			if (!GlobalSettings.containsSettingCalled("personsubstitutionsfile"))
			{
				GlobalSettings.addSetting("personsubstitutionsfile", "PersonSubstitutions.xml");
			}
			if (!GlobalSettings.containsSettingCalled("gendersubstitutionsfile"))
			{
				GlobalSettings.addSetting("gendersubstitutionsfile", "GenderSubstitutions.xml");
			}
			if (!GlobalSettings.containsSettingCalled("defaultpredicates"))
			{
				GlobalSettings.addSetting("defaultpredicates", "DefaultPredicates.xml");
			}
			if (!GlobalSettings.containsSettingCalled("substitutionsfile"))
			{
				GlobalSettings.addSetting("substitutionsfile", "Substitutions.xml");
			}
			if (!GlobalSettings.containsSettingCalled("aimldirectory"))
			{
				GlobalSettings.addSetting("aimldirectory", "aiml");
			}
			if (!GlobalSettings.containsSettingCalled("configdirectory"))
			{
				GlobalSettings.addSetting("configdirectory", "config");
			}
			if (!GlobalSettings.containsSettingCalled("logdirectory"))
			{
				GlobalSettings.addSetting("logdirectory", "logs");
			}
			if (!GlobalSettings.containsSettingCalled("maxlogbuffersize"))
			{
				GlobalSettings.addSetting("maxlogbuffersize", "64");
			}
			if (!GlobalSettings.containsSettingCalled("notacceptinguserinputmessage"))
			{
				GlobalSettings.addSetting("notacceptinguserinputmessage", "This bot is currently set to not accept user input.");
			}
			if (!GlobalSettings.containsSettingCalled("stripperregex"))
			{
				GlobalSettings.addSetting("stripperregex", "[^0-9a-zA-Z]");
			}
			Person2Substitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("person2substitutionsfile")));
			PersonSubstitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("personsubstitutionsfile")));
			GenderSubstitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("gendersubstitutionsfile")));
			DefaultPredicates.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("defaultpredicates")));
			Substitutions.loadSettings(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("substitutionsfile")));
			loadSplitters(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("splittersfile")));
		}

		private void loadSplitters(string pathToSplitters)
		{
			FileInfo fileInfo = new FileInfo(pathToSplitters);
			if (fileInfo.Exists)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(pathToSplitters);
				if (xmlDocument.ChildNodes.Count == 2 && xmlDocument.LastChild.HasChildNodes)
				{
					IEnumerator enumerator = xmlDocument.LastChild.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							XmlNode xmlNode = (XmlNode)enumerator.Current;
							if ((xmlNode.Name == "item") & (xmlNode.Attributes.Count == 1))
							{
								string value = xmlNode.Attributes["value"].Value;
								Splitters.Add(value);
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			if (Splitters.Count == 0)
			{
				Splitters.Add(".");
				Splitters.Add("!");
				Splitters.Add("?");
				Splitters.Add(";");
			}
		}

		public void loadSplittersXml(XmlDocument splittersXmlDoc)
		{
			if (splittersXmlDoc.ChildNodes.Count == 2 && splittersXmlDoc.LastChild.HasChildNodes)
			{
				IEnumerator enumerator = splittersXmlDoc.LastChild.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						XmlNode xmlNode = (XmlNode)enumerator.Current;
						if ((xmlNode.Name == "item") & (xmlNode.Attributes.Count == 1))
						{
							string value = xmlNode.Attributes["value"].Value;
							Splitters.Add(value);
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (Splitters.Count == 0)
			{
				Splitters.Add(".");
				Splitters.Add("!");
				Splitters.Add("?");
				Splitters.Add(";");
			}
		}

		public void writeToLog(string message)
		{
			LastLogMessage = message;
			if (IsLogging)
			{
				LogBuffer.Add(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
				if (LogBuffer.Count > MaxLogBufferSize - 1)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(PathToLogs);
					if (!directoryInfo.Exists)
					{
						directoryInfo.Create();
					}
					string path = DateTime.Now.ToString("yyyyMMdd") + ".log";
					FileInfo fileInfo = new FileInfo(Path.Combine(PathToLogs, path));
					StreamWriter streamWriter = fileInfo.Exists ? fileInfo.AppendText() : fileInfo.CreateText();
					foreach (string item in LogBuffer)
					{
						streamWriter.WriteLine(item);
					}
					streamWriter.Close();
					LogBuffer.Clear();
				}
			}
			if (!object.Equals(null, this.WrittenToLog))
			{
				this.WrittenToLog();
			}
		}

		public Result Chat(string rawInput, string UserGUID)
		{
			Request request = new Request(rawInput, new User(UserGUID, this), this);
			return Chat(request);
		}

		public Result Chat(Request request)
		{
			Result result = new Result(request.user, this, request);
			if (isAcceptingUserInput)
			{
				SplitIntoSentences splitIntoSentences = new SplitIntoSentences(this);
				string[] array = splitIntoSentences.Transform(request.rawInput);
				string[] array2 = array;
				foreach (string text in array2)
				{
					result.InputSentences.Add(text);
					string item = generatePath(this, text, request.user.getLastBotOutput(), request.user.Topic, isUserInput: true);
					result.NormalizedPaths.Add(item);
				}
				foreach (string normalizedPath in result.NormalizedPaths)
				{
					SubQuery subQuery = new SubQuery(normalizedPath);
					UnityEngine.Debug.LogWarning("normalized path: " + normalizedPath);
					subQuery.Template = Graphmaster.evaluate(string.Empty, normalizedPath, subQuery, request, MatchState.UserInput, new StringBuilder());
					result.SubQueries.Add(subQuery);
				}
				foreach (SubQuery subQuery2 in result.SubQueries)
				{
					if (subQuery2.Template.Length > 0)
					{
						try
						{
							XmlNode node = AIMLTagHandler.getNode(subQuery2.Template);
							string text2 = processNode(node, subQuery2, request, result, request.user);
							if (text2.Length > 0)
							{
								result.OutputSentences.Add(text2);
							}
						}
						catch (Exception ex)
						{
							if (WillCallHome)
							{
								phoneHome(ex.Message, request);
							}
							writeToLog("WARNING! A problem was encountered when trying to process the input: " + request.rawInput + " with the template: \"" + subQuery2.Template + "\"");
						}
					}
				}
			}
			else
			{
				result.OutputSentences.Add(NotAcceptingUserInputMessage);
			}
			result.Duration = DateTime.Now - request.StartedOn;
			request.user.addResult(result);
			return result;
		}

		private string processNode(XmlNode node, SubQuery query, Request request, Result result, User user)
		{
			if (request.StartedOn.AddMilliseconds(request.bot.TimeOut) < DateTime.Now)
			{
				request.bot.writeToLog("WARNING! Request timeout. User: " + request.user.UserID + " raw input: \"" + request.rawInput + "\" processing template: \"" + query.Template + "\"");
				request.hasTimedOut = true;
				return string.Empty;
			}
			string text = node.Name.ToLower();
			if (text == "template")
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (node.HasChildNodes)
				{
					IEnumerator enumerator = node.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							XmlNode node2 = (XmlNode)enumerator.Current;
							stringBuilder.Append(processNode(node2, query, request, result, user));
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
				return stringBuilder.ToString();
			}
			AIMLTagHandler aIMLTagHandler = null;
			aIMLTagHandler = getBespokeTags(user, query, request, result, node);
			if (object.Equals(null, aIMLTagHandler))
			{
				switch (text)
				{
				case "bot":
					aIMLTagHandler = new bot(this, user, query, request, result, node);
					break;
				case "condition":
					aIMLTagHandler = new condition(this, user, query, request, result, node);
					break;
				case "date":
					aIMLTagHandler = new date(this, user, query, request, result, node);
					break;
				case "formal":
					aIMLTagHandler = new formal(this, user, query, request, result, node);
					break;
				case "gender":
					aIMLTagHandler = new gender(this, user, query, request, result, node);
					break;
				case "get":
					aIMLTagHandler = new get(this, user, query, request, result, node);
					break;
				case "gossip":
					aIMLTagHandler = new gossip(this, user, query, request, result, node);
					break;
				case "id":
					aIMLTagHandler = new id(this, user, query, request, result, node);
					break;
				case "input":
					aIMLTagHandler = new input(this, user, query, request, result, node);
					break;
				case "javascript":
					aIMLTagHandler = new javascript(this, user, query, request, result, node);
					break;
				case "learn":
					aIMLTagHandler = new learn(this, user, query, request, result, node);
					break;
				case "lowercase":
					aIMLTagHandler = new lowercase(this, user, query, request, result, node);
					break;
				case "person":
					aIMLTagHandler = new person(this, user, query, request, result, node);
					break;
				case "person2":
					aIMLTagHandler = new person2(this, user, query, request, result, node);
					break;
				case "random":
					aIMLTagHandler = new random(this, user, query, request, result, node);
					break;
				case "sentence":
					aIMLTagHandler = new sentence(this, user, query, request, result, node);
					break;
				case "set":
					aIMLTagHandler = new set(this, user, query, request, result, node);
					break;
				case "size":
					aIMLTagHandler = new size(this, user, query, request, result, node);
					break;
				case "sr":
					aIMLTagHandler = new sr(this, user, query, request, result, node);
					break;
				case "srai":
					aIMLTagHandler = new srai(this, user, query, request, result, node);
					break;
				case "star":
					aIMLTagHandler = new star(this, user, query, request, result, node);
					break;
				case "system":
					aIMLTagHandler = new system(this, user, query, request, result, node);
					break;
				case "that":
					aIMLTagHandler = new that(this, user, query, request, result, node);
					break;
				case "thatstar":
					aIMLTagHandler = new thatstar(this, user, query, request, result, node);
					break;
				case "think":
					aIMLTagHandler = new think(this, user, query, request, result, node);
					break;
				case "topicstar":
					aIMLTagHandler = new topicstar(this, user, query, request, result, node);
					break;
				case "uppercase":
					aIMLTagHandler = new uppercase(this, user, query, request, result, node);
					break;
				case "version":
					aIMLTagHandler = new version(this, user, query, request, result, node);
					break;
				default:
					aIMLTagHandler = null;
					break;
				}
			}
			if (object.Equals(null, aIMLTagHandler))
			{
				return node.InnerText;
			}
			if (aIMLTagHandler.isRecursive)
			{
				if (node.HasChildNodes)
				{
					IEnumerator enumerator2 = node.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							XmlNode xmlNode = (XmlNode)enumerator2.Current;
							if (xmlNode.NodeType != XmlNodeType.Text)
							{
								xmlNode.InnerXml = processNode(xmlNode, query, request, result, user);
							}
						}
					}
					finally
					{
						IDisposable disposable2;
						if ((disposable2 = (enumerator2 as IDisposable)) != null)
						{
							disposable2.Dispose();
						}
					}
				}
				return aIMLTagHandler.Transform();
			}
			string str = aIMLTagHandler.Transform();
			XmlNode node3 = AIMLTagHandler.getNode("<node>" + str + "</node>");
			if (node3.HasChildNodes)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				IEnumerator enumerator3 = node3.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						XmlNode node4 = (XmlNode)enumerator3.Current;
						stringBuilder2.Append(processNode(node4, query, request, result, user));
					}
				}
				finally
				{
					IDisposable disposable3;
					if ((disposable3 = (enumerator3 as IDisposable)) != null)
					{
						disposable3.Dispose();
					}
				}
				return stringBuilder2.ToString();
			}
			return node3.InnerXml;
		}

		public static string generatePath(Bot bot, string pattern, string that, string topicName, bool isUserInput)
		{
			string empty = string.Empty;
			string text = "*";
			string text2 = "*";
			if (!isUserInput)
			{
				empty = pattern.Trim();
				text = that.Trim();
				text2 = topicName.Trim();
			}
			else
			{
				empty = Normalize(bot, pattern, isUserInput).Trim();
				text = Normalize(bot, that, isUserInput).Trim();
				text2 = Normalize(bot, topicName, isUserInput).Trim();
			}
			if (empty.Length > 0)
			{
				if (text.Length == 0)
				{
					text = "*";
				}
				if (text2.Length == 0)
				{
					text2 = "*";
				}
				if (text.Length > MaxThatSize)
				{
					text = "*";
				}
				return empty + " <THAT> " + text + " <TOPIC> " + text2;
			}
			return string.Empty;
		}

		public static string Normalize(Bot bot, string input, bool isUserInput)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ApplySubstitutions applySubstitutions = new ApplySubstitutions(bot);
			StripIllegalCharacters stripIllegalCharacters = new StripIllegalCharacters(bot);
			string text = applySubstitutions.Transform(input);
			string[] array = text.Split(" \r\n\t".ToCharArray());
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string text3 = (!isUserInput) ? ((!(text2 == "*") && !(text2 == "_")) ? stripIllegalCharacters.Transform(text2) : text2) : stripIllegalCharacters.Transform(text2);
				stringBuilder.Append(text3.Trim() + " ");
			}
			return stringBuilder.ToString().Replace("  ", " ");
		}

		public AIMLTagHandler getBespokeTags(User user, SubQuery query, Request request, Result result, XmlNode node)
		{
			if (CustomTags.ContainsKey(node.Name.ToLower()))
			{
				TagHandler tagHandler = CustomTags[node.Name.ToLower()];
				AIMLTagHandler aIMLTagHandler = tagHandler.Instantiate(LateBindingAssemblies);
				if (object.Equals(null, aIMLTagHandler))
				{
					return null;
				}
				aIMLTagHandler.user = user;
				aIMLTagHandler.query = query;
				aIMLTagHandler.request = request;
				aIMLTagHandler.result = result;
				aIMLTagHandler.templateNode = node;
				aIMLTagHandler.bot = this;
				return aIMLTagHandler;
			}
			return null;
		}

		public void loadCustomTagHandlers(string pathToDLL)
		{
			Assembly assembly = Assembly.LoadFrom(pathToDLL);
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				object[] customAttributes = types[i].GetCustomAttributes(inherit: false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					if (customAttributes[j] is CustomTagAttribute)
					{
						if (!LateBindingAssemblies.ContainsKey(assembly.FullName))
						{
							LateBindingAssemblies.Add(assembly.FullName, assembly);
						}
						TagHandler tagHandler = new TagHandler();
						tagHandler.AssemblyName = assembly.FullName;
						tagHandler.ClassName = types[i].FullName;
						tagHandler.TagName = types[i].Name.ToLower();
						if (CustomTags.ContainsKey(tagHandler.TagName))
						{
							throw new Exception("ERROR! Unable to add the custom tag: <" + tagHandler.TagName + ">, found in: " + pathToDLL + " as a handler for this tag already exists.");
						}
						CustomTags.Add(tagHandler.TagName, tagHandler);
					}
				}
			}
		}

		public void phoneHome(string errorMessage, Request request)
		{
			MailMessage mailMessage = new MailMessage("donotreply@aimlbot.com", AdminEmail);
			mailMessage.Subject = "WARNING! AIMLBot has encountered a problem...";
			string text = "Dear Botmaster,\n\n            This is an automatically generated email to report errors with your bot.\n\n            At *TIME* the bot encountered the following error:\n\n            \"*MESSAGE*\"\n\n            whilst processing the following input:\n\n            \"*RAWINPUT*\"\n\n            from the user with an id of: *USER*\n\n            The normalized paths generated by the raw input were as follows:\n\n            *PATHS*\n\n            Please check your AIML!\n\n            Regards,\n\n            The AIMLbot program.\n            ";
			text = text.Replace("*TIME*", DateTime.Now.ToString());
			text = text.Replace("*MESSAGE*", errorMessage);
			text = text.Replace("*RAWINPUT*", request.rawInput);
			text = text.Replace("*USER*", request.user.UserID);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string normalizedPath in request.result.NormalizedPaths)
			{
				stringBuilder.Append(normalizedPath + Environment.NewLine);
			}
			text = (mailMessage.Body = text.Replace("*PATHS*", stringBuilder.ToString()));
			mailMessage.IsBodyHtml = false;
			try
			{
				if (mailMessage.To.Count > 0)
				{
					SmtpClient smtpClient = new SmtpClient();
					smtpClient.Send(mailMessage);
				}
			}
			catch
			{
			}
		}
	}
}
