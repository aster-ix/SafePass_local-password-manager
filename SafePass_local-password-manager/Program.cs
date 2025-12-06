using System.Security.Cryptography;
using System.Text;


namespace SafePass_local_password_manager
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main()
        {
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
                
                string masterKey = login.MasterKey;
                
                var manager = new PasswordEntryManager(masterKey);
                
                Application.Run(new MainForm(manager));
            }


        }
    }
    
    public class PasswordEntry
    {
        
        public int Id { get; set; }
        public string? Service { get; set; }
        public string? Username { get; set; }
        public string? EncryptedPassword { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        
    }
}
    
