// DecompilerFi decompiler from Assembly-CSharp.dll class: RandNotificationManager
using Common.Managers;
using UnityEngine;

public class RandNotificationManager : RetentionNotificationsManager
{
	public class RandNotificationDefinition : RetentionNotificationDefinition
	{
		public override string text
		{
			get
			{
				string[] array = _text.Split(';');
				string text = array[Random.Range(0, array.Length)];
				return Manager.Get<TranslationsManager>().GetText(text, text);
			}
			set
			{
				base.text = value;
			}
		}

		public RandNotificationDefinition(string callbackData, uint delayInDays)
			: base(callbackData, delayInDays)
		{
		}
	}

	public override void Init()
	{
		SetDefinitions(new RandNotificationDefinition[4]
		{
			new RandNotificationDefinition("retention.1d", 1u),
			new RandNotificationDefinition("retention.3d", 3u),
			new RandNotificationDefinition("retention.7d", 7u),
			new RandNotificationDefinition("retention.30d", 30u)
		});
		base.Init();
	}
}
