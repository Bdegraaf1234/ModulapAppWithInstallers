using PluginInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsMod
{
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
		private string FilePath { get; set; }
		IPlugin plugin;
		#endregion

		#region Static
		public void LoadPlugins()
		{
			try
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.InitialDirectory = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\WinFormsMod\Plugins";
				ofd.ShowDialog();
				string dllPath = ofd.FileName;
				plugin = LoadAssembly(dllPath);
				if (plugin.Name == "ExportToCsv")
				{
					exportAsCsvToolStripMenuItem.Visible = true;
				}
			}
			catch (Exception)
			{
				exportAsCsvToolStripMenuItem.Visible = false;
			}
		}

		private IPlugin LoadAssembly(string assemblyPath)
		{
			string assembly = Path.GetFullPath(assemblyPath);
			Assembly ptrAssembly = Assembly.LoadFile(assembly);
			foreach (Type item in ptrAssembly.GetTypes())
			{
				if (!item.IsClass) continue;
				if (item.GetInterfaces().Contains(typeof(IPlugin)))
				{
					return (IPlugin)Activator.CreateInstance(item);
				}
			}
			throw new Exception("Invalid DLL, Interface not found!");
		}

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

			FilePath = ofd.FileName;

			char separator = DetermineSeparator();

			//Parse according to separator
			Data = ParseAsDataTable(FilePath, separator);

			dataGridView1.DataSource = Data;
			if (((DataTable)dataGridView1.DataSource).Rows.Count == 0)
			{
				return;
			}
		}

		private char DetermineSeparator()
		{
			// determine if the input table is csv or tsv
			StreamReader reader = new StreamReader(FilePath);

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

		private void exportAsCsvToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "CSV|*.csv";
			saveFileDialog.Title = "Save a CSV file";
			saveFileDialog.ShowDialog();

			if (saveFileDialog.FileName == null)
			{
				return;
			}

			bool isFileSaved = plugin.Execute(Data, saveFileDialog.FileName);
			if (isFileSaved)
			{
				MessageBox.Show("The CSV file has been exported with success");
			}
			else
			{
				MessageBox.Show("The export operation has failed");
			}
		}

		private void loadToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			LoadPlugins();
		}
	}
}