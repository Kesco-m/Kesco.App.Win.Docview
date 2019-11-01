using System;
using System.Collections;
using System.ComponentModel;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Kesco.App.Win.DocView.CommandManagement
{
    public class CommandManager : Component
    {
        // Member Variables
        private CommandsList _commands;
        private readonly Hashtable _hashCommandExecutors;
        private Timer _timer;
        private bool _block;
        private readonly IContainer components;

        // Constructor
        public CommandManager()
        {
            _commands = new CommandsList(this);
            _hashCommandExecutors = new Hashtable();
            _timer = new Timer(1000);
            _timer.Elapsed += timer_Elapsed;
            _timer.AutoReset = false;
            _block = false;

            // Setup idle processing
            Application.Idle += OnIdle;

            // By default, menus and toolbars are known
            RegisterCommandExecutor("System.Windows.Forms.MenuItem", new MenuCommandExecutor());

            RegisterCommandExecutor("System.Windows.Forms.ToolBarButton", new ToolbarCommandExecutor());


            RegisterCommandExecutor("System.Windows.Forms.ToolStripButton", new ToolStripCommandExecutor());

            RegisterCommandExecutor("System.Windows.Forms.ToolStripDropDownButton", new ToolStripCommandExecutor());

            RegisterCommandExecutor("System.Windows.Forms.ToolStripSplitButton", new ToolStripCommandExecutor());

            RegisterCommandExecutor("Kesco.Lib.Win.Document.Items.ToolStripSplitButtonCheckable",
                                    new ToolStripCommandExecutor());

            RegisterCommandExecutor("System.Windows.Forms.ToolStripDropDownItem", new ToolStripCommandExecutor());

            RegisterCommandExecutor("System.Windows.Forms.ToolStripMenuItem", new ToolStripCommandExecutor());
        }

        // Commands Property: Fetches the Command collection
        public CommandsList Commands
        {
            get { return _commands; }
        }

        protected override void Dispose(bool disposing)
        {
            _commands = null;
            if (disposing)
            {
                Application.Idle -= OnIdle;
                if (_timer != null)
                {
                    _timer.Elapsed -= timer_Elapsed;
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }
                _hashCommandExecutors.Clear();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Command Executor association methods
        internal void RegisterCommandExecutor(string strType, CommandExecutor executor)
        {
            _hashCommandExecutors.Add(strType, executor);
        }

        internal CommandExecutor GetCommandExecutor(object instance)
        {
            return _hashCommandExecutors[instance.GetType().ToString()]
                   as CommandExecutor;
        }

        //  Handler for the Idle application event.
		private void OnIdle(object sender, EventArgs args)
		{

			if(_block || Form.ActiveForm == null || _commands == null)
				return;

#if AdvancedLogging
			try
			{
				Kesco.Lib.Log.Logger.EnterMethod(this, "OnIdle(object sender, EventArgs args)");
#endif
			_block = true;
			try
			{
				Console.WriteLine(@"{0}: reinit button", DateTime.Now.ToString("HH:mm:ss fff"));
			}
			catch(Exception ex)
			{
				Lib.Log.Logger.WriteEx(ex);
			}
			if(_timer == null)
			{
				_timer = new Timer(1000);
				_timer.Elapsed += timer_Elapsed;
				_timer.AutoReset = false;
			}
			_timer.Stop();
			_timer.Interval = 1000;
			_timer.Start();

			IDictionaryEnumerator myEnumerator = _commands.GetEnumerator();
			while(myEnumerator.MoveNext())
			{
				var cmd = myEnumerator.Value as Command;
				if(cmd != null)
					cmd.ProcessUpdates();
			}
#if AdvancedLogging
			}
			finally
			{
				Kesco.Lib.Log.Logger.LeaveMethod(this, "OnIdle(object sender, EventArgs args)");
			}
#endif
		}

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _block = false;
        }
    }
}