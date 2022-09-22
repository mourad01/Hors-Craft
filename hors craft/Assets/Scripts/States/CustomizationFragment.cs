// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CustomizationFragment
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using GameUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CustomizationFragment : Fragment
	{
		public class CustomizationStartParameter : FragmentStartParameter
		{
			public CustomizationFragment parentFragment;

			public CustomizationStartParameter(CustomizationFragment parentFragment = null)
			{
				this.parentFragment = parentFragment;
			}
		}

		public bool clothesTabEnabled = true;

		public bool petsTabEnabled = true;

		public GameObject clothesTabPrefab;

		public GameObject petsTabPrefab;

		public Button clothesButton;

		public Button petsButton;

		public Camera cameraPlayerPreview;

		public ModelDragRotator rotator;

		public GameObject currencyTracker;

		public List<GameObject> objectsToMoveIfBanner;

		private Vector3 playerOffset = new Vector3(-1.8f, -0.4f, 3.4f);

		private Vector3 petOffset = new Vector3(-2f, -0.16f, 3.8f);

		private Canvas clothesButtonCanvas;

		private Canvas petsButtonCanvas;

		private GameObject clothesInstance;

		private GameObject petsInstance;

		private bool hideBody;

		private HumanRepresentation humanRep;

		private PetRepresentation petRep;

		private bool wasGraphicVisible;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			wasGraphicVisible = PlayerGraphic.GetControlledPlayerInstance().graphicRepresentation.activeSelf;
			clothesButtonCanvas = clothesButton.GetComponent<Canvas>();
			petsButtonCanvas = petsButton.GetComponent<Canvas>();
			int @int = PlayerPrefs.GetInt("numberOfWatchedRewardedAdsClothes", 0);
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			SetButtonListeners(@int);
			EnableTabs();
			EnablePetButton();
			if (clothesTabEnabled)
			{
				clothesButton.onClick.Invoke();
			}
			else if (petsTabEnabled)
			{
				petsButton.onClick.Invoke();
			}
			bool active = Manager.Get<ModelManager>().clothesSetting.GetUnlockType() == ItemsUnlockModel.SoftCurrency;
			currencyTracker.SetActive(active);
			if (clothesInstance != null)
			{
				objectsToMoveIfBanner.Add(clothesInstance);
			}
			if (petsInstance != null)
			{
				objectsToMoveIfBanner.Add(petsInstance);
			}
		}

		private void EnableTabs()
		{
			clothesButton.gameObject.SetActive(clothesTabEnabled);
			if (clothesTabEnabled)
			{
				clothesButtonCanvas.sortingOrder = 3;
			}
			else
			{
				clothesButtonCanvas.sortingOrder = 1;
			}
			petsButton.gameObject.SetActive(petsTabEnabled);
			if (petsTabEnabled && clothesTabEnabled)
			{
				petsButtonCanvas.sortingOrder = 1;
			}
			else if (petsTabEnabled)
			{
				petsButtonCanvas.sortingOrder = 3;
			}
		}

		private void EnablePetButton()
		{
			if (petsButton.gameObject.activeSelf)
			{
				petsButton.gameObject.SetActive(Manager.Get<ModelManager>().petSetting.GetPetsEnabled());
			}
		}

		private void SetButtonListeners(int numberOfWatchedRewardedAds)
		{
			clothesButton.onClick.AddListener(delegate
			{
				if (clothesInstance == null)
				{
					clothesInstance = Object.Instantiate(clothesTabPrefab, base.transform);
					clothesInstance.GetComponent<Fragment>().Init(new CustomizationStartParameter(this));
				}
				if (petRep != null)
				{
					HidePetRep();
				}
				InitHumanRepresentation();
				clothesButtonCanvas.sortingOrder = 3;
				petsButtonCanvas.sortingOrder = 1;
				clothesInstance.SetActive(value: true);
				if (petsInstance != null)
				{
					petsInstance.SetActive(value: false);
				}
			});
			petsButton.onClick.AddListener(delegate
			{
				if (petsInstance == null)
				{
					petsInstance = Object.Instantiate(petsTabPrefab, base.transform);
					petsInstance.GetComponent<Fragment>().Init(new CustomizationStartParameter(this));
				}
				if (humanRep != null)
				{
					humanRep.UIModeOff();
					humanRep = null;
					if (hideBody)
					{
						PlayerGraphic.GetControlledPlayerInstance().HideBodyAndLegs();
					}
				}
				if (petRep != null)
				{
					ShowPetRep();
				}
				clothesButtonCanvas.sortingOrder = 1;
				petsButtonCanvas.sortingOrder = 3;
				if (clothesInstance != null)
				{
					clothesInstance.SetActive(value: false);
				}
				petsInstance.SetActive(value: true);
				petsInstance.GetComponent<Fragment>().UpdateFragment();
			});
		}

		public override void Disable()
		{
			base.Disable();
			if (humanRep != null)
			{
				humanRep.UIModeOff();
				humanRep = null;
				if (hideBody)
				{
					PlayerGraphic.GetControlledPlayerInstance().HideBodyAndLegs();
				}
			}
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
			if (clothesTabEnabled && clothesInstance != null && clothesInstance.activeSelf)
			{
				InitHumanRepresentation();
			}
		}

		public override void Destroy()
		{
			PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
			if (controlledPlayerInstance != null)
			{
				controlledPlayerInstance.graphicRepresentation.SetActive(wasGraphicVisible);
			}
			if (humanRep != null)
			{
				humanRep.UIModeOff();
				humanRep = null;
				if (hideBody)
				{
					PlayerGraphic.GetControlledPlayerInstance().HideBodyAndLegs();
				}
			}
			if (petRep != null)
			{
				petRep.UIModeOff();
			}
			if (Manager.Contains<SurvivalManager>() && Manager.Get<SurvivalManager>().IsCombatTime())
			{
				PlayerGraphic.GetControlledPlayerInstance().HideHands();
			}
			base.Destroy();
		}

		public void SetBodyPart(BodyPart part, int index)
		{
			humanRep.graphic.SetBodyPartMaterial(part, index);
			if (!humanRep.graphic.isNPC)
			{
				humanRep.graphic.SaveClothesToPlayerPrefs();
			}
		}

		public void InitHumanRepresentation()
		{
			if (humanRep == null)
			{
				hideBody = !Manager.Get<StateMachineManager>().GetStateInstance<GameplayState>().hasToShowBody;
				PlayerGraphic.GetControlledPlayerInstance().ShowBodyAndLegs();
				humanRep = new HumanRepresentation(PlayerGraphic.GetControlledPlayerInstance());
				humanRep.UIModeOn(SetPlayerRepresentationPlace);
				humanRep.graphic.ShowPlayerGraphic();
				PlayerGraphic.GetControlledPlayerInstance().TryToEnableHeadThings(CameraController.instance.defaultCameraPreset != CameraController.CameraPresets.FPP);
			}
		}

		public void InitPetRepresentation(GameObject petPrefab)
		{
			if (petRep != null)
			{
				petRep.UIModeOff();
			}
			GameObject petObject = Object.Instantiate(petPrefab);
			petRep = new PetRepresentation(petObject);
			petRep.UIModeOn(SetPetRepresentationPlace);
		}

		public void ShowPetRep()
		{
			petRep.Show();
		}

		public void HidePetRep()
		{
			petRep.Hide();
		}

		public HumanRepresentation GetHumanRep()
		{
			return humanRep;
		}

		private void SetPlayerRepresentationPlace(GameObject player)
		{
			player.transform.position = cameraPlayerPreview.transform.position + playerOffset;
			player.transform.localRotation = Quaternion.Euler(0f, 146f, 0f);
			rotator.modelToRotate = player;
		}

		private void SetPetRepresentationPlace(GameObject petObj)
		{
			petObj.transform.position = cameraPlayerPreview.transform.position + petOffset;
			petObj.transform.localRotation = Quaternion.Euler(0f, 146f, 0f);
			rotator.modelToRotate = petObj;
		}
	}
}
