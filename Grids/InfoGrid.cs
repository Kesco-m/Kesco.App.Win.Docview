using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC;

namespace Kesco.App.Win.DocView.Grids
{
	public class InfoGrid : Grid
	{
		protected Lib.Win.Document.Classes.CommandBackgroundWorker loader;

		#region Variables

		private object _oldCellValue;
		public int DocID { get; private set; }

		#endregion

		public InfoGrid()
		{
			InitializeComponent();
			SelectionMode = DataGridViewSelectionMode.CellSelect;
			AllowUserToResizeColumns = true;
			DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;

			LostFocus += InfoGrid_LostFocus;
			CellDoubleClick += InfoGrid_CellDoubleClick;
			CellEndEdit += InfoGrid_CellEndEdit;
			SizeChanged += Grid_SizeChanged;

			loader = new Lib.Win.Document.Classes.CommandBackgroundWorker() { WorkerSupportsCancellation = true };
			loader.DoWork += loader_DoWork;
			loader.RunWorkerCompleted += loader_RunWorkerCompleted;
		}

		protected override void OnSelectionChanged(EventArgs e)
		{
			//((DataGridView)base).OnSelectionChanged(e);
		}

		protected override void Grid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.DataPropertyName = e.Column.Name;
			e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
			if(e.Column.Visible)
				e.Column.MinimumWidth = Style.GetColumnWidth(e.Column.Name);
		}

		protected void InfoGrid_LostFocus(object sender, EventArgs e)
		{
			ClearSelection();
			Cursor = Cursors.Default;
			Invalidate();
		}

		public override void SetSort(string value)
		{
		}

		public override string GetSort()
		{
			return string.Empty;
		}

		#region EditCell

		protected void InfoGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			RealCellEndEdit(e.ColumnIndex, e.RowIndex);
		}

		private void RealCellEndEdit(int col, int row)
		{
			this[col, row].Value = _oldCellValue;
			ReadOnly = true;
			InvalidateCell(col, row);
		}

		protected void InfoGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if(IsFine)
				if(Columns[e.ColumnIndex] is DataGridViewTextBoxColumn)
				{
					try
					{
						_oldCellValue = this[e.ColumnIndex, e.RowIndex].Value;
					}
					catch
					{
						return;
					}

					ReadOnly = false;
					BeginEdit(false);
				}
		}

		#endregion

		#region Is

		public bool IsInfo()
		{
			return (Style != null && Style.IsInfo());
		}

		#endregion

		#region Load

		public void LoadInfo(int docID)
		{
			Cursor = Cursors.WaitCursor;

			DocID = docID;

			if(IsCurrentCellInEditMode)
				RealCellEndEdit(CurrentColumnIndex, CurrentRowIndex);

			if(IsFine && GetDataView() != null && (docID == 0 || DocID != docID))
				GetDataView().Table.Rows.Clear();

			loader.CancelAsync();

			if(Style == null)
				Style = new Styles.FormattedStyles.InfoStyle(this);
			else
				Style.Save();
			if(docID > 0)
			{
				if(!loader.IsBusy)
					loader.RunWorkerAsync();
				else
                    Console.WriteLine("{0}: LoadInfo Worker not ready", DateTime.Now.ToString("HH:mm:ss fff"));
			}

			if(docID == 0)
				Cursor = Cursors.Default;
		}

		private void loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if(!loader.CancellationPending && !e.Cancelled)
			{
				if(e.Result is DataTable)
				{
					if(IsFine && GetDataView() != null)
						Style.ReloadData(e.Result as DataTable);
					else
						Style.LoadData(e.Result as DataTable);

					ClearSelection();
				}
			}
			else if(DocID > 0)
				loader.RunWorkerAsync();

			Cursor = Cursors.Default;
		}

		private void loader_DoWork(object _sender, DoWorkEventArgs e)
		{
			if(e.Result != null)
				try
				{
					using(var dtt = e.Result as DataTable)
					{
						if(dtt != null)
							dtt.Dispose();
					}
					e.Result = null;
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}

			using(DataTable dt = Environment.MessageData.GetDocMessages(DocID, out loader.Cmd))
			{
				var it = new DataTable();

				if(loader.CancellationPending)
					return;

				// adding columns
				// employees
				it.Columns.Add(new DataColumn(Environment.MessageData.EmployeesField) { DataType = Type.GetType("System.String") });

				// dataMessage
				it.Columns.Add(new DataColumn(Environment.MessageData.DateMessageField) { DataType = Type.GetType("System.String") });

				// title
				it.Columns.Add(new DataColumn(Environment.MessageData.TitleField) { DataType = Type.GetType("System.Boolean") });

				// read
				it.Columns.Add(new DataColumn(Environment.MessageData.ReadField) { DataType = Type.GetType("System.Boolean") });

				// origialEmployees
				it.Columns.Add(new DataColumn(Environment.MessageData.OriginalEmployeesField) { DataType = Type.GetType("System.String") });

				// originalDataMessage
				it.Columns.Add(new DataColumn(Environment.MessageData.OriginalDateMessageField) { DataType = Type.GetType("System.String") });

				// originalDataMessage
				it.Columns.Add(new DataColumn(Environment.MessageData.DateField) { DataType = Type.GetType("System.DateTime") });

				// direction
				it.Columns.Add(new DataColumn(Environment.MessageData.DirectionField) { DataType = Type.GetType("System.Int32") });

				if(loader.CancellationPending)
					return;

				string oldSender = null;
				DateTime oldSent = DateTime.MinValue;
				int lastSenderRowIndex = -1;

				using(DataTableReader dr = dt.CreateDataReader())
					while(!loader.CancellationPending && dr.Read())
					{
						var employeeSenderID = (Int32)dr[Environment.MessageData.DirectionField];
						var sender = (string)dr[Environment.MessageData.SenderField];
						var recipients = (string)dr[Environment.MessageData.RecipientsField];
						var sent = (DateTime)dr[Environment.MessageData.SentField];
						var message = (string)dr[Environment.MessageData.NameField];
						object obj = dr[Environment.MessageData.ReadField];
						bool read = !obj.Equals(DBNull.Value);

						if(sender != oldSender || sent != oldSent)
						{
							// inserting empty line
							if(it.Rows.Count > 0)
								it.Rows.Add(new object[] { "", "", false, true, "", "", DateTime.MinValue, 0 });

							string dateStr = sent.AddHours(LocalObject.GetTimeDiff().Hours).ToString();
							it.Rows.Add(new object[] { sender, dateStr, true, true, sender, dateStr, sent, employeeSenderID });

							lastSenderRowIndex = it.Rows.Count - 1;
						}

						it.Rows.Add(new object[] { recipients, message, false, read, recipients, message, sent, employeeSenderID });
						if(!read)
							it.Rows[lastSenderRowIndex][3] = read;
					}
				if(loader.CancellationPending)
				{
					it.Dispose();
					it = null;
				}

				e.Result = it;
			}
		}

		#endregion

		private void Grid_SizeChanged(object sender, EventArgs e)
		{
			if(IsCurrentCellInEditMode)
				RealCellEndEdit(CurrentColumnIndex, CurrentRowIndex);
			AutoResizeColumns();
			AutoResizeRows();
		}

		private void InitializeComponent()
		{
			((ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// InfoGrid
			// 
			this.CellBorderStyle = DataGridViewCellBorderStyle.None;
			this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			this.RowTemplate.Height = 18;
			((ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);

		}
	}
}