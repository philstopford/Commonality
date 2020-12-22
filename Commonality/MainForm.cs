using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Mime;
using Eto.Forms;
using Eto.Drawing;

namespace Commonality
{

	/*
	class MyPoco
	{
		public string Text { get; set; }
		public bool Check { get; set; }
		public string Text2 { get; set; }
		
		public Color C { get; set; }
	}
	public partial class MainForm : Form
	{
		private ObservableCollection<MyPoco> oc;
		void gridFormat(object sender, GridCellFormatEventArgs e)
		{
			e.BackgroundColor = ((MyPoco) e.Item).C;
		}
		public MainForm()
		{
			Title = "Commonality";
			ClientSize = new Size(400, 350);

			
			
			oc = new ObservableCollection<MyPoco>();
			oc.Add(new MyPoco { Text="Row 1", Check= true, C = new Color(1.0f, 0.0f, 0.0f) });
			oc.Add(new MyPoco { Text="Row 2", Check= false, C = new Color(red:0.0f, green:1.0f, blue:0.1f) });

			GridView grid = new GridView{DataStore = oc};
			
			grid.Columns.Add(new GridColumn
			{
				DataCell = new TextBoxCell{Binding = Binding.Property<MyPoco, string>(r => r.Text), },
				HeaderText = "Text"
			});

			grid.Columns.Add(new GridColumn
			{
				DataCell = new CheckBoxCell(){Binding = Binding.Property<MyPoco, bool?>(r => r.Check)},
				HeaderText = "Check"
			});

			grid.CellFormatting += gridFormat;

			Content = grid;
		}
	}
	
	*/

	class MyTable
	{
		public List<Row> rows;

		public MyTable(string[] lines)
		{
			rows = new List<Row>();
			for (int i = 0; i < lines.Length; i++)
			{
				rows.Add(new Row(lines[i]));
			}
		}
	}
	
	class Row
	{
		public Data[] data { get; set; }

		public Row(string text)
		{
			string[] tokens = text.Split(new[] {','});

			data = new Data[tokens.Length];

			// Now we need to pattern match and also do the color computation.
			List<string> uniqueStrings = new List<string>();
			uniqueStrings.Add(tokens[0]);
			int[] colIndex = new int[tokens.Length];
			colIndex[0] = 0;
			for (int i = 1; i < tokens.Length; i++)
			{
				int cIndex = uniqueStrings.IndexOf(tokens[i]);
				if (cIndex == -1)
				{
					uniqueStrings.Add(tokens[i]);
					cIndex = uniqueStrings.Count - 1;
				}

				colIndex[i] = cIndex;
			}

			/* Figure out colors. This is not entirely trivial.
			   We don't want adjacent changes in the same color channel.
			 */

			int colorSpacing = uniqueStrings.Count;
			int colorOffset = 50;

			Color[] colors = new Color[colorSpacing];

			if (colorSpacing == 1)
			{
				colors[0] = new Color((float) (colorOffset) / 255, (float) (colorOffset) / 255,
					(float) (colorOffset) / 255);
			}
			else
			{
				// Divide colorSpacing by 3 to take account of the channel split.
				// Also clamp the range to avoid too-dark or too-bright colors being assigned.
				float colorStepTemp = ((250.0f - (2 * colorOffset)) / (colorSpacing));
				int colorStep = (int) Math.Floor(colorStepTemp);
				int RValue = colorOffset, GValue = colorOffset, BValue = colorOffset;
				for (int i = 0; i < colorSpacing; i++)
				{
					// RValue, GValue and BValue used below to hold channel color value as Int.
					// RCount, GCount and BCount used below to record number of iterations for each channel
					// Look at the loop value and tweak appropriate channel based on modulo 3.
					if (i <= 2)
					{
						// fudge for the first 3 runs through to deal with the issues.
						switch (i)
						{
							case 0:
								RValue += colorStep;
								break;
							case 1:
								RValue -= colorStep;
								GValue += colorStep;
								break;
							case 2:
								GValue -= colorStep;
								BValue += colorStep;
								break;
						}
					}
					else
					{
						if (i == 3)
						{
							// get values set up accordingly.
							RValue = colorOffset + colorStep;
							GValue = colorOffset + colorStep;
							BValue = colorOffset + colorStep;
						}
						
						switch (i % 3)
						{
							case 0:
								RValue += colorStep;
								break;
							case 1:
								RValue -= colorStep;
								GValue += colorStep;
								break;
							case 2:
								GValue -= colorStep;
								BValue += colorStep;
								break;
						}
					}
					colors[i] = new Color((float) (RValue) / 255, (float) (GValue) / 255,
						(float) (BValue) / 255);				}
			}
			
			// Now set up our data list.
			for (int i = 0; i < tokens.Length; i++)
			{
				data[i] = new Data();
				data[i].Text = tokens[i];
				data[i].C = colors[uniqueStrings.IndexOf(tokens[i])];
			}
		}
	}
	
	class Data
	{
		public string Text { get; set; }
		
		public Color C { get; set; }
	}

	public partial class MainForm : Form
	{

		string[] makeCSV()
		{
			Random rng = new Random();
			int rows = rng.Next(1, 10);

			string[] ret = new string[rows];

			string[] words = new[] {"bob", "bosley", "brown fox", "smell"};

			for (int row = 0; row < rows; row++)
			{
				int cols = 10;
				int index = rng.Next(1, words.Length) - 1;
				string line = words[index];
				if (cols > 1)
				{
					for (int col = 1; col < cols; col++)
					{
						index = rng.Next(1, words.Length) - 1;
						line += "," + words[index];
					}
				}

				ret[row] = line;
			}

			return ret;
		}
		
		public MainForm()
		{
			Title = "Commonality";
			ClientSize = new Size(400, 350);

			string[] lines = makeCSV();

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
				for (int c = 0; c < colCount; c++)
				{
					TableCell tc = new TableCell();
					tr.Cells.Add(tc);
					Label l = new Label
					{
						Text = myTable.rows[r].data[c].Text, BackgroundColor = myTable.rows[r].data[c].C
					};
					tc.Control = l;
				}
			}
			
			s.Content = TableLayout.AutoSized(tl);

		}
	}
}
