using System.Drawing;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Grids.Styles.FormattedStyles
{
	/// <summary>
	/// базовый стиль для отображения списка с форматированием ячейки
	/// </summary>
	public class FormattedStyle : Style
	{
		/// <summary>
		/// базовый конструктор
		/// </summary>
		/// <param name="grid"></param>
		protected FormattedStyle(Grid grid) : base(grid)
		{
		}

		#region Format

		/// <summary>
		/// переформатирование ячейки с изменением шрифта
		/// </summary>
		/// <param name="e"></param>
		public override void FormatGridCells(DataGridViewCellPaintingEventArgs e)
		{
			e.CellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
			if(needBoldField != null)
				FormatBold(e);

			if(needColorField != null)
				FormatColor(e);

			if(needUnderlineField != null)
				FormatUnderline(e);
		}

		/// <summary>
		/// проверка и изменение на жирный шрифт
		/// </summary>
		/// <param name="e"></param>
		protected virtual void FormatBold(DataGridViewCellPaintingEventArgs e)
		{
			if(!grid.GetBoolValue(e.RowIndex, needBoldField))
				e.CellStyle.Font = new Font(e.CellStyle.Font.Name, e.CellStyle.Font.Size, FontStyle.Bold);
		}

		/// <summary>
		/// проверка и изменениея цвета шрифта
		/// </summary>
		/// <param name="e"></param>
		protected virtual void FormatColor(DataGridViewCellPaintingEventArgs e)
		{

		}

		/// <summary>
		/// проверка и изменение шрифта на курсив
		/// </summary>
		/// <param name="e"></param>
		protected virtual void FormatUnderline(DataGridViewCellPaintingEventArgs e)
		{
			if(grid.GetBoolValue(e.RowIndex, needUnderlineField))
				e.CellStyle.Font = new Font(e.CellStyle.Font.Name, e.CellStyle.Font.Size, FontStyle.Italic);
		}

		#endregion
	}
}