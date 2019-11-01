using System;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.CommandManagement
{
    // Toolbar command executor
    public class ToolbarCommandExecutor : CommandExecutor
    {
        public override void InstanceAdded(object item, Command cmd)
        {
            var button = (ToolBarButton)item;
            var handler = new ToolBarButtonClickEventHandler(toolbar_ButtonClick);

            // Attempt to remove the handler first, in case we have already 
            // signed up for the event in this toolbar
            if (button != null)
            {
                button.Parent.ButtonClick -= handler;
                button.Parent.ButtonClick += handler;
            }

            base.InstanceAdded(item, cmd);
        }

        // State setters
        public override void Enable(object item, bool bEnable)
        {
            var button = item as ToolBarButton;
            if (button != null && button.Enabled != bEnable)
                button.Enabled = bEnable;
        }

        public override void ImageIndex(object item, int index)
        {
            var button = item as ToolBarButton;
            if (button != null && button.ImageIndex != index)
                button.ImageIndex = index;
        }

        public override void Check(object item, bool bCheck)
        {
            var button = item as ToolBarButton;
            if (button == null) return;
            if (button.Style == ToolBarButtonStyle.ToggleButton && button.Pushed == bCheck) return;

            button.Style = ToolBarButtonStyle.ToggleButton;
            button.Pushed = bCheck;
        }

        // Execution event handler
        private void toolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs args)
        {
            try
            {
                var cmd = GetCommandForInstance(args.Button);

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
