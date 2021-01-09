using System;
using Eto;
using Eto.Forms;

namespace Commonality.Wpf
{
	class MainClass
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Style.Add<Eto.Wpf.Forms.Controls.GridViewHandler>(null, h =>
			{
				h.Control.EnableColumnVirtualization = true;
			});
			new Application(Eto.Platforms.Wpf).Run(new MainForm());
		}
	}
}
