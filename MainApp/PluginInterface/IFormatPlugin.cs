using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterface
{
	public interface IFormatPlugin
	{
		string Name { get; }

		string GetDescription();

		DataTable Execute(DataTable data, string[] newNames);

		event EventHandler OnExecute;
	}
}
