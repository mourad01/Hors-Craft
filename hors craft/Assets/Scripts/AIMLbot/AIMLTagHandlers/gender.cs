// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.gender
using AIMLbot.Normalize;
using AIMLbot.Utils;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class gender : AIMLTagHandler
	{
		public gender(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "gender")
			{
				if (templateNode.InnerText.Length > 0)
				{
					return ApplySubstitutions.Substitute(bot, bot.GenderSubstitutions, templateNode.InnerText);
				}
				XmlNode node = AIMLTagHandler.getNode("<star/>");
				star star = new star(bot, user, query, request, result, node);
				templateNode.InnerText = star.Transform();
				if (templateNode.InnerText.Length > 0)
				{
					return ProcessChange();
				}
				return string.Empty;
			}
			return string.Empty;
		}
	}
}
