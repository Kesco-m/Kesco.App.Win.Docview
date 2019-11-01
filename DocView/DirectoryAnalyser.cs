using System;
using System.IO;

namespace Kesco.App.Win.DocView
{
    public class DirectoryAnalyser
    {
        public static bool IsAccessible(string path)
        {
            try
            {
                var di = new DirectoryInfo(path);
                di.GetFiles();
                Console.WriteLine("{0}: Directory exists: {1}",  DateTime.Now.ToString("HH:mm:ss fff"), path);
                return true;
            }
            catch
            {
                Console.WriteLine("{0}: Directory NOT exists: {1}", DateTime.Now.ToString("HH:mm:ss fff"), path);
                return false;
            }
        }
    }
}