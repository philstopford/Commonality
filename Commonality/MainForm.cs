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
		Command quitCommand, helpCommand, aboutCommand, openSim;
		CreditsScreen aboutBox;
		string helpPath;
		bool helpAvailable;
		private Panel p;
		
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

			// Figure out whether we should display the help menu, if documentation is available.
			helpPath = Path.Combine(EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources), "Documentation", "index.html");
			helpAvailable = File.Exists(helpPath);
			
			DocumentControl dc = new DocumentControl();
			Content = dc;

			DocumentPage dp = new DocumentPage();
			dp.Text = "Test";
			
			dc.Pages.Add(dp);
			
			Scrollable s = new Scrollable();
			dp.Content = s;

			p = new Panel();
			s.Content = TableLayout.AutoSized(p);

			commands();
			doStuff(makeCSV(maxRows: 200, maxCols: 100));
		}

		async void doStuff(string[] lines)
		{

			MyTable myTable = new MyTable(lines);

			int rowCount = myTable.rows.Count;
			int colCount = myTable.rows[0].data.Length;

			Task fileLoadTask = Task.Run(() =>
			{
				Application.Instance.Invoke(() =>
				{
					TableLayout tl = new TableLayout();

					for (int r = 0; r < rowCount; r++)
					{
						TableRow tr = new TableRow();
						tl.Rows.Add(tr);
					}

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

					p.Content = tl;
				});
			});
			try
			{
				await fileLoadTask;
			}
			catch (Exception)
			{
			}
		}
		
        void commands()
        {
            Closed += quitHandler;

            quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += quit;
            Application.Instance.Terminating += quitHandler; // write our prefs out.

            aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += aboutMe;

            helpCommand = new Command { MenuText = "Help...", Shortcut = Keys.F1 };
            helpCommand.Executed += launchHelp;
            
            openSim = new Command { MenuText = "Open", ToolBarText = "Open", Shortcut = Application.Instance.CommonModifier | Keys.O };
            openSim.Executed += openHandler;
            
            // create menu
            Menu = new MenuBar
            {
                Items = {
                    //File submenu
                    new ButtonMenuItem { Text = "&File", Items = { openSim } },
                },
                QuitItem = quitCommand,
                HelpItems = {
                    helpCommand
                },
                AboutItem = aboutCommand
            };

            helpCommand.Enabled = helpAvailable;
        }
        
        void quit(object sender, EventArgs e)
        {
	        //savePrefs();
	        Application.Instance.Quit();
        }

        void openHandler(object sender, EventArgs e)
        {
	        // Need to request input file location and name.
	        OpenFileDialog ofd = new OpenFileDialog()
	        {
		        Title = "Choose file to load",
		        MultiSelect = false,
		        Filters =
		        {
			        new FileFilter("CSV Files (*.csv)", ".csv")
		        }
	        };
	        if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
	        {
		        doLoad(ofd.FileName);
	        }
        }

        void doLoad(string filename)
        {
	        string[] lines = File.ReadAllLines(filename);
	        doStuff(lines);
	        
        }
        void quitHandler(object sender, EventArgs e)
        {
	        //savePrefs();
        }
        
        void aboutMe(object sender, EventArgs e)
        {
	        if (aboutBox == null || !aboutBox.Visible)
	        {
		        string creditText = "Version " + CentralProperties.version + ", " +
		                            "© " + CentralProperties.author + " 2020" + "\r\n\r\n";
		        creditText += "\r\n\r\n";
		        creditText += "Libraries used:\r\n";
		        creditText += "  Eto.Forms : UI framework\r\n\thttps://github.com/picoe/Eto/wiki\r\n";
		        aboutBox = new CreditsScreen(this, creditText);
	        }
	        Point location = new Point(Location.X + (Width - aboutBox.Width) / 2,
		        Location.Y + (Height - aboutBox.Height) / 2);
	        aboutBox.Location = location;
	        aboutBox.Show();
        }
        
        void launchHelp(object sender, EventArgs e)
        {
	        if (helpAvailable)
	        {
		        new Process
		        {
			        StartInfo = new ProcessStartInfo(@helpPath)
			        {
				        UseShellExecute = true
			        }
		        }.Start();
	        }
        }
        
	}
}
