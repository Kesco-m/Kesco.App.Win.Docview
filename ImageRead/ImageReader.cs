using System;
using System.Data;
using System.IO;
using Kesco.Lib.Win.Error;

namespace Kesco.App.Win.DocView.ImageRead
{
    /// <summary>
    ///   Читает список картинок в указанной папке
    /// </summary>
    public class ImageReader
    {
        protected const string dateField = "Дата";
        protected const string changedDateField = "ДатаИзменения";
        protected const string nameField = "Имя";
        protected const string typeField = "Тип";
        protected const string fullNameField = "ПолноеИмя";
        protected const string descrField = "Описание";

        private string path;

        public ImageReader(string path)
        {
            this.path = path;
        }

        #region Accessors

        public string DateField
        {
            get { return dateField; }
        }

        public string ChangedDateField
        {
            get { return changedDateField; }
        }

        public string NameField
        {
            get { return nameField; }
        }

        public string TypeField
        {
            get { return typeField; }
        }

        public string FullNameField
        {
            get { return fullNameField; }
        }

        public string DescrField
        {
            get { return descrField; }
        }

        #endregion

        public DataTable GetImages(DateTime minDate)
        {
            var table = new DataTable();
            AddColumns(table);

            try
            {
                var dir = new DirectoryInfo(path);

                Console.WriteLine("{0}: Dir get files", DateTime.Now.ToString("HH:mm:ss fff"));

                FileInfo[] files = dir.GetFiles();

                Console.WriteLine("{0}: Beginning cycle", DateTime.Now.ToString("HH:mm:ss fff"));

                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo f = files[i];

                    Console.Write("File " + i + "/" + files.Length + ": " + f.Name + ".");
                    Console.Write("ext.");

                    if ((ImageType(f.Extension) != null))
                    {
                        Console.Write("date.");

                        if (f.CreationTime >= minDate)
                        {
                            Console.Write("row.");

                            DataRow row = table.NewRow();

                            Console.Write("fill.");

                            FillRow(row, f);

                            Console.Write("pos.");

                            int insPos = -1;
                            for (int j = 0; j < table.Rows.Count; j++)
                            {
                                var jDate = (DateTime) table.Rows[j][DateField];
                                var nDate = (DateTime) row[DateField];
                                if (DateTime.Compare(jDate, nDate) <= 0)
                                {
                                    insPos = j;
                                    break;
                                }
                            }

                            Console.Write("add.");

                            if (insPos != -1)
                                table.Rows.InsertAt(row, insPos);
                            else
                                table.Rows.Add(row);
                        }
                    }
                    Console.WriteLine("{0}: done", DateTime.Now.ToString("HH:mm:ss fff"));
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
                ErrorShower.OnShowError(null, ex.Message, "");
            }

            return table;
        }

        public DataTable GetImages()
        {
            return GetImages(DateTime.MinValue);
        }

        internal virtual string ImageType(string ext)
        {
            try
            {
                switch (ext.Replace(".", "").ToLower())
                {
                    case "pdf":
                        return "PDF";
                    case "bmp":
                        return "BMP";

                    case "jpg":
                    case "jpe":
                    case "jpeg":
                        return "JPEG";

                    case "gif":
                        return "GIF";

                    case "pcx":
                        return "PCX";

                    case "png":
                        return "PNG";

                    case "tif":
                    case "tiff":
                        return "TIFF";
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
            return null;
        }

        protected virtual void AddColumns(DataTable t)
        {
            t.Columns.Add(NameField);
            t.Columns.Add(DateField, typeof (DateTime));
            t.Columns.Add(TypeField);
            t.Columns.Add(FullNameField);
        }

        protected virtual void FillRow(DataRow row, FileInfo f)
        {
            row[NameField] = f.Name;
            row[DateField] = f.CreationTimeUtc;
            row[TypeField] = ImageType(f.Extension);
            row[FullNameField] = f.FullName;
        }
    }
}