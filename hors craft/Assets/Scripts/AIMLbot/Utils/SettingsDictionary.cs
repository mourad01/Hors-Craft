// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.Utils.SettingsDictionary
using AIMLbot.Normalize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AIMLbot.Utils
{
	public class SettingsDictionary
	{
		private Dictionary<string, string> settingsHash = new Dictionary<string, string>();

		private List<string> orderedKeys = new List<string>();

		protected Bot bot;

		public int Count => orderedKeys.Count;

		public XmlDocument DictionaryAsXML
		{
			get
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", string.Empty);
				xmlDocument.AppendChild(newChild);
				XmlNode xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "root", string.Empty);
				xmlDocument.AppendChild(xmlNode);
				foreach (string orderedKey in orderedKeys)
				{
					XmlNode xmlNode2 = xmlDocument.CreateNode(XmlNodeType.Element, "item", string.Empty);
					XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("name");
					xmlAttribute.Value = orderedKey;
					XmlAttribute xmlAttribute2 = xmlDocument.CreateAttribute("value");
					xmlAttribute2.Value = settingsHash[orderedKey];
					xmlNode2.Attributes.Append(xmlAttribute);
					xmlNode2.Attributes.Append(xmlAttribute2);
					xmlNode.AppendChild(xmlNode2);
				}
				return xmlDocument;
			}
		}

		public string[] SettingNames
		{
			get
			{
				string[] array = new string[orderedKeys.Count];
				orderedKeys.CopyTo(array, 0);
				return array;
			}
		}

		public SettingsDictionary(Bot bot)
		{
			this.bot = bot;
		}

		public void loadSettings(string pathToSettings)
		{
			if (pathToSettings.Length > 0)
			{
				FileInfo fileInfo = new FileInfo(pathToSettings);
				if (fileInfo.Exists)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(pathToSettings);
					loadSettings(xmlDocument);
					return;
				}
				throw new FileNotFoundException();
			}
			throw new FileNotFoundException();
		}

		public void loadSettings(XmlDocument settingsAsXML)
		{
			clearSettings();
			XmlNodeList childNodes = settingsAsXML.DocumentElement.ChildNodes;
			IEnumerator enumerator = childNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode xmlNode = (XmlNode)enumerator.Current;
					if (((xmlNode.Name == "item") & (xmlNode.Attributes.Count == 2)) && ((xmlNode.Attributes[0].Name == "name") & (xmlNode.Attributes[1].Name == "value")))
					{
						string value = xmlNode.Attributes["name"].Value;
						string value2 = xmlNode.Attributes["value"].Value;
						if (value.Length > 0)
						{
							addSetting(value, value2);
						}
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

		public void addSetting(string name, string value)
		{
			string text = MakeCaseInsensitive.TransformInput(name);
			if (text.Length > 0)
			{
				removeSetting(text);
				orderedKeys.Add(text);
				settingsHash.Add(MakeCaseInsensitive.TransformInput(text), value);
			}
		}

		public void removeSetting(string name)
		{
			string text = MakeCaseInsensitive.TransformInput(name);
			orderedKeys.Remove(text);
			removeFromHash(text);
		}

		private void removeFromHash(string name)
		{
			string key = MakeCaseInsensitive.TransformInput(name);
			settingsHash.Remove(key);
		}

		public void updateSetting(string name, string value)
		{
			string text = MakeCaseInsensitive.TransformInput(name);
			if (orderedKeys.Contains(text))
			{
				removeFromHash(text);
				settingsHash.Add(MakeCaseInsensitive.TransformInput(text), value);
			}
		}

		public void clearSettings()
		{
			orderedKeys.Clear();
			settingsHash.Clear();
		}

		public string grabSetting(string name)
		{
			string text = MakeCaseInsensitive.TransformInput(name);
			if (containsSettingCalled(text))
			{
				return settingsHash[text];
			}
			return string.Empty;
		}

		public bool containsSettingCalled(string name)
		{
			string text = MakeCaseInsensitive.TransformInput(name);
			if (text.Length > 0)
			{
				return orderedKeys.Contains(text);
			}
			return false;
		}

		public void Clone(SettingsDictionary target)
		{
			foreach (string orderedKey in orderedKeys)
			{
				target.addSetting(orderedKey, grabSetting(orderedKey));
			}
		}
	}
}
