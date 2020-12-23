using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Eto.Drawing;

namespace Commonality
{
	class delegates
    {
		public delegate void UpdateUIStatus(string text);
		public UpdateUIStatus updateUIStatus { get; set; }

		public delegate void UpdateUIProgress(double val);
		public UpdateUIProgress updateUIProgress { get; set; }

		public delegate void UpdateUIProgress2(int count, int max);
		public UpdateUIProgress2 updateUIProgress2 { get; set; }

		/*
		void timerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			progressUIWrapper((double)counter / rowCount);
		}
		*/
		public void statusUIWrapper(string text)
		{
			updateUIStatus?.Invoke(text);
		}

		public void progressUIWrapper(double val)
		{
			updateUIProgress?.Invoke(val);
		}

	}

	class TableData
    {
		public double progress = 0.0;
		public int rowCount;
		public int colCount;
		public int cellCounter;
		public int updateI;

		public TableData()
        {
			rowCount = 0;
			colCount = 0;
			cellCounter = 0;
			updateI = 1;
        }
    }

	class MyTable
	{
		public delegates myDelegates;

		private double progress;
		private string[] lines_;

		TableData td;

		public Row[] rows;
		private System.Timers.Timer timer;

		public MyTable()
		{
			myDelegates = new delegates();
			td = new TableData();
		}
		public void parse(string[] lines)
		{
			lines_ = lines;
			myDelegates.updateUIProgress?.Invoke(0.0);
			td.rowCount = lines.Length;

			// CSV must be rectangular, or this will fail.

			td.colCount = lines[0].Split(new[] { ',' }).Length;

			rows = new Row[td.rowCount];
			
			myDelegates.statusUIWrapper("Processing...");
			td.cellCounter = 0;
			td.progress = 0.0;
			td.updateI = (int)Math.Ceiling((float)(td.rowCount * td.colCount) / 100);
			if (td.updateI < 1)
            {
				td.updateI = 1;
            }

			ParallelOptions pco = new ParallelOptions();
			Parallel.For(0, td.rowCount, pco, (i, loopState) =>
				{
					rows[i] = new Row(ref td);
					rows[i].myDelegates = myDelegates;
					rows[i].parse(lines[i]);
				}
			);

			myDelegates.statusUIWrapper("Done");
		}
	}
	
	class Row
	{
		public delegates myDelegates;
		public TableData td;

		public Data[] data { get; set; }
		
		public Row(ref TableData td_)
        {
			myDelegates = new delegates();
			td = td_;
        }

		public void parse(string text)
		{
			string[] tokens = text.Split(new[] {','});

			data = new Data[td.colCount];

			// Now we need to find our unique strings.
			List<string> uniqueStrings = new List<string>();
			uniqueStrings.Add(tokens[0]);

			for (int i = 1; i < td.colCount; i++)
			{
				try
				{
					int cIndex = uniqueStrings.IndexOf(tokens[i]);
					if (cIndex == -1)
					{
						uniqueStrings.Add(tokens[i]);
					}

				}
				catch (Exception)
                {
                }
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
			ParallelOptions pco = new ParallelOptions();

			Parallel.For(0, tokens.Length, pco, (i, loopState) =>
			{
				data[i] = new Data();
				data[i].Text = tokens[i];
				data[i].C = colors[uniqueStrings.IndexOf(tokens[i])];
				Interlocked.Increment(ref td.cellCounter);
				if (td.cellCounter % td.updateI == 0)
				{
					td.progress += 0.01f;
					myDelegates.progressUIWrapper(td.progress);
				}
			});
		}
	}
	
	class Data
	{
		public string Text { get; set; }
		
		public Color C { get; set; }
	}
}