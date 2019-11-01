using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Data.DALC.Documents;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document;
using Kesco.Lib.Win.Document.Blocks;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Document.Dialogs;

namespace Kesco.App.Win.DocView.PropertiesDialogs
{
	public class PropertiesDBDocDialog : FreeDialog
	{
		private int typeID;

		private int originalTypeID;
		private string originalName;
		private string originalNumber;
		private DateTime originalDate;
		private bool originalProtectedDoc;
		private string originalDescription;
		private Dictionary<int, int> originalPersons;

        private int _docCreator;
        private DateTime _docCreateDate;
		private int _editorId;
		private DateTime _editedDate = DateTime.MinValue;

		private int sType = -1;

		private bool typeEnabled = true;
		private bool typeNameEnable = true;
		private bool numberEnabled = true;
		private bool dateEnabled = true;
		private bool descrEnabled = true;
		private TextBox description;
		private Button buttonEdit;
		private Button buttonOK;
		private Button buttonCancel;
		private IContainer components;
		private Label label1;
		private TextBox number;
		private GroupBox groupBoxPersons;
		private GroupBox groupBoxDocument;
		private Label label2;
		private Label label3;
		private GroupBox groupBox1;
		private Button buttonLook;
		private Label label5;
		private Label lblEditor;
		private Label lblEdited;
		private PersonBlock personBlock;
		private DocTypeBlock docTypeBlock;
		private CheckBox checkBoxProtected;
		private CheckBox checkBoxNoNumber;
		private GroupBox groupBox2;
		private TextBox textBoxName;
		private CheckBox checkBoxName;
		private ToolTip toolTip1;
		private DateBlock dateBlock;
		private NewWindowDocumentButton newWindowDocumentButton;

        private readonly HoverLinkLabel _linkDocAuthor;
		private readonly HoverLinkLabel _linkEditor;
        private Label lblDocAurtor;
        private Label lblDocCreateDate;

		private Container component;

		public PropertiesDBDocDialog(int docId)
		{
			InitializeComponent();
			DoubleBuffered = true;

			DocID = docId;

			_linkEditor = new HoverLinkLabel(this)
				{
					AutoSize = true,
					Location = new Point(lblEditor.Right, lblEditor.Location.Y),
					Size = new Size(lblEdited.Location.X - 3 - Location.X, lblEditor.Height)
				};
			Controls.Add(_linkEditor);

            _linkDocAuthor = new HoverLinkLabel(this)
                {
                    AutoSize = true,
                    Location = new Point(lblDocAurtor.Right, lblDocAurtor.Location.Y),
                    Size = new Size(lblDocAurtor.Location.X - 3 - Location.X, lblDocAurtor.Height)
                };
            Controls.Add(_linkDocAuthor);

			dateBlock.RaiseDateBlockEvent += HandleDateBlockEvent;
		}

		#region Accessors

		public bool AddImage { get; private set; }

		public int DocID { get; private set; }

