using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Kesco.Lib.Win;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Error;

namespace Kesco.App.Win.DocView.Dialogs
{
    public partial class StampEditDialog : FreeDialog
    {
        private const int maxLen = 50;

        private int createStampID = -1;
        private int _stampId = -1;
        private Bitmap _stampImage;
        private bool edited;
        private bool close;

        public string StampName
        {
            get { return textName.Text.Trim(); }
        }

        /// <summary>
        ///   Редактируемое изображение
        /// </summary>
        public Image StampImage
        {
            get { return _stampImage ?? pictureStamp.Image; }

            set
            {
                if (_stampImage != null)
                    _stampImage.Dispose();

                pictureStamp.Image = _stampImage = new Bitmap(value);
                _stampImage.SetResolution(value.VerticalResolution, value.HorizontalResolution);
            }
        }

        /// <summary>
        ///   Конструктор диалоговой формы - создание
        /// </summary>
        public StampEditDialog()
        {
            // сразу добавим право на штамп создающему пользователю
            InitializeComponent();
            Text += " - " + Environment.StringResources.GetString("creating");
            textName.MaxLength = textNameEn.MaxLength = maxLen;
            CheckOK();
        }

        /// <summary>
        ///   Конструктор диалоговой формы - редактирование
        /// </summary>
        /// <param name="stampId"> код штампа </param>
        /// <param name="image"> изображение штампа </param>
        public StampEditDialog(int stampId, Image image)
        {
            _stampId = stampId;
            InitializeComponent();
            Text += " - " + Environment.StringResources.GetString("editing");
            textName.MaxLength = textNameEn.MaxLength = maxLen;
            pictureStamp.Image = image;
            var name = Environment.StampData.GetField(Environment.StampData.NameField, _stampId) as string;
            if (name == null)
            {
                close = true;
                return;
            }
            textName.Text = name;
            name = Environment.StampData.GetField(Environment.StampData.NameEnField, _stampId) as string;
            if (name == null)
            {
                close = true;
                return;
            }
            textNameEn.Text = name;
            edited = false;
            buttonReplace.Visible = _stampId < 0;
            CheckOK();
        }

        private void CheckOK()
        {
            btnOK.Enabled = StampImage != null && textName.Text.Trim().Length > 0 && textNameEn.Text.Trim().Length > 0;
        }

        private void textName_KeyPress(object sender, KeyPressEventArgs e)
        {
            CheckOK();
        }

        private void btnRights_Click(object sender, EventArgs e)
        {
            CheckNew(true);
            if (_stampId < 0)
                return;

            using (var dlg = new RightsListDialog(_stampId))
                dlg.ShowDialog(this);
        }

        /// <summary>
        ///   Получение байтов из изображения
        /// </summary>
        /// <param name="image"> изображение для извлечения байтов </param>
        /// <returns> массив байтов </returns>
        public static byte[] GetImageBytes(Image image)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    image.Save(stream, ImageFormat.Bmp);
                }
                catch (Exception ex)
                {
                    Lib.Win.Data.Env.WriteToLog(ex);
                }
                if (stream.Length > 0)
                {
                    var imageBytes = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(imageBytes, 0, (int) stream.Length);
                    return imageBytes;
                }
                return null;
            }
        }

        private void CheckNew(bool warning)
        {
            if (_stampId >= 0)
                return;
            string name = textName.Text.Trim();
            string nameEn = textNameEn.Text.Trim();
            if (name.Length < 1 || nameEn.Length < 1)
            {
                ErrorShower.OnShowError(null,
                                        Environment.StringResources.GetString(
                                            "StampEditDialog.CheckNew.ErrorMessage1"),
                                        Environment.StringResources.GetString("Warning"));
                return;
            }
            if (StampImage == null)
            {
                ErrorShower.OnShowError(null,
                                        Environment.StringResources.GetString(
                                            "StampEditDialog.CheckNew.ErrorMessage2"),
                                        Environment.StringResources.GetString("Warning"));
                return;
            }
            byte[] imagebyte = GetImageBytes(StampImage);
            if (imagebyte == null || imagebyte.Length < 10)
            {
                ErrorShower.OnShowError(null,
                                        Environment.StringResources.GetString(
                                            "StampEditDialog.CheckNew.ErrorMessage2"),
                                        Environment.StringResources.GetString("Warning"));
                return;
            }
            if (warning &&
                MessageBox.Show("Сохранить штамп?", Environment.StringResources.GetString("Confirmation"),
                                MessageBoxButtons.YesNoCancel) == DialogResult.No)
                return;
            if (createStampID < 1)
            {
                _stampId = Environment.StampData.AddStamp(imagebyte, name, nameEn);

                if (_stampId > 0)
                {
                    Environment.StampRightsData.SetStampRights(_stampId,
                                                               new List<StampRight>
                                                                   {new StampRight {UserId = Environment.CurEmp.ID}});
                    Environment.StampChecksData.SetStampRules(null, _stampId,
                                                              new StampRule {UserId = Environment.CurEmp.ID});
                }
            }
            else
            {
                if (Environment.StampData.SetStampImage(createStampID, imagebyte, name, nameEn))
                {
                    _stampId = createStampID;
                    buttonReplace.Visible = false;
                }
            }
        }

        private void textName_TextChanged(object sender, EventArgs e)
        {
            edited = true;
            CheckOK();
        }

        private void btnRules_Click(object sender, EventArgs e)
        {
            CheckNew(true);
            if (_stampId < 0)
                return;
            using (RulesListDialog dlg = new RulesListDialog(_stampId))
                dlg.ShowDialog(this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CheckNew(false);
            if (_stampId <= 0)
                return;
            if (edited)
            {
                string name = textName.Text.Trim();
                string nameEn = textNameEn.Text.Trim();
                if (name.Length < 1 || nameEn.Length < 1)
                {
                    ErrorShower.OnShowError(null,
                                            Environment.StringResources.GetString(
                                                "StampEditDialog.CheckNew.ErrorMessage1"),
                                            Environment.StringResources.GetString("Warning"));
                    return;
                }

                if (
                    !Environment.StampData.SetField(Environment.StampData.NameField, SqlDbType.NVarChar,
                                                    _stampId, name))
                    return;

                if (
                    !Environment.StampData.SetField(Environment.StampData.NameEnField, SqlDbType.VarChar,
                                                    _stampId, nameEn))
                    return;
            }
            End(DialogResult);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult);
        }

        private void StampEditDialog_Load(object sender, EventArgs e)
        {
            if (close)
                End(DialogResult.Cancel);
        }

        internal void SetEditStampID(int addedID)
        {
            try
            {
                createStampID = addedID;
                textName.Text = Environment.StampData.GetField(Environment.StampData.NameField, createStampID) as string;
                textNameEn.Text =
                    Environment.StampData.GetField(Environment.StampData.NameEnField, createStampID) as string;
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            try
            {
                var ssd = new SelectStampDialog(-1) {Text = "Выбирите штамп для замены изображения"};
                if (ssd.ShowDialog() == DialogResult.OK)
                    SetEditStampID(ssd.GetStampID());
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }
    }
}