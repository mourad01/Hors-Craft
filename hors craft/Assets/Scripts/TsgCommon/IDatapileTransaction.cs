// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.IDatapileTransaction
using System.Collections.Generic;

namespace TsgCommon
{
	public interface IDatapileTransaction
	{
		DatapileIdRef lookup(string schema, string table, Dictionary<string, object> dictionary);

		DatapileIdRef insert(string schema, string table, Dictionary<string, object> dictionary);

		DatapileIdRef ensure(string schema, string table, Dictionary<string, object> dictionary);

		void commit();
	}
}
