// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.AnimalsReward
using Common.Gameplay;
using Common.Managers;
using States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class AnimalsReward : Reward
	{
		public override void ClaimReward()
		{
			PetsList petsList2 = Manager.Get<PetManager>().petsList;
			petsList2.InitPetsList();
			PetsList.Pets[] petsList = petsList2.petsList;
			int[] array = (from i in Enumerable.Range(0, petsList.Length)
				where !petsList[i].unlocked && CustomizationPetsTabFragment.AdsNeededForPet(i) > 0
				select i).ToArray();
			if (array != null && array.Length > 0)
			{
				petsList[array[0]].unlocked = true;
				PlayerPrefs.SetInt("petsList." + array[0], 1);
			}
		}

		public override List<Sprite> GetSprites()
		{
			PetsList petsList2 = Manager.Get<PetManager>().petsList;
			petsList2.InitPetsList();
			PetsList.Pets[] petsList = petsList2.petsList;
			int[] array = (from i in Enumerable.Range(0, petsList.Length)
				where !petsList[i].unlocked && CustomizationPetsTabFragment.AdsNeededForPet(i) > 0
				select i).ToArray();
			if (array == null || array.Length <= 0)
			{
				return new List<Sprite>();
			}
			return petsList[array[0]].prefab.GetComponent<AnimalMob>().mobSprite.AsList();
		}
	}
}
