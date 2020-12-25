using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Eto;
using Eto.Forms;
using Eto.Drawing;

namespace Commonality
{
	public partial class MainForm : Form
	{
		private string[] lines;
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
		
		public MainForm()
		{
			Title = CentralProperties.productName + " " + CentralProperties.version;
			ClientSize = new Size(400, 350);

			// Figure out whether we should display the help menu, if documentation is available.
			helpPath = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), "Documentation", "index.html");
			helpAvailable = File.Exists(helpPath);

			commands();

			Content = new MyPanel();

			return;

			DocumentControl dc = new DocumentControl();
			Content = dc;

			DocumentPage dp = new DocumentPage();
			dp.Text = "Test";
			
			dc.Pages.Add(dp);


			Scrollable s = new Scrollable();
			p = new Panel();
			tl = new TableLayout();
			p.Content = tl;
			s.Content = TableLayout.AutoSized(p);

			Panel outerPanel = new Panel();
			dp.Content = outerPanel;
			TableLayout outerTable = new TableLayout();
			outerPanel.Content = outerTable;
			outerTable.Rows.Add(new TableRow() {ScaleHeight = true});
			outerTable.Rows[0].Cells.Add(new TableCell() {Control = s});
			outerTable.Rows.Add(new TableRow());

			statusBar = new Panel();
			outerTable.Rows[1].Cells.Add(new TableCell() {Control = statusBar});
			
			TableLayout statusTable = new TableLayout();
			statusBar.Content = statusTable;
			statusTable.Rows.Add(new TableRow());

			lbl_statusBar = new Label();
			lbl_statusBar.Text = "Hello";
			statusTable.Rows[0].Cells.Add(new TableCell() {Control = lbl_statusBar});

			progressBar = new ProgressBar();
			progressBar.MinValue = 0;
			progressBar.MaxValue = 100;
			statusTable.Rows[0].Cells.Add(new TableCell() {Control = progressBar});
			
			makeCSV(maxRows: 100, maxCols: 25);

			myTable = new MyTable();
			myTable.myDelegates.updateUIProgress = updateProgress;
			myTable.myDelegates.updateUIStatus = updateStatus;

			doStuff();
		}
	}
}
