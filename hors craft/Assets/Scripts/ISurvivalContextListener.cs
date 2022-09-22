// DecompilerFi decompiler from Assembly-CSharp.dll class: ISurvivalContextListener
using System;

public interface ISurvivalContextListener
{
	bool AddBySurvivalManager();

	Type[] ContextTypes();

	Action[] OnContextsUpdated();
}
