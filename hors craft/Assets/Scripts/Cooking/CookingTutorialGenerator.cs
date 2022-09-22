// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.CookingTutorialGenerator
using Common.Managers;
using Common.Utils;
using States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking
{
	public class CookingTutorialGenerator
	{
		private WorkController workController;

		private GenericTutorial tutorial;

		private GenericTutorial.TutorialStep step;

		private CookingGameplayState gameplay;

		private GameObject tutorialPrefab;

		private List<Recipe> recipes;

		private Customer customer;

		private int i;

		private int currentRecipeIndex;

		private Product tutorialProduct;

		public CookingTutorialGenerator(CookingGameplayState gameplay, GameObject tutorialPrefab)
		{
			workController = Manager.Get<CookingManager>().workController;
			this.gameplay = gameplay;
			this.tutorialPrefab = tutorialPrefab;
		}

		public static void ShowRecipe(Product product, GameObject infoPanelPrefab)
		{
			GameObject mask = InstantiateMask();
			mask.GetComponentInChildren<Image>().color = new Color(0f, 0f, 0f, 0.7f);
			mask.AddComponent<KeepAsLastSibling>();
			GameObject instance = UnityEngine.Object.Instantiate(infoPanelPrefab, Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
			instance.transform.localScale = Vector3.one;
			instance.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
			instance.transform.position = new Vector3(Screen.width, Screen.height, 0f) / 2f;
			instance.transform.SetParent(mask.transform, worldPositionStays: true);
			RecipeTutorial component = instance.GetComponent<RecipeTutorial>();
			WorkController workController = Manager.Get<CookingManager>().workController;
			List<Recipe> recipesToProduct = workController.recipesList.GetRecipesToProduct(product);
			recipesToProduct.Reverse();
			Sprite y = null;
			foreach (Recipe item in recipesToProduct)
			{
				Sprite sprite = item.deviceScript.GetSprite();
				if (sprite != y)
				{
					component.AddItem(sprite);
					y = sprite;
				}
			}
			GameObject buttonObject = new GameObject("Button");
			buttonObject.transform.SetParent(mask.transform);
			RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = buttonObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			image.raycastTarget = true;
			Button button = buttonObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.onClick.AddListener(delegate
			{
				UnityEngine.Object.Destroy(mask);
				UnityEngine.Object.Destroy(instance);
				UnityEngine.Object.Destroy(buttonObject);
			});
		}

		private static GameObject InstantiateMask()
		{
			GameObject gameObject = new GameObject("Mask");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			gameObject.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = gameObject.AddComponent<Image>();
			image.raycastTarget = true;
			image.preserveAspect = false;
			return gameObject;
		}

		public void ShowTutorial(Customer customer, Product product)
		{
			this.customer = customer;
			tutorialProduct = product;
			recipes = workController.recipesList.GetRecipesToProduct(tutorialProduct);
			recipes.Reverse();
			GameObject gameObject = UnityEngine.Object.Instantiate(tutorialPrefab);
			tutorial = gameObject.GetComponent<GenericTutorial>();
			tutorial.pauseGameOnTutorial = false;
			tutorial.startMethod = GenericTutorial.TutorialStartMethod.ON_START;
			tutorial.autoChangeSteps = false;
			tutorial.blockInputBetweenSteps = true;
			workController.ResetKitchen();
			tutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
			GenerateTutorialStep();
		}

		private void GenerateTutorialStep()
		{
			if (MealReady())
			{
				GenerateGiveMealToCustomerStep();
				return;
			}
			if (currentRecipeIndex >= recipes.Count)
			{
				GeneratePickupTutorialStep(tutorialProduct);
				return;
			}
			Recipe recipe = recipes[currentRecipeIndex];
			if (recipe.product != null && !workController.worker.heldProducts.Any((Product p) => p.GetKey() == recipe.productScript.GetKey()))
			{
				GeneratePickupTutorialStep(recipe.productScript);
				return;
			}
			step = GenerateStep(recipe.device);
			step.configInfoPanelAfterSpawn = ConfigTutorialRecipeInfoPanel(recipe);
			if (recipe.deviceScript is Product)
			{
				ITapObject tapObject = FindDeviceWithProduct(recipe.deviceScript);
				step.element = tapObject.GetGameObject();
			}
			SetTutorialCallbacksNormal();
			AddTutorialStep(step);
		}

		private void GeneratePickupTutorialStep(Product product)
		{
			ITapObject tapObject = FindDeviceWithProduct(product);
			step = GenerateStep(tapObject.GetGameObject());
			step.configInfoPanelAfterSpawn = ConfigTutorialRecipeInfoPanel(product, tapObject);
			SetTutorialCallbacksPickup();
			AddTutorialStep(step);
		}

		private GenericTutorial.TutorialStep GenerateStep(GameObject element)
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			tutorialStep.delayBefore = 0f;
			tutorialStep.delayAfter = 9999f;
			tutorialStep.element = element;
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			StorageDevice component = element.GetComponent<StorageDevice>();
			if (component != null)
			{
				tutorialStep.element = (from slot in component.slots
					where slot.CanTap(workController.worker)
					select slot).Random().gameObject;
			}
			return tutorialStep;
		}

		private void SetTutorialCallbacksNormal()
		{
			step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
			step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
				action = delegate
				{
					i++;
					currentRecipeIndex++;
					GenerateTutorialStep();
				}
			});
			AddOnElementClickCallback();
		}

		private void SetTutorialCallbacksPickup()
		{
			step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
			step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
				action = delegate
				{
					i++;
					GenerateTutorialStep();
				}
			});
			AddOnElementClickCallback();
		}

		private void AddOnElementClickCallback()
		{
			if (step.tutorialCallbacks == null)
			{
				step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
			}
			step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
				action = delegate
				{
					workController.worker.AddToQueue(step.element.GetComponentInChildren<ITapObject>());
					workController.worker.onInteraction = OnWorkerInteractionWith();
				}
			});
		}

		private Action<GameObject> OnWorkerInteractionWith()
		{
			return delegate(GameObject go)
			{
				float num = 0.2f;
				TimeDeviceSlot component = go.GetComponent<TimeDeviceSlot>();
				if (component != null && component.status == TimeDeviceSlot.Status.COOKING)
				{
					num += component.baseCookingDevice.GetTimeRequired();
				}
				tutorial.NextStep(num);
			};
		}
		
		private ITapObject FindDeviceWithProduct(IUsable product)
		{
			List<StorageDevice> devicesOfType = workController.workPlace.GetDevicesOfType<StorageDevice>();
			List<StorageDevice> source = (from d in devicesOfType
				where d.GetPlacedProducts().Any((IPickable p) => p.GetKey() == product.GetKey())
				select d).ToList();
			return (from d in source
					select new
					{
						d,
						d.slots
					} into _003C_003E__TranspIdent0
					from d in source let validSlot = d.slots.FirstOrDefault((StorageDeviceSlot slot) => slot.placedItem != null && slot.placedItem.GetKey() == product.GetKey())
				where validSlot != null && validSlot.CanTap(workController.worker)
				select validSlot).FirstOrDefault();
		}

		private void GenerateGiveMealToCustomerStep()
		{
			step = GenerateStep(customer.gameObject);
			step.configInfoPanelAfterSpawn = delegate(GameObject go)
			{
				go.SetActive(value: false);
			};
			step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
			step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
				action = EndTutorial
			});
			step.configInfoPanelAfterSpawn = ConfigGiveOrderToCustomerInfo(tutorialProduct.GetSprite());
			AddOnElementClickCallback();
			AddTutorialStep(step);
		}

		private Action<GameObject> ConfigTutorialRecipeInfoPanel(Recipe recipe)
		{
			return delegate(GameObject go)
			{
				ConfigTutorialRecipe(recipe, go);
			};
		}

		private Action<GameObject> ConfigTutorialRecipeInfoPanel(Product product, ITapObject device)
		{
			return delegate(GameObject go)
			{
				RecipeTutorial component = go.GetComponent<RecipeTutorial>();
				component.Clear();
				component.AddItem(product.GetSprite());
				string empty = string.Empty;
				TimeDeviceSlot timeDeviceSlot = device as TimeDeviceSlot;
				empty = ((!(timeDeviceSlot != null) || timeDeviceSlot.status != TimeDeviceSlot.Status.BURNING) ? ("cooking.tut." + product.GetKey()) : "cooking.tut.burn");
				component.SetText(empty, "TAKE " + product.TranslatedName);
			};
		}

		private Action<GameObject> ConfigGiveOrderToCustomerInfo(Sprite sprite)
		{
			return delegate(GameObject go)
			{
				RecipeTutorial component = go.GetComponent<RecipeTutorial>();
				component.Clear();
				component.AddItem(sprite);
				string key = "cooking.tut.give.customer";
				component.SetText(key, "GIVE IT TO THE CUSTOMER");
			};
		}

		private void ConfigTutorialRecipe(Recipe recipe, GameObject recipePanelGO)
		{
			RecipeTutorial component = recipePanelGO.GetComponent<RecipeTutorial>();
			component.Clear();
			string str = "cooking.tut.";
			if (recipe.product == null)
			{
				component.AddItem(recipe.deviceScript.GetSprite());
			}
			else
			{
				component.AddItem(recipe.productScript.GetSprite());
				component.AddItem(recipe.deviceScript.GetSprite());
				component.AddResult(recipe.resultScript.GetSprite());
				str = str + recipe.productScript.GetKey() + ".";
			}
			str += recipe.deviceScript.GetKey();
			component.SetText(str, string.Empty);
		}

		private void AddTutorialStep(GenericTutorial.TutorialStep step)
		{
			GenericTutorial.TutorialStep[] array = new GenericTutorial.TutorialStep[tutorial.tutorialSteps.Length + 1];
			Array.Copy(tutorial.tutorialSteps, array, tutorial.tutorialSteps.Length);
			array[array.Length - 1] = step;
			tutorial.tutorialSteps = array;
		}

		private bool MealReady()
		{
			return workController.worker.heldProducts.Any((Product p) => p.GetKey() == tutorialProduct.GetKey());
		}

		private void EndTutorial()
		{
			workController.workData.ProductTutorialShown(tutorialProduct.Key);
			UnityEngine.Object.Destroy(tutorial.gameObject);
			gameplay.EndTutorial();
		}

		public void ShowTrashTutorial()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(tutorialPrefab);
			tutorial = gameObject.GetComponent<GenericTutorial>();
			tutorial.pauseGameOnTutorial = false;
			tutorial.startMethod = GenericTutorial.TutorialStartMethod.ON_START;
			tutorial.autoChangeSteps = true;
			tutorial.blockInputBetweenSteps = true;
			workController.ResetKitchen();
			tutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
			GenerateFirstStep();
			GenerateSecondStep();
			GenerateThirdStep();
			Generate4thStep();
			Generate5thStep();
			Generate6thStep();
		}

		private void GenerateFirstStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			StorageDevice storageDevice = (from d in workController.workPlace.GetDevicesOfType<StorageDevice>()
				where !(d is TimeDevice)
				select d).First();
			StorageDeviceSlot storageDeviceSlot = storageDevice.slots.First();
			tutorialStep.delayBefore = 0f;
			tutorialStep.delayAfter = 0f;
			tutorialStep.element = storageDeviceSlot.GetGameObject();
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			tutorialStep.configInfoPanelAfterSpawn = ConfigCustomInfoPanel("cooking.tut.trash.1", "If you need to put something down you can do it on this table", storageDevice.GetSprite());
			AddTutorialStep(tutorialStep);
		}

		private void GenerateSecondStep()
		{
			BaseIngredientDevice baseIngredientDevice = (from d in workController.workPlace.GetDevicesOfType<BaseIngredientDevice>()
				where d.GetKey() == "pizza.base" || d.GetKey() == "ingredient1"
				select d).First();
			GenericTutorial.TutorialStep tutorialStep = GenerateStep(baseIngredientDevice.gameObject);
			tutorialStep.configInfoPanelAfterSpawn = ConfigCustomInfoPanel("cooking.tut.trash.2", "Take pizza base", baseIngredientDevice.GetSprite());
			AddOnElementClickCallback(tutorialStep, baseIngredientDevice.GetGameObject());
			AddTutorialStep(tutorialStep);
		}

		private void GenerateThirdStep()
		{
			StorageDevice storageDevice = (from d in workController.workPlace.GetDevicesOfType<StorageDevice>()
				where !(d is TimeDevice)
				select d).First();
			StorageDeviceSlot storageDeviceSlot = storageDevice.slots.First();
			GenericTutorial.TutorialStep tutorialStep = GenerateStep(storageDeviceSlot.gameObject);
			tutorialStep.configInfoPanelAfterSpawn = ConfigCustomInfoPanel("cooking.tut.trash.3", "Put it on the table", storageDevice.GetSprite());
			AddOnElementClickCallback(tutorialStep, storageDeviceSlot.GetGameObject());
			AddTutorialStep(tutorialStep);
		}

		private void Generate4thStep()
		{
			GenericTutorial.TutorialStep tutorialStep = new GenericTutorial.TutorialStep();
			TrashDevice trashDevice = workController.workPlace.GetDevicesOfType<TrashDevice>().First();
			tutorialStep.delayBefore = 0f;
			tutorialStep.delayAfter = 0f;
			tutorialStep.element = trashDevice.GetGameObject();
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			tutorialStep.configInfoPanelAfterSpawn = ConfigCustomInfoPanel("cooking.tut.trash.4", "You can throw things away into the trashcan", trashDevice.GetSprite());
			AddTutorialStep(tutorialStep);
		}

		private void Generate5thStep()
		{
			StorageDevice storageDevice = (from d in workController.workPlace.GetDevicesOfType<StorageDevice>()
				where !(d is TimeDevice)
				select d).First();
			StorageDeviceSlot storageDeviceSlot = storageDevice.slots.First();
			GenericTutorial.TutorialStep tutorialStep = GenerateStep(storageDeviceSlot.gameObject);
			tutorialStep.configInfoPanelAfterSpawn = ConfigCustomInfoPanel("cooking.tut.trash.5", "Take pizza base from the table", storageDevice.GetSprite());
			AddOnElementClickCallback(tutorialStep, storageDeviceSlot.GetGameObject());
			AddTutorialStep(tutorialStep);
		}

		private void Generate6thStep()
		{
			TrashDevice trashDevice = workController.workPlace.GetDevicesOfType<TrashDevice>().First();
			GenericTutorial.TutorialStep tutorialStep = GenerateStep(trashDevice.GetGameObject());
			tutorialStep.configInfoPanelAfterSpawn = ConfigCustomInfoPanel("cooking.tut.trash.6", "Put it to trash", trashDevice.GetSprite());
			AddOnElementClickCallback(tutorialStep, trashDevice.GetGameObject());
			tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
				action = EndTrashTutorial
			});
			AddTutorialStep(tutorialStep);
		}

		private Action<GameObject> ConfigCustomInfoPanel(string translationKey, string defaultText, Sprite sprite = null)
		{
			return delegate(GameObject go)
			{
				RecipeTutorial component = go.GetComponent<RecipeTutorial>();
				component.Clear();
				if (sprite != null)
				{
					component.AddItem(sprite);
				}
				component.SetText(translationKey, defaultText);
			};
		}

		private void AddOnElementClickCallback(GenericTutorial.TutorialStep step, GameObject go)
		{
			if (step.tutorialCallbacks == null)
			{
				step.tutorialCallbacks = new List<GenericTutorial.TutorialCallback>();
			}
			step.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_ELEMENT_CLICK,
				action = delegate
				{
					workController.worker.AddToQueue(go.GetComponentInChildren<ITapObject>());
					workController.worker.onInteraction = OnWorkerInteractionWith();
				}
			});
		}

		private void EndTrashTutorial()
		{
			UnityEngine.Object.Destroy(tutorial.gameObject);
			gameplay.EndTutorial();
		}

		public void ShowRecipeTutorial(GameObject orderPanel, Product product)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(tutorialPrefab);
			tutorial = gameObject.GetComponent<GenericTutorial>();
			tutorial.pauseGameOnTutorial = false;
			tutorial.startMethod = GenericTutorial.TutorialStartMethod.ON_START;
			tutorial.autoChangeSteps = true;
			tutorial.blockInputBetweenSteps = true;
			tutorial.tutorialSteps = new GenericTutorial.TutorialStep[0];
			GenerateFirstRecipeStep(orderPanel, product);
		}

		private void GenerateFirstRecipeStep(GameObject orderPanel, Product product)
		{
			GenericTutorial.TutorialStep tutorialStep = GenerateStep(orderPanel);
			tutorialStep.delayBefore = 0f;
			tutorialStep.delayAfter = 0f;
			tutorialStep.element = orderPanel;
			tutorialStep.shape = GenericTutorial.Shape.CUSTOM;
			tutorialStep.configInfoPanelAfterSpawn = ConfigCustomInfoPanel("cooking.tut.recipe.1", "Tap on customer's order to see the recipe", product.GetSprite());
			tutorialStep.tutorialCallbacks.Add(new GenericTutorial.TutorialCallback
			{
				type = GenericTutorial.CallbackType.ON_STEP_CHANGE,
				action = delegate
				{
					EndTrashTutorial();
					gameplay.ShowRecipeTutorial(product);
				}
			});
			AddTutorialStep(tutorialStep);
		}
	}
}
