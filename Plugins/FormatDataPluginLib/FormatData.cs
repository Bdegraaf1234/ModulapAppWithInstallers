using PluginInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatDataPluginLib
{
	public class FormatData : IFormatPlugin
	{
		public string Name => "ChangeColumnNames";

		public event EventHandler OnExecute;

		public DataTable Execute(DataTable data, string[] newNames)
		{
			try
			{
				return ChangeColumNames(ref data, newNames);
			}
			catch (Exception exc)
			{
				//TODO error handling in plugins?
				return null;
			}
		}

		public string GetDescription()
		{
			return "Change your column names";
		}

		public static DataTable ChangeColumNames(ref DataTable inputTable, string[] newNames)
		{
			List<int> idcsToRemove = new List<int>();
			for (int i = 0; i < newNames.Length; i++)
			{
				string name = (string)newNames[i];
				if (name == null)
				{
					idcsToRemove.Add(i);
					continue;
				}
				inputTable.Columns[i].ColumnName = name;
			}
			if (idcsToRemove.Any())
			{
				foreach (int idx in idcsToRemove)
				{
					inputTable.Columns.RemoveAt(idx);
				}
			}
			return inputTable;
		}
	}
}
