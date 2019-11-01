using System;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.CommandManagement
{
    // ToolStrip command executor
    public class ToolStripCommandExecutor : CommandExecutor
    {
        public override void InstanceAdded(object item, Command cmd)
        {
            var button = item as ToolStripItem;
            var handler = new EventHandler(toolStripButton_Click);

            // Attempt to remove the handler first, in case we have already 
            // signed up for the event in this toolbar
            if (button != null)
            {
                button.Click -= handler;
                button.Click += handler;
            }

            base.InstanceAdded(item, cmd);
        }

        // State setters
        public override void Enable(object item, bool bEnable)
        {
            var button = item as ToolStripItem;
            if (button != null && button.Enabled != bEnable)
                button.Enabled = bEnable;
        }

        public override void ImageIndex(object item, int index)
        {
            var button = item as ToolStripItem;
            if (button != null && button.ImageIndex != index)
                button.ImageIndex = index;
        }

        public override void Check(object item, bool bCheck)
        {
            var button = item as ToolStripButton;
            if (button != null)
                button.Checked = bCheck;
        }

        // Execution event handler
        private void toolStripButton_Click(object sender, EventArgs args)
        {
            try
            {
                ToolStripSplitButton button = sender as ToolStripSplitButton;
                if (button != null && !button.ButtonPressed)
                    return;

                var cmd = GetCommandForInstance(sender);
                if (cmd != null)
                    cmd.Execute();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }
    }
}
