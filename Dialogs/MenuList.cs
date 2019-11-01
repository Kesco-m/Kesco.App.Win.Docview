using System;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Kesco.App.Win.DocView.Misc;

namespace Kesco.App.Win.DocView.Dialogs
{
	public class MenuList : Lib.Win.FreeDialog
	{
		private int maxHeight;
		private string zoom;
		private Lib.Win.Document.Controls.MenuList _menuList = null;
		private Label labelNo;
		private Button buttonAdd;
		private IContainer components;

		public MenuList(int DocID, Rectangle rect, string zoom)
		{
			if(DocID <= 0)
				return;

			DocumentID = DocID;
			this.zoom = zoom;
			InitializeComponent();
			_menuList.DeleteLink += MenuListDeleteLink;

			_menuList.DocID = DocumentID;
			_menuList.CheckLinkDoc = Environment.CheckLinkDoc;
			var screenRect = Screen.FromRectangle(rect).WorkingArea;
			int count = _menuList.GetLinksCount();
			Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), screenRect);
			if(count > 0)
			{
				// если количество элементов больше 7 пересчитываем размеры
				if(count > 7)
				{
					//максимальный размер вниз
					int dy1 = screenRect.Bottom - rect.Bottom;
					// максимальный размер вверх
					int dy2 = rect.Y - 1;
					// если можем выпадаем вниз
					if(dy1 > 16 * (count - 7) + _menuList.Size.Height)
						// выпадаем вниз на расчетную длину
						Size = new Size(_menuList.Size.Width, _menuList.Size.Height + 16 * (count - 7));
					else
					{
						// если размеры вверх больше размера вниз выпадаем вверх
						if(dy1 < dy2)
						{
							Size = dy2 > 16 * (count - 7) + _menuList.Size.Height
									   ? new Size(_menuList.Size.Width, _menuList.Size.Height + 16 * (count - 7))
									   : new Size(_menuList.Size.Width, dy2);
						}
						else
						{
							// выпадаем вниз на максимальную длину
							Size = new Size(_menuList.Size.Width, dy1 - 1);
						}
					}
				}
				else
					// открываем в размер конторола
					Size = _menuList.Size;
			}
			else
			{
				_menuList.Visible = false;
				labelNo.Visible = true;
				Size = new Size(labelNo.Size.Width + 2, labelNo.Size.Height + 2);
			}
			maxHeight = count > 7 ? Size.Height : screenRect.Height;
			int locationX = (screenRect.Right - rect.X - Size.Width > 0 || screenRect.Width / 2 > rect.X - screenRect.X)
								? rect.X
								: (screenRect.Right - Size.Width - 1);
			int locationY = (screenRect.Bottom - rect.Y - Size.Height - rect.Height > 0 ||
							 screenRect.Height / 2 > rect.Y + rect.Height - screenRect.Y)
								? (rect.Y + rect.Height + 3)
								: (rect.Y - Size.Height);
			Location = new Point(locationX, locationY);
			MaximizedBounds = new Rectangle(new Point(0, 0), new Size(screenRect.Width, maxHeight));
			MaximumSize = MaximizedBounds.Size;
		}

		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			var resources = new System.Resources.ResourceManager(typeof(MenuList));
			this._menuList = new Lib.Win.Document.Controls.MenuList();
			this.labelNo = new System.Windows.Forms.Label();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// menuList
			// 
			this._menuList.AccessibleDescription = resources.GetString("_menuList.AccessibleDescription");
			this._menuList.AccessibleName = resources.GetString("_menuList.AccessibleName");
			this._menuList.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
			this._menuList.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("_menuList.Anchor")));
			this._menuList.AutoScroll = ((bool)(resources.GetObject("_menuList.AutoScroll")));
			this._menuList.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("_menuList.AutoScrollMargin")));
			this._menuList.AutoScrollMinSize =
				((System.Drawing.Size)(resources.GetObject("_menuList.AutoScrollMinSize")));
			this._menuList.BackColor = System.Drawing.SystemColors.ControlLight;
			this._menuList.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_menuList.BackgroundImage")));
			this._menuList.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("_menuList.Dock")));
			this._menuList.Enabled = ((bool)(resources.GetObject("_menuList.Enabled")));
			this._menuList.Font = ((System.Drawing.Font)(resources.GetObject("_menuList.Font")));
			this._menuList.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("_menuList.ImeMode")));
			this._menuList.Location = ((System.Drawing.Point)(resources.GetObject("_menuList.Location")));
			this._menuList.Name = "_menuList";
			this._menuList.NoClose = false;
			this._menuList.RightToLeft =
				((System.Windows.Forms.RightToLeft)(resources.GetObject("_menuList.RightToLeft")));
			this._menuList.Size = ((System.Drawing.Size)(resources.GetObject("_menuList.Size")));
			this._menuList.TabIndex = ((int)(resources.GetObject("_menuList.TabIndex")));
			this._menuList.Visible = ((bool)(resources.GetObject("_menuList.Visible")));
			this._menuList.Zoom = "100";
			this._menuList.TreeMouseUp += new System.Windows.Forms.MouseEventHandler(this.menuList_TreeMouseUp);
			this._menuList.TreeKeysUp += new System.Windows.Forms.KeyEventHandler(this.menuList_TreeKeysUp);
			// 
			// labelNo
			// 
			this.labelNo.AccessibleDescription = resources.GetString("labelNo.AccessibleDescription");
			this.labelNo.AccessibleName = resources.GetString("labelNo.AccessibleName");
			this.labelNo.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelNo.Anchor")));
			this.labelNo.AutoSize = ((bool)(resources.GetObject("labelNo.AutoSize")));
			this.labelNo.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelNo.Dock")));
			this.labelNo.Enabled = ((bool)(resources.GetObject("labelNo.Enabled")));
			this.labelNo.Font = ((System.Drawing.Font)(resources.GetObject("labelNo.Font")));
			this.labelNo.Image = ((System.Drawing.Image)(resources.GetObject("labelNo.Image")));
			this.labelNo.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelNo.ImageAlign")));
			this.labelNo.ImageIndex = ((int)(resources.GetObject("labelNo.ImageIndex")));
			this.labelNo.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelNo.ImeMode")));
			this.labelNo.Location = ((System.Drawing.Point)(resources.GetObject("labelNo.Location")));
			this.labelNo.Name = "labelNo";
			this.labelNo.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelNo.RightToLeft")));
			this.labelNo.Size = ((System.Drawing.Size)(resources.GetObject("labelNo.Size")));
			this.labelNo.TabIndex = ((int)(resources.GetObject("labelNo.TabIndex")));
			this.labelNo.Text = resources.GetString("labelNo.Text");
			this.labelNo.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelNo.TextAlign")));
			this.labelNo.Visible = ((bool)(resources.GetObject("labelNo.Visible")));
			// 
			// buttonAdd
			// 
			this.buttonAdd.AccessibleDescription = resources.GetString("buttonAdd.AccessibleDescription");
			this.buttonAdd.AccessibleName = resources.GetString("buttonAdd.AccessibleName");
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonAdd.Anchor")));
			this.buttonAdd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonAdd.BackgroundImage")));
			this.buttonAdd.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonAdd.Dock")));
			this.buttonAdd.Enabled = ((bool)(resources.GetObject("buttonAdd.Enabled")));
			this.buttonAdd.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonAdd.FlatStyle")));
			this.buttonAdd.Font = ((System.Drawing.Font)(resources.GetObject("buttonAdd.Font")));
			this.buttonAdd.Image = ((System.Drawing.Image)(resources.GetObject("buttonAdd.Image")));
			this.buttonAdd.ImageAlign =
				((System.Drawing.ContentAlignment)(resources.GetObject("buttonAdd.ImageAlign")));
			this.buttonAdd.ImageIndex = ((int)(resources.GetObject("buttonAdd.ImageIndex")));
			this.buttonAdd.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonAdd.ImeMode")));
			this.buttonAdd.Location = ((System.Drawing.Point)(resources.GetObject("buttonAdd.Location")));
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.RightToLeft =
				((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonAdd.RightToLeft")));
			this.buttonAdd.Size = ((System.Drawing.Size)(resources.GetObject("buttonAdd.Size")));
			this.buttonAdd.TabIndex = ((int)(resources.GetObject("buttonAdd.TabIndex")));
			this.buttonAdd.Text = resources.GetString("buttonAdd.Text");
			this.buttonAdd.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonAdd.TextAlign")));
			this.buttonAdd.Visible = ((bool)(resources.GetObject("buttonAdd.Visible")));
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// MenuList
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.ControlBox = false;
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.labelNo);
			this.Controls.Add(this._menuList);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "MenuList";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.ShowInTaskbar = false;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new EventHandler(MenuList_Load);
			this.ResumeLayout(false);
		}

		private void MenuList_Load(object sender, EventArgs e)
		{
			if(this.Size.Height > maxHeight)
			{
				this.Size = new Size(this.Size.Width, maxHeight);
				this.MaximumSize = new Size(this.MaximumSize.Width, maxHeight);
			}
		}

		#endregion

		#region Accessors

		public int DocumentID { get; private set; }

		#endregion

		protected override void WndProc(ref Message m)
		{
			try
			{
				if(m.Msg == 6 && (int)m.WParam == 0 && !_menuList.NoClose)
					Lib.Win.Document.Win32.User32.PostMessage(Handle, 16, IntPtr.Zero, IntPtr.Zero);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex, m.ToString());
			}
			base.WndProc(ref m);
		}

		private void menuList_TreeMouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)
				return;
			int linkedDocID = _menuList.GetLinkedDocID();
			if(linkedDocID <= 0)
				return;
			Environment.NewWindow(linkedDocID, zoom, new Context(ContextMode.Catalog));
			_menuList.NoClose = true;
			System.Timers.Timer timer = new System.Timers.Timer(200) { AutoReset = false };
			timer.Elapsed += timer_Elapsed;
			timer.Start();
		}

		private void menuList_TreeKeysUp(object sender, KeyEventArgs e)
		{
			if(e.Modifiers != Keys.None || e.KeyData != Keys.Enter)
				return;
			int linkedDocID = _menuList.GetLinkedDocID();
			if(linkedDocID <= 0)
				return;
			Environment.NewWindow(linkedDocID, zoom, new Context(ContextMode.Catalog));
			_menuList.NoClose = true;
			Close();
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			End(DialogResult.Abort);
		}

		private void MenuListDeleteLink(int parentDocID, int childDocID)
		{
			Environment.UndoredoStack.Add("RemoveLink", Environment.StringResources.GetString("RemoveLink"),
										  Environment.StringResources.GetString("RemoveLink"),
										  Environment.StringResources.GetString("RemoveLink"),
										  Lib.Win.Document.UndoRedoCommands.RemoveLink,
										  new object[] { parentDocID, childDocID }, Environment.CurEmp.ID);
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(Disposing || IsDisposed)
				return;

			if(InvokeRequired)
				Invoke(new ElapsedEventHandler(timer_Elapsed), new[] { sender, e });
			else
				Close();
		}
	}
}