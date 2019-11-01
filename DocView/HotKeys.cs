using System;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Select;

namespace Kesco.App.Win.DocView
{
    /// <summary>
    ///   Клавиши, участвующие в сочетаниях
    /// </summary>
    internal enum HotKeyModifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

	public class HotKeys
	{
		/// <summary>
		///   Зарегистрировать сочетания горячих клавиш
		/// </summary>
		/// <param name="handle"> Дескриптор окна, который будет получать и обрабатывать сообщения </param>
		public static void Register(IntPtr handle)
		{
			try
			{
				Lib.Win.Document.Win32.User32.RegisterHotKey(handle, 1, (int)HotKeyModifier.Control, Keys.F8.GetHashCode());
				Lib.Win.Document.Win32.User32.RegisterHotKey(handle, 2, (int)HotKeyModifier.None, Keys.F1.GetHashCode());
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		/// <summary>
		///   Удалить зарегистрированные сочетания горячих клавиш
		/// </summary>
		/// <param name="handle"> Дескриптор окна, который получал и обрабатывал сообщения </param>
		public static void UnRegister(IntPtr handle)
		{
			try
			{
				Lib.Win.Document.Win32.User32.UnregisterHotKey(handle, 1);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		/// <summary>
		///   Если обработчик получил сигнал о нажатии горячих клавих, отправить параметр сообщения на обработку и выполнение команд
		/// </summary>
		/// <param name="m_WParam"> поле WParam сообщения </param>
		public static void DoWork(IntPtr m_WParam)
		{
			try
			{
				Form aForm = Form.ActiveForm;
				if(aForm == null)
					return;
				switch(m_WParam.ToInt32())
				{
					case 1:
						if(Form.ActiveForm == null)
							return;
						if(Environment.PreviosTypeID > 0)
						{
							string url =
								Environment.DocTypeData.GetField(Environment.DocTypeData.URLField,
																 Environment.PreviosTypeID).ToString();
							string paramStr = ((Lib.Win.Document.Environment.PersonID > 0) ? ("&currentperson=" + Lib.Win.Document.Environment.PersonID.ToString()) : "") +
											  (Environment.PreviosDirection.Equals(SelectTypeDialog.Direction.Out) ? "&docDir=out"
												   : (Environment.PreviosDirection.Equals(SelectTypeDialog.Direction.In) ? "&docDir=in" : ""));
							if(string.IsNullOrEmpty(url))
								throw new Exception("Нет формы создания документа");

							if(paramStr.Length > 0)
							{
								url += (url.IndexOf("?") > 0) ? "&" : "?";
								url += paramStr.TrimStart("&".ToCharArray());
							}
							Lib.Win.Document.Environment.IEOpenOnURL(url);
						}
						else
							Environment.CmdManager.Commands["AddDocData"].Execute();
						break;
					case 2:
						Control cntr = Environment.FindControlAtPoint(aForm, aForm.PointToClient(Cursor.Position));
						if(cntr != null && cntr.Tag != null && cntr.Tag is string && !string.IsNullOrEmpty(cntr.Tag.ToString()))
							Help.ShowPopup(cntr, cntr.Tag.ToString(), Cursor.Position);
						else
						{
							string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DocViewHELP.chm");
							if(System.IO.File.Exists(path))
							{
								if(Environment.HelpTopics.ContainsKey(aForm.GetType()))
									Help.ShowHelp(Program.hide, path, HelpNavigator.Topic, Environment.HelpTopics[aForm.GetType()]);
								else
									Help.ShowHelp(Program.hide, path, HelpNavigator.Topic, Environment.HelpTopics[typeof(Forms.MainFormDialog)]);
							}
						}
						break;
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}
	}
}