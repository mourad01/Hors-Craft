// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.size
using AIMLbot.Utils;
using System;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class size : AIMLTagHandler
	{
		public size(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "size")
			{
				return Convert.ToString(bot.Size);
			}
			return string.Empty;
		}
	}
}
