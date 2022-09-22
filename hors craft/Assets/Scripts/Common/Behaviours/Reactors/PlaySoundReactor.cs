// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.Reactors.PlaySoundReactor
using Common.Managers.Audio;

namespace Common.Behaviours.Reactors
{
	public class PlaySoundReactor : AbstractReactor
	{
		public Sound soundConfig;

		public bool calculatePanStereo = true;

		public bool calculateVolumeFromDistance = true;

		public float maxDistance = 150f;

		public override void React()
		{
			soundConfig.Play(base.transform);
		}
	}
}
