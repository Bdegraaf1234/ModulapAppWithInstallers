// <copyright file="Form1.cs" company="Biomolecular Mass Spectrometry and Proteomics (http://hecklab.com)">
// This file is part of HeckLib.
//
// HeckLib is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2.1 of the License, or
// (at your option) any later version.
//
// HeckLib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with HeckLib; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModularWinForms
{
	/// <summary>
	/// template for the form class of a windows forms app project.
	/// </summary>
	public partial class Form1 : Form
	{
		#region Constructor(s)

		/// <summary>
		/// Initializes a new instance of the <see cref="Form1"/> class.
		/// </summary>
		public Form1()
		{
			this.InitializeComponent();
		}

		#endregion

		#region Data
		private Dictionary<DataGridViewColumn, ComboBox> ColumnsAndControls { get; set; } = new Dictionary<DataGridViewColumn, ComboBox>();
		private DataTable Data { get; set; }
		private string Path { get; set; }
		#endregion

		#region Static
		private static DataTable ParseAsDataTable(string strFilePath, char sep)
		{
			DataTable dt = new DataTable();
			using (StreamReader sr = new StreamReader(strFilePath))
			{
				string[] headers = sr.ReadLine().Split(sep);
				foreach (string header in headers)
				{
					if (dt.Columns.Contains(header))
					{
						MessageBox.Show("multiple columns with the name \"" + header + "\" found. Please rename these columns");
						return new DataTable();
					}
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
		#endregion

		#region events
		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.ShowDialog();

			Path = ofd.FileName;

			char separator = DetermineSeparator();

			//Parse according to separator
			Data = ParseAsDataTable(Path, separator);

			dataGridView1.DataSource = Data;
			if (((DataTable)dataGridView1.DataSource).Rows.Count == 0)
			{
				return;
			}
		}

		private char DetermineSeparator()
		{
			// determine if the input table is csv or tsv
			StreamReader reader = new StreamReader(Path);

			// parse the header
			string header = reader.ReadLine();

			// check which charcter occurs the most
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

			reader.Close();
			return separator;
		}
		#endregion
	}
}
