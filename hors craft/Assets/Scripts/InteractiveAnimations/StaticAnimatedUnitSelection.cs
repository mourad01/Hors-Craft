// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.StaticAnimatedUnitSelection
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveAnimations
{
	public class StaticAnimatedUnitSelection : MonoBehaviour
	{
		public enum AnimationsShowMode
		{
			Single,
			PredefinedList
		}

		[HideInInspector]
		public AnimationsShowMode showMode;

		[HideInInspector]
		public GameObject selectionPrefab;

		private bool showDropdown;

		private int listEntrySelected;

		private int listEntryNo;

		private Vector2 listPosition;

		private List<StaticAnimation> animationsList = new List<StaticAnimation>();

		private List<GUIContent> availableAnimationsList = new List<GUIContent>();

		private bool isSelecting;

		private Vector3 mousePosition1;

		private List<StaticAnimationUnit> selectedUnits = new List<StaticAnimationUnit>();

		private StaticAnimationUnit[] foundUnits;

		private HumanMob[] foundHumanMobs;

		private InteractiveAnimationsManager animationsManager;

		private void Awake()
		{
			animationsManager = Manager.Get<InteractiveAnimationsManager>();
		}

		private void Update()
		{
			if (!showDropdown)
			{
				if (Input.GetMouseButtonDown(0))
				{
					isSelecting = true;
					mousePosition1 = UnityEngine.Input.mousePosition;
					if (!Input.GetKey(KeyCode.LeftShift))
					{
						selectedUnits.Clear();
						foundUnits = UnityEngine.Object.FindObjectsOfType<StaticAnimationUnit>();
						foundHumanMobs = UnityEngine.Object.FindObjectsOfType<HumanMob>();
						StaticAnimationUnit[] array = foundUnits;
						foreach (StaticAnimationUnit staticAnimationUnit in array)
						{
							if (!(staticAnimationUnit.selection == null))
							{
								UnityEngine.Object.Destroy(staticAnimationUnit.selection.gameObject);
								staticAnimationUnit.selection = null;
								if (staticAnimationUnit.animationDefinition == null)
								{
									UnityEngine.Object.Destroy(staticAnimationUnit);
								}
							}
						}
					}
				}
				if (Input.GetMouseButtonUp(0))
				{
					HumanMob[] array2 = foundHumanMobs;
					foreach (HumanMob humanMob in array2)
					{
						if (!(humanMob == null) && IsWithinSelectionBounds(humanMob.gameObject))
						{
							selectedUnits.Add(humanMob.GetComponent<StaticAnimationUnit>());
						}
					}
					isSelecting = false;
				}
				if (isSelecting)
				{
					HumanMob[] array3 = foundHumanMobs;
					foreach (HumanMob humanMob2 in array3)
					{
						StaticAnimationUnit staticAnimationUnit2 = humanMob2.GetComponent<StaticAnimationUnit>();
						if (IsWithinSelectionBounds(humanMob2.gameObject) || selectedUnits.Contains(staticAnimationUnit2))
						{
							if (staticAnimationUnit2 == null)
							{
								staticAnimationUnit2 = humanMob2.gameObject.AddComponent<StaticAnimationUnit>();
							}
							if (!(staticAnimationUnit2.selection != null))
							{
								staticAnimationUnit2.selection = UnityEngine.Object.Instantiate(selectionPrefab);
								staticAnimationUnit2.selection.transform.SetParent(staticAnimationUnit2.transform, worldPositionStays: false);
							}
						}
						else if (!(staticAnimationUnit2 == null) && !(staticAnimationUnit2.selection == null))
						{
							UnityEngine.Object.Destroy(staticAnimationUnit2.selection.gameObject);
							staticAnimationUnit2.selection = null;
							if (staticAnimationUnit2.animationDefinition == null)
							{
								UnityEngine.Object.Destroy(staticAnimationUnit2);
							}
						}
					}
				}
				if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
				{
					listPosition = UnityEngine.Input.mousePosition;
					showDropdown = true;
				}
			}
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				showDropdown = false;
			}
		}

		public void SwitchMode(AnimationsShowMode mode)
		{
			showMode = mode;
			animationsList.Clear();
			availableAnimationsList.Clear();
			listEntryNo = 0;
			listEntrySelected = 0;
			availableAnimationsList.Add(new GUIContent("None"));
			switch (showMode)
			{
			case AnimationsShowMode.Single:
				foreach (SingleAnimation availableSingleAnimation in animationsManager.availableSingleAnimations)
				{
					animationsList.Add(availableSingleAnimation);
					availableAnimationsList.Add(new GUIContent(availableSingleAnimation.name));
				}
				break;
			case AnimationsShowMode.PredefinedList:
				foreach (PredefinedListAnimation availablePredefinedListAnimation in animationsManager.availablePredefinedListAnimations)
				{
					animationsList.Add(availablePredefinedListAnimation);
					availableAnimationsList.Add(new GUIContent(availablePredefinedListAnimation.name));
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private bool IsWithinSelectionBounds(GameObject selectedGameObject)
		{
			if (!isSelecting)
			{
				return false;
			}
			Camera main = Camera.main;
			return RectSelectionUtil.GetViewportBounds(main, mousePosition1, UnityEngine.Input.mousePosition).Contains(main.WorldToViewportPoint(selectedGameObject.transform.position));
		}

		private void List(bool expandList, GUIContent[] listToUse, GUIStyle boxStyle)
		{
			Rect rect = default(Rect);
			rect.x = listPosition.x;
			rect.y = (float)Screen.height - listPosition.y;
			rect.width = 200f;
			rect.height = 20f;
			Rect rect2 = rect;
			if (expandList)
			{
				Rect position = new Rect(rect2.x, rect2.y + 20f, rect2.width, boxStyle.CalcHeight(listToUse[0], 1f) * (float)listToUse.Length);
				GUI.Box(position, string.Empty, boxStyle);
				listEntrySelected = GUI.SelectionGrid(position, listEntrySelected, listToUse, 1);
				if (listEntryNo != listEntrySelected)
				{
					SetUnitInteractiveAnimation(listEntrySelected);
				}
			}
		}

		private void SetUnitInteractiveAnimation(int index)
		{
			listEntryNo = index;
			showDropdown = false;
			foreach (StaticAnimationUnit selectedUnit in selectedUnits)
			{
				if (index == 0)
				{
					UnityEngine.Object.Destroy(selectedUnit);
				}
				else
				{
					StaticAnimation staticAnimation = animationsList[index - 1];
					selectedUnit.SetStaticAnimation(staticAnimation, animationsManager.interactiveAnimationsList.interactiveAnimations.IndexOf(staticAnimation));
				}
			}
		}

		private void OnDestroy()
		{
			foreach (StaticAnimationUnit selectedUnit in selectedUnits)
			{
				if (selectedUnit.selection != null)
				{
					UnityEngine.Object.Destroy(selectedUnit.selection);
				}
				if (selectedUnit.animationDefinition == null)
				{
					UnityEngine.Object.Destroy(selectedUnit);
				}
			}
		}

		private void OnGUI()
		{
			List(showDropdown, availableAnimationsList.ToArray(), "box");
			GUI.skin.label.fontSize = 16;
			GUI.skin.button.fontSize = 16;
			if (isSelecting)
			{
				Rect screenRect = RectSelectionUtil.GetScreenRect(mousePosition1, UnityEngine.Input.mousePosition);
				RectSelectionUtil.DrawScreenRect(screenRect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
				RectSelectionUtil.DrawScreenRectBorder(screenRect, 2f, new Color(0.8f, 0.8f, 0.95f));
			}
		}
	}
}
