using System.Text.Json;

namespace SafePass_local_password_manager;


    public interface IPasswordRepo
    {
        void Create(PasswordEntry entry); 
        List<PasswordEntry> GetAll();
        PasswordEntry? GetById(int id);
        void Update(PasswordEntry entry);
        void Delete(int id);
        bool VerifyMasterKey(EncryptService encryptService);
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
            if (File.Exists(_passFilePath))
            {
                File.SetAttributes(_passFilePath, FileAttributes.Normal);
            }
    
            File.WriteAllText(_passFilePath, json);
            File.SetAttributes(_passFilePath, FileAttributes.Hidden);
         
        }

        public void Create(PasswordEntry entry)
        {
            
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
                
            if (exist == null) 
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
            
            if (exist == null)
            {
                return;
            }
            _all.Remove(exist); 
            
            SaveData();
        }

        public bool VerifyMasterKey(EncryptService service)
        {
            var check = _all.FirstOrDefault(e => e.Id == 0);
            if (check == null)
            {
                check = new PasswordEntry
                {
                    Id = 0,
                    Service = "__$#system#$__",
                    Username = "-",
                    EncryptedPassword = service.Encrypt("VERIFICATION"),
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };
                _all.Add(check);
                SaveData();
                return true;
            }

            try
            {
                string decrypted = service.Decrypt(check.EncryptedPassword!);
                return decrypted == "VERIFICATION";
            }
            catch
            {
                return false;
            }
        }
    }
