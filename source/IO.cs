using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace source
{
    public class IO
    {

		public static DataTable ParseAsDataTable(string strFilePath)
		{
			DataTable dt = new DataTable();
			using (StreamReader sr = new StreamReader(strFilePath))
			{
				string headString = sr.ReadLine();
				char sep = GetSeparator(headString);
				string[] headers = headString.Split(sep);
				foreach (string header in headers)
				{
					if (dt.Columns.Contains(header))
						throw new Exception("multiple columns with the name \"" + header + "\" found. Please rename these columns");
					dt.Columns.Add(header);
				}
				while (!sr.EndOfStream)
				{
					string[] rows = sr.ReadLine().Split(sep);
					DataRow dr = dt.NewRow();
					for (int i = 0; i < headers.Length; i++)
					{
						dr[i] = rows[i];
					}

					dt.Rows.Add(dr);
				}
			}

			return dt;
		}

		private static char GetSeparator(string header)
		{
			// check which character occurs the most
			Dictionary<char, int> characteroccurance = new Dictionary<char, int>();
			for (int i = 0; i < header.Length; ++i)
			{
				if (header[i] != '\t' && header[i] != ',')
				{
					continue;
				}

				if (!characteroccurance.ContainsKey(header[i]))
				{
					characteroccurance.Add(header[i], 0);
				}

				characteroccurance[header[i]]++;
			}

			// set the separator
			char separator = '\t';
			int maxoccurance = 0;
			foreach (char chr in characteroccurance.Keys)
			{
				if (characteroccurance[chr] > maxoccurance)
				{
					maxoccurance = characteroccurance[chr];
					separator = chr;
				}
			}
			return separator;
		}
	}
}
