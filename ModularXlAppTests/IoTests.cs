using System;
using FormatDataPluginLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using source;
using static ModularXlAppTests.TestEnvironment;

namespace ModularXlAppTests
{
	[TestClass]
	public class IoTests
	{
		[DataTestMethod]
		[DataRow("b", 2)]
		[DataRow(null, 1)]
		public void ChangeColumnNamesTest(string name1, int idx)
		{
			var outTable = IO.ParseAsDataTable(Input.PLINKCSV);
			for (int i = 0; i < 16; i++)
			{
				outTable.Columns.RemoveAt(0);
			}
			string[] newNames = new string[]
			{
				"a",
				name1,
				"c",
				"d",
				"e",
			};
			var changedTable = FormatData.ChangeColumNames(ref outTable, newNames);
			Assert.AreEqual("c", changedTable.Columns[idx].ColumnName);
		}

		[TestMethod]
		public void ParseDataTable()
		{
			var outTable = IO.ParseAsDataTable(Input.PLINKCSV);
			Assert.IsNotNull(outTable);
		}
	}
}
