using System;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView.Grids.Styles
{
    /// <summary>
    /// Стиль для отображения списка сканов
    /// </summary>
    public class OuterScanerStyle : ScanerStyle
    {
        /// <summary>
        /// Конструктор стиля для отображения списка сканов
        /// </summary>
        /// <param name="grid"></param>
        protected OuterScanerStyle(DocGrid grid)
            : base(grid)
        {
            FriendlyName = Environment.StringResources.GetString("OuterScaner");

            // Свойства изображения
            _imagePropertiesItem = new MenuItem {Text = Environment.StringResources.GetString("Properties")};
            _imagePropertiesItem.Click += ImagePropertiesItem_Click;
        }

        /// <summary>
        /// Событие изменения файла зображения
        /// </summary>
        internal event Action FileChanged;

        #region Private fields
        private static OuterScanerStyle outerInstance;
        private static DocGrid outerGrid;

        /// <summary>
        /// Кнопка меню свойств изображения
        /// </summary>
        private readonly MenuItem _imagePropertiesItem; 
        #endregion

        public new static Style Instance(DocGrid grid)
        {
            if (outerInstance == null || outerGrid != grid)
            {
                outerInstance = new OuterScanerStyle(grid);
                outerGrid = grid;
            }

            return outerInstance;
        }

        public static void Clear()
        {
            outerGrid = null;
            outerInstance = null;
        }

        /// <summary>
        /// Создание конеткстного меню сетки
        /// </summary>
        /// <returns></returns>
        public override ContextMenu BuildContextMenu()
        {
            var contextMenu = new ContextMenu();

            if (grid.IsSingle)
                contextMenu.MenuItems.AddRange(new[] {_imagePropertiesItem});

            return contextMenu;
        }

        public override bool IsShownInSettings()
        {
            return true;
        }

        /// <summary>
        /// Обработчик нажатия кнопки меню свойств изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagePropertiesItem_Click(object sender, EventArgs e)
        {
            string fullFileName = grid.GetValue(grid.CurrentRowIndex, Environment.ImageReader.FullNameField).ToString();

            var sDialog = new PropertiesDialogs.PropertiesScanDialog(fullFileName);

            sDialog.DialogEvent += PropertiesScanDialog_DialogEvent;
            sDialog.Show();
        }

        /// <summary>
        /// Обработчик события PropertiesScanDialog
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void PropertiesScanDialog_DialogEvent(object source, Lib.Win.DialogEventArgs e)
        {
            try
            {
                if (e.Dialog.DialogResult == DialogResult.OK)
                    InvokeFileChangedEvent();
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        /// <summary>
        /// Вызов события
        /// </summary>
        private void InvokeFileChangedEvent()
        {
            var handler = FileChanged;
            if (handler != null) handler();
        }
    }
}