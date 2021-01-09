using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace Commonality
{
    public partial class MainForm
    {
        private void quit(object sender, EventArgs e)
        {
            //savePrefs();
            Application.Instance.Quit();
        }

        private void openHandler(object sender, EventArgs e)
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

        private void saveAsHandler(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog()
            {
                Title = "Choose file to save",
                Filters =
                {
                    new FileFilter("CSV Files (*.csv)", ".csv")
                }
            };
            if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                doSave(ofd.FileName);
            }
        }
        
        private void doSave(string filename)
        {
            List<string> linesToWrite = new List<string>();
            // Map our collection into a list of strings. Need to do this to handle re-ordered columns, etc.
            int rowCount = cd.data.Count;
            for (int row = 0; row < rowCount; row++)
            {
                string line = "";
                int cols = cd.data[row].Length;
                for (int col = 0; col < cols; col++)
                {
                    line += cd.data[row][col].Text;
                    if (col != cols - 1)
                    {
                        line += ",";
                    }
                }
                linesToWrite.Add(line);
            }
            File.WriteAllLines(filename, linesToWrite);
        }
        private void doLoad(string filename)
        {
            lines = File.ReadAllLines(filename);
            processData();
        }

        private void quitHandler(object sender, EventArgs e)
        {
            //savePrefs();
        }

        private void aboutMe(object sender, EventArgs e)
        {
            if (aboutBox == null || !aboutBox.Visible)
            {
                string creditText = "Version " + CentralProperties.version + ", " +
                                    "© " + CentralProperties.author + " 2020-2021" + "\r\n\r\n";
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

        private void launchHelp(object sender, EventArgs e)
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