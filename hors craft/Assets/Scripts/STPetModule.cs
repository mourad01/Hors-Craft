// DecompilerFi decompiler from Assembly-CSharp.dll class: STPetModule
using Common.Managers;
using Common.Model;
using Common.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "STPet", menuName = "SaveTransformsModule/STPetModule")]
public class STPetModule : STModule
{
	public override GameObject Spawn(Settings settings)
	{
		GameObject gameObject = base.Spawn(settings);
		string @string = settings.GetString("individual.data");
		PetManager.PetIndividualData petIndividualData = JSONHelper.Deserialize<PetManager.PetIndividualData>(@string);
		PlayerGraphic.SkinInfo skinInfo = null;
		if (settings.HasString("skin.data"))
		{
			string string2 = settings.GetString("skin.data");
			skinInfo = JSONHelper.Deserialize<PlayerGraphic.SkinInfo>(string2);
		}
		int @int = settings.GetInt("id");
		Pettable component = gameObject.GetComponent<Pettable>();
		if (component != null)
		{
			component.LoadTamedPetData(@int, petIndividualData);
			AnimalMob componentInChildren = component.GetComponentInChildren<AnimalMob>();
			if (componentInChildren != null)
			{
				ConfigAnimal(componentInChildren, petIndividualData);
			}
		}
		HumanMob componentInChildren2 = component.GetComponentInChildren<HumanMob>();
		if (componentInChildren2 != null)
		{
			ConfigHuman(componentInChildren2, petIndividualData, skinInfo);
		}
		return gameObject;
	}

	private void ConfigAnimal(AnimalMob animalMob, PetManager.PetIndividualData petIndividualData)
	{
		float scaleEveryMobBy = Manager.Get<MobsManager>().scaleEveryMobBy;
		animalMob.runSpeed *= scaleEveryMobBy;
		animalMob.wanderDistanceFrom *= scaleEveryMobBy;
		animalMob.wanderDistanceTo *= scaleEveryMobBy;
		animalMob.wanderSpeed *= scaleEveryMobBy;
	}

	private void ConfigHuman(HumanMob humanMob, PetManager.PetIndividualData petIndividualData, PlayerGraphic.SkinInfo skinInfo)
	{
		if (humanMob.GetComponentInChildren<PlayerGraphic>() != null && humanMob.hasToSetGraphic)
		{
			if (skinInfo != null)
			{
				humanMob.gameObject.GetComponentInChildren<PlayerGraphic>().SetCustomSkin(skinInfo);
			}
			else
			{
				humanMob.SetSkin(petIndividualData.outfitIndex);
			}
		}
	}

	public override Settings Save(AbstractSaveTransform controller)
	{
		Settings settings = base.Save(controller);
		Pettable componentInChildren = controller.gameObject.GetComponentInChildren<Pettable>();
		if (componentInChildren != null)
		{
			componentInChildren.prefabName = GetName(settings);
			string value = JSONHelper.ToJSON(new PetManager.PetIndividualData(componentInChildren));
			settings.SetString("individual.data", value);
			settings.SetInt("id", componentInChildren.id);
		}
		PlayerGraphic componentInChildren2 = controller.gameObject.GetComponentInChildren<PlayerGraphic>();
		if (componentInChildren2 != null && componentInChildren2.skinInfo != null)
		{
			string value2 = JSONHelper.ToJSON(componentInChildren2.skinInfo);
			settings.SetString("skin.data", value2);
		}
		return settings;
	}
}
