using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Eto;
using Eto.Forms;
using Eto.Drawing;

namespace Commonality
{
    public partial class MainForm : Form
	{
		private string[] lines;

		CommonalityData cd;

		void makeCSV(int maxRows, int maxCols)
		{
			Random rng = new Random();
			int rows = rng.Next(1, maxRows);

			string[] ret = new string[rows];

			string[] words = new[] {"the", "quick", "brown fox", "jumps"};

			ParallelOptions pco = new ParallelOptions();

			Parallel.For(0, rows, pco,(row, loopState) =>
			{
				int index = rng.Next(1, words.Length) - 1;
				string line = words[index];
				if (maxCols > 1)
				{
					for (int col = 1; col < maxCols; col++)
					{
						index = rng.Next(1, words.Length) - 1;
						line += "," + words[index];
					}
				}

				ret[row] = line;
			});

			lines = ret;
		}

		void makeCommonalityPanel()
		{
			var grid = new GridView();

			for (int i = 0; i < cd.data[0].Count; i++)
			{
				var col = i;
				var cellBinding = Binding.Property((List<CommonalityCellData> r) => r[col]);
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

		private void grid_CellFormatting(object sender, GridCellFormatEventArgs e)
		{
			var col = (CommonalityGridColumn)e.Column;
			var cellData = col.CellBinding.GetValue(e.Item);
			e.BackgroundColor = cellData.CellColor;
			e.ForegroundColor = cellData.TextColor;
		}

		private IList<List<CommonalityCellData>> PopulateData(int rows, int columns)
		{
			var data = new ObservableCollection<List<CommonalityCellData>>();
			int colorId = 0;
			for (int row = 0; row < rows; row++)
			{
				var rowItem = new List<CommonalityCellData>();
				for (int col = 0; col < columns; col++)
				{
					var item = new CommonalityCellData();
					item.Text = $"R:{row},C:{col}";
					item.CellColor = Color.FromElementId(colorId++);
					rowItem.Add(item);
				}
				data.Add(rowItem);
			}
			return data;
		}
		public MainForm()
		{
			Title = CentralProperties.productName + " " + CentralProperties.version;
			ClientSize = new Size(400, 350);

			// Figure out whether we should display the help menu, if documentation is available.
			helpPath = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), "Documentation", "index.html");
			helpAvailable = File.Exists(helpPath);

			commands();


			makeCSV(1000, 10);

			processData();
		}

		void processData()
        {
			cd = new CommonalityData(lines);

			makeCommonalityPanel();
		}
	}
}
