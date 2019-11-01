using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Kesco.App.Win.DocView
{
    public class GuiConsole
    {
        private static bool hasConsole;
        private static IntPtr conOut;
        private static IntPtr oldOut;

        /*****************************************************************/

        #region DLLImport

        /*****************************************************************/

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = false)]
        protected static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern bool SetStdHandle(int nStdHandle, IntPtr hConsoleOutput);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern IntPtr CreateFile(
            string fileName,
            int desiredAccess,
            int shareMode,
            IntPtr securityAttributes,
            int creationDisposition,
            int flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        protected static extern bool CloseHandle(IntPtr handle);

        /*****************************************************************/

        #endregion DLLImport

        /*****************************************************************/

        public static void CreateConsole()
        {
            if (hasConsole)
                return;

            if (oldOut == IntPtr.Zero)
                oldOut = GetStdHandle(-11);

            if (! AllocConsole())
                throw new Exception("AllocConsole() failed");

            conOut = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);

            if (! SetStdHandle(-11, conOut))
                throw new Exception("SetStdHandle() failed");

            StreamToConsole();

            hasConsole = true;
        }

        private static void StreamToConsole()
        {
            Stream cstm = Console.OpenStandardOutput();
            var cstw = new StreamWriter(cstm, Encoding.GetEncoding(866)) {AutoFlush = true};
            Console.SetOut(cstw);
            Console.SetError(cstw);
        }

        private static void RemoveStream()
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            Console.SetError(new StreamWriter(Console.OpenStandardError()));
        }

        public static void ReleaseConsole()
        {
            if (! hasConsole)
                return;

            if (! CloseHandle(conOut))
                throw new Exception("CloseHandle() failed");

            conOut = IntPtr.Zero;

            if (! FreeConsole())
                throw new Exception("FreeConsole() failed");

            RemoveStream();
            if (! SetStdHandle(-11, oldOut))
                throw new Exception("SetStdHandle() failed");

            hasConsole = false;
        }
    }
}