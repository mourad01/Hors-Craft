// DecompilerFi decompiler from Assembly-CSharp.dll class: Danceable
using Uniblocks;
using UnityEngine;

public class Danceable : MonoBehaviour
{
	public string[] danceTriggers = new string[4]
	{
		"dance1",
		"dance2",
		"dance3",
		"dance4"
	};

	[HideInInspector]
	public bool isDancing;

	[HideInInspector]
	public DanceConfig.DanceStyle dancingStyle;

	private const float RESET_TIMER = 0.5f;

	private Animator humanAnimator;

	private RuntimeAnimatorController oldAnimator;

	private Collider triggerCollider;

	private string trigger = string.Empty;

	private float timer;

	private UsableIndicator indicator;

	private void Awake()
	{
		isDancing = false;
		indicator = GetComponent<UsableIndicator>();
	}

	private void Update()
	{
		if (isDancing && !triggerCollider)
		{
			if (timer <= 0f)
			{
				StopDance();
			}
			else
			{
				timer -= Time.deltaTime;
			}
		}
		if (isDancing && GetComponentInChildren<Animator>() == humanAnimator && humanAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			humanAnimator.SetTrigger(trigger);
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("DanceArea"))
		{
			timer = 0.5f;
			if (!isDancing)
			{
				InitDance();
				dancingStyle = collider.gameObject.GetComponent<DanceConfig>().dancingStyle;
				triggerCollider = collider;
			}
		}
	}

	private void InitDance()
	{
		if (indicator != null)
		{
			indicator.SpawnIndicator();
		}
		humanAnimator = GetComponentInChildren<Animator>();
		oldAnimator = humanAnimator.runtimeAnimatorController;
		humanAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("prefabs/party_animator");
		isDancing = true;
		GetComponent<MobNavigator>().enabled = false;
		if (string.IsNullOrEmpty(trigger))
		{
			trigger = danceTriggers[Random.Range(0, danceTriggers.Length)];
		}
		humanAnimator.SetTrigger(trigger);
	}

	private void StopDance()
	{
		if (indicator != null)
		{
			indicator.DestroyIndicator();
		}
		humanAnimator.runtimeAnimatorController = oldAnimator;
		isDancing = false;
		GetComponent<MobNavigator>().enabled = true;
	}
}
