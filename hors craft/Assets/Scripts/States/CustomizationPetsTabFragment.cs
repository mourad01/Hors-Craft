// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CustomizationPetsTabFragment
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using GameUI;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CustomizationPetsTabFragment : Fragment
	{
		public GameObject petsScrollParent;

		public GameObject petsElementPrefab;

		private PetsList.Pets[] pets;

		private int lastChoosedIndex;

		private PetItem lastChoosedPetItem;

		private PetManager petManager;

		private CustomizationFragment.CustomizationStartParameter startParam;

		public static int AdsNeededForPet(int id)
		{
			int num = PlayerPrefs.GetInt("numberOfWatchedRewardedAdsPets", 0);
			if (PlayerPrefs.GetInt("overridepetsadsnumber", 0) == 1)
			{
				num = 99999;
			}
			int petsPerAds = Manager.Get<ModelManager>().petSetting.GetPetsPerAds();
			int freePets = Manager.Get<ModelManager>().petSetting.GetFreePets();
			return Mathf.FloorToInt((float)(id - freePets) / (float)petsPerAds) + 1 - num;
		}

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			startParam = (parameter as CustomizationFragment.CustomizationStartParameter);
			petManager = Manager.Get<PetManager>();
			InitializePets();
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			RevalidateLists();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			RevalidateLists();
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			if (lastChoosedPetItem != null)
			{
				SetPetElement(lastChoosedPetItem, lastChoosedIndex);
			}
		}

		private void InitializePets()
		{
			int num = 0;
			this.pets = petManager.petsList.petsList;
			PetsList.Pets[] array = this.pets;
			for (int i = 0; i < array.Length; i++)
			{
				PetsList.Pets pets = array[i];
				GameObject gameObject = Object.Instantiate(petsElementPrefab, petsScrollParent.transform);
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.SetAsLastSibling();
				PetItem petItem = gameObject.GetComponent<PetItem>();
				int id = num;
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					SetPetElement(petItem, id);
				});
				petItem.SetValues(id, AdsNeededForPet(id));
				if (petManager.currentPet != null && petManager.currentPet.pettable.prefabName == pets.prefab.name)
				{
					gameObject.GetComponent<Button>().onClick.Invoke();
				}
				num++;
			}
		}

		private void SetPetElement(PetItem petItem, int index)
		{
			if (petItem.status == PetItem.Status.UNLOCKED)
			{
				if (lastChoosedPetItem != null)
				{
					lastChoosedPetItem.UnchooseItem();
				}
				startParam.parentFragment.InitPetRepresentation(pets[index].prefab);
				lastChoosedPetItem = petItem;
				lastChoosedIndex = index;
			}
		}

		private void RevalidateLists()
		{
			revalidatePetItems(petsScrollParent);
		}

		private void revalidatePetItems(GameObject list)
		{
			for (int i = 0; i < list.transform.childCount; i++)
			{
				PetItem component = list.transform.GetChild(i).GetComponent<PetItem>();
				if (component != null)
				{
					component.Revalidate(AdsNeededForPet(i - 1));
				}
			}
		}

		private void OnDestroy()
		{
			if (petManager.currentPet != null)
			{
				if (petManager.currentPet.GetComponent<AnimalMob>().mountMode)
				{
					PlayerGraphic.GetControlledPlayerInstance().GetComponent<PlayerMovement>().ForceUnmount();
				}
				petManager.UnregisterPet(petManager.currentPet.pettable);
				UnityEngine.Object.Destroy(petManager.currentPet.gameObject);
			}
			if (lastChoosedPetItem != null)
			{
				(Manager.Get<SaveTransformsManager>().modules.FirstOrDefault((AbstractSTModule m) => m is STPetFriendModule) as STPetFriendModule).ClearAllFreezed();
				GameObject gameObject = Object.Instantiate(lastChoosedPetItem.pet.gameObject);
				gameObject.SetLayerRecursively(LayerMask.NameToLayer("Mobs"));
				Vector3 b = Random.insideUnitSphere * 5f;
				b.y = 3f;
				gameObject.transform.position = CameraController.instance.MainCamera.transform.position + b;
				Pettable component = gameObject.GetComponent<Pettable>();
				component.prefabName = lastChoosedPetItem.pet.name;
				component.ForceTamed();
			}
		}

		private void OnEnable()
		{
			RevalidateLists();
		}
	}
}
