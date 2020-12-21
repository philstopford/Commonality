using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Mime;
using Eto.Forms;
using Eto.Drawing;

namespace Commonality
{

	class MyPoco
	{
		public string Text { get; set; }
		public bool Check { get; set; }
		
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
}
