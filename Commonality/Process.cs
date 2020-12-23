using System;
using System.Threading;
using System.Threading.Tasks;
using Eto.Forms;

namespace Commonality
{ 
    public partial class MainForm
    {
        private int pCounter;
        private int pMax;
        private async void doStuff()
        {
            MyTable myTable = new MyTable();
            myTable.myDelegates.updateUIProgress = updateProgress;
            myTable.myDelegates.updateUIStatus = updateStatus;
            Task<string> parseTask = Task.Run(() =>
            {
                myTable.parse(lines);

                return "OK";
            });
            try
            {
                string ok = await parseTask;
            }
            catch (Exception)
            {
            }

            int rowCount = myTable.rows.Length;
            int colCount = myTable.rows[0].data.Length;

            Application.Instance.Invoke(() => updateProgress(0.0));

            pMax = rowCount * colCount;

            int updateI = (int)Math.Ceiling((float)(pMax) / 100);
            if (updateI < 1)
            {
                updateI = 1;
            }
            pCounter = 0;
            updateProgress(0);
            updateStatus("Drawing.");

            p.Content = null;

            Task t = Task.Run(() =>
            {
                timer = new System.Timers.Timer();
                // Set up timers for the UI refresh
                timer.AutoReset = true;
                timer.Interval = CentralProperties.timer_interval;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsed);
                timer.Start();
            });

            Task<string> fileLoadTask = Task.Run(() =>
            {
                Application.Instance.Invoke(() =>
                {
                    tl = new TableLayout();

                    for (int r = 0; r < rowCount; r++)
                    {
                        TableRow tr = new TableRow();

                        for (int c = 0; c < colCount; c++)
                        {
                            TableCell tc = new TableCell();
                            tr.Cells.Add(tc);
                            Label l = new Label
                            {
                                Text = myTable.rows[r].data[c].Text, BackgroundColor = myTable.rows[r].data[c].C
                            };
                            tc.Control = l;
                            pCounter++;
                            if (pCounter % updateI == 0)
                            {
                                pbar();
                            }
                        }

                        tl.Rows.Add(tr);
                    }

                });

                return "Complete";
            });
            try
            {
                string done = await fileLoadTask;
                p.Content = tl;
                updateStatus(done);

            }
            catch (Exception)
            {
            }

            timer.Stop();
            timer.Dispose();
        }

        void timerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            pbar();
        }

        void pbar()
        {
            updateProgress((double)pCounter / pMax);
        }
    }
}