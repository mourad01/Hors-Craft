// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoveBarModule
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class LoveBarModule : GameplayModule
	{
		public Image loveBar;

		public Image loveIcon;

		public Sprite heartSprite;

		private float timer;

		private bool blockedDecline;

		private bool showHeartsWhenActive;

		private Vector3 heartStartPosition;

		private int number;

		private float gainPerHeart;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.IN_LOVE
		};

		public override void Init()
		{
			base.Init();
		}

		protected override void Update()
		{
			base.Update();
			if (blockedDecline && timer <= 0f)
			{
				blockedDecline = false;
			}
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			if (showHeartsWhenActive)
			{
				StartCoroutine(SpawnHearts(heartStartPosition, number, gainPerHeart));
				showHeartsWhenActive = false;
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_LOVE))
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			UpdateLoveBarProgress();
			UpdateLoveIcon();
			UpdateHeartAnimation();
		}

		private void UpdateLoveBarProgress()
		{
			LoveContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<LoveContext>(Fact.IN_LOVE);
			if (!blockedDecline)
			{
				if (factContext != null)
				{
					loveBar.fillAmount = factContext.loveValue / factContext.maxLoveValue;
				}
				base.gameObject.SetActive(factContext != null);
			}
		}

		private void UpdateLoveIcon()
		{
			SpriteContainerContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<SpriteContainerContext>(Fact.IN_LOVE);
			if (factContext != null)
			{
				loveIcon.sprite = factContext.sprite;
			}
		}

		private void UpdateHeartAnimation()
		{
			LoveContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<LoveContext>(Fact.IN_LOVE);
			DatedHeartContext factContext2 = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<DatedHeartContext>(Fact.IN_LOVE);
			if (factContext != null && factContext2 != null)
			{
				blockedDecline = true;
				timer = 3f;
				int num = 5 + (int)(factContext2.progress * 9f);
				float value = factContext2.value;
				float num2 = value / (float)num;
				float num3 = num2 / factContext.maxLoveValue;
				heartStartPosition = factContext2.heartStartPosition;
				number = num;
				gainPerHeart = num3;
				showHeartsWhenActive = true;
			}
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_LOVE, factContext2);
		}

		private void OnHeartFinish(float progress)
		{
			if (loveBar != null)
			{
				loveBar.fillAmount += progress;
				timer = 0.5f;
			}
		}

		private Action OnHeartFinishAction(float progress)
		{
			return delegate
			{
				OnHeartFinish(progress);
			};
		}

		private IEnumerator SpawnHearts(Vector3 pos, int count, float progress)
		{
			while (count > 0)
			{
				PickupJumpingToYourPocketUI.SpawnPickup(scale: UnityEngine.Random.Range(0.6f, 1.2f), startPosition: pos + UnityEngine.Random.insideUnitSphere * 0.2f, target: loveBar.gameObject, sprite: heartSprite, onFinish: OnHeartFinishAction(progress));
				count--;
				yield return new WaitForSeconds(0.15f);
			}
		}
	}
}
