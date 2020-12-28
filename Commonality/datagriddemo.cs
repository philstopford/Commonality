using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Commonality
{

    public class MyCellData
    {
        public string Text { get; set; }
        public Color Color { get; set; }
    }

    public class MyGridColumn : GridColumn
    {
        public IndirectBinding<MyCellData> CellBinding { get; set; }
    }

    public partial class MyPanel : Panel
    {
        public MyPanel()
        {
            var grid = new GridView();

            var data = PopulateData(1000, 10);
            for (int i = 0; i < 10; i++)
            {
                var col = i;
                var cellBinding = Binding.Property((List<MyCellData> r) => r[col]);
                var column = new MyGridColumn
                {
                    HeaderText = $"Col: {i}",
                    CellBinding = cellBinding,
                    DataCell = new TextBoxCell
                    {
                        Binding = cellBinding.Child((MyCellData m) => m.Text)
                    }
                };
                grid.Columns.Add(column);
            }

            grid.CellFormatting += grid_CellFormatting;


            grid.DataStore = data;


            Content = grid;

        }

        private void grid_CellFormatting(object sender, GridCellFormatEventArgs e)
        {
            var col = (MyGridColumn)e.Column;
            var cellData = col.CellBinding.GetValue(e.Item);
            e.BackgroundColor = cellData.Color;
        }

        private IList<List<MyCellData>> PopulateData(int rows, int columns)
        {
            var data = new ObservableCollection<List<MyCellData>>();
            int colorId = 0;
            for (int row = 0; row < rows; row++)
            {
                var rowItem = new List<MyCellData>();
                for (int col = 0; col < columns; col++)
                {
                    var item = new MyCellData();
                    item.Text = $"R:{row},C:{col}";
                    item.Color = Color.FromElementId(colorId++);
                    rowItem.Add(item);
                }
                data.Add(rowItem);
            }
            return data;
        }
    }
}