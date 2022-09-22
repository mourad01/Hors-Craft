// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.condition
using AIMLbot.Utils;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class condition : AIMLTagHandler
	{
		public condition(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
			isRecursive = false;
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "condition")
			{
				if (templateNode.Attributes.Count == 2)
				{
					string text = string.Empty;
					string text2 = string.Empty;
					if (templateNode.Attributes[0].Name == "name")
					{
						text = templateNode.Attributes[0].Value;
					}
					else if (templateNode.Attributes[0].Name == "value")
					{
						text2 = templateNode.Attributes[0].Value;
					}
					if (templateNode.Attributes[1].Name == "name")
					{
						text = templateNode.Attributes[1].Value;
					}
					else if (templateNode.Attributes[1].Name == "value")
					{
						text2 = templateNode.Attributes[1].Value;
					}
					if ((text.Length > 0) & (text2.Length > 0))
					{
						string input = user.Predicates.grabSetting(text);
						Regex regex = new Regex(text2.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"), RegexOptions.IgnoreCase);
						if (regex.IsMatch(input))
						{
							return templateNode.InnerXml;
						}
					}
				}
				else if (templateNode.Attributes.Count == 1)
				{
					if (templateNode.Attributes[0].Name == "name")
					{
						string value = templateNode.Attributes[0].Value;
						IEnumerator enumerator = templateNode.ChildNodes.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								XmlNode xmlNode = (XmlNode)enumerator.Current;
								if (xmlNode.Name.ToLower() == "li")
								{
									if (xmlNode.Attributes.Count == 1)
									{
										if (xmlNode.Attributes[0].Name.ToLower() == "value")
										{
											string input2 = user.Predicates.grabSetting(value);
											Regex regex2 = new Regex(xmlNode.Attributes[0].Value.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"), RegexOptions.IgnoreCase);
											if (regex2.IsMatch(input2))
											{
												return xmlNode.InnerXml;
											}
										}
									}
									else if (xmlNode.Attributes.Count == 0)
									{
										return xmlNode.InnerXml;
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
				}
				else if (templateNode.Attributes.Count == 0)
				{
					IEnumerator enumerator2 = templateNode.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							XmlNode xmlNode2 = (XmlNode)enumerator2.Current;
							if (xmlNode2.Name.ToLower() == "li")
							{
								if (xmlNode2.Attributes.Count == 2)
								{
									string text3 = string.Empty;
									string text4 = string.Empty;
									if (xmlNode2.Attributes[0].Name == "name")
									{
										text3 = xmlNode2.Attributes[0].Value;
									}
									else if (xmlNode2.Attributes[0].Name == "value")
									{
										text4 = xmlNode2.Attributes[0].Value;
									}
									if (xmlNode2.Attributes[1].Name == "name")
									{
										text3 = xmlNode2.Attributes[1].Value;
									}
									else if (xmlNode2.Attributes[1].Name == "value")
									{
										text4 = xmlNode2.Attributes[1].Value;
									}
									if ((text3.Length > 0) & (text4.Length > 0))
									{
										string input3 = user.Predicates.grabSetting(text3);
										Regex regex3 = new Regex(text4.Replace(" ", "\\s").Replace("*", "[\\sA-Z0-9]+"), RegexOptions.IgnoreCase);
										if (regex3.IsMatch(input3))
										{
											return xmlNode2.InnerXml;
										}
									}
								}
								else if (xmlNode2.Attributes.Count == 0)
								{
									return xmlNode2.InnerXml;
								}
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
			}
			return string.Empty;
		}
	}
}
