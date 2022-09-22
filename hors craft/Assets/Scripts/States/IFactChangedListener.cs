// DecompilerFi decompiler from Assembly-CSharp.dll class: States.IFactChangedListener
using System.Collections.Generic;

namespace States
{
	public interface IFactChangedListener
	{
		void OnFactsChanged(HashSet<Fact> facts);
	}
}
