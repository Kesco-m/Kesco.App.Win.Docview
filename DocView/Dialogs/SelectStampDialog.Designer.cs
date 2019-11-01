using System.Drawing;
using System.Collections.Generic;

namespace Kesco.App.Win.DocView.Dialogs
{
	partial class SelectStampDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if(components != null)
					components.Dispose();
				if (_stampImages != null)
					foreach (KeyValuePair<int, Image> stamp in _stampImages)
						stamp.Value.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectStampDialog));
			this.listViewStamps = new System.Windows.Forms.ListView();
			this.imageListStamps = new System.Windows.Forms.ImageList(this.components);
			this.btnSelect = new System.Windows.Forms.Button();
			this.btnEdit = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.buttonFromFile = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listViewStamps
			// 
			resources.ApplyResources(this.listViewStamps, "listViewStamps");
			this.listViewStamps.LargeImageList = this.imageListStamps;
			this.listViewStamps.MultiSelect = false;
			this.listViewStamps.Name = "listViewStamps";
			this.listViewStamps.ShowGroups = false;
			this.listViewStamps.ShowItemToolTips = true;
			this.listViewStamps.UseCompatibleStateImageBehavior = false;
			this.listViewStamps.SelectedIndexChanged += new System.EventHandler(this.listViewStamps_SelectedIndexChanged);
			this.listViewStamps.DoubleClick += new System.EventHandler(this.listViewStamps_DoubleClick);
			// 
			// imageListStamps
			// 
			this.imageListStamps.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this.imageListStamps, "imageListStamps");
			this.imageListStamps.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// btnSelect
			// 
			resources.ApplyResources(this.btnSelect, "btnSelect");
			this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.UseVisualStyleBackColor = true;
			// 
			// btnEdit
			// 
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// btnDelete
			// 
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// buttonFromFile
			// 
			resources.ApplyResources(this.buttonFromFile, "buttonFromFile");
			this.buttonFromFile.Name = "buttonFromFile";
			this.buttonFromFile.UseVisualStyleBackColor = true;
			this.buttonFromFile.Click += new System.EventHandler(this.buttonFromFile_Click);
			// 
			// SelectStampDialog
			// 
			this.AcceptButton = this.btnSelect;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ControlBox = false;
			this.Controls.Add(this.buttonFromFile);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnSelect);
			this.Controls.Add(this.listViewStamps);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectStampDialog";
			this.ShowIcon = false;
			this.Load += new System.EventHandler(this.SelectStamp_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewStamps;
		private System.Windows.Forms.ImageList imageListStamps;
		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button buttonFromFile;
	}
}