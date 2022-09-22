// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.uppercase
using AIMLbot.Utils;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class uppercase : AIMLTagHandler
	{
		public uppercase(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "uppercase")
			{
				return templateNode.InnerText.ToUpper(bot.Locale);
			}
			return string.Empty;
		}
	}
}
