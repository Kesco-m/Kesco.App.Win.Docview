using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Kesco.App.Win.DocView.Interface;
using Kesco.App.Win.DocView.Misc;
using Kesco.Lib.Win.Data.DALC.Documents.Search;

namespace Kesco.App.Win.DocView.Forms
{
	/// <summary>
	/// форма поиска
	/// </summary>
	public class XmlSearchForm : Lib.Win.Document.Search.OptionsDialog
	{
		private bool canClose;
		private bool linkDoc;

		public XmlSearchForm(int docID) : this()
		{
			try
			{
				DocID = docID;

				using(DataTable dt = Environment.DocData.GetDocPersonsLite(docID, false))
				using(DataTableReader dr = dt.CreateDataReader())
				{
					if(dr.HasRows)
					{
						XmlElement child = xml.CreateElement("Option");
						if(xml.DocumentElement != null)
							xml.DocumentElement.AppendChild(child);
						string temp = "";
						while(dr.Read())
							temp += (temp.Length > 0 ? "," : "") + dr[Environment.PersonData.IDField];

						child.SetAttribute("name", "ЛицаКонтрагенты");
						child.SetAttribute("value", temp);
						child.SetAttribute("mode", "and");
					}
					dr.Close();
					dr.Dispose();
					dt.Dispose();
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		public XmlSearchForm(int docID, bool linkDoc) : this(docID)
		{
			this.linkDoc = linkDoc;
		}

		/// <summary>
		/// Конструктор. Поиск документов и создания связей.
		/// </summary>
		/// <param name="docId"></param>
		/// <param name="linkDoc"></param>
		/// <param name="xml"></param>
		public XmlSearchForm(int docId, bool linkDoc, string xml) : this(xml)
		{
			DocID = docId;
			this.linkDoc = linkDoc;
		}

		public XmlSearchForm()
		{
			StartPosition = FormStartPosition.CenterScreen;
		}

		public XmlSearchForm(string xml) : base(xml)
		{
			StartPosition = FormStartPosition.CenterScreen;
		}

		public XmlSearchForm(string xml, EnabledFeatures features) : base(xml, features)
		{
			StartPosition = FormStartPosition.CenterScreen;
		}

		public XmlSearchForm(XmlDocument xml) : base(xml.OuterXml)
		{
			StartPosition = FormStartPosition.CenterScreen;
		}

		public XmlSearchForm(int id, EnabledFeatures features) : base(id, features)
		{
			StartPosition = FormStartPosition.CenterScreen;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if(DialogResult == DialogResult.OK)
			{
				if(!canClose && !linkDoc)
				{
					BeginInvoke((MethodInvoker)(StartClosed));
					e.Cancel = true;
				}
				else
					Cursor = Cursors.Default;
			}
			base.OnClosing(e);
		}

		public int DocID { get; set; }


		private void StartClosed()
		{
			string xmlString = base.GetXML();
			Visible = false;
			string sqlString = Options.GetSQL(xmlString);
			//клас для принудительного завершения ассинхронной операции
			var cancel = new Canceler();
			Cursor = Cursors.WaitCursor;
			//запуск функции, котрая сначала выключит все формы, потом запускает ассинхронную операцию, по окончанию которой формы будут включены, эту ассинхронную операцию завершает либо таймер, либо окончание поиска
			//функиция приостанавливает поток, пока не будет реально запущена ассинхронная операция ожидания
			windowsEnabler.WindowsWait(SynchronizationContext.Current, cancel);

			SearchDocsDelegate searchBegin = Environment.DocData.SearchDocs;
			searchBegin.BeginInvoke(Environment.DocData.GetFoundDocsIDQuery(sqlString, true), base.GetInFound(),
									base.GetToFound(), Environment.CurEmp.ID, Forms.MainFormDialog.maxSearchResults,
									(IAsyncResult ar) =>
									{
										var searchBegindel = ar.AsyncState as SearchDocsDelegate;
										if(searchBegindel != null)
											searchBegindel.EndInvoke(ar);
										cancel.Cancel();
										//ожидание завершения WindowsWait//возможно нужно ждать завершения включения окон
										windowsEnabler.AsyncWaitHandleClose.WaitOne(2000);
										canClose = true;
										if(InvokeRequired)
											Invoke((MethodInvoker)(Close));
										else
											Close();
									}, searchBegin);
		}

		private IManagerWindowsWait windowsEnabler = new ManagerWindow();

		public IManagerWindowsWait WindowsEnabler
		{
			get { return windowsEnabler; }
			set { windowsEnabler = value; }
		}

		private delegate Boolean SearchDocsDelegate(
			String docId, Boolean inFoundm, Boolean toFound, Int32 empID, Int32 maxSearchResults);
	}
}