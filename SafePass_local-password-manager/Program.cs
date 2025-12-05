using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SafePass_local_password_manager
{
    class Program
    {
        static void Main()
        {
         
       
            Console.WriteLine("-test-");
            Console.Write("master key: ");
            string? masterPassword = Console.ReadLine();

           
            var encryption = new EncryptService(masterPassword);

            
            string originalPassword = "testPassword123";
            Console.WriteLine($"\norig: {originalPassword}");

            
            string encrypted = encryption.Encrypt(originalPassword);
            Console.WriteLine($"encrypted: {encrypted}");

     
            string decrypted = encryption.Decrypt(encrypted);
            Console.WriteLine($"decrypted: {decrypted}");

      
            if (originalPassword == decrypted)
            {
                Console.WriteLine("\n[+] test");
            }
            else
            {
                Console.WriteLine("\n[-] test");
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
    
    //CRUD interface - просто чтобы отдельно показать реализацию
    public interface IPasswordRepo
    {
        void Create();
        List<PasswordEntry> GetAll();
        PasswordEntry GetById(int id);
        void Update(PasswordEntry entry);
        void Delete(int id);
    }

    public class PasswordRepo : IPasswordRepo
    {
        private readonly string _passFilePath = "passwords.json";
        private List<PasswordEntry> _all;

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

        public string Encrypt(string password)
        {
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
                }


            }

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
}
    
