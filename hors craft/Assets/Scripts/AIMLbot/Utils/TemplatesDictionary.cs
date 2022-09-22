// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.Utils.TemplatesDictionary
using AIMLbot.Normalize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace AIMLbot.Utils
{
	public class TemplatesDictionary
	{
		private class HasChildrenToTemplate
		{
			public bool hasChildren;

			public string template;
		}

		public const int INITIAL_CAPACITY = 262144;

		private Dictionary<string, HasChildrenToTemplate> dict;

		public TemplatesDictionary()
		{
			dict = new Dictionary<string, HasChildrenToTemplate>(262144);
		}

		public void LoadFromStream(StreamReader reader)
		{
			while (!reader.EndOfStream)
			{
				string key = MakeCaseInsensitive.TransformInput(reader.ReadLine()).Trim();
				string template = reader.ReadLine();
				string value = reader.ReadLine();
				dict.Add(key, new HasChildrenToTemplate
				{
					hasChildren = !string.IsNullOrEmpty(value),
					template = template
				});
			}
			UnityEngine.Debug.LogWarning("TemplatesDictionary.Count: " + dict.Count);
		}

		public string evaluate(string processedPath, string path, SubQuery query, Request request, MatchState matchstate, StringBuilder wildcard)
		{
			if (request.StartedOn.AddMilliseconds(request.bot.TimeOut) < DateTime.Now)
			{
				request.bot.writeToLog("WARNING! Request timeout. User: " + request.user.UserID + " raw input: \"" + request.rawInput + "\"");
				request.hasTimedOut = true;
				return string.Empty;
			}
			path = path.Trim();
			processedPath = processedPath.Trim();
			if (!dict[processedPath].hasChildren)
			{
				if (path.Length > 0)
				{
					storeWildCard(path, wildcard);
				}
				UnityEngine.Debug.LogWarning("END - no children found - " + dict[processedPath].template);
				return dict[processedPath].template;
			}
			if (path.Length == 0)
			{
				return dict[processedPath].template;
			}
			string[] array = path.Split(" \r\n\t".ToCharArray());
			string text = MakeCaseInsensitive.TransformInput(array[0]);
			string path2 = path.Substring(text.Length, path.Length - text.Length);
			string text2 = (processedPath + " _").Trim();
			if (dict.ContainsKey(text2))
			{
				StringBuilder stringBuilder = new StringBuilder();
				storeWildCard(array[0], stringBuilder);
				string text3 = evaluate(text2, path2, query, request, matchstate, stringBuilder);
				if (text3.Length > 0)
				{
					if (stringBuilder.Length > 0)
					{
						switch (matchstate)
						{
						case MatchState.UserInput:
							query.InputStar.Add(stringBuilder.ToString());
							stringBuilder.Remove(0, stringBuilder.Length);
							break;
						case MatchState.That:
							query.ThatStar.Add(stringBuilder.ToString());
							break;
						case MatchState.Topic:
							query.TopicStar.Add(stringBuilder.ToString());
							break;
						}
					}
					return text3;
				}
			}
			text2 = (processedPath + " " + text).Trim();
			if (dict.ContainsKey(text2))
			{
				MatchState matchstate2 = matchstate;
				if (text == "<THAT>")
				{
					matchstate2 = MatchState.That;
				}
				else if (text == "<TOPIC>")
				{
					matchstate2 = MatchState.Topic;
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				string text4 = evaluate(text2, path2, query, request, matchstate2, stringBuilder2);
				if (text4.Length > 0)
				{
					if (stringBuilder2.Length > 0)
					{
						switch (matchstate)
						{
						case MatchState.UserInput:
							query.InputStar.Add(stringBuilder2.ToString());
							stringBuilder2.Remove(0, stringBuilder2.Length);
							break;
						case MatchState.That:
							query.ThatStar.Add(stringBuilder2.ToString());
							stringBuilder2.Remove(0, stringBuilder2.Length);
							break;
						case MatchState.Topic:
							query.TopicStar.Add(stringBuilder2.ToString());
							stringBuilder2.Remove(0, stringBuilder2.Length);
							break;
						}
					}
					return text4;
				}
			}
			text2 = (processedPath + " *").Trim();
			if (dict.ContainsKey(text2))
			{
				StringBuilder stringBuilder3 = new StringBuilder();
				storeWildCard(array[0], stringBuilder3);
				string text5 = evaluate(text2, path2, query, request, matchstate, stringBuilder3);
				if (text5.Length > 0)
				{
					if (stringBuilder3.Length > 0)
					{
						switch (matchstate)
						{
						case MatchState.UserInput:
							query.InputStar.Add(stringBuilder3.ToString());
							stringBuilder3.Remove(0, stringBuilder3.Length);
							break;
						case MatchState.That:
							query.ThatStar.Add(stringBuilder3.ToString());
							break;
						case MatchState.Topic:
							query.TopicStar.Add(stringBuilder3.ToString());
							break;
						}
					}
					return text5;
				}
			}
			if (processedPath.EndsWith("_") || processedPath.EndsWith("*"))
			{
				storeWildCard(array[0], wildcard);
				return evaluate(processedPath, path2, query, request, matchstate, wildcard);
			}
			wildcard = new StringBuilder();
			return string.Empty;
		}

		private void storeWildCard(string word, StringBuilder wildcard)
		{
			if (wildcard.Length > 0)
			{
				wildcard.Append(" ");
			}
			wildcard.Append(word);
		}
	}
}
