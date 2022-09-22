// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.RunningCrosspromoInstance
using UnityEngine;

namespace TsgCommon.Crosspromo
{
	internal class RunningCrosspromoInstance
	{
		private float reloadTime;

		private CrosspromoIconCreator iconCreator;

		private CrosspromoDefinition definition;

		public long bannerId;

		public bool isAvaliable = true;

		public RunningCrosspromoInstanceConfig config
		{
			get;
			private set;
		}

		public bool started
		{
			get;
			private set;
		}

		public RunningCrosspromoInstance(RunningCrosspromoInstanceConfig config)
		{
			this.config = config;
		}

		public void StartWithDefinition(CrosspromoDefinition definition)
		{
			isAvaliable = false;
			this.definition = definition;
			reloadTime = Time.realtimeSinceStartup + definition.duration;
			iconCreator = new CrosspromoIconCreator(config.canvas, config.percentPosOnScreen, config.headerText, definition.texture, OnCrosspromoClick, config.xButtonEnabled, delegate
			{
				reloadTime = 0f;
			}, config.animatorIndex, (!definition.isGif) ? string.Empty : definition.textureUrl);
			started = true;
			if (config.showCallback != null)
			{
				config.showCallback();
			}
		}

		private void OnCrosspromoClick()
		{
			MonoBehaviourSingleton<MultiCrosspromoController>.get.clickedBannerId = bannerId;
			if (config.clickCallback != null)
			{
				config.clickCallback();
			}
			string text = definition.clickUrl;
			if (!text.Contains("http://") && !text.Contains("https://"))
			{
				text = "https://" + text;
			}
			else if (text.Contains("http://") && !text.Contains("https://"))
			{
				text = text.Replace("http://", "https://");
			}
			MonoBehaviourSingleton<MultiCrosspromoController>.get.ReloadRunningInstance(this);
			Application.OpenURL(text);
		}

		public bool HasToBeReloaded()
		{
			return Time.realtimeSinceStartup > reloadTime;
		}

		public void Dispose()
		{
			isAvaliable = true;
			if (started)
			{
				iconCreator.Dispose();
			}
		}
	}
}
