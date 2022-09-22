// DecompilerFi decompiler from Assembly-CSharp.dll class: PettableHuman
using com.ootii.Cameras;
using Common.Managers;
using States;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PettableHuman : Pettable
{
	private PlayerGraphic _mobPlayerGraphic;

	private bool moveQueried;

	private bool hasFlower;

	private GameObject flowerPrefab;

	private GameObject flowerUngrabParticlesPrefab;

	private float flowerTimer;

	public int outfitIndex => base.gameObject.GetComponent<HumanMob>().skinIndex;

	private PlayerGraphic mobPlayerGraphic
	{
		get
		{
			if (_mobPlayerGraphic == null)
			{
				_mobPlayerGraphic = GetComponentInChildren<PlayerGraphic>();
			}
			return _mobPlayerGraphic;
		}
		set
		{
			_mobPlayerGraphic = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (IsRecruitSoldierEnabled())
		{
			customParticlesPrefabName = "orders";
		}
	}

	public void DanceFail()
	{
		base.tameValue = Mathf.Max(0f, base.tameValue - -1f);
	}

	public void DanceTame(Transform pivot, Vector3 position, string layer)
	{
		base.tameValue = Mathf.Min(tameMaxValue, base.tameValue + 10f);
		GameObject prefab = Resources.Load<GameObject>("prefabs/hearts");
		PlayParticles(prefab, pivot, position + base.mob.transform.position, layer);
	}

	public override void Tame()
	{
		base.Tame();
		if (mobPlayerGraphic != null && hasFlower)
		{
			UnGrabFlower();
		}
	}

	protected override void RunTameCallbacks()
	{
		Manager.Get<QuestManager>().OnPetHuman(id);
		if (!base.tamed)
		{
			if (base.tameValue >= tameMaxValue)
			{
				LastTame();
			}
			else
			{
				RunAway();
			}
		}
	}

	private bool CanPropose()
	{
		return mobPlayerGraphic != null && mobPlayerGraphic.holdingPivot != null;
	}

	protected override void LastTame()
	{
		if (CanPropose() && !IsRecruitSoldierEnabled())
		{
			AskForProposal();
		}
		else
		{
			LastTameHuman();
		}
	}

	private void LastTameHuman()
	{
		base.LastTame();
		if (mobPlayerGraphic != null && mobPlayerGraphic.holdingPivot != null && !IsRecruitSoldierEnabled() && !hasFlower)
		{
			GrabFlower();
		}
	}

	private void AskForProposal()
	{
		Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
		{
			configureLeftButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "dating.no";
				text.defaultText = "NO";
				button.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				});
			},
			configureRightButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "dating.yes";
				text.defaultText = "YES";
				button.onClick.AddListener(delegate
				{
					LastTameHuman();
					Manager.Get<StateMachineManager>().PopState();
					Manager.Get<QuestManager>().OnDateHuman(id);
					if (Manager.Contains<AbstractAchievementManager>())
					{
						Manager.Get<AbstractAchievementManager>().RegisterEvent("npc.relationship");
					}
				});
			},
			configureMessage = delegate(TranslateText text)
			{
				text.translationKey = "dating.proposal";
				text.defaultText = "Do you wanna date this {0}?";
				text.AddVisitor(delegate(string t)
				{
					string empty = string.Empty;
					if (GetComponent<HumanMob>().currentGender == Skin.Gender.FEMALE)
					{
						empty = Manager.Get<TranslationsManager>().GetText("dating.woman", string.Empty);
						if (string.IsNullOrEmpty(empty))
						{
							empty = "girl";
						}
					}
					else
					{
						empty = Manager.Get<TranslationsManager>().GetText("dating.man", string.Empty);
						if (string.IsNullOrEmpty(empty))
						{
							empty = "boy";
						}
					}
					return t.Replace("{0}", empty);
				});
			}
		});
	}

	protected override void RunAway()
	{
		base.mob.DisableBehaviourTree(4f);
		base.mob.animator.SetTrigger("refuse");
		if (!moveQueried)
		{
			moveQueried = true;
			StartCoroutine(DoAfterTime(delegate
			{
				MoveAwayHuman();
			}, 2f));
		}
	}

	private void MoveAwayHuman()
	{
		Vector3 position = CameraController.instance.MainCamera.transform.position;
		Vector3 normalized = (base.transform.position - position).normalized;
		float num = UnityEngine.Random.Range(-90f, 90f);
		float num2 = Mathf.Cos((float)Math.PI / 180f * num);
		float num3 = Mathf.Sin((float)Math.PI / 180f * num);
		Vector3 direction = new Vector3(num2 * normalized.x - num3 * normalized.z, 0f, num3 * normalized.x + num2 * normalized.z);
		base.mob.MoveAway(direction, UnityEngine.Random.Range(4f, 8f), 0f);
		moveQueried = false;
	}

	protected override void OnTamed()
	{
		MoveModeChange();
		base.mob.navigator.SetDestination(CameraController.instance.MainCamera.transform.position);
		base.mob.navigator.speed = base.mob.runSpeed;
		if (IsRecruitSoldierEnabled())
		{
			base.gameObject.AddComponent<Recruit>();
			return;
		}
		Manager.Get<PetManager>().RegisterPet(this);
		Manager.Get<PetManager>().TryToUnlockTamedPet(this);
		Manager.Get<QuestManager>().HandleMobIndicator(base.mob.gameObject);
	}

	protected override void Update()
	{
		base.Update();
		if (hasFlower)
		{
			if (flowerTimer <= 0f)
			{
				UnGrabFlower();
			}
			else
			{
				flowerTimer -= Time.deltaTime;
			}
		}
	}

	public void GrabFlower(float duration = 5f)
	{
		if (flowerPrefab == null)
		{
			flowerPrefab = Resources.Load<GameObject>("prefabs/Flower");
		}
		GameObject go = UnityEngine.Object.Instantiate(flowerPrefab);
		mobPlayerGraphic.Grab(go);
		hasFlower = true;
		flowerTimer = duration;
	}

	public void UnGrabFlower()
	{
		mobPlayerGraphic.UnGrab();
		if (flowerUngrabParticlesPrefab == null)
		{
			flowerUngrabParticlesPrefab = Resources.Load<GameObject>("prefabs/FlowerParticles");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(flowerUngrabParticlesPrefab);
		gameObject.transform.SetParent(mobPlayerGraphic.holdingPivot.transform, worldPositionStays: false);
		hasFlower = false;
	}

	private bool IsRecruitSoldierEnabled()
	{
		return SurvivalContextsBroadcaster.instance.GetContext<SoldiersRecruiterContext>()?.isRecruitSoldierEnabled ?? false;
	}

	protected override bool HasToBePatient()
	{
		HospitalManager hospitalManager = Manager.Get<HospitalManager>();
		if (hospitalManager == null || (hospitalManager.vampireMode.enabled && base.tamed))
		{
			return false;
		}
		return hospitalManager.patientsType == HospitalManager.PatientsType.Human;
	}

	private IEnumerator DoAfterTime(Action action, float timer)
	{
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		action();
	}
}
