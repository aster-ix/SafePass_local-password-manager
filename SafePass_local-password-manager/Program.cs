using System.Security.Cryptography;
using System.Text;
using System.Text.Json;



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


    // данные по паролю
    public class PasswordEntry
    {
        
        public int Id { get; set; }
        public string? Service { get; set; }
        public string? Username { get; set; }
        public string? EncryptedPassword { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        
    }

    public class PasswordEntryManager(string masterPassword)
    {
        private readonly IPasswordRepo _passwordRepo = new PasswordRepo();                  // IDE говорит принимать не интерфейс, а сам класс
                                                                                            // звучит как поломка D в SOLID // в общем лучше игнорить
        private readonly EncryptService _encryptService = new EncryptService(masterPassword);
        
        // почему-то еще рекомендация принимать мастер пароль в класс за вводную переменную, чтобы убрать конструктор
        // ну ок, потестим
        /*public PasswordEntryManager(string masterPassword)
        {
            _passwordRepo = new PasswordRepo();
            _encryptService = new EncryptService(masterPassword);
        }*/
        

        public void AddPassword(string service, string username, string password)
        {
            var newEntry = new PasswordEntry
            {
                Service = service,
                Username = username,
                EncryptedPassword = _encryptService.Encrypt(password)
            };
            _passwordRepo.Create(newEntry);
        }

        public List<PasswordEntry> GetAllPasswords()
        {
            return _passwordRepo.GetAll();
        }
        
        public string? GetDecryptedPassword(int id)
        {
            var entry = _passwordRepo.GetById(id);
            return entry != null ? _encryptService.Decrypt(entry.EncryptedPassword!) : null;
        }

        public void UpdatePassword(int id, string service, string username, string password)
        {
            var entry = _passwordRepo.GetById(id);
            if (entry == null)
            {
                return;
            }
          
            entry.Service = service;
            entry.Username = username;
            entry.EncryptedPassword = _encryptService.Encrypt(password);
            _passwordRepo.Update(entry);
            
        }

        public void DeletePassword(int id)
        {
            _passwordRepo.Delete(id);
        }
        
    }
    
    //CRUD interface - просто чтобы отдельно показать реализацию
    public interface IPasswordRepo
    {
        void Create(PasswordEntry entry); 
        List<PasswordEntry> GetAll();
        PasswordEntry? GetById(int id);
        void Update(PasswordEntry entry);
        void Delete(int id);
    }

    public class PasswordRepo : IPasswordRepo
    {
        private readonly string _passFilePath = "passwords.json";
        private List<PasswordEntry> _all = new();

        public PasswordRepo()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(_passFilePath))
            {
                var json = File.ReadAllText(_passFilePath);
                _all = JsonSerializer.Deserialize<List<PasswordEntry>>(json) ?? new List<PasswordEntry>();
            }
            else
            {
                _all = new List<PasswordEntry>();
            }
        }

        private void SaveData()
        {
            var json = JsonSerializer.Serialize(_all, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_passFilePath, json);
        }

        public void Create(PasswordEntry entry)
        {

            // в теории any проверяет в целом есть ли что-то в _all, если есть, то берет максимальный айди
            // в коллекции + 1, если нет то просто 1

            // TODO: [не критично] есть ли эффективнее метод чем через Any? => хотя в теории это и так максимально быстро
            entry.Id = _all.Any() ? _all.Max(i => i.Id) + 1 : 1;
            entry.Created = DateTime.Now;
            entry.Updated = DateTime.Now;
            _all.Add(entry);
            SaveData();
        }

        public List<PasswordEntry> GetAll()
        {
            return _all;
        }

        public PasswordEntry? GetById(int id)
        {
            return _all.FirstOrDefault(i => i.Id == id);
        }

        public void Update(PasswordEntry entry)
        {
            var exist = GetById(entry.Id);
                
            if (exist == null) // та же ошибка что в Delete
            {
                return;
            }
            
            exist.Updated = DateTime.Now;
            exist.EncryptedPassword = entry.EncryptedPassword;
            exist.Service = entry.Service;
            exist.Username = entry.Username;
            SaveData();

            
        }

        public void Delete(int id)
        {
            var exist = GetById(id);
            
            // тут оказывается сам IDE говорит что лучше reverse nesting.
            // реализация только в одной функции для теста
            
            //я гений что это написано XDD
            /*if (exist != null)
            {
                return;
            }*/
            
            if (exist == null)
            {
                return;
            }
            _all.Remove(exist); // -> тут не надо уверять ибо была ошибка в проверке на null
            
            SaveData();
        }
    }



    // сам процесс шифрования
    public class EncryptService
    {
        
        // 32 байта = 256 бит -- ключ шифрования
        private readonly byte[] _key;
        // 16 байт = 128 бит -- вектор инициализации
        private readonly byte[] _iv;
        
        public EncryptService(string masterKey)
        {
            using (var sha256 = SHA256.Create())
            {
                // тут берем мастер пароль, его кодируем в байты и из этих байтов создаем ключ в виде sha256
                _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(masterKey));
                // тут уже берется мастер пароль + статичные случайные данные (соль) //  так как явное превышение 16 байтов - то ограничиваем
                _iv = sha256.ComputeHash(Encoding.UTF8.GetBytes(masterKey + "A(:dfk094FJ3!#;f$)")).Take(16).ToArray();
            }
        }

        public string Encrypt(string? password)
        {

            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }
            
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(password);
                        }
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }}
     




        }

        public string Decrypt(string password)
        {
        
            
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(password)))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }

                }

            }
        }
    }
    
    
    //
    // интерфейс
    //


    
}
    
