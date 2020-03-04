using PluginInterface;
using source;
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
		IExportPlugin plugin;
		#endregion

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

		private IExportPlugin LoadAssembly(string assemblyPath)
		{
			string assembly = Path.GetFullPath(assemblyPath);
			Assembly ptrAssembly = Assembly.LoadFile(assembly);
			foreach (Type item in ptrAssembly.GetTypes())
			{
				if (!item.IsClass) continue;
				if (item.GetInterfaces().Contains(typeof(IExportPlugin)))
				{
					return (IExportPlugin)Activator.CreateInstance(item);
				}
			}
			throw new Exception("Invalid DLL, Interface not found!");
		}

		#region events
		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.ShowDialog();

			FilePath = ofd.FileName;

			//Parse according to separator
			try
			{
				Data = IO.ParseAsDataTable(FilePath);

			}
			catch (Exception)
			{
				MessageBox.Show("Your input file could not be parsed. Please check for errors.");
			}

			dataGridView1.DataSource = Data;
			if (((DataTable)dataGridView1.DataSource).Rows.Count == 0)
				return;
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