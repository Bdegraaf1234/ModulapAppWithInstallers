using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterface
{
	public interface IPlugin
	{
		string Name { get; }

		string GetDescription();

		bool Execute(DataTable data, string filePath);

		event EventHandler OnExecute;
	}

}
