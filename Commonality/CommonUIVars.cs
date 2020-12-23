using Eto.Forms;

namespace Commonality
{
    public partial class MainForm
    {
        private Command quitCommand;
        private Command helpCommand;
        private Command aboutCommand;
        private Command openCSV;
        private Command saveAsCSV;
        private CreditsScreen aboutBox;
        private string helpPath;
        private bool helpAvailable;
        private Panel p;
        private Panel statusBar;
        private ProgressBar progressBar;
        private Label lbl_statusBar;

        private int labelFontSize = 20;

        private TableLayout tl;

        private System.Timers.Timer timer;

        private MyTable myTable;
    }
}