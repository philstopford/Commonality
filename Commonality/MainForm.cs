using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Mime;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;

namespace Commonality
{
	public partial class MainForm : Form
	{

		string[] makeCSV(int maxRows, int maxCols)
		{
			Random rng = new Random();
			int rows = rng.Next(1, maxRows);

			string[] ret = new string[rows];

			string[] words = new[] {"bob", "bosley", "brown fox", "smell"};

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

			return ret;
		}
		
		public MainForm()
		{
			Title = "Commonality";
			ClientSize = new Size(400, 350);

			string[] lines = makeCSV(maxRows: 200, maxCols: 100);

			MyTable myTable = new MyTable(lines);

			int rowCount = myTable.rows.Count;
			int colCount = myTable.rows[0].data.Length;
			
			Scrollable s = new Scrollable();
			Content = s;

			TableLayout tl = new TableLayout();

			for (int r = 0; r < rowCount; r++)
			{
				TableRow tr = new TableRow();
				tl.Rows.Add(tr);
			}

			ParallelOptions pco = new ParallelOptions();

			//Parallel.For(0, rowCount, pco, (row, loopState) =>
			for (int row = 0; row < rowCount; row++)
			{
				for (int c = 0; c < colCount; c++)
				{
					TableCell tc = new TableCell();
					tl.Rows[row].Cells.Add(tc);
					Label l = new Label
					{
						Text = myTable.rows[row].data[c].Text, BackgroundColor = myTable.rows[row].data[c].C
					};
					tc.Control = l;
				}
			}
			//);
			
			s.Content = TableLayout.AutoSized(tl);

		}
	}
}
