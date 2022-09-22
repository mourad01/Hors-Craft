// DecompilerFi decompiler from Assembly-CSharp.dll class: STHumanModule
using Common.Managers;
using Common.Model;
using InteractiveAnimations;
using UnityEngine;

[CreateAssetMenu(fileName = "STHuman", menuName = "SaveTransformsModule/STHumanModule")]
public class STHumanModule : STModule
{
	public override GameObject Spawn(Settings settings)
	{
		GameObject gameObject = base.Spawn(settings);
		if (settings.HasInt("animations.data"))
		{
			int @int = settings.GetInt("animations.data");
			StaticAnimationUnit staticAnimationUnit = gameObject.AddComponent<StaticAnimationUnit>();
			StaticAnimation sAnimation = Manager.Get<InteractiveAnimationsManager>().interactiveAnimationsList.interactiveAnimations[@int];
			staticAnimationUnit.SetStaticAnimation(sAnimation, @int);
			staticAnimationUnit.animationDefinition.animationIndex = @int;
		}
		HumanMob componentInChildren = gameObject.GetComponentInChildren<HumanMob>();
		if (componentInChildren != null)
		{
			componentInChildren.Init(new HumanMob.HumanParameters(Random.value));
		}
		return gameObject;
	}

	public override Settings Save(AbstractSaveTransform controller)
	{
		Settings settings = base.Save(controller);
		StaticAnimationUnit componentInChildren = controller.gameObject.GetComponentInChildren<StaticAnimationUnit>();
		if (componentInChildren != null)
		{
			StaticAnimation animationDefinition = componentInChildren.animationDefinition;
			int animationIndex = animationDefinition.animationIndex;
			settings.SetInt("animations.data", animationIndex);
		}
		return settings;
	}
}
