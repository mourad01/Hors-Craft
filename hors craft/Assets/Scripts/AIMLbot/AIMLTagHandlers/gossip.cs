// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.gossip
using AIMLbot.Utils;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class gossip : AIMLTagHandler
	{
		public gossip(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "gossip" && templateNode.InnerText.Length > 0)
			{
				bot.writeToLog("GOSSIP from user: " + user.UserID + ", '" + templateNode.InnerText + "'");
			}
			return string.Empty;
		}
	}
}
