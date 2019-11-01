using System;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Document.Controls;

namespace Kesco.App.Win.DocView.PropertiesDialogs.FaxItem
{
    public partial class FaxPropertyEdiorControl : FaxPropertyControl
    {
        public FaxPropertyEdiorControl()
        {
            InitializeComponent();
        }

        private new HoverLinkLabel labelValue;

        public FaxPropertyEdiorControl(string propertyName, string propertyValue, Int32 ID)
            : base(propertyName, propertyValue)
        {
            if (ID <= 0)
                return;
            try
            {
                labelValue = new HoverLinkLabel(FindForm())
                {
                    Dock = DockStyle.Fill,
                    Location = new Point(128, 0),
                    Name = "labelValue",
                    AutoSize = true,
                    Size = new Size(160, 16),
                    TabIndex = 1,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Url = Lib.Win.Document.Environment.UsersURL + ID,
                    Text = propertyValue,
                    Caption = string.Format("№{0} {1}", ID, propertyValue)
                };

                Controls.Add(labelValue);
                labelValue.BringToFront();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }
    }
}