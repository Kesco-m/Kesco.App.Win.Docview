using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Kesco.App.Win.DocView
{
    /// <summary>
    ///   Listen for DDE Execute Messages.
    /// </summary>
    public class DDEListener : Component, IDisposable
    {
        private Container components;
        //this class inherits Windows.Forms.NativeWindow and provides an Event for message processing
        protected Kesco.Lib.Win.Document.Win32.DummyWindowWithMessages m_Window = new Kesco.Lib.Win.Document.Win32.DummyWindowWithMessages();

        public DDEListener(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            m_Window.ProcessMessage += MessageEvent;
        }

        public DDEListener()
        {
            InitializeComponent();
            if (!DesignMode)
            {
                m_Window.ProcessMessage += MessageEvent;
            }
        }

        public DDEListener(string AppName, string ActionName)
            : this()
        {
            this.AppName = AppName;
            this.ActionName = ActionName;
        }

        public new void Dispose()
        {
            m_Window.ProcessMessage -= MessageEvent;
        }

        /// <summary>
        ///   Event is fired after WM_DDEExecute
        /// </summary>
        public event DDEExecuteEventHandler OnDDEExecute;

        /// <summary>
        ///   The Application Name to listen for
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        ///   The Action Name to listen for
        /// </summary>
        public string ActionName { get; set; }

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        protected bool isInitiated;

        /// <summary>
        ///   Processing the Messages
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="m"> </param>
        /// <param name="Handled"> </param>
        protected void MessageEvent(object sender, ref Message m, ref bool Handled)
        {
            //A client wants to Initiate a DDE connection
            if ((m.Msg == (int) Lib.Win.Document.Win32.Msgs.WM_DDE_INITIATE))
            {
#if(Debug)
					Debug.WriteLine("WM_DDE_INITIATE!");
#endif
                //Get the ATOMs for AppName and ActionName
				ushort a1 = Lib.Win.Document.Win32.Kernel32.GlobalAddAtom(Marshal.StringToHGlobalAnsi(AppName));
                ushort a2 = Lib.Win.Document.Win32.Kernel32.GlobalAddAtom(Marshal.StringToHGlobalAnsi(ActionName));

                //The LParam of the Message contains the ATOMs for AppName and ActionName
                var s1 = (ushort) (((uint) m.LParam) & 0xFFFF);
                var s2 = (ushort) ((((uint) m.LParam) & 0xFFFF0000) >> 16);

                //Return when the ATOMs are not equal.
                if ((a1 != s1) || (a2 != s2)) return;

                //At this point we know that this application should answer, so we send
                //a WM_DDE_ACK message confirming the connection
                IntPtr po = Lib.Win.Document.Win32.User32.PackDDElParam((int) Lib.Win.Document.Win32.Msgs.WM_DDE_ACK, (IntPtr) a1, (IntPtr) a2);
                Lib.Win.Document.Win32.User32.SendMessage(m.WParam, (int) Lib.Win.Document.Win32.Msgs.WM_DDE_ACK, m_Window.Handle, po);
                //Release ATOMs
                Lib.Win.Document.Win32.Kernel32.GlobalDeleteAtom(a1);
                Lib.Win.Document.Win32.Kernel32.GlobalDeleteAtom(a2);
                isInitiated = true;
                Handled = true;
            }

            //After the connection is established the Client should send a WM_DDE_EXECUTE message
            if ((m.Msg == (int) Lib.Win.Document.Win32.Msgs.WM_DDE_EXECUTE))
            {
#if(Debug)
					Debug.WriteLine("WM_DDE_EXECUTE!");
#endif
                //prevent errors
                if (!isInitiated) return;
                //LParam contains the Execute string, so we must Lock the memory block passed and
                //read the string. The Marshal class provides helper functions
                IntPtr pV = Lib.Win.Document.Win32.Kernel32.GlobalLock(m.LParam);
                string s3 = Marshal.PtrToStringAuto(pV) ?? string.Empty;
                Lib.Win.Document.Win32.Kernel32.GlobalUnlock(m.LParam);
                //After the message has been processed, a WM_DDE_ACK message is sent
                IntPtr lP = Lib.Win.Document.Win32.User32.PackDDElParam((int) Lib.Win.Document.Win32.Msgs.WM_DDE_ACK, (IntPtr) 1, m.LParam);
				Lib.Win.Document.Win32.User32.PostMessage(m.WParam, (int)Lib.Win.Document.Win32.Msgs.WM_DDE_ACK, m_Window.Handle, lP);
#if(Debug)
					Debug.WriteLine(s3);
#endif
                //now we split the string in Parts (every command should have [] around the text)
                //the client could send multiple commands
                int s = (s3.IndexOf('[') == 0) ? 1 : 0;
                int l = (s3.LastIndexOf(']') == (s3.Length - 1)) ? (s3.Length - s - 1) : (s3.Length - s);
                string sarr = "";
                if (s >= 0 && l >= 0 && (s + l) <= s3.Length)
                    sarr = s3.Substring(s, l);
                if (!string.IsNullOrEmpty(sarr))
                {
                    if (OnDDEExecute != null) OnDDEExecute(this, new[] {sarr});
                }
                Handled = true;
            }

            //After the WM_DDE_EXECUTE message the client should Terminate the connection
            if (m.Msg == (int) Lib.Win.Document.Win32.Msgs.WM_DDE_TERMINATE)
            {
#if(Debug)
					Debug.WriteLine("WM_DDE_TERMINATE");
#endif
                if (!isInitiated) return;
                //Confirm termination
                Lib.Win.Document.Win32.User32.PostMessage(m.WParam, (int) Lib.Win.Document.Win32.Msgs.WM_DDE_TERMINATE, m_Window.Handle, (IntPtr) 0);
                Handled = true;
                isInitiated = false;
            }
        }
    }

    public delegate void DDEExecuteEventHandler(object Sender, string[] Commands);
}