		#endregion

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesDBDocDialog));
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.number = new System.Windows.Forms.TextBox();
			this.groupBoxDocument = new System.Windows.Forms.GroupBox();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.checkBoxNoNumber = new System.Windows.Forms.CheckBox();
			this.docTypeBlock = new Kesco.Lib.Win.Document.Blocks.DocTypeBlock();
			this.dateBlock = new Kesco.Lib.Win.Document.Blocks.DateBlock();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxName = new System.Windows.Forms.CheckBox();
			this.description = new System.Windows.Forms.TextBox();
			this.groupBoxPersons = new System.Windows.Forms.GroupBox();
			this.personBlock = new Kesco.Lib.Win.Document.Blocks.PersonBlock();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonLook = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.lblEditor = new System.Windows.Forms.Label();
			this.lblEdited = new System.Windows.Forms.Label();
			this.checkBoxProtected = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.newWindowDocumentButton = new Kesco.Lib.Win.Document.Controls.NewWindowDocumentButton(this.components);
			this.lblDocAurtor = new System.Windows.Forms.Label();
			this.lblDocCreateDate = new System.Windows.Forms.Label();
			this.groupBoxDocument.SuspendLayout();
			this.groupBoxPersons.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonEdit
			// 
			resources.ApplyResources(this.buttonEdit, "buttonEdit");
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// number
			// 
			resources.ApplyResources(this.number, "number");
			this.number.Name = "number";
			// 
			// groupBoxDocument
			// 
			this.groupBoxDocument.Controls.Add(this.textBoxName);
			this.groupBoxDocument.Controls.Add(this.checkBoxNoNumber);
			this.groupBoxDocument.Controls.Add(this.docTypeBlock);
			this.groupBoxDocument.Controls.Add(this.dateBlock);
			this.groupBoxDocument.Controls.Add(this.number);
			this.groupBoxDocument.Controls.Add(this.label1);
			this.groupBoxDocument.Controls.Add(this.label2);
			this.groupBoxDocument.Controls.Add(this.label3);
			this.groupBoxDocument.Controls.Add(this.checkBoxName);
			resources.ApplyResources(this.groupBoxDocument, "groupBoxDocument");
			this.groupBoxDocument.Name = "groupBoxDocument";
			this.groupBoxDocument.TabStop = false;
			// 
			// textBoxName
			// 
			resources.ApplyResources(this.textBoxName, "textBoxName");
			this.textBoxName.Name = "textBoxName";
			this.toolTip1.SetToolTip(this.textBoxName, resources.GetString("textBoxName.ToolTip"));
			// 
			// checkBoxNoNumber
			// 
			resources.ApplyResources(this.checkBoxNoNumber, "checkBoxNoNumber");
			this.checkBoxNoNumber.Name = "checkBoxNoNumber";
			this.checkBoxNoNumber.UseVisualStyleBackColor = true;
			this.checkBoxNoNumber.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// docTypeBlock
			// 
			this.docTypeBlock.ID = 0;
			resources.ApplyResources(this.docTypeBlock, "docTypeBlock");
			this.docTypeBlock.Name = "docTypeBlock";
			this.docTypeBlock.Selected += new Kesco.Lib.Win.Document.Blocks.BlockEventHandler(this.docTypeBlock_Selected);
			this.docTypeBlock.DocTypeTextChanged += new System.EventHandler(this.docTypeBlock_DocTypeTextChanged);
			// 
			// dateBlock
			// 
			resources.ApplyResources(this.dateBlock, "dateBlock");
			this.dateBlock.Name = "dateBlock";
			this.dateBlock.Leave += new System.EventHandler(this.dateBlock_Leave);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// checkBoxName
			// 
			resources.ApplyResources(this.checkBoxName, "checkBoxName");
			this.checkBoxName.Name = "checkBoxName";
			this.toolTip1.SetToolTip(this.checkBoxName, resources.GetString("checkBoxName.ToolTip"));
			this.checkBoxName.UseVisualStyleBackColor = true;
			this.checkBoxName.CheckedChanged += new System.EventHandler(this.checkBoxName_CheckedChanged);
			// 
			// description
			// 
			resources.ApplyResources(this.description, "description");
			this.description.Name = "description";
			// 
			// groupBoxPersons
			// 
			this.groupBoxPersons.Controls.Add(this.personBlock);
			resources.ApplyResources(this.groupBoxPersons, "groupBoxPersons");
			this.groupBoxPersons.Name = "groupBoxPersons";
			this.groupBoxPersons.TabStop = false;
			// 
			// personBlock
			// 
			this.personBlock.Able = true;
			resources.ApplyResources(this.personBlock, "personBlock");
			this.personBlock.Name = "personBlock";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.buttonLook);
			this.groupBox1.Controls.Add(this.label5);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// buttonLook
			// 
			resources.ApplyResources(this.buttonLook, "buttonLook");
			this.buttonLook.Name = "buttonLook";
			this.buttonLook.Click += new System.EventHandler(this.buttonLook_Click);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// lblEditor
			// 
			resources.ApplyResources(this.lblEditor, "lblEditor");
			this.lblEditor.Name = "lblEditor";
			// 
			// lblEdited
			// 
			resources.ApplyResources(this.lblEdited, "lblEdited");
			this.lblEdited.Name = "lblEdited";
			// 
			// checkBoxProtected
			// 
			resources.ApplyResources(this.checkBoxProtected, "checkBoxProtected");
			this.checkBoxProtected.Name = "checkBoxProtected";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.description);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// newWindowDocumentButton
			// 
			resources.ApplyResources(this.newWindowDocumentButton, "newWindowDocumentButton");
			this.newWindowDocumentButton.Name = "newWindowDocumentButton";
			this.newWindowDocumentButton.TabStop = false;
			this.newWindowDocumentButton.UseMnemonic = false;
			this.newWindowDocumentButton.UseVisualStyleBackColor = true;
			// 
			// lblDocAurtor
			// 
			resources.ApplyResources(this.lblDocAurtor, "lblDocAurtor");
			this.lblDocAurtor.Name = "lblDocAurtor";
			// 
			// lblDocCreateDate
			// 
			resources.ApplyResources(this.lblDocCreateDate, "lblDocCreateDate");
			this.lblDocCreateDate.Name = "lblDocCreateDate";
			// 
			// PropertiesDBDocDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.lblDocAurtor);
			this.Controls.Add(this.lblDocCreateDate);
			this.Controls.Add(this.newWindowDocumentButton);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.checkBoxProtected);
			this.Controls.Add(this.lblEditor);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBoxPersons);
			this.Controls.Add(this.groupBoxDocument);
			this.Controls.Add(this.buttonEdit);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.lblEdited);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertiesDBDocDialog";
			this.Load += new System.EventHandler(this.DocProperties_Load);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PropertiesDBDocDialog_KeyUp);
			this.groupBoxDocument.ResumeLayout(false);
			this.groupBoxDocument.PerformLayout();
			this.groupBoxPersons.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private void UpdateControls()
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(UpdateControls));
				return;
			}
			if(Disposing || IsDisposed)
				return;

			bool editMode = buttonOK.Enabled;
			try
			{
				docTypeBlock.Enabled = editMode && typeEnabled;
				checkBoxName.Enabled = editMode && typeNameEnable;
				textBoxName.ReadOnly = !editMode || !typeNameEnable || !checkBoxName.Checked;
				if(editMode && !typeNameEnable)
				{
					checkBoxName.Checked = false;
					textBoxName.Text = "";
				}
				number.ReadOnly = !editMode || !numberEnabled || checkBoxNoNumber.Checked;
				dateBlock.Enabled = editMode && dateEnabled;
				description.ReadOnly = !(editMode && descrEnabled);
				checkBoxProtected.Enabled = editMode && numberEnabled;
				personBlock.Able = editMode;
				checkBoxNoNumber.Enabled = editMode && numberEnabled;

				newWindowDocumentButton.UnSet();
				newWindowDocumentButton.Set(DocID);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private void buttonEdit_Click(object sender, EventArgs e)
		{
			buttonOK.Enabled = true;
			buttonEdit.Enabled = false;

			UpdateControls();
		}

		private void HandleDateBlockEvent(object sender, DateBlockEventArgs d)
		{
			personBlock.HandleDateBlockEvent(sender, d);
		}

		private void DocProertiesPersons_Load(DateTime date)
		{
			using(DataTable dt = Environment.DocData.GetDocPersons(DocID, date))
			using(DataTableReader dr = dt.CreateDataReader())
			{
				if(dr.HasRows)
				{
					originalPersons = new Dictionary<int, int>();
					while(dr.Read())
					{
						var personId = (int)dr[Environment.PersonData.IDField];
						var person = (string)dr[Environment.PersonData.NameField];
						var position = (byte)dr[Environment.DocData.PersonPositionField];
						bool isValid = 0 < (int)dr[Environment.DocData.PersonIsValidField];

						if(!originalPersons.ContainsKey(personId))
							originalPersons.Add(personId, position);
						else if(originalPersons[personId] < position)
							originalPersons[personId] = position;
						if(!string.IsNullOrEmpty(person))
							personBlock.AddPerson(personId, person, position, isValid);
					}
				}
				dr.Close();
				dr.Dispose();
				dt.Dispose();
			}
		}

		private void DocProperties_Load(object sender, EventArgs e)
		{
            if (DesignerDetector.IsComponentInDesignMode(this))
                return;

			groupBoxPersons.Text = Environment.PersonWord.GetForm(Cases.I, true, true);

			if(DocID != 0)
			{
				Text += " #" + DocID;

				DataRow row = Environment.DocData.GetDocProperties(DocID, Environment.CurCultureInfo.TwoLetterISOLanguageName);

				if(row != null)
				{
					// description
					description.Text = row[Environment.DocData.DescriptionField] as string ?? string.Empty;
					originalDescription = description.Text;

					// type id
					object obj = row[Environment.DocData.DocTypeIDField];
					if(obj is int)
					{
						typeID = (int)obj;
						docTypeBlock.Text = row[Environment.DocData.DocTypeField].ToString();
						docTypeBlock.ID = typeID;
					}

					originalTypeID = typeID;

					// название
					originalName = row[Environment.DocData.NameField] as string ?? "";
					checkBoxName.Checked = !string.IsNullOrEmpty(originalName);
					textBoxName.ReadOnly = true;

					textBoxName.Text = originalName.Trim();

					// number
					originalNumber = row[Environment.DocData.NumberField] as string;
					if(string.IsNullOrEmpty(originalNumber))
					{
						if(originalNumber == null)
							originalNumber = "";
						checkBoxNoNumber.Checked = true;
						number.ReadOnly = true;
					}
					else
					{
						number.Text = originalNumber;
						checkBoxNoNumber.Checked = false;
					}

					// date
					obj = row[Environment.DocData.DateField];
					if(obj is DateTime)
					{
						dateBlock.Value = (DateTime)obj;
						originalDate = dateBlock.Value;
					}

					// protected
					obj = row[Environment.DocData.ProtectedField];
					if(obj is byte)
						checkBoxProtected.Checked = obj.Equals((byte)1);
					originalProtectedDoc = checkBoxProtected.Checked;

					// persons
					DocProertiesPersons_Load(dateBlock.Value);

                    #region Создал, редактировал

                    // Создал
                    obj = row[Environment.DocData.CreatorField];

				    if (obj is int)
				        _docCreator = (int) obj;

				    if (_docCreator > 0)
				    {
                        var cr = new Employee(_docCreator, Environment.EmpData);

                        _linkDocAuthor.Url = Lib.Win.Document.Environment.UsersURL + cr.ID;
                        _linkDocAuthor.Text = Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? cr.ShortName : cr.ShortEngName;
                        _linkDocAuthor.Caption = string.Format("№{0} {1}", cr.ID, _linkDocAuthor.Text);
				    }

				    // Дата создания
                    obj = row[Environment.DocData.CreateDateField];
				    if (obj is DateTime)
				        _docCreateDate = (DateTime) obj;

				    if (_docCreateDate > DateTime.MinValue)
				        lblDocCreateDate.Text += _docCreateDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");

				    // Изменил
					obj = row[Environment.DocData.EditorField];
				    if (obj is int)
				        _editorId = (int) obj;

					// Дата последнего изменения
					obj = row[Environment.DocData.EditedField];
				    if (obj is DateTime)
				        _editedDate = (DateTime) obj;

                    // Если отличается автор и редактор либо отличается дата создания от даты редактирования
                    // То показываем поля изменил, изменено на форме
				    if (_editorId != _docCreator || _editedDate != _docCreateDate)
				    {
                        lblEdited.Visible = true;
                        lblEditor.Visible = true;

				        if (_editorId > 0)
                        {
                            var ed = new Employee(_editorId, Environment.EmpData);

                            _linkEditor.Url = Lib.Win.Document.Environment.UsersURL + ed.ID;
                            _linkEditor.Text = Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? ed.ShortName : ed.ShortEngName;
                            _linkEditor.Caption = string.Format("№{0} {1}", ed.ID, _linkEditor.Text);
                        }

				        if (_editedDate > DateTime.MinValue)
				            lblEdited.Text += _editedDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
                    }
                    else
                    {
                        lblEdited.Visible = false;
                        lblEditor.Visible = false;
                    }

				    #endregion

                    // Проверяем, какие поля доступны для изменения
					//////////////////////////////////////////////////

					bool eformSigned;
					bool docSigned;
					Environment.DocSignatureData.IsSigns(DocID, out eformSigned, out docSigned);
					if(docSigned || eformSigned)
					{
						typeEnabled = false;
						typeNameEnable = false;
						numberEnabled = false;
						dateEnabled = false;
						//descrEnabled = false;
					}
					else if(Environment.DocDataData.IsDataPresent(DocID))
					{
						typeEnabled = false;
					}

					if(typeNameEnable)
						typeNameEnable = row[Environment.DocTypeData.NameExistField].Equals((byte)1);

					newWindowDocumentButton.Set(DocID, row);
				}
			}
			else
			{
				Text = Environment.StringResources.GetString("DocProperties.DocProperties_Load.Message1");
				buttonEdit_Click(sender, e);
				buttonOK.Text = Environment.StringResources.GetString("Save");
			}

			UpdateControls();
		}

		void dateBlock_Leave(object sender, EventArgs e)
		{
			if(!dateBlock.IsEmpty() && dateBlock.Check())
			{
				
			}
		}


		private void buttonOK_Click(object sender, EventArgs e)
		{
			typeID = docTypeBlock.ID;
			if(typeID == 0)
			{
				docTypeBlock.SelectDialog();
				return;
			}

			string nameStr = "";
			if(typeNameEnable)
				nameStr = textBoxName.Text.Trim();

            if ((!checkBoxName.Checked || nameStr.Length == 0) && typeNameEnable && !(_editorId == Environment.CurEmp.ID && typeID == originalTypeID))
			{
				var dialog = new CheckTypeDialog();
                dialog.SetStartPosition(this, 0, 60);
                if (dialog.ShowDialog() != DialogResult.Yes)
				{
					checkBoxName.Checked = true;
					textBoxName.Focus();
					return;
				}

				nameStr = "";
			}

			DateTime docDate = originalDate;

			if(dateBlock.IsEmpty())
			{
				docDate = default(DateTime);
			}
			else
			{
				if(!dateBlock.Check())
				{
					MessageBox.Show(
						dateBlock.Error + "." + System.Environment.NewLine +
						Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message1") + ".",
						Environment.StringResources.GetString("InputError"));
					dateBlock.Focus();
					return;
				}

                if (dateBlock.Value > DateTime.Now.Date && !(_editorId == Environment.CurEmp.ID && dateBlock.Value == originalDate))
                {
                    var dialog = new CheckDateDialog();
                    dialog.SetStartPosition(this, 0, 60);
                    if (dialog.ShowDialog() != DialogResult.Yes)
                    {
                        dateBlock.Focus();
                        return;
                    }
                }

				docDate = dateBlock.Value;
			}

			string numberStr = number.Text;
			bool protectedDoc = checkBoxProtected.Checked;
			string descrStr = description.Text;

			int personCount = personBlock.Count;

			if(personCount == 0)
			{
				MessageBox.Show(
					Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message2") +
					Environment.PersonWord.GetForm(Cases.R, false, false),
					Environment.StringResources.GetString("InputError"));
				return;
			}

			string personIDs = personBlock.PersonIDs.Aggregate("", (current, t) => current + (((current.Length > 0) ? "," : "") + t));

			byte type = docTypeBlock.SearchType;
			if(sType != -1)
				type = (byte)sType;
			// проверяем, есть ли похожий документ уже
			DataTable dt = Environment.DocData.GetSimilarDocs(typeID, numberStr, docDate, personIDs, DocID, -1);
			if(dt.Rows.Count > 0)
			{
				Dialogs.SimilarDocsDialog dialog = new Dialogs.SimilarDocsDialog(dt, typeID, docTypeBlock.Text, nameStr,
												   numberStr, docDate, protectedDoc,
												   personIDs, DocID, descrStr, personCount) { SearchType = type };
				dialog.DialogEvent += SimilarDocsDialog_DialogEvent;
				ShowSubForm(dialog);
				return;
			}

			CheckAndSave(docDate, nameStr, numberStr, protectedDoc, descrStr, personCount);
		}

		private void CheckAndSave(DateTime docDate, string nameStr, string numberStr, bool protectedDoc, string descrStr,
								  int personCount)
		{
            // Проверка соответствия типов документа контрагентам (резидент/нерезидент -> счет/инвойс)
            string personIDs = personBlock.PersonIDs.Aggregate("", (current, t) => current + (((current.Length > 0) ? "," : "") + t));
			if(!string.IsNullOrEmpty(personIDs))
			{
				bool ret1 = typeID == DocTypeDALC.TYPE_ID_SCHET || typeID == DocTypeDALC.TYPE_ID_SCHET_FACTURA;
				bool ret2 = typeID == DocTypeDALC.TYPE_ID_INVOICE || typeID == DocTypeDALC.TYPE_ID_INVOICE_PROFORMA;
				int ret = 0;
				if(ret1 && (ret = Lib.Win.Document.Environment.PersonData.CheckPersonsCountry(personIDs, ret1)) != Lib.Win.Data.DALC.Directory.PersonDALC.AREA_CODE_RUSSIAN_FEDERATION && ret != 0)
				{
					// для типов счёт/счёт-фактура только резиденты
					var checkCountryDialog = new CheckTypeDialogCountry();
					checkCountryDialog.Controls["lblConfirmationDescription"].Text = String.Format("{0}", Lib.Win.Document.Environment.StringResources.GetString("CheckTypeDialogCountry_OnlyRussianCompany"));
					checkCountryDialog.Controls["lblConfirmationQuestion"].Text += String.Format(" {0}", Lib.Win.Document.Environment.DocTypeData.GetDocType(typeID, Lib.Win.Document.Environment.CurCultureInfo.TwoLetterISOLanguageName));
					checkCountryDialog.ShowDialog();
					if(checkCountryDialog.DialogResult == DialogResult.No)
						return;
				}
				else if(ret2 && Lib.Win.Document.Environment.PersonData.CheckPersonsCountry(personIDs, ret2) == 0)
				{
					// для типов инвойс/инвойс-проформа одна из компаний должна быть нерезидентом
					var checkCountryDialog = new CheckTypeDialogCountry();
					checkCountryDialog.Controls["lblConfirmationDescription"].Text = String.Format("{0}", Lib.Win.Document.Environment.StringResources.GetString("CheckTypeDialogCountry_MustHaveNotResident"));
					checkCountryDialog.Controls["lblConfirmationQuestion"].Text += String.Format(" {0}", Lib.Win.Document.Environment.DocTypeData.GetDocType(typeID, Lib.Win.Document.Environment.CurCultureInfo.TwoLetterISOLanguageName));
					checkCountryDialog.ShowDialog();
					if(checkCountryDialog.DialogResult == DialogResult.No)
						return;
				}
			}

			if((typeID != originalTypeID) || (nameStr != originalName) || (numberStr != originalNumber) ||
				(docDate != originalDate) || (descrStr != originalDescription) || (protectedDoc != originalProtectedDoc))
			{
				if(!checkBoxNoNumber.Checked && numberStr != originalNumber)
				{
					if(string.IsNullOrEmpty(numberStr))
					{
						if(
							MessageBox.Show(
								Lib.Win.Document.Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning1"),
								Lib.Win.Document.Environment.StringResources.GetString("InputError"),
								MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) ==
							DialogResult.No)
						{
							number.Focus();
							return;
						}

						checkBoxNoNumber.Checked = true;
						numberStr = "";
						number.Text = "";
					}
					else if(!Regex.IsMatch(numberStr, @"\d+"))
					{
						switch((new NumberConfirmDialog(numberStr)).ShowDialog())
						{
							case DialogResult.No:

								checkBoxNoNumber.Checked = true;
								numberStr = "";
								number.Text = "";
								break;
							case DialogResult.Yes:

								number.Focus();
								return;
						}
					}
				}

				if(!dateBlock.IsEmpty())
					docDate = dateBlock.Value;

				if(personCount == 1)
				{
					if(MessageBox.Show(
						Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message6") +
						Environment.PersonWord.GetForm(Cases.V, false, false) +
						"." + System.Environment.NewLine +
						Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message7") +
						Environment.PersonWord.GetForm(Cases.R, true, false) +
						"." + System.Environment.NewLine + System.Environment.NewLine +
						Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message8") +
						Environment.PersonWord.GetForm(Cases.T, false, false) + "?",
						Environment.PersonWord.GetForm(Cases.I, true, true), MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
						return;
				}

				SaveDoc(false, 0, 0, nameStr, numberStr, docDate, protectedDoc, descrStr, personCount);
			}
			else
			{
				if(personCount == 1)
				{
					if(MessageBox.Show(
						Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message6") +
						Environment.PersonWord.GetForm(Cases.V, false, false) +
						"." + System.Environment.NewLine +
						Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message7") +
						Environment.PersonWord.GetForm(Cases.R, true, false) +
						"." + System.Environment.NewLine + System.Environment.NewLine +
						Environment.StringResources.GetString("DocProperties.buttonOK_Click.Message8") +
						Environment.PersonWord.GetForm(Cases.T, false, false) + "?",
						Environment.PersonWord.GetForm(Cases.I, true, true), MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
						return;
				}

				if(SavePerson(personCount))
				{
					End(DialogResult.OK);
				}
			}
		}

		private void SimilarDocsDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			Enabled = true;
			Focus();
			sType = -1;

			var dialog = e.Dialog as Dialogs.SimilarDocsDialog;
			if(dialog == null)
				return;

			switch(dialog.DialogResult)
			{
				case DialogResult.Cancel:
					return;

				case DialogResult.OK:
					SaveDoc(true, dialog.MainDocID, dialog.SecDocID, dialog.NameStr, dialog.NumberStr, dialog.DocDate,
							dialog.ProtectedDoc, dialog.DescrStr, dialog.PersonCount);
					break;
				default:
					CheckAndSave(dialog.DocDate, dialog.NameStr, dialog.NumberStr, dialog.ProtectedDoc, dialog.DescrStr,
								 dialog.PersonCount);
					break;
			}
		}

		private void SaveDoc(bool merge, int mainDocID, int secDocID, string nameStr, string numberStr, DateTime docDate,
							 bool protectedDoc, string descrStr, int personCount)
		{
			DateTime newEditedDate = DateTime.MinValue;
			string newEditorName = "";

			// checking changes
			DataRow row = Environment.DocData.GetDocProperties(DocID,
															   Environment.CurCultureInfo.TwoLetterISOLanguageName);

			// editor
			object obj = row[Environment.DocData.EditorField];
			if(obj is int)
			{
				int newEditorID = (int)obj;
				var newEd = new Employee(newEditorID, Environment.EmpData);
				newEditorName = Environment.CurCultureInfo.Name.StartsWith("ru") ? newEd.ShortName : newEd.ShortEngName;
			}

			// edited
			obj = row[Environment.DocData.EditedField];
			if(obj is DateTime)
				newEditedDate = (DateTime)obj;

			bool save = true;

			if(_editedDate < newEditedDate)
				save = (MessageBox.Show(Environment.StringResources.GetString("DocProperties.SaveDoc.Message1") + "."
										+ System.Environment.NewLine + System.Environment.NewLine +
										Environment.StringResources.GetString("Change") + ": " + newEditorName + System.Environment.NewLine +
										Environment.StringResources.GetString("DateChange") + ": " + newEditedDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss") +
										System.Environment.NewLine + System.Environment.NewLine +
										Environment.StringResources.GetString("DocProperties.SaveDoc.Message2"),
										Environment.StringResources.GetString("DocProperties.SaveDoc.Title1"),
										MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
										MessageBoxDefaultButton.Button2) == DialogResult.Yes);

			if(!save)
				return;

			if(!merge || mainDocID.Equals(DocID))
			{
				if(save = (Environment.DocData.SetDocProperties(DocID, typeID, nameStr, numberStr, docDate, protectedDoc, descrStr) > 0))
				{
					save = SavePerson(personCount);
				}
			}

			if(!save)
				return;

			if(merge)
				Environment.DocData.DeleteDoc(mainDocID, secDocID, true);
			else
				Environment.UndoredoStack.Add("EditDocProp", Environment.StringResources.GetString("EditDocProp"),
								   Environment.StringResources.GetString("UndoEditDocProp") + " [" + DocID.ToString() + "]",
								   Environment.StringResources.GetString("RedoEditDocProp") + " [" + DocID.ToString() + "]",
								   UndoRedoCommands.EditDocProp,
									   new object[13]
                                        {
                                            DocID, originalTypeID, originalName, originalNumber, originalDate,
                                            originalProtectedDoc, originalDescription, typeID, nameStr, numberStr,
											docDate, protectedDoc, descrStr
                                        }, Environment.CurEmp.ID);

			End(DialogResult.OK);
		}

		private bool SavePerson(int personCount)
		{
			bool save = true;
			if(personCount > 0)
			{
				int[] personIDs = personBlock.PersonIDs;
				var pIDs = personIDs.Where(t => originalPersons == null || (originalPersons != null && (!originalPersons.ContainsKey(t) || originalPersons[t] == 0))).ToList();

				if(pIDs.Count > 0 || personIDs.Length != originalPersons.Count)
				{
					save = Environment.DocData.SetDocPersons(DocID, pIDs.ToArray());
					if(save)
						Environment.UndoredoStack.Add("AddPerson", Environment.StringResources.GetString("EditPersons"),
													  Environment.StringResources.GetString("CancelEditPersons") + " [" + DocID.ToString() + "]",
													  Environment.StringResources.GetString("RedoEditPersons") + " [" + DocID.ToString() + "]",
													  UndoRedoCommands.AddPerson,
													  new object[]  {  DocID,  originalPersons != null ? originalPersons.Keys.ToArray() : null, pIDs.ToArray() },
													  Environment.CurEmp.ID);
				}
			}
			return save;
		}

		private void buttonLook_Click(object sender, EventArgs e)
		{
			DocUserListDialog dialog = new DocUserListDialog(DocID);
			ShowSubForm(dialog);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if(checkBoxNoNumber.Checked)
				number.Text = "";
			number.ReadOnly = checkBoxNoNumber.Checked;
		}

		private void checkBoxName_CheckedChanged(object sender, EventArgs e)
		{
			textBoxName.ReadOnly = !typeNameEnable || !checkBoxName.Checked;
		}

		private void docTypeBlock_DocTypeTextChanged(object sender, EventArgs e)
		{
			typeNameEnable = docTypeBlock.ID > 0 && Environment.DoesDocTypeNameExist(docTypeBlock.ID);
			UpdateControls();
			if(!typeNameEnable)
				textBoxName.Text = "";
		}

		private void docTypeBlock_Selected(object source, BlockEventArgs e)
		{
			typeNameEnable = docTypeBlock.ID > 0 && Environment.DoesDocTypeNameExist(docTypeBlock.ID);
			UpdateControls();
			if(!typeNameEnable)
				textBoxName.Text = "";
		}

		private void PropertiesDBDocDialog_KeyUp(object sender, KeyEventArgs e)
		{
			newWindowDocumentButton.ProcessKey(e.KeyData);
		}
	}
}