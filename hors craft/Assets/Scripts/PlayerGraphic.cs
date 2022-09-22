// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerGraphic
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGraphic : MonoBehaviour
{
	[Serializable]
	public class SkinInfo
	{
		public bool useDressupHead;

		public int headIndex;

		public int bodyIndex;

		public int legsIndex;

		public List<int> dressupThings = new List<int>();

		public string npcName;
	}

	private static PlayerGraphic controlledPlayerInstance;

	public bool isNPC;

	private const int HEAD_MAT_INDEX = 0;

	private const int BODY_MAT_INDEX = 0;

	private const int LEGS_MAT_INDEX = 1;

	public GameObject mainBody;

	public GameObject head;

	public GameObject leftLeg;

	public GameObject rightLeg;

	public GameObject leftArm;

	public GameObject rightArm;

	public GameObject leftArmLower;

	public GameObject rightArmLower;

	public GameObject leftLegLower;

	public GameObject rightLegLower;

	public GameObject tail;

	public GameObject holdingPivot;

	public GameObject hatPivot;

	public GameObject graphicRepresentation;

	public float scaleFactor = 1f;

	public DressupSkinList.HeadInfo.HeadType headType = DressupSkinList.HeadInfo.HeadType.DRESSUP;

	private SkinInfo _skinInfo;

	private int currentHead;

	private int currentBody;

	private int currentLegs;

	public EaseType easeType = EaseType.OutExpo;

	public Color startColor = new Color32(88, 88, 88, byte.MaxValue);

	public float duration = 0.6f;

	public bool hasLegs => !(leftLeg == null) && !(rightLeg == null);

	public SkinInfo skinInfo
	{
		get
		{
			return _skinInfo;
		}
		set
		{
			_skinInfo = value;
		}
	}

	public Animator animator
	{
		get;
		private set;
	}

	public Skin.Gender gender
	{
		get;
		private set;
	}

	public static PlayerGraphic GetControlledPlayerInstance()
	{
		return controlledPlayerInstance;
	}

	protected virtual void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		if (!isNPC)
		{
			controlledPlayerInstance = this;
			LoadSavedClothes();
			SetAllClothes();
		}
	}

	private void Start()
	{
		if (!isNPC)
		{
			TryToEnableHeadThings(CameraController.instance.defaultCameraPreset != CameraController.CameraPresets.FPP);
		}
	}

	public virtual void SetHat(GameObject hat)
	{
		hat.transform.SetParent(hatPivot.transform, worldPositionStays: false);
	}

	public virtual void Grab(GameObject go)
	{
		go.transform.SetParent(holdingPivot.transform, worldPositionStays: false);
		animator.SetBool("holding", value: true);
	}

	public virtual void UnGrab()
	{
		animator.SetBool("holding", value: false);
		if ((bool)holdingPivot)
		{
			IEnumerator enumerator = holdingPivot.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}

	public virtual void ShowHands()
	{
		SetEnableRenderers(leftArm, enable: true);
		SetEnableRenderers(rightArm, enable: true);
	}

	public virtual void HideHands()
	{
		SetEnableRenderers(leftArm, enable: false);
		SetEnableRenderers(rightArm, enable: false);
	}

	private void SetEnableRenderers(GameObject parent, bool enable)
	{
		Renderer[] componentsInChildren = parent.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = enable;
		}
	}

	public virtual void ShowBodyAndLegs()
	{
		if (hasLegs)
		{
			leftLeg.GetComponent<Renderer>().enabled = true;
			rightLeg.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			tail.SetActive(value: true);
		}
		mainBody.GetComponent<Renderer>().enabled = true;
	}

	public virtual void HideBodyAndLegs()
	{
		if (hasLegs)
		{
			leftLeg.GetComponent<Renderer>().enabled = false;
			rightLeg.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			tail.SetActive(value: false);
		}
		mainBody.GetComponent<Renderer>().enabled = false;
	}

	public void LoadSavedClothes()
	{
		IEnumerator enumerator = Enum.GetValues(BodyPart.Body.GetType()).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BodyPart part = (BodyPart)enumerator.Current;
				SetCurrentCloth(part, PlayerPrefs.GetInt($"clothes.{part.ToString().ToLower()}", 0));
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void SaveClothesToPlayerPrefs()
	{
		IEnumerator enumerator = Enum.GetValues(BodyPart.Body.GetType()).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BodyPart part = (BodyPart)enumerator.Current;
				PlayerPrefs.SetInt($"clothes.{part.ToString().ToLower()}", GetCurrentCloth(part));
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public virtual void SetAllClothes()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(BodyPart)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BodyPart part = (BodyPart)enumerator.Current;
				SetBodyPartMaterial(part, GetCurrentCloth(part));
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public virtual void SetWholeBodyl(int index)
	{
		SetMaterial(BodyPart.Body, index);
		SetMaterial(BodyPart.Head, index);
		SetMaterial(BodyPart.Legs, index);
		GameObject[] hatsList = Manager.Get<SkinManager>().hatsList;
		GameObject[] itemsToHoldInHandList = Manager.Get<SkinManager>().itemsToHoldInHandList;
		if (!hatsList.IsNullOrEmpty() && isNPC && UnityEngine.Random.Range(0f, 1f) < Manager.Get<SkinManager>().hatSpawnChance)
		{
			SetHat(UnityEngine.Object.Instantiate(hatsList[UnityEngine.Random.Range(0, hatsList.Length)]));
		}
		if (!itemsToHoldInHandList.IsNullOrEmpty() && isNPC && UnityEngine.Random.Range(0f, 1f) < Manager.Get<SkinManager>().itemsToHoldSpawnChance)
		{
			Grab(UnityEngine.Object.Instantiate(itemsToHoldInHandList[UnityEngine.Random.Range(0, itemsToHoldInHandList.Length)]));
		}
	}

	public virtual void SetBodyPartMaterial(BodyPart part, int index)
	{
		SetMaterial(part, index);
	}

	private void SetCurrentCloth(BodyPart part, int value)
	{
		switch (part)
		{
		case BodyPart.Head:
			currentHead = value;
			break;
		case BodyPart.Body:
			currentBody = value;
			break;
		case BodyPart.Legs:
			currentLegs = value;
			break;
		}
	}

	public virtual int GetCurrentCloth(BodyPart part)
	{
		switch (part)
		{
		case BodyPart.Head:
			return currentHead;
		case BodyPart.Body:
			return currentBody;
		case BodyPart.Legs:
			return currentLegs;
		default:
			return -1;
		}
	}

	private void SetMaterial(BodyPart part, int index, SkinList skinList = null)
	{
		if (mainBody == null)
		{
			return;
		}
		if (skinList == null)
		{
			skinList = ((isNPC || !(SkinList.customPlayerSkinList != null)) ? SkinList.instance : SkinList.customPlayerSkinList);
		}
		DressupSkinList instance = DressupSkinList.instance;
		if (instance != null && headType == DressupSkinList.HeadInfo.HeadType.DEFAULT && instance.AlternativeSkinList != null)
		{
			skinList = instance.AlternativeSkinList;
			index = UnityEngine.Random.Range(0, skinList.possibleSkins.Count);
		}
		Material clothes = skinList.GetClothes(index);
		switch (part)
		{
		case BodyPart.Head:
			gender = skinList.possibleSkins[index].gender;
			currentHead = index;
			if (instance != null)
			{
				head.GetComponent<MeshRenderer>().material = instance.GetHeadMaterial(gender, headType);
			}
			else
			{
				head.GetComponent<MeshRenderer>().material = clothes;
			}
			if (!isNPC)
			{
				PlayHeadColorBump();
			}
			break;
		case BodyPart.Body:
			ChangeBodyMaterial(clothes);
			currentBody = index;
			if (!isNPC)
			{
				PlayBodyColorBump();
			}
			break;
		case BodyPart.Legs:
			ChangeLegsMaterial(clothes);
			currentLegs = index;
			if (!isNPC)
			{
				PlayLegsColorBump();
			}
			break;
		}
	}

	public virtual void SetRandomSkin(SkinList skinList)
	{
		int index = UnityEngine.Random.Range(0, skinList.possibleSkins.Count);
		SetSkin(skinList, index);
	}

	public virtual void SetSkin(SkinList skinList, int index)
	{
		SetMaterial(BodyPart.Body, index, skinList);
		SetMaterial(BodyPart.Head, index, skinList);
		SetMaterial(BodyPart.Legs, index, skinList);
	}

	public virtual void SetRandomSkinWithGender(SkinList skinList, Skin.Gender gender)
	{
		int index = 0;
		for (int i = 0; i < 10; i++)
		{
			Skin randomItem = skinList.possibleSkins.GetRandomItem();
			if (randomItem.gender == gender)
			{
				index = skinList.possibleSkins.IndexOf(randomItem);
				break;
			}
		}
		SetSkin(skinList, index);
	}

	private void ChangeBodyMaterial(Material mat)
	{
		Material[] materials = mainBody.GetComponent<Renderer>().materials;
		materials[0] = mat;
		mainBody.GetComponent<Renderer>().materials = materials;
		leftArm.GetComponent<MeshRenderer>().material = mat;
		rightArm.GetComponent<MeshRenderer>().material = mat;
		if (leftArmLower != null)
		{
			MeshRenderer component = leftArmLower.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.material = mat;
			}
		}
		if (rightArmLower != null)
		{
			MeshRenderer component2 = rightArmLower.GetComponent<MeshRenderer>();
			if (component2 != null)
			{
				component2.material = mat;
			}
		}
	}

	private void ChangeLegsMaterial(Material mat)
	{
		if (!(leftLeg == null) || !(rightLeg == null))
		{
			Material[] materials = mainBody.GetComponent<Renderer>().materials;
			materials[1] = mat;
			mainBody.GetComponent<Renderer>().materials = materials;
			leftLeg.GetComponent<MeshRenderer>().material = mat;
			rightLeg.GetComponent<MeshRenderer>().material = mat;
			if (leftLegLower != null)
			{
				leftLegLower.GetComponent<MeshRenderer>().material = mat;
			}
			if (rightLegLower != null)
			{
				rightLegLower.GetComponent<MeshRenderer>().material = mat;
			}
		}
	}

	public virtual void ToggleHead(bool newState)
	{
		head.gameObject.SetActive(newState);
	}

	public void TryToEnableHeadThings(bool enable)
	{
		DressupClothesPlacement componentInChildren = GetControlledPlayerInstance().GetComponentInChildren<DressupClothesPlacement>();
		try
		{
			if (componentInChildren != null)
			{
				if (enable)
				{
					componentInChildren.ShowHat();
				}
				else
				{
					componentInChildren.HideHat();
				}
				componentInChildren.ToggleHair(enable);
				GetControlledPlayerInstance().ToggleHead(enable);
			}
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError("Error while working on player representation! : " + arg);
		}
	}

	public void HidePlayerGraphic()
	{
		mainBody.SetActive(value: false);
	}

	public void ShowPlayerGraphic()
	{
		mainBody.SetActive(value: true);
	}

	public void SetDressupHead(int index)
	{
		DressupSkinList instance = DressupSkinList.instance;
		if (instance != null)
		{
			head.GetComponent<MeshRenderer>().material = instance.GetHeadMaterial(index);
		}
	}

	public void SetCustomSkin(SkinInfo skinInfo)
	{
		if (!skinInfo.useDressupHead)
		{
			SetBodyPartMaterial(BodyPart.Head, skinInfo.headIndex);
		}
		SetBodyPartMaterial(BodyPart.Body, skinInfo.bodyIndex);
		SetBodyPartMaterial(BodyPart.Legs, skinInfo.legsIndex);
		DressupClothesPlacement componentInChildren = GetComponentInChildren<DressupClothesPlacement>();
		if (componentInChildren != null)
		{
			componentInChildren.ignoreStart = true;
			if (skinInfo.useDressupHead)
			{
				SetDressupHead(skinInfo.headIndex);
			}
			foreach (int dressupThing in skinInfo.dressupThings)
			{
				componentInChildren.SpawnById(dressupThing);
			}
		}
		HumanMob componentInParent = GetComponentInParent<HumanMob>();
		if (componentInParent != null)
		{
			componentInParent.skinIndex = skinInfo.bodyIndex;
		}
		this.skinInfo = skinInfo;
	}

	public void PlayHeadColorBump()
	{
		SetColorBump(head);
	}

	public void PlayBodyColorBump()
	{
		SetColorBump(leftArm);
		SetColorBump(rightArm);
		SetColorBump(mainBody);
	}

	public void PlayLegsColorBump()
	{
		SetColorBump(leftLeg);
		SetColorBump(rightLeg);
	}

	private void SetColorBump(GameObject go)
	{
		if (!(go == null))
		{
			MaterialColorBump materialColorBump = go.GetComponent<MaterialColorBump>();
			if (materialColorBump == null)
			{
				materialColorBump = go.AddComponent<MaterialColorBump>();
			}
			materialColorBump.easing = easeType;
			materialColorBump.startColor = startColor;
			materialColorBump.duration = duration;
			materialColorBump.endColor = Color.black;
			materialColorBump.materials = go.GetComponent<MeshRenderer>().materials.ToList();
			materialColorBump.Start();
		}
	}
}
