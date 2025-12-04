namespace SafePass_local_password_manager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
       
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}