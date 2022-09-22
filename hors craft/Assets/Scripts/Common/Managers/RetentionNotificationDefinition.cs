// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RetentionNotificationDefinition
using UnityEngine;

namespace Common.Managers
{
	public class RetentionNotificationDefinition
	{
		public string callbackData;

		public uint delayInDays;

		protected string _text;

		private const string ALREADY_OPENED_PREFIX_ID = "retention.notification.already.opened.";

		public virtual string text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public bool textsPresent => !string.IsNullOrEmpty(text);

		public bool openedOnceAlready
		{
			get
			{
				return PlayerPrefs.GetInt("retention.notification.already.opened." + callbackData, 0) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("retention.notification.already.opened." + callbackData, 1);
			}
		}

		public RetentionNotificationDefinition(string callbackData, uint delayInDays)
		{
			this.callbackData = callbackData;
			this.delayInDays = delayInDays;
		}
	}
}
