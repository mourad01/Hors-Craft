// DecompilerFi decompiler from Assembly-CSharp.dll class: PetItem
using Common.Managers;
using States;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PetItem : MonoBehaviour
{
	public enum Status
	{
		UNLOCKED,
		LOCKED,
		HIDDEN
	}

	[Serializable]
	public struct VoxelToIcon
	{
		public PettableFriend.SearchingForVoxel voxel;

		public Sprite icon;
	}

	public VoxelToIcon[] voxelBlockIconRepresentation;

	public Button petItemButton;

	public Text petName;

	public Image petImage;

	public GameObject petSpecialitySearching;

	public GameObject petSpecialityMountable;

	public Text numberOfAdsNeeded;

	public GameObject lockedPetOverlay;

	public GameObject lockedPetMask;

	public GameObject lockedPetUnknownSymbol;

	public GameObject lockedPetTVSymbol;

	public Image borderImage;

	public Image headerImage;

	private TranslateText petNameTranslation;

	[SerializeField]
	private Color unchoosedColor = Color.black;

	[SerializeField]
	private Color choosedColor;

	private bool spawnOnWorld;

	private int petIndex;

	private PetsList.Pets[] petsList;

	private Image petSpecialitySearchingIcon;

	private PettableFriend.Speciality petSpeciality;

	private const string TRANSLATION_ID = "pet.name.";

	public Status status
	{
		get;
		private set;
	}

	public Pettable pet
	{
		get;
		private set;
	}

	private void Awake()
	{
		petNameTranslation = petName.GetComponent<TranslateText>();
		petSpecialitySearchingIcon = petSpecialitySearching.GetComponentInChildren<Image>();
		petsList = Manager.Get<PetManager>().petsList.petsList;
		InitButton();
	}

	private void InitButton()
	{
		petItemButton.onClick.AddListener(delegate
		{
			switch (status)
			{
			case Status.UNLOCKED:
				ChooseItem();
				break;
			case Status.LOCKED:
			{
				WatchXAdsPopUpStateStartParameter watchXAdsPopUpStateStartParameter = new WatchXAdsPopUpStateStartParameter();
				watchXAdsPopUpStateStartParameter.voxelSpritesToUnlock.Clear();
				watchXAdsPopUpStateStartParameter.type = AdsCounters.Pets;
				watchXAdsPopUpStateStartParameter.reason = StatsManager.AdReason.XCRAFT_PETS;
				watchXAdsPopUpStateStartParameter.description = "Watch {0} more ads to unlock this pet!";
				watchXAdsPopUpStateStartParameter.translationKey = "pets.popup.locked.message";
				watchXAdsPopUpStateStartParameter.numberOfAdsNeeded = numberOfAdsNeeded.text.ToInt(1);
				Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(watchXAdsPopUpStateStartParameter);
				break;
			}
			case Status.HIDDEN:
				Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
				{
					configureMessage = delegate(TranslateText t)
					{
						t.translationKey = "pets.popup.hidden.message";
						t.defaultText = "You have to find and tame this pet in the game if you want to use him as your pet!";
					},
					configureLeftButton = delegate(Button b, TranslateText t)
					{
						t.translationKey = "pets.popup.button.back";
						t.defaultText = "back";
						b.gameObject.SetActive(value: false);
					},
					configureRightButton = delegate(Button b, TranslateText t)
					{
						t.translationKey = "pets.popup.button.ok";
						t.defaultText = "yes";
						b.onClick.AddListener(delegate
						{
							Manager.Get<StateMachineManager>().PopState();
						});
					}
				});
				break;
			}
		});
	}

	private void InitPetItem()
	{
		if (pet == null)
		{
			pet = petsList[petIndex].prefab.GetComponent<Pettable>();
		}
		if (pet.searchingForVoxel != PettableFriend.SearchingForVoxel.NONE)
		{
			if (Manager.Get<PetManager>().resourcesEnabled)
			{
				petSpeciality = PettableFriend.Speciality.SEARCHING;
			}
			else
			{
				petSpeciality = PettableFriend.Speciality.NONE;
			}
		}
		else
		{
			petSpeciality = PettableFriend.Speciality.MOUNTABLE;
		}
		UpdatePetItem();
	}

	private void UpdatePetItem()
	{
		petNameTranslation.translationKey = "pet.name." + pet.name.ToLower();
		petNameTranslation.defaultText = pet.petName;
		petNameTranslation.ForceRefresh();
		petImage.sprite = pet.GetComponent<AnimalMob>().mobSprite;
		switch (petSpeciality)
		{
		case PettableFriend.Speciality.MOUNTABLE:
			petSpecialitySearching.SetActive(value: false);
			petSpecialityMountable.SetActive(value: true);
			break;
		case PettableFriend.Speciality.SEARCHING:
			for (int i = 0; i < voxelBlockIconRepresentation.Length; i++)
			{
				if (voxelBlockIconRepresentation[i].voxel == pet.searchingForVoxel)
				{
					petSpecialitySearchingIcon.sprite = voxelBlockIconRepresentation[i].icon;
				}
			}
			petSpecialitySearching.SetActive(value: true);
			petSpecialityMountable.SetActive(value: false);
			break;
		case PettableFriend.Speciality.NONE:
			petSpecialitySearching.SetActive(value: false);
			petSpecialityMountable.SetActive(value: false);
			break;
		}
		switch (status)
		{
		case Status.UNLOCKED:
			lockedPetUnknownSymbol.SetActive(value: false);
			lockedPetTVSymbol.SetActive(value: false);
			lockedPetOverlay.SetActive(value: false);
			lockedPetMask.SetActive(value: false);
			break;
		case Status.LOCKED:
			lockedPetUnknownSymbol.SetActive(value: false);
			lockedPetTVSymbol.SetActive(value: true);
			lockedPetOverlay.SetActive(value: true);
			lockedPetMask.SetActive(value: true);
			break;
		case Status.HIDDEN:
			lockedPetUnknownSymbol.SetActive(value: true);
			lockedPetTVSymbol.SetActive(value: false);
			lockedPetOverlay.SetActive(value: true);
			lockedPetMask.SetActive(value: true);
			break;
		}
	}

	public void ChooseItem()
	{
		headerImage.color = choosedColor;
		borderImage.color = choosedColor;
		petName.color = Color.black;
	}

	public void UnchooseItem()
	{
		headerImage.color = unchoosedColor;
		borderImage.color = unchoosedColor;
		petName.color = Color.white;
	}

	public void SetValues(int index, int value)
	{
		petIndex = index;
		numberOfAdsNeeded.text = value.ToString();
		if (petsList[petIndex].unlocked)
		{
			status = Status.UNLOCKED;
		}
		else if (petsList[petIndex].spawnOnWorld)
		{
			status = Status.HIDDEN;
		}
		else if (value > 0)
		{
			status = Status.LOCKED;
		}
		else
		{
			status = Status.UNLOCKED;
		}
		InitPetItem();
	}

	public void Revalidate(int value, bool unlocked = false)
	{
		numberOfAdsNeeded.text = value.ToString();
		if (petsList[petIndex].unlocked)
		{
			status = Status.UNLOCKED;
		}
		else if (petsList[petIndex].spawnOnWorld)
		{
			status = Status.HIDDEN;
		}
		else if (value > 0)
		{
			status = Status.LOCKED;
		}
		else
		{
			status = Status.UNLOCKED;
		}
		UpdatePetItem();
	}
}
