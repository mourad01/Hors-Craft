// DecompilerFi decompiler from Assembly-CSharp.dll class: McpeSteeringModule
using Common.Managers;
using Common.Model;

public class McpeSteeringModule : ModelModule
{
	private McpeContext mcpeContext;

	private string keyEnabled()
	{
		return "mcpe.steering.enabled";
	}

	private string keyTutorialEnabled()
	{
		return "mcpe.steering.tutorial.enabled";
	}

	private string keyInteractionDistance()
	{
		return "mcpe.steering.interaction.distance";
	}

	private string keyTapBeforeTime()
	{
		return "mcpe.steering.tap.before.time";
	}

	private string keyDigAfterTime()
	{
		return "mcpe.steering.dig.after.time";
	}

	private string keyDigNextVoxelTimeInterval()
	{
		return "mcpe.steering.dig.next.voxel.interval";
	}

	private string keyDragThreshold()
	{
		return "mcpe.steering.drag.threshold";
	}

	private string keyFlyInCameraDirection()
	{
		return "mcpe.fly.in.camera.direction";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyEnabled(), defaultValue: false);
		descriptions.AddDescription(keyTutorialEnabled(), defaultValue: false);
		descriptions.AddDescription(keyInteractionDistance(), 15f);
		descriptions.AddDescription(keyTapBeforeTime(), 0.2f);
		descriptions.AddDescription(keyDigAfterTime(), 0.45f);
		descriptions.AddDescription(keyDigNextVoxelTimeInterval(), 0.3f);
		descriptions.AddDescription(keyDragThreshold(), 5f);
		descriptions.AddDescription(keyFlyInCameraDirection(), defaultValue: false);
	}

	public override void OnModelDownloaded()
	{
		if (GetEnabled())
		{
			if (mcpeContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.MCPE_STEERING, mcpeContext);
			}
			mcpeContext = new McpeContext
			{
				flyInCameraDirection = GetFlyInCameraDirection()
			};
			MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.MCPE_STEERING, mcpeContext);
		}
		else
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.MCPE_STEERING);
		}
		if (!(GetInteractionDistance() < 0.1f))
		{
			McpeSteering get = MonoBehaviourSingleton<McpeSteering>.get;
			get.interactionDistance = GetInteractionDistance();
			get.tapBeforeTime = GetTapBeforeTime();
			get.digAfterTime = GetDigAfterTime();
			get.digNextVoxelTimeInterval = GetDigNextVoxelTimeInterval();
			get.dragThreshold = GetDragThreshold();
		}
	}

	public bool GetEnabled()
	{
		return base.settings.GetBool(keyEnabled());
	}

	public bool GetTutorialEnabled()
	{
		return base.settings.GetBool(keyTutorialEnabled());
	}

	public float GetInteractionDistance()
	{
		return base.settings.GetFloat(keyInteractionDistance());
	}

	public float GetTapBeforeTime()
	{
		return base.settings.GetFloat(keyTapBeforeTime());
	}

	public float GetDigAfterTime()
	{
		return base.settings.GetFloat(keyDigAfterTime());
	}

	public float GetDigNextVoxelTimeInterval()
	{
		return base.settings.GetFloat(keyDigNextVoxelTimeInterval());
	}

	public float GetDragThreshold()
	{
		return base.settings.GetFloat(keyDragThreshold());
	}

	public bool GetFlyInCameraDirection()
	{
		return base.settings.GetBool(keyFlyInCameraDirection());
	}
}
