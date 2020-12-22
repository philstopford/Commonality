using System;
using System.Threading.Tasks;
using Eto.Forms;

namespace Commonality
{ 
    public partial class MainForm
    {
        private async void doStuff()
        {
            MyTable myTable = new MyTable();
            myTable.myDelegates.updateUIProgress = updateProgress;
            myTable.myDelegates.updateUIStatus = updateStatus;
            Task parseTask = Task.Run(() =>
            {
                myTable.parse(lines);
            });
            try
            {
                await parseTask;
            }
            catch (Exception)
            {
            }

            int rowCount = myTable.rows.Length;
            int colCount = myTable.rows[0].data.Length;

            Application.Instance.Invoke(() => updateProgress(0.0));

            int updateI = (int)Math.Ceiling((float)(rowCount) / 100);
            if (updateI < 1)
            {
                updateI = 1;
            }
            double progress = 0.0;

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
                            if (row % updateI == 0)
                            {
                                progress += 0.01f;
                                Application.Instance.Invoke(() => updateProgress(progress));
                            }
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
    }
}