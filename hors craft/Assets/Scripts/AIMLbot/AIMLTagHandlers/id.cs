// DecompilerFi decompiler from Assembly-CSharp.dll class: AIMLbot.AIMLTagHandlers.id
using AIMLbot.Utils;
using System.Xml;

namespace AIMLbot.AIMLTagHandlers
{
	public class id : AIMLTagHandler
	{
		public id(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
			: base(bot, user, query, request, result, templateNode)
		{
		}

		protected override string ProcessChange()
		{
			if (templateNode.Name.ToLower() == "id")
			{
				return user.UserID;
			}
			return string.Empty;
		}
	}
}
