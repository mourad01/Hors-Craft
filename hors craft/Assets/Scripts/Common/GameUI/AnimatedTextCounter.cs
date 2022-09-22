// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.GameUI.AnimatedTextCounter
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Common.GameUI
{
	public class AnimatedTextCounter : MonoBehaviour
	{
		public bool playCoinBumpSound = true;

		private Text text;

		private int current;

		private int target;

		private float lastBumpTime;

		private const float BUMPS_INTERVAL = 0.15f;

		private const float BUMP_X_SCALE = 1.125f;

		private const float BUMP_Y_SCALE = 1.1f;

		private void Awake()
		{
			text = GetComponent<Text>();
			current = 0;
			RefreshValue();
		}

		private void OnGUI()
		{
			if (current != target)
			{
				if (Time.realtimeSinceStartup > lastBumpTime + 0.15f)
				{
					Bump();
				}
			}
			else
			{
				base.transform.localScale = Vector3.one;
			}
			Vector3 localScale = base.transform.localScale;
			if (localScale.x > 1f)
			{
				float t = (Time.realtimeSinceStartup - lastBumpTime) / 0.15f;
				base.transform.localScale = new Vector3(Mathf.Lerp(1.125f, 1f, t) * 1.1f, Mathf.Lerp(1.1f, 1f, t), 1f);
			}
		}

		private void Bump()
		{
			lastBumpTime = Time.realtimeSinceStartup;
			base.transform.localScale = new Vector3(1.125f, 1.1f, 1f);
			int num = Mathf.Max(1, (target - current) / 2);
			current = Mathf.Min(current + num, target);
			RefreshValue();
			if (playCoinBumpSound)
			{
				Sound sound = new Sound();
				sound.clip = Manager.Get<ClipsManager>().GetClipFor("coin_bump");
				sound.mixerGroup = Manager.Get<MixersManager>().uiMixerGroup;
				Sound sound2 = sound;
				sound2.Play();
			}
		}

		public void Animate(int from, int to)
		{
			current = from;
			target = to;
		}

		public void AnimateTo(int to, bool immediate)
		{
			if (immediate || current >= to)
			{
				current = (target = to);
				RefreshValue();
			}
			else
			{
				target = to;
				Bump();
			}
		}

		private void RefreshValue()
		{
			text.text = current.ToString();
		}
	}
}
