// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.Utils.Node
using AIMLbot.Normalize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AIMLbot.Utils
{
	[Serializable]
	public class Node
	{
		public class NameToNode
		{
			public int nameHash;

			public Node node;

			public NameToNode(int nameHash, Node node)
			{
				this.nameHash = nameHash;
				this.node = node;
			}
		}

		public List<NameToNode> children;

		public string template = string.Empty;

		public string word = string.Empty;

		public int NumberOfChildNodes => (children != null) ? children.Count : 0;

		public Node(bool allocateMore = false)
		{
			if (allocateMore)
			{
				children = new List<NameToNode>(4096);
			}
		}

		public void SaveIntoStream(HashSet<string> alreadyWritterWords, StreamWriter writer, string pathWords, int depth = 0)
		{
			string input = (!string.IsNullOrEmpty(pathWords)) ? (pathWords + " " + word) : word;
			string text = MakeCaseInsensitive.TransformInput(input).Trim();
			if (!alreadyWritterWords.Contains(text))
			{
				writer.WriteLine(text);
				writer.WriteLine(template.Replace('\n', ' '));
				writer.WriteLine((NumberOfChildNodes <= 0) ? string.Empty : "1");
				alreadyWritterWords.Add(text);
			}
			if (children != null)
			{
				foreach (NameToNode child in children)
				{
					child.node.SaveIntoStream(alreadyWritterWords, writer, text, depth + 1);
				}
			}
		}

		public void addCategory(List<string> pathWords, string template)
		{
			if (pathWords.Count == 0)
			{
				this.template = template;
				return;
			}
			Node child = GetChild(pathWords[0].GetHashCode());
			if (child != null)
			{
				pathWords.RemoveAt(0);
				child.addCategory(pathWords, template);
				return;
			}
			child = new Node();
			child.word = pathWords[0];
			pathWords.RemoveAt(0);
			child.addCategory(pathWords, template);
			if (children == null)
			{
				children = new List<NameToNode>();
			}
			children.Add(new NameToNode(child.word.GetHashCode(), child));
		}

		private Node GetChild(int hash)
		{
			if (children == null)
			{
				return null;
			}
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].nameHash == hash)
				{
					return children[i].node;
				}
			}
			return null;
		}

		public string evaluate(string path, SubQuery query, Request request, MatchState matchstate, StringBuilder wildcard)
		{
			if (request.StartedOn.AddMilliseconds(request.bot.TimeOut) < DateTime.Now)
			{
				request.bot.writeToLog("WARNING! Request timeout. User: " + request.user.UserID + " raw input: \"" + request.rawInput + "\"");
				request.hasTimedOut = true;
				return string.Empty;
			}
			path = path.Trim();
			if (children == null || children.Count == 0)
			{
				if (path.Length > 0)
				{
					storeWildCard(path, wildcard);
				}
				return template;
			}
			if (path.Length == 0)
			{
				return template;
			}
			string[] array = path.Split(" \r\n\t".ToCharArray());
			string text = MakeCaseInsensitive.TransformInput(array[0]);
			string path2 = path.Substring(text.Length, path.Length - text.Length);
			Node child = GetChild("_".GetHashCode());
			if (child != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				storeWildCard(array[0], stringBuilder);
				string text2 = child.evaluate(path2, query, request, matchstate, stringBuilder);
				if (text2.Length > 0)
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
					return text2;
				}
			}
			child = GetChild(text.GetHashCode());
			if (child != null)
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
				string text3 = child.evaluate(path2, query, request, matchstate2, stringBuilder2);
				if (text3.Length > 0)
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
					return text3;
				}
			}
			child = GetChild("*".GetHashCode());
			if (child != null)
			{
				StringBuilder stringBuilder3 = new StringBuilder();
				storeWildCard(array[0], stringBuilder3);
				string text4 = child.evaluate(path2, query, request, matchstate, stringBuilder3);
				if (text4.Length > 0)
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
					return text4;
				}
			}
			if (word == "_" || word == "*")
			{
				storeWildCard(array[0], wildcard);
				return evaluate(path2, query, request, matchstate, wildcard);
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
