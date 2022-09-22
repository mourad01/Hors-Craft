// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.Reactors.AnimatorTriggerReactor
using UnityEngine;

namespace Common.Behaviours.Reactors
{
	public class AnimatorTriggerReactor : AbstractReactor
	{
		[Header("Trigger name to be triggered.")]
		public string triggerName = string.Empty;

		[Header("Destination. Leave empty if animator is self.")]
		public Animator triggerIn;

		private Animator animator;

		private void Awake()
		{
			animator = ((!(triggerIn != null)) ? GetComponent<Animator>() : triggerIn);
		}

		public override void React()
		{
			animator.SetTrigger(triggerName);
		}
	}
}
