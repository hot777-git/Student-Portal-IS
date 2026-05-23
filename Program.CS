using System;
using System.Windows.Forms;
using System.IO;

namespace HawassaUniversity
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Add global exception handling
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                // Log startup
                File.AppendAllText("error_log.txt", $"{DateTime.Now}: Application starting...\r\n");
                
                // Test database connection before launching
                if (!DatabaseHelper.TestConnection())
                {
                    MessageBox.Show(
                        "Cannot connect to the database.\n\n" +
                        "Please ensure:\n" +
                        "1. SQL Server is running\n" +
                        "2. HawassaUniversityDB has been created\n" +
                        "3. The connection string in DatabaseHelper.cs is correct",
                        "Connection Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                
                File.AppendAllText("error_log.txt", $"{DateTime.Now}: Starting main form...\r\n");
                Application.Run(new frmMain());
            }
            catch (Exception ex)
            {
                File.AppendAllText("error_log.txt", $"{DateTime.Now}: FATAL ERROR: {ex.ToString()}\r\n");
                MessageBox.Show($"Fatal Error: {ex.Message}\n\nDetails saved to error_log.txt", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            File.AppendAllText("error_log.txt", $"{DateTime.Now}: Thread Exception: {e.Exception.ToString()}\r\n");
            MessageBox.Show($"Error: {e.Exception.Message}\n\nCheck error_log.txt for details", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.AppendAllText("error_log.txt", $"{DateTime.Now}: Unhandled Exception: {e.ExceptionObject.ToString()}\r\n");
            MessageBox.Show($"Unhandled Error: {e.ExceptionObject.ToString()}\n\nCheck error_log.txt for details", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
