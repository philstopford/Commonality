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
	}
	class MyTable
	{
		public delegates myDelegates;

		private int counter;
		private int max;
		private double progress;
		private string[] lines_;
		
		void timerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			progressUIWrapper((double)counter / max);
		}

		void statusUIWrapper(string text)
		{
			myDelegates.updateUIStatus?.Invoke(text);
		}

		void progressUIWrapper(double val)
		{
			myDelegates.updateUIProgress?.Invoke(val);
		}
		
		public Row[] rows;
		private System.Timers.Timer timer;

		public MyTable()
		{
			myDelegates = new delegates();
		}
		public void parse(string[] lines)
		{
			lines_ = lines;
			myDelegates.updateUIProgress?.Invoke(0.0);
			max = lines.Length;
			rows = new Row[max];
			
			statusUIWrapper("Processing...");
			counter = 0;
			int updateI = (int)Math.Ceiling((float)(max) / 100);
			if (updateI < 1)
            {
				updateI = 1;
            }
			double progress = 0.0;

			/*
			timer = new System.Timers.Timer();
			timer.AutoReset = true;
			timer.Elapsed += TimerOnElapsed;
			timer.Interval = CentralProperties.timer_interval;
			timer.Start();
			*/
			ParallelOptions pco = new ParallelOptions();
			Parallel.For(0, max, pco, (i, loopState) =>
				{
					rows[i] = new Row();
					rows[i].myDelegates = myDelegates;
					rows[i].parse(lines[i]);
					Interlocked.Increment(ref counter);
					if (counter % updateI == 0)
                    {
						progress += 0.01f;
						progressUIWrapper(progress);
                    }
				}
			);
			/*
			timer.Stop();
			timer.Dispose();
			*/
			statusUIWrapper("Done");
		}
	}
	
	class Row
	{
		public delegates myDelegates;

		public Data[] data { get; set; }
		
		public Row()
        {
			myDelegates = new delegates();
        }

		public void parse(string text)
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
			ParallelOptions pco = new ParallelOptions();

			Parallel.For(0, tokens.Length, pco, (i, loopState) =>
			{
				data[i] = new Data();
				data[i].Text = tokens[i];
				data[i].C = colors[uniqueStrings.IndexOf(tokens[i])];
			});
		}
	}
	
	class Data
	{
		public string Text { get; set; }
		
		public Color C { get; set; }
	}
}