using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace photoel2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(true);
                Application.Run(new PhotoElForm());
            }
            catch(Exception e)
            {
                StringBuilder sb = new StringBuilder();
                do
                {
                    sb.AppendLine("== Exception:");
                    sb.AppendLine(e.Message);
                    sb.AppendLine(e.StackTrace);
                    e = e.InnerException;
                } while (e != null);
                string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "photoel2.txt");
                System.IO.File.WriteAllText(path, sb.ToString());
                MessageBox.Show("Error log store in " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
