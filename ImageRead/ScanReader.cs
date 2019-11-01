using System;
using System.Data;
using System.IO;
using Kesco.Lib.Win.Document;

namespace Kesco.App.Win.DocView.ImageRead
{
	public class ScanReader : ImageReader
	{
		public ScanReader(string path)
			: base(path)
		{
		}

		protected override void AddColumns(DataTable t)
		{
			try
			{
				t.Columns.Add(dateField, typeof(DateTime));
				t.Columns.Add(changedDateField, typeof(DateTime));
				t.Columns.Add(descrField);
				t.Columns.Add(fullNameField);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		protected override void FillRow(DataRow row, FileInfo f)
		{
			try
			{
				ScanInfo info = TextProcessor.ParseScanInfo(f);
				if(info != null)
				{
					row[dateField] = info.Date;
					row[descrField] = info.Descr;
				}
				else
				{
					row[dateField] = f.CreationTime;
					row[descrField] = Path.GetFileNameWithoutExtension(f.Name);
				}
				row[changedDateField] = f.LastWriteTime;
				row[fullNameField] = f.FullName;
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		internal override string ImageType(string ext)
		{
			try
			{
				switch(ext.Replace(".", "").ToLower())
				{
					case "pdf":
						return "PDF";
					case "tif":
						return "TIF";
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
			return null;
		}
	}
}