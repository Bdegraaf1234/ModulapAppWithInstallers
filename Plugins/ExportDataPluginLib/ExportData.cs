using CsvHelper;
using PluginInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportDataPluginLib
{
	public class ExportData : IPlugin
	{
		public string Name => "ExportToCsv";

		public event EventHandler OnExecute;

		public bool Execute(DataTable data, string filePath)
		{
			try
			{
				using (TextWriter textWriter = File.CreateText(filePath))
				{
					CsvWriter csvWriter = new CsvWriter(textWriter);
					csvWriter.Configuration.Delimiter = ";";
					csvWriter.WriteRecords(data.Rows);
				}

				return true;
			}
			catch (Exception exc)
			{
				return false;
			}
		}

		public string GetDescription()
		{
			return "Export data to CSV";
		}
	}
}
