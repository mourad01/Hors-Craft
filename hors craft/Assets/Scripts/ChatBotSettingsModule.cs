// DecompilerFi decompiler from Assembly-CSharp.dll class: ChatBotSettingsModule
using Common.Managers;
using Common.Model;

public class ChatBotSettingsModule : ModelModule
{
	private string keyChatBotEnabled()
	{
		return "chatbot.enabled";
	}

	private string keyWebChatBotEnabled()
	{
		return "chatbot.web.enabled";
	}

	private string keyWebChatBotTimeout()
	{
		return "chatbot.web.timeout";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyChatBotEnabled(), defaultValue: true);
		descriptions.AddDescription(keyWebChatBotEnabled(), defaultValue: false);
		descriptions.AddDescription(keyWebChatBotTimeout(), 8);
	}

	public override void OnModelDownloaded()
	{
	}

	public bool IsChatBotEnabled()
	{
		return base.settings.GetBool(keyChatBotEnabled());
	}

	public bool IsWebChatBotEnabled()
	{
		return base.settings.GetBool(keyWebChatBotEnabled(), defaultValue: false);
	}

	public float TimeoutForWeb()
	{
		return base.settings.GetFloat(keyWebChatBotTimeout(), 8f);
	}
}
