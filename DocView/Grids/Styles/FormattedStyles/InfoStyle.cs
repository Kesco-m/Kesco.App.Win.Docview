using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles
{
	/// <summary>
	/// Стиль для отображения списка сообщений по документу
	/// </summary>
	public class InfoStyle : FormattedStyle
	{
		#region Constructor & Instance

		protected internal InfoStyle(InfoGrid grid) : base(grid)
		{
			Lib.Win.Data.DALC.Documents.MessageDALC messageData = new Lib.Win.Data.DALC.Documents.MessageDALC(null);
			keyField = messageData.DateMessageField;
			this.grid.ColumnHeadersVisible = false;
			this.grid.GridColor = SystemColors.Window;

			needBoldField = null;
			needColorField = messageData.ReadField;
			needUnderlineField = messageData.TitleField;
		}

		public static Style Instance(InfoGrid grid)
		{
			return new InfoStyle(grid);
		}

		#endregion

		#region Columns

		public override int GetColumnWidth(string colName)
		{
			if(colName == Environment.MessageData.EmployeesField)
				return 80;

			if(colName == Environment.MessageData.DateMessageField)
				return 160;

			return 0;
		}

		public override bool IsColumnSystem(string column)
		{
			return (column != Environment.MessageData.EmployeesField && column != Environment.MessageData.DateMessageField);
		}

		public override bool IsColumnVisible(string column)
		{
			return !IsColumnSystem(column);
		}

		#endregion

		#region Format

		protected override void FormatColor(DataGridViewCellPaintingEventArgs e)
		{
			object objTime = grid.GetValue(e.RowIndex, Environment.MessageData.DateField);
			object objID = grid.GetValue(e.RowIndex, "КодСотрудникаОтправителя");
			bool read = grid.GetBoolValue(e.RowIndex, Environment.MessageData.ReadField);
			bool time = false;

			if(objTime is DateTime)
				time = ((DateTime)objTime > grid.ImageTime);
			if(objID is int && Environment.CurEmp.ID != (int)objID)
				e.CellStyle.ForeColor = Color.MediumBlue;

			if(!time && read)
				e.CellStyle.ForeColor = Color.Silver;
			else
			{
				if(read)
				{
					if(objID is int && Environment.CurEmp.ID != (int)objID)
						e.CellStyle.ForeColor = Color.CornflowerBlue;
					else
						e.CellStyle.ForeColor = Color.DimGray;
				}
				if(!time)
					e.CellStyle.ForeColor = Color.Silver;
			}
		}

		#endregion

		public override void Init()
		{
			int index = 2;
			foreach(ColParam col in colParams.Values.Where(col => IsColumnSystem(col.Name)))
			{
				index++;
				col.Index = index;
				col.Visible = false;
			}

			colParams.GetParam(Environment.MessageData.DateMessageField).Index = 1;
			colParams.GetParam(Environment.MessageData.EmployeesField).Index = 0;
			colParams.GetParam(Environment.MessageData.DateMessageField).Visible = true;
			colParams.GetParam(Environment.MessageData.EmployeesField).Visible = true;
			base.Init();
			colParams.WasInited = false;
		}

		public override bool ReloadData(DataTable dt)
		{
			try
			{
				grid.DataSource = dt.DefaultView;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
				return false;
			}
			finally
			{
				grid.ResumeLayout();
			}
			return true;
		}

		public override bool IsInfo()
		{
			return true;
		}
	}
}