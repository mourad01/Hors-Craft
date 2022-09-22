// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TouchIndicatorModule
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class TouchIndicatorModule : GameplayModule
	{
		public Image indicator;

		public Image fill;

		private float nextFillTime;

		private float fillStartTime;

		private float fillTimeLenght;

		private McpeSteering context => MonoBehaviourSingleton<McpeSteering>.get;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.MCPE_STEERING
		};

		public override void Init()
		{
			base.Init();
			if (Application.isPlaying)
			{
				context.RegisterPingCallback(OnPing);
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING))
			{
				if (base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (context.isPointerDown)
			{
				indicator.enabled = true;
				if (!context.isMoving && Time.time <= nextFillTime)
				{
					fill.enabled = true;
					float num = Mathf.Lerp(0f, 1f, (Time.time - fillStartTime) / fillTimeLenght);
					fill.transform.localScale = new Vector3(num, num, 1f);
				}
				else
				{
					fill.enabled = false;
				}
				Vector2 v;
				if (context.pointerID.HasValue)
				{
					int value = context.pointerID.Value;
					int i;
					for (i = 0; i < UnityEngine.Input.touchCount && UnityEngine.Input.GetTouch(i).fingerId != value; i++)
					{
					}
					v = ((i >= UnityEngine.Input.touchCount) ? ((Vector2)Input.mousePosition) : UnityEngine.Input.GetTouch(i).position);
				}
				else
				{
					v = UnityEngine.Input.mousePosition;
				}
				base.gameObject.transform.position = v;
			}
			else
			{
				indicator.enabled = false;
				fill.enabled = false;
			}
		}

		private void OnPing(float fillLenght)
		{
			fillTimeLenght = fillLenght;
			fillStartTime = Time.time;
			nextFillTime = Time.time + fillLenght;
		}
	}
}
