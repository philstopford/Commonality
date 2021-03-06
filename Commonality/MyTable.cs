﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using color;
using Eto.Drawing;
using Eto.Forms;

namespace Commonality
{
    public class CommonalityCellData
	{
		public string Text { get; set; }

		public Color CellColor { get; set; }
	}

	public class CommonalityGridColumn : GridColumn
	{
		public IndirectBinding<CommonalityCellData> CellBinding { get; set; }
	}


	public class CommonalityData
	{
		public ObservableCollection<CommonalityCellData[]> data;

		ParallelOptions pco;

		public CommonalityData(ref ParallelOptions pco_)
		{
			init(ref pco_, new string[] { "" });
		}

		public CommonalityData(ref ParallelOptions pco_, string[] rows)
		{
			init(ref pco_, rows);
		}

		void init(ref ParallelOptions pco_, string[] rows)
		{
			pco = pco_;
			data = new ObservableCollection<CommonalityCellData[]>();
			for (int i = 0; i < rows.Length; i++)
            {
				data.Add(parseRow(rows[i]));
            }
        }
		
		CommonalityCellData[] parseRow(string text)
		{
			string[] tokens = text.Split(new[] {','});

			int tokenCount = tokens.Length;
			CommonalityCellData[] ret = new CommonalityCellData[tokenCount];

			// Now we need to find our unique strings.
			List<string> uniqueStrings = new List<string>();
			uniqueStrings.Add(tokens[0]);

			for (int i = 1; i < tokenCount; i++)
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
			Parallel.For(0, tokens.Length, pco, (i, loopState) =>
			// for (int i = 0; i < tokens.Length; i++)
			{
				ret[i] = new CommonalityCellData() { Text = tokens[i], CellColor = colors[uniqueStrings.IndexOf(tokens[i])] };
			}
			);

			return ret;
		}
	}
	
}
