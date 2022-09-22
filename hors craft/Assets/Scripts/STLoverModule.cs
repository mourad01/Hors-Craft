// DecompilerFi decompiler from Assembly-CSharp.dll class: STLoverModule
using Common.Managers;
using Common.Model;
using Common.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "STLover", menuName = "SaveTransformsModule/STLoverModule")]
public class STLoverModule : STPetModule
{
	public override GameObject Spawn(Settings settings)
	{
		GameObject gameObject = base.Spawn(settings);
		string @string = settings.GetString("relationship.info");
		LovedOne.RelationshipInfo info = JSONHelper.Deserialize<LovedOne.RelationshipInfo>(@string);
		Manager.Get<LoveManager>().LoadLover(gameObject, info);
		return gameObject;
	}

	public override Settings Save(AbstractSaveTransform controller)
	{
		Settings settings = base.Save(controller);
		LovedOne component = controller.GetComponent<LovedOne>();
		string value = JSONHelper.ToJSON(component.info);
		settings.SetString("relationship.info", value);
		return settings;
	}
}
