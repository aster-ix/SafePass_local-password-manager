namespace SafePass_local_password_manager;
using System.Security.Cryptography;
using System.Text;
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