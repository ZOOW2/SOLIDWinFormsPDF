namespace SOLIDWinFormsPDF
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string connectionString = "server = localhost; user = root; password = root; database = files";

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1(connectionString, new PRead()));
        }
    }
}