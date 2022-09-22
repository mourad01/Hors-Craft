// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.sr
using AIMLbot.Utils;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class sr : AIMLTagHandler
	{
		public sr(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "sr")
			{
				XmlNode node = AIMLTagHandler.getNode("<star/>");
				star star = new star(bot, user, query, request, result, node);
				string str = star.Transform();
				XmlNode node2 = AIMLTagHandler.getNode("<srai>" + str + "</srai>");
				srai srai = new srai(bot, user, query, request, result, node2);
				return srai.Transform();
			}
			return string.Empty;
		}
	}
}
