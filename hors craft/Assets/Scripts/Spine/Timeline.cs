// DecompilerFi decompiler from Assembly-CSharp.dll class: Spine.Timeline
namespace Spine
{
	public interface Timeline
	{
		int PropertyId
		{
			get;
		}

		void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> events, float alpha, MixPose pose, MixDirection direction);
	}
}
