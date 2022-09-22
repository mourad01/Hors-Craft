// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChooseGenderExe
using Common.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	[CreateAssetMenu(menuName = "ScriptableObjects/Initial Popups/Choose Gender Exe")]
	public class ChooseGenderExe : InitialPopupExecution
	{
		public SkinList femaleSkinList;

		public SkinList maleSkinList;

		private int genderFromPrefs
		{
			get
			{
				return PlayerPrefs.GetInt("gender", -1);
			}
			set
			{
				PlayerPrefs.SetInt("gender", value);
			}
		}

		public override void Show()
		{
			int genderFromPrefs = this.genderFromPrefs;
			if (genderFromPrefs == -1)
			{
				ShowChooseGenderPopup();
				return;
			}
			SetGenderList((Skin.Gender)genderFromPrefs);
			PlayerGraphic.GetControlledPlayerInstance().SetAllClothes();
			PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<DressupClothesPlacement>().SpawnRandomHair((Skin.Gender)genderFromPrefs);
		}

		private void ShowChooseGenderPopup()
		{
			Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
			{
				configureMessage = delegate(TranslateText t)
				{
					t.translationKey = "choose.gender.message";
					t.defaultText = "choose your gender!";
				},
				configureLeftButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						ChooseGender(Skin.Gender.FEMALE);
						Manager.Get<StateMachineManager>().PopState();
					});
					t.translationKey = "choose.gender.female";
					t.defaultText = "female";
				},
				configureRightButton = delegate(Button b, TranslateText t)
				{
					b.onClick.AddListener(delegate
					{
						ChooseGender(Skin.Gender.MALE);
						Manager.Get<StateMachineManager>().PopState();
					});
					t.translationKey = "choose.gender.male";
					t.defaultText = "male";
				}
			});
		}

		private void ChooseGender(Skin.Gender gender)
		{
			genderFromPrefs = (int)gender;
			SetGenderList(gender);
			PlayerGraphic.GetControlledPlayerInstance().SetRandomSkin(SkinList.customPlayerSkinList);
			PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<DressupClothesPlacement>().SpawnRandomHair(gender);
		}

		private void SetGenderList(Skin.Gender gender)
		{
			List<Skin> possibleSkins = (from s in SkinList.instance.possibleSkins
				where s.gender == gender
				select s).ToList();
			GameObject gameObject = new GameObject("Skin list for " + gender.ToString());
			SkinList.customPlayerSkinList = gameObject.AddComponent<SkinList>();
			SkinList.customPlayerSkinList.possibleSkins = possibleSkins;
		}
	}
}
