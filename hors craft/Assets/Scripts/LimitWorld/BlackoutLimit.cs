// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.BlackoutLimit
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/Limit/BlackoutLimit")]
	public class BlackoutLimit : Limit
	{
		[SerializeField]
		public AnimationCurve blackoutCurve;

		private BlackoutContext _context;

		public BlackoutContext context
		{
			[CompilerGenerated]
			get
			{
				return _context ?? (_context = new BlackoutContext
				{
					blackoutLevel = 0f
				});
			}
		}

		public override EventTypeLW eventType
		{
			[CompilerGenerated]
			get
			{
				return EventTypeLW.PlayerMoved;
			}
		}

		public void PushContext(float level)
		{
			context.blackoutLevel = level;
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.BLACKOUT, context))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.BLACKOUT);
			}
		}

		public override bool ProcessEvent(DataLW data)
		{
			PlayerMovement playerMovement = (PlayerMovement)data.target;
			PushContext(Mathf.Clamp01(limitShape.GetDistanceToLimit(playerMovement.transform.position, doChunkCorrection)));
			return true;
		}

		public override void ResetLimit()
		{
			base.ResetLimit();
			PushContext(0f);
		}
	}
}
