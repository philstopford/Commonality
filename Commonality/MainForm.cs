using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Eto;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using System.Data;
using entropyRNG;

namespace Commonality
{
    public partial class MainForm : Form
	{
		private string[] lines;

		CommonalityData cd;

		ParallelOptions pco;

		void makeCSV(int maxRows, int maxCols)
		{
			int rows = Convert.ToInt32(RNG.nextdouble() * maxRows);

			string[] ret = new string[rows];

			string[] words = new[] {"the", "quick", "brown fox", "jumps"};

			Parallel.For(0, rows, pco,(row, loopState) =>
			{
				int index = Convert.ToInt32(RNG.nextdouble() * (words.Length - 1));
				string line = words[index];
				if (maxCols > 1)
				{
					for (int col = 1; col < maxCols; col++)
					{
						index = Convert.ToInt32(RNG.nextdouble() * (words.Length - 1));
						line += "," + words[index];
					}
				}

				ret[row] = line;
			});

			lines = ret;
		}

		void makeCommonalityPanel()
		{
			GridView grid = new GridView();
			createHeaderContextMenu();
			grid.ContextMenu = headerCM;
			grid.AllowMultipleSelection = true;
			grid.AllowColumnReordering = true;
			grid.ShowHeader = true;

			for (int i = 0; i < cd.data[0].Length; i++)
			{
				var col = i;
				var cellBinding = Binding.Property((CommonalityCellData[] r) => r[col]);
				var column = new CommonalityGridColumn
				{
					HeaderText = $"Col: {i}",
					CellBinding = cellBinding,
					DataCell = new TextBoxCell
					{
						Binding = cellBinding.Child((CommonalityCellData m) => m.Text)
					}
				};
				grid.Columns.Add(column);
			}

			grid.CellFormatting += grid_CellFormatting;

			grid.DataStore = cd.data;

			Content = grid;

		}

		void createHeaderContextMenu()
		{
			headerCM = new ContextMenu();
			int itemIndex = 0;
			ButtonMenuItem lb_test = new ButtonMenuItem() { Text = "Test" };
			headerCM.Items.Add(lb_test);
			headerCM.Items[itemIndex].Click += delegate
			{
			};
			itemIndex++;
		}


		private void grid_CellFormatting(object sender, GridCellFormatEventArgs e)
		{
			var col = (CommonalityGridColumn)e.Column;
			var cellData = col.CellBinding.GetValue(e.Item);
			e.BackgroundColor = cellData.CellColor;
			e.ForegroundColor = inverseColor(e.BackgroundColor);
		}

		Color inverseColor(Color source)
        {
			Color ret = new Color(Math.Abs(1.0f - source.B), Math.Abs(1.0f - source.R), Math.Abs(1.0f - source.G));
			return ret;
		}

		public MainForm()
		{
			Title = CentralProperties.productName + " " + CentralProperties.version;
			ClientSize = new Size(400, 350);

			pco = new ParallelOptions();

			// Figure out whether we should display the help menu, if documentation is available.
			helpPath = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), "Documentation", "index.html");
			helpAvailable = File.Exists(helpPath);

			commands();

			makeCSV(1000, 1000);

			processData();
		}

		void processData()
        {
			cd = new CommonalityData(ref pco, lines);

			makeCommonalityPanel();
		}
	}
}
