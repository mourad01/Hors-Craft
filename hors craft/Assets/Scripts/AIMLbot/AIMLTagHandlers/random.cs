// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.random
using AIMLbot.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class random : AIMLTagHandler
	{
		public random(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
			isRecursive = false;
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "random" && templateNode.HasChildNodes)
			{
				List<XmlNode> list = new List<XmlNode>();
				IEnumerator enumerator = templateNode.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						XmlNode xmlNode = (XmlNode)enumerator.Current;
						if (xmlNode.Name == "li")
						{
							list.Add(xmlNode);
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
				if (list.Count > 0)
				{
					Random random = new Random();
					XmlNode xmlNode2 = list[random.Next(list.Count)];
					return xmlNode2.InnerXml;
				}
			}
			return string.Empty;
		}
	}
}
