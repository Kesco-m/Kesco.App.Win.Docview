using System;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.CommandManagement
{
    // Menu command executor
    public class MenuCommandExecutor : CommandExecutor
    {
        public override void InstanceAdded(object item, Command cmd)
        {
            var mi = item as MenuItem;
            if (mi != null)
                mi.Click += menuItem_Click;
            base.InstanceAdded(item, cmd);
        }

        // State setters
        public override void Enable(object item, bool bEnable)
        {
            var mi = item as MenuItem;
            if (mi != null)
                mi.Enabled = bEnable;
        }

        public override void Check(object item, bool bCheck)
        {
            var mi = item as MenuItem;
            if (mi != null)
                mi.Checked = bCheck;
        }

        // Execution event handler
        private void menuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var cmd = GetCommandForInstance(sender);
                cmd.Execute();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        public override void ImageIndex(object item, int index)
        {
        }
    }
}
