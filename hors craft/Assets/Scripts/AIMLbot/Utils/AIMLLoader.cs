// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.Utils.AIMLLoader
using AIMLbot.Normalize;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AIMLbot.Utils
{
	public class AIMLLoader
	{
		private Bot bot;

		private Node baseNode;

		public AIMLLoader(Node baseNode)
		{
			this.baseNode = baseNode;
		}

		public void loadAIML()
		{
		}

		public void loadAIML(string path)
		{
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*.aiml");
				if (files.Length > 0)
				{
					string[] array = files;
					foreach (string filename in array)
					{
						loadAIMLFile(filename);
					}
					return;
				}
				throw new FileNotFoundException("Could not find any .aiml files in the specified directory (" + path + "). Please make sure that your aiml file end in a lowercase aiml extension, for example - myFile.aiml is valid but myFile.AIML is not.");
			}
			throw new FileNotFoundException("The directory specified as the path to the AIML files (" + path + ") cannot be found by the AIMLLoader object. Please make sure the directory where you think the AIML files are to be found is the same as the directory specified in the settings file.");
		}

		public void loadAIMLFile(string filename)
		{
			XmlReader xml = XmlReader.Create(filename);
			loadAIMLFromXML(xml, filename);
		}

		public void loadAIMLFromXML(XmlReader xml, string filename)
		{
			while (xml.Read())
			{
				if (xml.NodeType == XmlNodeType.Element)
				{
					if (xml.Name == "topic")
					{
						processTopic(xml, filename);
					}
					else if (xml.Name == "category")
					{
						processCategory(xml, filename);
					}
				}
			}
		}

		private void processTopic(XmlReader xml, string filename)
		{
			string topicName = "*";
			string attribute = xml.GetAttribute("name");
			if (!string.IsNullOrEmpty(attribute))
			{
				topicName = attribute;
			}
			if (xml.ReadToDescendant("category"))
			{
				do
				{
					processCategory(xml, topicName, filename);
				}
				while (xml.ReadToNextSibling("category"));
			}
		}

		private void processCategory(XmlReader xml, string filename)
		{
			processCategory(xml, "*", filename);
		}

		private void processCategory(XmlReader xml, string topicName, string filename)
		{
			string pattern = string.Empty;
			string that = "*";
			string template = string.Empty;
			while (xml.Read() && (!(xml.Name == "category") || xml.NodeType != XmlNodeType.EndElement))
			{
				if (xml.NodeType == XmlNodeType.Element)
				{
					if (xml.Name == "pattern")
					{
						pattern = xml.ReadInnerXml();
					}
					else if (xml.Name == "that")
					{
						that = xml.ReadInnerXml();
					}
					else if (xml.Name == "template")
					{
						template = xml.ReadOuterXml();
					}
				}
			}
			string text = generatePath(pattern, that, topicName, isUserInput: false);
			if (text.Length > 0)
			{
				List<string> pathWords = new List<string>(MakeCaseInsensitive.TransformInput(text).Trim().Split(' '));
				baseNode.addCategory(pathWords, template);
				if (bot != null)
				{
					bot.Size++;
				}
			}
		}

		public string generatePath(XmlReader xml, string topicName, bool isUserInput)
		{
			string pattern = xml.ReadInnerXml();
			string that = "*";
			if (xml.ReadToNextSibling("that"))
			{
				that = xml.ReadInnerXml();
			}
			return generatePath(pattern, that, topicName, isUserInput);
		}

		public string generatePath(string pattern, string that, string topicName, bool isUserInput)
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
				empty = Normalize(pattern, isUserInput).Trim();
				text = Normalize(that, isUserInput).Trim();
				text2 = Normalize(topicName, isUserInput).Trim();
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
				if (text.Length > Bot.MaxThatSize)
				{
					text = "*";
				}
				return empty + " <that> " + text + " <topic> " + text2;
			}
			return string.Empty;
		}

		public string Normalize(string input, bool isUserInput)
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
	}
}
