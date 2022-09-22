// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.LimitedWorld
using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;

namespace LimitWorld
{
	public class LimitedWorld : MonoBehaviourSingleton<LimitedWorld>
	{
		private bool? _active;

		[SerializeField]
		[Reorderable]
		private Limit[] limits;

		[SerializeField]
		private Dictionary<EventTypeLW, List<Limit>> event2Limit = new Dictionary<EventTypeLW, List<Limit>>();

		public bool drawGizmos;

		public bool active
		{
			get
			{
				bool? active = _active;
				if (!active.HasValue)
				{
					InitDictionary();
					_active = (event2Limit.Count > 0);
				}
				return _active.Value;
			}
		}

		private void InitDictionary()
		{
			event2Limit.Clear();
			if (limits == null || limits.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < limits.Length; i++)
			{
				if (!event2Limit.TryGetValue(limits[i].eventType, out List<Limit> value))
				{
					value = new List<Limit>();
					event2Limit.Add(limits[i].eventType, value);
				}
				value.Add(limits[i]);
				bool inited = limits[i].inited;
			}
		}

		public void RaiseEvent(DataLW target, EventTypeLW type)
		{
			if (event2Limit.TryGetValue(type, out List<Limit> value))
			{
				value.ForEach(delegate(Limit limit)
				{
					limit.ProcessEvent(target);
				});
			}
		}

		public bool RaiseEventForResult(DataLW target, EventTypeLW type)
		{
			bool result = false;
			if (!active)
			{
				return result;
			}
			if (!event2Limit.TryGetValue(type, out List<Limit> value))
			{
				return result;
			}
			result = true;
			int num = 0;
			while (result && num < value.Count)
			{
				result = value[num].ProcessEvent(target);
				num++;
			}
			return result;
		}

		public bool IsInBounds(Vector3 worldPoint)
		{
			if (limits == null || limits.Length < 1)
			{
				return true;
			}
			bool flag = true;
			for (int i = 0; i < limits.Length; i++)
			{
				flag = (flag && limits[i].InBound(worldPoint));
			}
			return flag;
		}

		[ContextMenu("Reset")]
		public void ResetLimits()
		{
			if (limits != null && limits.Length > 0)
			{
				Limit[] array = limits;
				foreach (Limit limit in array)
				{
					limit.ResetLimit();
				}
			}
		}

		[ContextMenu("ReSetup")]
		public void ReSetupLimits()
		{
			if (limits != null && limits.Length > 0)
			{
				Limit[] array = limits;
				foreach (Limit limit in array)
				{
					limit.ReSetup();
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (drawGizmos && limits != null && limits.Length > 0)
			{
				Limit[] array = limits;
				foreach (Limit limit in array)
				{
				}
			}
		}
	}
}
