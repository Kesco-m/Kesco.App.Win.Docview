using System;
using Kesco.Lib.Log;

namespace Kesco.App.Win.DocView.Grids.Styles
{
	public class ColParam
	{
		public int Index = -1;
		public int Width = 100;
		public bool Visible = true;
		public string Name = string.Empty;
		public string HeaderName = string.Empty;
		public Type DataType = typeof(string);
		public bool IsKeyField = false;
		public bool IsSystemField = false;
		public bool ToRemove = false;

		public ColParam(string name, string value)
		{
			Name = name;
			string[] s = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			try
			{
                // Заявка №27393
                // Ошибка парсинга
                // Проявляется, если value == ""
				if(s!= null && s.Length > 4)
				{
					int.TryParse(s[0], out Index);
					int.TryParse(s[1], out Width);
					bool.TryParse(s[2], out Visible);

					bool.TryParse(s[3], out IsKeyField);
					bool.TryParse(s[4], out IsSystemField);

					HeaderName = s[5];
				}
				else
					HeaderName = name;
			}
			catch(Exception ex)
			{
				HeaderName = name;

                Logger.Exception(ex.Message, ex);
			}
		}

		public ColParam(string name, string headerName, string value)
			: this(name, value)
		{
			HeaderName = headerName;
		}

		public void LoadValuesFrom(ColParam colParam)
		{
			Name = colParam.Name;
			HeaderName = colParam.HeaderName;
			Index = colParam.Index;
			Width = colParam.Width;
			Visible = colParam.Visible;
			DataType = colParam.DataType;
		}
	}
}