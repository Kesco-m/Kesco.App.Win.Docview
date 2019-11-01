using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Kesco.App.Win.DocView.Forms
{
	public class HideForm : Form
	{
		private IContainer components;

		private DDEListener ddeListener;
		public bool _closeClick;

		protected override void WndProc(ref Message m)
		{
			if((int)Lib.Win.Document.Win32.Msgs.WM_SYSCOMMAND == m.Msg && (int)m.WParam == Lib.Win.Document.Win32.User32.SC_CLOSE)
				_closeClick = true;
			if((int)Lib.Win.Document.Win32.Msgs.WM_COPYDATA == m.Msg && m.LParam != null)
			{
				try
				{
					string text = string.Empty;
					var cd = new Lib.Win.Document.Win32.User32.COPYDATASTRUCT();
					cd = (Lib.Win.Document.Win32.User32.COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(Lib.Win.Document.Win32.User32.COPYDATASTRUCT));
					if(cd.dwData == 1024)
					{
						if(cd.cbData > 0)
						{
							var B = new byte[cd.cbData];
							IntPtr lpData = cd.lpData;
							Marshal.Copy(lpData, B, 0, cd.cbData);
							text = Encoding.UTF8.GetString(B);

                            Console.WriteLine("{0}: Message: {1}", DateTime.Now.ToString("HH:mm:ss fff"), text);
						}
					}
					text = text.Trim();
					try
					{
						AnalyzeArgs(text);
					}
					catch(Exception ex)
					{
						Lib.Win.Data.Env.WriteToLog(ex);
						Kesco.Lib.Win.Error.ErrorShower.OnShowError(this, ex.Message,
												Environment.StringResources.GetString("Error"));
					}
					m.Result = new IntPtr(1);
					return;
				}
				catch(Exception ex)
				{
					Lib.Win.Data.Env.WriteToLog(ex);
				}
			}

			if(m.Msg == (int)Lib.Win.Document.Win32.Msgs.WM_HOTKEY && ActiveForm != null)
				HotKeys.DoWork(m.WParam);

			base.WndProc(ref m);
		}

		public HideForm()
		{
			InitializeComponent();
			SetStyle(ControlStyles.Opaque, true);
			HotKeys.Register(Handle);
		}

		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= Lib.Win.Document.Win32.User32.WS_EX_TOOLWINDOW;
				return createParams;
			}
		}

		#region Windows Form Designer generated code

		/// <summary>
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.ddeListener = new DDEListener(this.components);
			this.SuspendLayout();
			// 
			// ddeListener
			// 
			this.ddeListener.ActionName = "System";
			this.ddeListener.AppName = "DocView";
			this.ddeListener.OnDDEExecute += new DDEExecuteEventHandler(this.ddeListener1_OnDDEExecute);
			// 
			// HideForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(116, 22);
			this.ControlBox = false;
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HideForm";
			this.Opacity = 0D;
			this.ShowInTaskbar = false;
			this.Text = "DocViewReciver";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.HideForm_Closing);
			this.Load += new System.EventHandler(this.HideForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private void HideForm_Closing(object sender, CancelEventArgs e)
		{
			if(_closeClick)
			{
				e.Cancel = true;
				_closeClick = false;
				HotKeys.UnRegister(Handle);
			}
		}

		public static int SendMessage(IntPtr wndHandle, string sendText)
		{
			return SendMessage(wndHandle, sendText, true);
		}

		public static int SendMessage(IntPtr wndHandle)
		{
			return SendMessage(wndHandle, "", false);
		}


		public static int SendMessage(IntPtr wndHandle, string sendText, bool send)
		{
            Console.WriteLine("{0}: Send message: {1}", DateTime.Now.ToString("HH:mm:ss fff"), sendText);
			int result = 0;
			IntPtr lpB = IntPtr.Zero;
			IntPtr textPointer = IntPtr.Zero;
			try
			{
				Lib.Win.Document.Win32.User32.ShowWindow(wndHandle, 8);
				if(send)
				{
					byte[] B = Encoding.UTF8.GetBytes(sendText);
					lpB = Marshal.AllocHGlobal(B.Length);
					Marshal.Copy(B, 0, lpB, B.Length);
					var cdt = new Lib.Win.Document.Win32.User32.COPYDATASTRUCT { dwData = 1024, lpData = lpB, cbData = B.Length };
					textPointer = Marshal.AllocHGlobal(Marshal.SizeOf(cdt));
					Marshal.StructureToPtr(cdt, textPointer, false);
				}
				if(wndHandle != IntPtr.Zero)
				{
					result = Lib.Win.Document.Win32.User32.SendMessage(wndHandle, (int)Lib.Win.Document.Win32.Msgs.WM_COPYDATA,
										   (send && Program.MainFormDialog != null) ? Program.MainFormDialog.Handle : IntPtr.Zero, textPointer).ToInt32();
#if(DEBUG)
					if(result > 0)
                        Console.WriteLine("{0}: true {1}", DateTime.Now.ToString("HH:mm:ss fff"), result);
					else
                        Console.WriteLine("{0}: false {1}", DateTime.Now.ToString("HH:mm:ss fff"), result);
#endif
				}
				if(send)
				{
					Marshal.FreeHGlobal(lpB);
					Marshal.FreeHGlobal(textPointer);
				}
			}
			catch(Exception ex)
			{
				result = 0;
				Kesco.Lib.Win.Error.ErrorShower.OnShowError(null, ex.Message, Environment.StringResources.GetString("Error"));
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			return result;
		}

		public static bool SendMessageNotify(IntPtr wndHandle, string sendText)
		{
			return SendMessageNotify(wndHandle, sendText, true);
		}

		public static bool SendMessageNotify(IntPtr wndHandle, string sendText, bool send)
		{
#if(DEBUG)
            Console.WriteLine("{0}: Send Notify message: {1}", DateTime.Now.ToString("HH:mm:ss fff"), sendText);
#endif
			int result = 0;
			IntPtr lpB = IntPtr.Zero;
			IntPtr textPointer = IntPtr.Zero;
			try
			{
				Lib.Win.Document.Win32.User32.ShowWindow(wndHandle, 8);
				if(send)
				{
					byte[] B = Encoding.UTF8.GetBytes(sendText);
					lpB = Marshal.AllocHGlobal(B.Length);
					Marshal.Copy(B, 0, lpB, B.Length);
					var cdt = new Lib.Win.Document.Win32.User32.COPYDATASTRUCT { dwData = 1024, lpData = lpB, cbData = B.Length };
					textPointer = Marshal.AllocHGlobal(Marshal.SizeOf(cdt));
					Marshal.StructureToPtr(cdt, textPointer, false);
				}
				if(wndHandle != IntPtr.Zero)
				{
					result = Lib.Win.Document.Win32.User32.PostMessage(wndHandle, (int)Lib.Win.Document.Win32.Msgs.WM_COPYDATA,
												(send && Program.MainFormDialog != null) ? Program.MainFormDialog.Handle : IntPtr.Zero, textPointer);
				}
				if(send)
				{
					Marshal.FreeHGlobal(lpB);
					Marshal.FreeHGlobal(textPointer);
				}
			}
			catch(Exception ex)
			{
				result = 0;
				Kesco.Lib.Win.Error.ErrorShower.OnShowError(null, ex.Message, Environment.StringResources.GetString("Error"));
				Lib.Win.Data.Env.WriteToLog(ex);
			}

			return result == 0;
		}

		public static void AnalyzeArgs(string args)
		{
			if(Program.MainFormDialog == null || Program.MainFormDialog.Disposing || Program.MainFormDialog.IsDisposed)
				return;
			try
			{
				bool getFromTray = Program.MainFormDialog != null && !MainFormDialog.toTray;
				args = args.Trim();
				if(args.Length == 0)
				{
					if(getFromTray)
					{
						getFromTray = false;
						Program.MainFormDialog.GetFromTray();
					}
				}
				if(args.IndexOf("<") != 0)
				{
					if(getFromTray)
					{
						getFromTray = false;
						Program.MainFormDialog.GetFromTray();
					}
					string startString = args;
					if(args.IndexOf("<") > 0)
					{
						startString = args.Substring(0, args.IndexOf("<"));
						args = args.Remove(0, args.Trim().IndexOf("<")).Trim();
					}
					if(Program.MainFormDialog != null && startString.Length > 0 && !MainFormDialog.toTray)
					{
						string fileName = Kesco.Lib.Win.Document.TextProcessor.ReplaceKesco(startString).TrimStart('\"').TrimEnd('\"');
						if(File.Exists(fileName) || Directory.Exists(fileName))
						{
							if(Environment.General != null)
								Environment.General.LoadOption<int>("Page", 0).Value = 0;
							MainFormDialog.returnID = 0;
							MainFormDialog.returnFileName = fileName;
							Environment.CmdManager.Commands["Return"].Execute();
						}
					}
				}
				if(args.IndexOf("<") == 0)
				{
					var doc = new XmlDocument();
					try
					{
						doc.LoadXml("<root>" + args.Replace("\\\"", "\"").Replace("\"\"\"", "&quot;") + "</root>");
					}
					catch
					{
						Lib.Win.Data.Env.WriteToLog("Bad xml\n" + args);
						return;
					}
					for(int i = 0; i < doc.ChildNodes[0].ChildNodes.Count; i++)
					{
						XmlNode node = doc.ChildNodes[0].ChildNodes[i];

						switch(node.Name.ToLower())
						{
							case "tray":
								MainFormDialog.toTray = true;
								break;

							case "test":
								break;

							case "exit":
								if(Program.MainFormDialog != null)
									Environment.CmdManager.Commands["Exit"].Execute();
								else
									Application.Exit();
								break;
							//-- поиск подобных
							case "checksimilar":
								IntPtr wHandle = IntPtr.Zero;
								var control = "";
								var callbackKey = "";
								var callbackUrl = "";
								if(node.Attributes["wndHandle"] != null &&
									node.Attributes["wndHandle"].Value.Length > 0)
								{
									int h = 0;
									if(int.TryParse(node.Attributes["wndHandle"].Value, out h) && h > 0)
										wHandle = new IntPtr(h);
								}
								else if(node.Attributes["callbackKey"] != null && !string.IsNullOrEmpty(node.Attributes["callbackKey"].Value))
								{

									control = node.Attributes["control"].InnerText;
									callbackKey = node.Attributes["callbackKey"].InnerText;
									callbackUrl = node.Attributes["callbackUrl"].InnerText;
								}
								else
									return;
								int typeID = 0;
								string numberStr = "";
								DateTime docDate = DateTime.MinValue;
								int docID = 0;

								if(node.Attributes["type"] == null || !int.TryParse(node.Attributes["type"].Value, out typeID) || typeID < 1)
								{
                                    Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), (node.Attributes["type"] == null ? "type is null" : (" type bad : " + node.Attributes["type"].Value)));
									if(wHandle != IntPtr.Zero)
										SendMessage(wHandle, "-1");
									else if(!string.IsNullOrEmpty(callbackKey))
										SendAnswerMessageClass.PostAnswer(new Common.SendAnswerParams(control, callbackKey, callbackUrl, node.Name.ToLower()), "-1");
									return;
								}

								if(node.Attributes["number"] != null && node.Attributes["number"].Value.Length > 0)
								{
									numberStr = node.Attributes["number"].Value;
								}

								if(node.Attributes["date"] != null)
									if(!DateTime.TryParseExact(node.Attributes["date"].Value, "dd.MM.yyyy", null, DateTimeStyles.None, out docDate))
									{
                                        Console.WriteLine("{0}: date bad: {1}",DateTime.Now.ToString("HH:mm:ss fff"), node.Attributes["date"].Value);
										if(wHandle != IntPtr.Zero)
											SendMessage(wHandle, "-1");
										else if(!string.IsNullOrEmpty(callbackKey))
											SendAnswerMessageClass.PostAnswer(new Common.SendAnswerParams(control, callbackKey, callbackUrl, node.Name.ToLower()), "-1");
										return;
									}
								
								if(node.Attributes["id"] == null || !int.TryParse(node.Attributes["id"].Value, out docID) || docID < 0)
								{
                                    Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), (node.Attributes["id"] == null ? "id null" : (" id bad: " + node.Attributes["id"].Value)));
									if(wHandle != IntPtr.Zero)
										SendMessage(wHandle, "-1");
									else if(!string.IsNullOrEmpty(callbackKey))
										SendAnswerMessageClass.PostAnswer(new Common.SendAnswerParams(control, callbackKey, callbackUrl, node.Name.ToLower()), "-1");
									return;
								}
							
								string typeStr = "";
								try
								{
									typeStr = Environment.DocTypeData.GetDocType(typeID, Environment.CurCultureInfo. TwoLetterISOLanguageName);
								}
								catch
								{
                                    Console.WriteLine("{0}: typeStr bad", DateTime.Now.ToString("HH:mm:ss fff"));
									if(wHandle != IntPtr.Zero)
										SendMessage(wHandle, "-1");
									else if(!string.IsNullOrEmpty(callbackKey))
										SendAnswerMessageClass.PostAnswer(new Common.SendAnswerParams(control, callbackKey, callbackUrl, node.Name.ToLower()), "-1");
									return;
								}

								string personIDs = "";
								if(node.Attributes["personids"] != null)
								{
									personIDs = node.Attributes["personids"].Value;
								}
								byte ѕоискѕоЋицам = 0;
								if(node.Attributes["usepersonids"] != null)
									ѕоискѕоЋицам = Convert.ToByte(node.Attributes["usepersonids"].Value);
								bool returnEform = false;
								if(node.Attributes["return"] != null)
									returnEform = node.Attributes["return"].Value.Equals("1");
								//-- запрос к базе

								var selDocDialog = new Lib.Win.Document.Select.SelectDocDialog(typeID, typeStr, numberStr, docDate, personIDs, docID, false) { SearchType = ѕоискѕоЋицам, ReturnEform = returnEform };
								selDocDialog.DialogEvent += selDocDialog_DialogEvent;
								selDocDialog.Show();
								if(wHandle != IntPtr.Zero)
									SendAnswerMessageClass.Add(wHandle, selDocDialog.Handle);
								else if(!string.IsNullOrEmpty(callbackKey))
									SendAnswerMessageClass.Add(control, callbackKey, callbackUrl, node.Name.ToLower(), selDocDialog);
								selDocDialog.BringToFront();
								selDocDialog.Activate();
								break;

							case "opendoc":
								int id = 0;
								int imgID = -2;
								bool force = false;
								if(node.Attributes["id"] != null)
									if(!int.TryParse(node.Attributes["id"].Value, out id))
										id = 0;
								if(id > 0)
								{
									if(!Environment.DocData.IsDocAvailable(id))
									{
										Kesco.Lib.Win.Error.ErrorShower.OnShowError(null,
											Environment.StringResources.GetString("Document") +
											Environment.StringResources.GetString("Num") + id.ToString() + " " +
											Environment.StringResources.GetString("NotAvailable"),
											Environment.StringResources.GetString("Warning"));
										break;
									}
									if(Program.MainFormDialog != null)
									{
										if(Program.MainFormDialog.docControl.DocumentID == id)
										{
											Environment.RefreshDocs();
											Program.MainFormDialog.docControl.RefreshDoc();
										}
									}
									if(node.Attributes["imageid"] != null)
										if(!int.TryParse(node.Attributes["imageid"].Value, out imgID) || imgID < -2)
											imgID = -2;
									if(node.Attributes["replicate"] != null)
									{
										bool.TryParse(node.Attributes["replicate"].Value, out force);
									}
									if(node.Attributes["newwindow"] != null)
										if(node.Attributes["newwindow"].Value == "0")
										{
											if(getFromTray)
											{
												getFromTray = false;
												Program.MainFormDialog.GetFromTray();
											}
											if(Program.MainFormDialog != null)
											{
												MainFormDialog.returnID = id;
												MainFormDialog.returnForce = force;
												MainFormDialog.returnContext = null;
												Environment.CmdManager.Commands["Return"].Execute();
												break;
											}
										}
										else if(node.Attributes["newwindow"].Value == "1")
										{
											if(!Environment.OpenDocsContains(id) && Program.MainFormDialog != null &&
												MainFormDialog.curDocID == id)
											{
												Program.MainFormDialog.GetFromTray();
												Program.MainFormDialog.docControl.ImageID = imgID;
												Program.MainFormDialog.docControl.ForceRelicate = force;
												break;
											}
										}
									string zoom = string.IsNullOrEmpty(Environment.ZoomString) ? "100%" : Environment.ZoomString;
									bool needInv = false;
									if(Program.MainFormDialog != null)
									{
										if(Program.MainFormDialog.docControl.ImageDisplayed &&
											!string.IsNullOrEmpty(Program.MainFormDialog.zoom))
											zoom = Program.MainFormDialog.zoom;
										needInv = Program.MainFormDialog.InvokeRequired;
									}
									if(!needInv)
										Environment.NewWindow(id, zoom, new Context(Misc.ContextMode.Catalog), imgID, 1, null, false, true);
								}
								else
								{
									if(node.Attributes["cmd"] != null)
										switch(node.Attributes["cmd"].Value.ToLower())
										{
											case "print":
												if(node.Attributes["ids"] != null)
													PrintDocuments(Kesco.Lib.Win.Data.Business.V2.Entity.Str2Collection(node.Attributes["ids"].Value));
												break;
										}
								}
								break;
							case "path":
								if(getFromTray)
								{
									getFromTray = false;
									Program.MainFormDialog.GetFromTray();
								}
								if(Program.MainFormDialog != null)
								{
									if(node.Attributes["open"] != null)
									{
										string path = node.Attributes["open"].Value.ToUpper();
										Program.MainFormDialog.folders.SelectCatalogNode(path, 0);
									}
								}
								break;
							case "search":
								string xmlString =
									Lib.Win.Data.DALC.Documents.Search.Options.GetXMLFromSearchElement((XmlElement)node);
								var optionsDialog = new XmlSearchForm(xmlString) { StartPosition = FormStartPosition.CenterScreen };
								if(node.Attributes["wndHandle"] != null &&
									!string.IsNullOrEmpty(node.Attributes["wndHandle"].Value))
								{
									int h = 0;
									if(int.TryParse(node.Attributes["wndHandle"].Value, out h) && h > 0)
									{
										SendAnswerMessageClass.Add(new IntPtr(h), optionsDialog.Handle);
										optionsDialog.DialogEvent += optionsDialog_DialogEvent;
									}
								}
								else if(node.Attributes["callbackKey"] != null && !string.IsNullOrEmpty(node.Attributes["callbackKey"].Value))
								{
									control = "";
									callbackKey = "";
									callbackUrl = "";
									control = node.Attributes["control"].InnerText;
									callbackKey = node.Attributes["callbackKey"].InnerText;
									callbackUrl = node.Attributes["callbackUrl"].InnerText;
									if(string.IsNullOrEmpty(control) || string.IsNullOrEmpty(callbackUrl))
										break;
									SendAnswerMessageClass.Add(control, callbackKey, callbackUrl, node.Name.ToLower(), optionsDialog);
									optionsDialog.DialogEvent += optionsDialog_DialogEvent;
								}
								else
									if(Program.MainFormDialog != null)
										optionsDialog.DialogEvent += Program.MainFormDialog.SearchDialog_DialogEvent;
								optionsDialog.Show();
								optionsDialog.BringToFront();
								optionsDialog.Activate();

								break;

							case "sendmessage":
								if(node.Attributes["id"] != null &&
									!string.IsNullOrEmpty(node.Attributes["id"].Value))
								{
                                    Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), node.InnerText);
									docID = 0;
									if(int.TryParse(node.Attributes["id"].Value, out docID) && docID > 0)
									{
										if(!Environment.DocData.IsDocAvailable(docID))
										{
											Kesco.Lib.Win.Error.ErrorShower.OnShowError(null,
												Environment.StringResources.GetString("Document") +
												Environment.StringResources.GetString("Num") + docID.ToString() + " " +
												Environment.StringResources.GetString("NotAvailable"),
												Environment.StringResources.GetString("Warning"));
											break;
										}

										if(node.Attributes["opendoc"] != null && !string.IsNullOrEmpty(node.Attributes["opendoc"].Value))
										{
											bool open = false;
											if(bool.TryParse(node.Attributes["opendoc"].Value, out open)? open : "1".Equals(node.Attributes["opendoc"].Value))
											{
												if(!Environment.OpenDocsContains(docID))
												{
													string zoom = string.IsNullOrEmpty(Environment.ZoomString) ? "100%" : Environment.ZoomString;
													bool needInv = false;
													if(Program.MainFormDialog != null)
													{
														if(Program.MainFormDialog.docControl.ImageDisplayed &&
															!string.IsNullOrEmpty(Program.MainFormDialog.zoom))
															zoom = Program.MainFormDialog.zoom;
														needInv = Program.MainFormDialog.InvokeRequired;
													}
													if(!needInv)
														Environment.NewWindow(docID, zoom, new Context(Misc.ContextMode.Catalog), 0, 1, null, false, true);
												}
											}
										}
										string messstr = "";
										if(node.Attributes["message"] != null)
											messstr = node.Attributes["message"].Value ?? string.Empty;

										string empIDsStr = "";
										if(node.Attributes["empids"] != null)
											empIDsStr = node.Attributes["empids"].Value ?? string.Empty;
										List<int> empIDs = new List<int>();
										if(empIDsStr.Length > 0 && Regex.IsMatch(empIDsStr, "^\\d[\\d,]+$"))
										{
											string[ ] empIDStrs = empIDsStr.Split(",".ToCharArray());
											if(empIDStrs.Length > 0)
											{
												empIDs.AddRange(empIDStrs.Select(int.Parse));
											}
										}
										var messageDialog = new Lib.Win.Document.Dialogs.SendMessageDialog(docID, messstr);
										
										if(node.Attributes["checkall"] != null &&
											node.Attributes["checkall"].Value == "1")
											messageDialog.Check = true;

										if(Program.MainFormDialog != null)
										{
											if(MainFormDialog.curDocID == docID)
											{
												if(Program.MainFormDialog.docGrid.IsSingle &&
													Program.MainFormDialog.IsNotRead())
												{
													Environment.CmdManager.Commands["MarkReadMessages"].Execute();
												}
												if(Program.MainFormDialog.docGrid.IsWorkFolder())
												{
													messageDialog.EmpID = Program.MainFormDialog.curEmpID;
													messageDialog.FolderID = Program.MainFormDialog.curContext.ID;
												}
											}
											foreach(var dial in Environment.OpenDocs.Where(t => t.Key == docID).Select(t => t.Value as SubFormDialog).Where(dial => dial.docControl.ImageID == 0))
												dial.docControl.RefreshEForm(true);
										}

										messageDialog.Show();
										if(empIDs.Count > 0)
											messageDialog.AddUsers(empIDs);
										messageDialog.BringToFront();
										messageDialog.Activate();
									}
								}
								break;
							case "console":
								if(node.Attributes["stop"] != null)
									GuiConsole.ReleaseConsole();
								else
								{
									GuiConsole.CreateConsole();
                                    Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), args);
								}
								break;
							case "senddocs":
								if(node.Attributes["ids"] != null)
								{
									int filesize = 0;
									string email = "";
									if(node.Attributes["filesize"] != null)
										if(!int.TryParse(node.Attributes["filesize"].Value, out filesize) &&
											filesize < 1)
											filesize = 0;
									if(node.Attributes["email"] != null)
										email = node.Attributes["email"].Value;
								}
								break;
							case "printdocs":
								if(node.Attributes["ids"] != null)
									PrintDocuments(Kesco.Lib.Win.Data.Business.V2.Entity.Str2Collection(node.Attributes["ids"].Value));
								break;
						}
					}
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private static void optionsDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			try
			{
				var search = e.Dialog as XmlSearchForm;
				if(search == null)
					return;
				if(search.DialogResult == DialogResult.OK)
					SearchTest(search, search.GetXML());
				else
					AnswerSendMessage(search, "0");
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private static void SearchTest(Form handle, string xml)
		{
			try
			{
				int count = Environment.DocData.FoundDocsCount();
				if(count > 0 && count < MainFormDialog.maxSearchResults)
				{
					var dialog = new Lib.Win.Document.Select.SelectDocUniversalDialog(Environment.DocData.GetFoundDocsIDQuery(Environment.CurEmp.ID), xml);
					dialog.DialogEvent += selectDocUniversalDialog_DialogEvent;
					if(SendAnswerMessageClass.ContainsKey(handle.Handle))
						SendAnswerMessageClass.Add(SendAnswerMessageClass.Value(handle.Handle), dialog.Handle);
					else
						SendAnswerMessageClass.Value(handle).Form = dialog;
					dialog.Show();
				}
				else
				{
					if(count >= MainFormDialog.maxSearchResults)
					{
						if(MessageBox.Show(
							Environment.StringResources.GetString("HideForm.SearchTest.Message1") +
							MainFormDialog.maxSearchResults.ToString() + System.Environment.NewLine +
							Environment.StringResources.GetString("HideForm.SearchTest.Message2") +
							System.Environment.NewLine + System.Environment.NewLine +
							Environment.StringResources.GetString("Search.NotFound.Message2"),
							Environment.StringResources.GetString("Warning"), MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
							RestartSearch(handle, xml);
						else
							AnswerSendMessage(handle, "0");
					}
					else if(MessageBox.Show(Environment.StringResources.GetString("Search.NotFound.Message1")
											 + System.Environment.NewLine + System.Environment.NewLine +
											 Environment.StringResources.GetString("Search.NotFound.Message2"),
											 Environment.StringResources.GetString("Search.NotFound.Title"),
											 MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
											 MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						RestartSearch(handle, xml);
					else
						AnswerSendMessage(handle, "0");
				}

				SendAnswerMessageClass.Remove(handle.Handle);
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private static void selectDocUniversalDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			var dialog = e.Dialog as Lib.Win.Document.Select.SelectDocUniversalDialog;
			if(dialog == null || dialog.Disposing || dialog.IsDisposed)
				return;

			switch(dialog.DialogResult)
			{
				case DialogResult.OK:
					AnswerSendMessage(dialog, dialog.DocID.ToString());
					break;
				case DialogResult.Retry:
					{
						var searchDialog = new XmlSearchForm(dialog.XML);
						if(SendAnswerMessageClass.ContainsKey(dialog.Handle))
							SendAnswerMessageClass.Add(SendAnswerMessageClass.Value(dialog.Handle), searchDialog.Handle);
						else
							SendAnswerMessageClass.Value(dialog).Form = searchDialog;
						searchDialog.DialogEvent += optionsDialog_DialogEvent;
						searchDialog.Show();
					}
					break;
				default:
					AnswerSendMessage(dialog, "0");
					break;
			}

			SendAnswerMessageClass.Remove(dialog.Handle);
		}

		private static void RestartSearch(Form handle, string xml)
		{
			var searchDialog = new XmlSearchForm(xml);
			if(SendAnswerMessageClass.ContainsKey(handle.Handle))
				SendAnswerMessageClass.Add(SendAnswerMessageClass.Value(handle.Handle), searchDialog.Handle);
			else
				SendAnswerMessageClass.Value(handle).Form = searchDialog;
			searchDialog.DialogEvent += optionsDialog_DialogEvent;
			searchDialog.Show();
		}

		private static void AnswerSendMessage(Form handle, string sendString)
		{
#if(DEBUG)
            Console.WriteLine("{0}: Handle for answer window: {1} String: {2}", DateTime.Now.ToString("HH:mm:ss fff"), handle, sendString);
#endif
			if(SendAnswerMessageClass.ContainsKey(handle.Handle))
			{
				int processID = 0;
				IntPtr externHandle = SendAnswerMessageClass.Value(handle.Handle);
				SendAnswerMessageClass.Remove(handle.Handle);
#if(DEBUG)
                Console.WriteLine("{0}: Handle of answer window: {1}", DateTime.Now.ToString("HH:mm:ss fff"), externHandle);
#endif
				Lib.Win.Document.Win32.User32.GetWindowThreadProcessId(externHandle, ref processID);
				Lib.Win.Document.Win32.User32.AllowSetForegroundWindow(processID);
				Lib.Win.Document.Win32.User32.SetForegroundWindow(externHandle);

				SendMessage(externHandle, sendString);
			}
			else if( SendAnswerMessageClass.Contains(handle))
			{
				Common.SendAnswerParams sap = SendAnswerMessageClass.Value(handle);
				SendAnswerMessageClass.Remove(handle);
				SendAnswerMessageClass.PostAnswer( sap, sendString);
			}
		}

		private static void selDocDialog_DialogEvent(object source, Kesco.Lib.Win.DialogEventArgs e)
		{
			var dialog = e.Dialog as Lib.Win.Document.Select.SelectDocDialog;
			if(dialog == null)
				return;
			try
			{
				if(dialog.DialogResult == DialogResult.OK)
				{
					int docID = -2;
					Lib.Win.Document.Items.ListItem docItem = dialog.SelectedItem;
					if(docItem != null)
						docID = docItem.ID;
					AnswerSendMessage(dialog, docID.ToString());
				}
				else
				{
					AnswerSendMessage(dialog, "0");
				}
			}
			catch(Exception ex)
			{
				Lib.Win.Data.Env.WriteToLog(ex);
			}
		}

		private static void PrintDocuments(StringCollection values)
		{
			if(values != null && values.Count > 0)
			{
			}
		}

		private void HideForm_Load(object sender, EventArgs e)
		{
			//if(TestPrinter.CheckPrinterExists())
			//{
			//    string printerName = Lib.Win.Document.Environment.PrinterName;
			//}
		}

		private void ddeListener1_OnDDEExecute(object Sender, string[] Commands)
		{
			BeginInvoke(new DDEExecuteEventHandler(DDEExecute), new[] { Sender, Commands });
			Application.DoEvents();
		}

		private void DDEExecute(object Sender, string[] Commands)
		{
			Application.DoEvents();
			var re = new Regex(@"open\(\""(?<filename>(([A-Z]:\\[^/:\*\?<>\|]+\.\w{2,6})|(\\{2}[^/:\*\?<>\|]+\.\w{2,6})))\""\)",
					RegexOptions.IgnoreCase);
			Match ma = null;
			foreach(string s2 in Commands)
			{
				ma = re.Match(s2);
				if(ma.Success)
				{
					string fileName = Lib.Win.Document.TextProcessor.ReplaceKesco(ma.Groups["filename"].Value);
					if(File.Exists(fileName))
					{
						MainFormDialog.returnPath = "";
						MainFormDialog.returnID = 0;
						MainFormDialog.returnFileName = fileName;
						if(this.InvokeRequired)
							this.BeginInvoke((MethodInvoker)delegate
								{
									if(Environment.CmdManager != null && Environment.CmdManager.Commands["Return"] != null)
										Environment.CmdManager.Commands["Return"].Execute();
								});
						else
							if(Environment.CmdManager != null && Environment.CmdManager.Commands["Return"] != null)
								Environment.CmdManager.Commands["Return"].Execute();
					}
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if(Lib.Win.Document.Environment.Dpi <= 0)
				Lib.Win.Document.Environment.Dpi = e.Graphics.DpiX;
			base.OnPaint(e);
		}
	}
}