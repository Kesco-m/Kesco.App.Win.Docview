using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.App.Win.DocView.Dialogs
{
    /// <summary>
    ///   Диалог для выбора штампа (факсимиле).
    /// </summary>
    public partial class SelectStampDialog : FreeDialog
    {
        private int imageID;
        private Dictionary<int, Image> _stampImages;

        /// <summary>
        ///   Конструктор
        /// </summary>
        public SelectStampDialog(int imageID)
        {
            InitializeComponent();

            this.imageID = imageID;

            // принудительно делаем imageListStamps квадратным
            imageListStamps.ImageSize = new Size(imageListStamps.ImageSize.Width, imageListStamps.ImageSize.Width);
        }

        /// <summary>
        ///   Загрузка формы
        /// </summary>
        private void SelectStamp_Load(object sender, EventArgs e)
        {
            buttonFromFile.Visible = Lib.Win.Document.Environment.IsDomainAdmin();
            LoadStamps();
        }

        private void LoadStamps()
        {
            if (Cursor != Cursors.WaitCursor)
                Cursor = Cursors.WaitCursor;
            // получаем список штампов, доступных пользователю
            var stamps = Environment.StampData.GetStampsList(imageID, imageID == -1);
            _stampImages = new Dictionary<int, Image>(stamps.Count);

            listViewStamps.Items.Clear();
            foreach (var stamp in stamps)
            {
                // получаем изображение штампа из БД
                byte[] imageBytes = stamp.Value;
                if (imageBytes == null)
                    continue;

                using (var ms = new MemoryStream(imageBytes))
                {
                    using (var stampImage = new Bitmap(ms))
                    {
                        var listBmp = new Bitmap(stampImage);
                        float h = stampImage.HorizontalResolution, v = stampImage.VerticalResolution;
                        listBmp.MakeTransparent();
                        listBmp.SetResolution(h, v);
                        _stampImages.Add(stamp.Key.ID, listBmp);
                        stampImage.Dispose();
                    }
                }

                // добавляем в контрол вместе с изображениями
                listViewStamps.Items.Add(
                    new ListViewItem(stamp.Key.Name)
                        {
                            ForeColor = (stamp.Key.CanInsert ? ForeColor : Color.DarkGray),
                            Tag = stamp.Key,
                            ImageIndex =
                                imageListStamps.Images.Add(
                                    MakeQuadrate(_stampImages[stamp.Key.ID], !stamp.Key.CanInsert), Color.Transparent),
                            ToolTipText =
                                stamp.Key.CanInsert
                                    ? stamp.Key.Name
                                    : Environment.StringResources.GetString("needSigns") + System.Environment.NewLine +
                                      stamp.Key.NeedSigns
                        });
            }
            if (Cursor != Cursors.Default)
                Cursor = Cursors.Default;
        }

        /// <summary>
        ///   Редактирование штампа
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            ListViewItem li = listViewStamps.SelectedItems[0];
            int stampId = ((StampAddItem) li.Tag).ID;

            using (var dlg = new StampEditDialog(stampId, _stampImages[stampId]))
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadStamps();
                    listViewStamps_SelectedIndexChanged(this, EventArgs.Empty);
                }
        }

        private void listViewStamps_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = 1 == listViewStamps.SelectedItems.Count;
            btnSelect.Enabled = btnEdit.Enabled &&
                                ((StampAddItem) listViewStamps.SelectedItems[0].Tag).CanInsert;
            btnDelete.Enabled = listViewStamps.SelectedItems.Count > 0;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
			if(MessageBox.Show( Environment.StringResources.GetString("SelectStampDialog_Message1")) ==  System.Windows.Forms.DialogResult.Yes)
            foreach (ListViewItem item in listViewStamps.SelectedItems)
            {
                if (Environment.StampData.SetField(Environment.StampData.DeleteField, SqlDbType.TinyInt,
                                                   ((StampAddItem) item.Tag).ID, 1))
                    listViewStamps.Items.Remove(item);
            }
        }

        private void listViewStamps_DoubleClick(object sender, EventArgs e)
        {
            if (btnSelect.Enabled)
                btnSelect.PerformClick();
        }

        /// <summary>
        ///   Делает из картинку квадратной по большей стороне, источник по центру
        /// </summary>
        private Image MakeQuadrate(Image source, bool gray)
        {
            // вычисляем сторону квадрата и место, в котором нарисуем исходный
            int edgeSize;
            int x, y;
            if (source.Height > source.Width)
            {
                edgeSize = source.Height;
                y = 0;
                x = (edgeSize - source.Width)/2;
            }
            else
            {
                edgeSize = source.Width;
                x = 0;
                y = (edgeSize - source.Height)/2;
            }

            var bitmap = new Bitmap(edgeSize, edgeSize);
            bitmap.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.White, 0, 0, edgeSize, edgeSize);
                if (gray)
                {
                    float[][] ptsArray = {
                                             new float[] {1, 0, 0, 0, 0},
                                             new float[] {0, 1, 0, 0, 0},
                                             new float[] {0, 0, 1, 0, 0},
                                             new[] {0, 0, 0, 0.2f, 0},
                                             new float[] {0, 0, 0, 0, 1}
                                         };
                    var cm = new ColorMatrix(ptsArray);
                    var imgAttribs = new ImageAttributes();
                    imgAttribs.SetColorMatrix(cm,
                                              ColorMatrixFlag.Default,
                                              ColorAdjustType.Default);
                    g.DrawImage(source, new Rectangle(x, y, source.Width, source.Height), 0, 0, source.Width,
                                source.Height, GraphicsUnit.Pixel, imgAttribs);
                }
                else
                    g.DrawImage(source, x, y, source.Width, source.Height);
            }

            return bitmap;
        }

        /// <summary>
        ///   Получение изображения выбранного штампа
        /// </summary>
        /// <returns> </returns>
        public Image GetSelectedStamp()
        {
            if (DialogResult.OK == DialogResult &&
                1 == listViewStamps.SelectedItems.Count)
            {
                Image stampImage =
                    _stampImages[((StampAddItem) listViewStamps.SelectedItems[0].Tag).ID];
                var bmp = new Bitmap(stampImage);
                bmp.SetResolution(stampImage.HorizontalResolution, stampImage.VerticalResolution);
                return bmp;
            }

            return null;
        }

        public int GetStampID()
        {
            return DialogResult.OK == DialogResult && 1 == listViewStamps.SelectedItems.Count
                       ? ((StampAddItem) listViewStamps.SelectedItems[0].Tag).ID
                       : 0;
        }

        private void buttonFromFile_Click(object sender, EventArgs e)
        {
            var openImageFileDialog = new OpenFileDialog
                                          {
                                              DefaultExt = "bmp",
                                              Filter = "BMP|*.bmp",
                                              Title = "Выберите файл с изображением"
                                          };
            if (openImageFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var bmp = new Bitmap(openImageFileDialog.FileName))
                    if (bmp != null)
                    {
                        var dlg = new StampEditDialog {StampImage = bmp};
                        dlg.DialogEvent += dlg_DialogEvent;
                        ShowSubForm(dlg);
                    }
            }
        }

        private void dlg_DialogEvent(object source, DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.OK)
                LoadStamps();
            listViewStamps.Select();
            listViewStamps.Focus();
        }
    }
}