// DecompilerFi decompiler from Assembly-CSharp.dll class: Patient
using Common.Managers;
using Gameplay;
using UnityEngine;

public class Patient : MonoBehaviour
{
	public HospitalManager.PatientConfig patientConfig;

	private GameObject equipmentIcon;

	private GameObject superstarIndicator;

	private GameObject patientHealthSlider;

	private Health health;

	private Pettable pettable;

	private PlayerGraphic playerGraphic;

	private HumanMob humanMob;

	private HospitalManager hospitalManager;

	public bool gotDisese = true;

	private float superstarPatientChance;

	private float TAKE_DMG_EVERY = 1f;

	private float timer;

	private float sickNextTime;

	private HospitalManager.PatientConfig tempPatient;

	private void Awake()
	{
		hospitalManager = Manager.Get<HospitalManager>();
		superstarPatientChance = Manager.Get<ModelManager>().hospitalSettings.GetSuperstarPatientChance();
		health = GetComponent<Health>();
		if (health == null)
		{
			health = base.gameObject.AddComponent<Health>();
		}
		pettable = GetComponent<Pettable>();
		playerGraphic = GetComponentInChildren<PlayerGraphic>();
		humanMob = GetComponent<HumanMob>();
		GameObject gameObject = UnityEngine.Object.Instantiate(hospitalManager.patientEquipmentIcon, base.transform);
		gameObject.transform.localPosition = hospitalManager.patientEquipmentIconOffset;
		equipmentIcon = gameObject.transform.GetChild(0).gameObject;
		superstarIndicator = equipmentIcon.transform.GetChild(0).gameObject;
		patientHealthSlider = gameObject.transform.GetChild(1).gameObject;
		patientHealthSlider.GetComponent<Renderer>().material.SetFloat("Fill", 1f);
		GetDisese();
		TAKE_DMG_EVERY = Manager.Get<ModelManager>().hospitalSettings.GetPatientSickTimer();
		sickNextTime = Manager.Get<ModelManager>().hospitalSettings.GetPatientSickNextTime();
	}

	private void FixedUpdate()
	{
		if (pettable.tamed)
		{
			return;
		}
		if (timer > TAKE_DMG_EVERY)
		{
			if (!hospitalManager.vampireMode.enabled)
			{
				health.hp -= health.maxHp / 100f;
			}
			timer = 0f;
			if (health.hp <= health.maxHp * sickNextTime && !gotDisese)
			{
				GetDisese();
			}
		}
		if (!hospitalManager.vampireMode.enabled)
		{
			patientHealthSlider.GetComponent<Renderer>().material.SetFloat("_Fill", health.hp / health.maxHp);
		}
		timer += Time.deltaTime;
	}

	public void TryToHeal()
	{
		if (hospitalManager.upgrades[patientConfig.upgradeIdNeededToHeal].level == 0)
		{
			Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("hospital.toast.need.better.items", "You need better equipment to heal this npc."));
		}
		else
		{
			Manager.Get<HospitalManager>().SetPatient(this);
		}
	}

	public void HealthDrop(float amount, bool isVampire = false)
	{
		if (!isVampire)
		{
			health.hp -= amount;
		}
		else
		{
			pettable.FearRunFromPlayerMode();
		}
	}

	public void HealUp(bool isVampire = false)
	{
		timer = 0f;
		health.hp = health.maxHp;
		equipmentIcon.gameObject.SetActive(value: false);
		patientHealthSlider.gameObject.SetActive(value: false);
		gotDisese = false;
		playerGraphic.UnGrab();
		if (isVampire)
		{
			pettable.FearFollowPlayerMode();
		}
	}

	public void GetDisese()
	{
		if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL))
		{
			tempPatient = hospitalManager.patients[hospitalManager.GetDisese()];
			patientConfig = new HospitalManager.PatientConfig(tempPatient);
			patientConfig.isSuperstar = (UnityEngine.Random.value < ((!hospitalManager.vampireMode.enabled) ? superstarPatientChance : 0f));
		}
		else
		{
			tempPatient = hospitalManager.patients[0];
			patientConfig = new HospitalManager.PatientConfig(tempPatient);
			patientConfig.isSuperstar = false;
		}
		equipmentIcon.GetComponent<Renderer>().sharedMaterial = hospitalManager.upgrades[patientConfig.upgradeIdNeededToHeal].icoMaterial;
		equipmentIcon.gameObject.SetActive(value: true);
		patientHealthSlider.gameObject.SetActive(!hospitalManager.vampireMode.enabled);
		if (patientConfig.isSuperstar)
		{
			superstarIndicator.SetActive(value: true);
		}
		else
		{
			superstarIndicator.SetActive(value: false);
		}
		gotDisese = true;
		GameObject itemForHandPrefab = hospitalManager.upgrades[patientConfig.upgradeIdNeededToHeal].itemForHandPrefab;
		playerGraphic.UnGrab();
		if (itemForHandPrefab != null)
		{
			playerGraphic.Grab(UnityEngine.Object.Instantiate(itemForHandPrefab));
		}
		if (hospitalManager.vampireMode.enabled && humanMob != null)
		{
			humanMob.SetSkin(humanMob.skinIndex);
		}
	}
}
