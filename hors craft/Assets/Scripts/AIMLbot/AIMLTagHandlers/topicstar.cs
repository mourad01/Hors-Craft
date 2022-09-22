// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.topicstar
using AIMLbot.Utils;
using System;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class topicstar : AIMLTagHandler
	{
		public topicstar(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "topicstar")
			{
				if (templateNode.Attributes.Count == 0)
				{
					if (query.TopicStar.Count > 0)
					{
						return query.TopicStar[0];
					}
					bot.writeToLog("ERROR! An out of bounds index to topicstar was encountered when processing the input: " + request.rawInput);
				}
				else if (templateNode.Attributes.Count == 1 && templateNode.Attributes[0].Name.ToLower() == "index" && templateNode.Attributes[0].Value.Length > 0)
				{
					try
					{
						int num = Convert.ToInt32(templateNode.Attributes[0].Value.Trim());
						if (query.TopicStar.Count > 0)
						{
							if (num > 0)
							{
								return query.TopicStar[num - 1];
							}
							bot.writeToLog("ERROR! An input tag with a bady formed index (" + templateNode.Attributes[0].Value + ") was encountered processing the input: " + request.rawInput);
						}
						else
						{
							bot.writeToLog("ERROR! An out of bounds index to topicstar was encountered when processing the input: " + request.rawInput);
						}
					}
					catch
					{
						bot.writeToLog("ERROR! A thatstar tag with a bady formed index (" + templateNode.Attributes[0].Value + ") was encountered processing the input: " + request.rawInput);
					}
				}
			}
			return string.Empty;
		}
	}
}
