// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.DatapileTransaction
using MiniJSON;
using System.Collections.Generic;

namespace TsgCommon
{
	public class DatapileTransaction : IDatapileTransaction
	{
		private Datapile datapile;

		private List<object> rows;

		public DatapileTransaction(Datapile datapile)
		{
			this.datapile = datapile;
		}

		public DatapileIdRef lookup(string schema, string table, Dictionary<string, object> dictionary)
		{
			return execute(DatapileOperation.LOOKUP, schema, table, dictionary);
		}

		public DatapileIdRef insert(string schema, string table, Dictionary<string, object> dictionary)
		{
			return execute(DatapileOperation.INSERT, schema, table, dictionary);
		}

		public DatapileIdRef ensure(string schema, string table, Dictionary<string, object> dictionary)
		{
			return execute(DatapileOperation.ENSURE, schema, table, dictionary);
		}

		public DatapileIdRef execute(DatapileOperation operation, string schema, string table, Dictionary<string, object> dictionary)
		{
			if (rows == null)
			{
				rows = new List<object>();
			}
			List<object> list = new List<object>();
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				string key = item.Key;
				object value = item.Value;
				dictionary2.Add("n", key);
				if (value is DatapileIdRef)
				{
					dictionary2.Add("ref", (value as DatapileIdRef).getIdRef());
				}
				else
				{
					dictionary2.Add("v", value);
				}
				list.Add(dictionary2);
			}
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3.Add("ts", Util.now());
			dictionary3.Add("sch", schema);
			dictionary3.Add("tbl", table);
			dictionary3.Add("op", operation.ToString().ToLower());
			dictionary3.Add("col", list);
			DatapileIdRef result = new DatapileIdRef(rows.Count);
			rows.Add(dictionary3);
			return result;
		}

		public void commit()
		{
			datapile.add(Json.Serialize(rows));
		}
	}
}
