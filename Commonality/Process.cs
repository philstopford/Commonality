using System;
using System.Threading.Tasks;
using Eto.Forms;

namespace Commonality
{
    public partial class MainForm
    {
        private async void doStuff(string[] lines)
        {
            MyTable myTable = new MyTable(lines);

            int rowCount = myTable.rows.Length;
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
    }
}