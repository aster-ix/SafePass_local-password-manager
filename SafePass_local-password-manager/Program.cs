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

        public class PasswordEntry
        {
            public int id {  get; set; }
            public string? service { get; set; }
            public string? username { get; set; } 
            public string? encryptedPassword { get; set; } 
            public DateTime created { get; set; }
            public DateTime updated { get; set; }
        }

        
    }
}