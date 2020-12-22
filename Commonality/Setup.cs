using Eto.Forms;

namespace Commonality
{
    public partial class MainForm
    {
        private void commands()
        {
            Closed += quitHandler;

            quitCommand = new Command {MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q};
            quitCommand.Executed += quit;
            Application.Instance.Terminating += quitHandler; // write our prefs out.

            aboutCommand = new Command {MenuText = "About..."};
            aboutCommand.Executed += aboutMe;

            helpCommand = new Command {MenuText = "Help...", Shortcut = Keys.F1};
            helpCommand.Executed += launchHelp;

            openCSV = new Command
                {MenuText = "Open", ToolBarText = "Open", Shortcut = Application.Instance.CommonModifier | Keys.O};
            openCSV.Executed += openHandler;

            saveAsCSV = new Command
            {
                MenuText = "Save As", ToolBarText = "Save As",
                Shortcut = Application.Instance.CommonModifier | Keys.Shift | Keys.S
            };
            saveAsCSV.Executed += saveAsHandler;

            // create menu
            Menu = new MenuBar
            {
                Items =
                {
                    //File submenu
                    new ButtonMenuItem {Text = "&File", Items = {openCSV, saveAsCSV}},
                },
                QuitItem = quitCommand,
                HelpItems =
                {
                    helpCommand
                },
                AboutItem = aboutCommand
            };

            helpCommand.Enabled = helpAvailable;
        }
    }
}