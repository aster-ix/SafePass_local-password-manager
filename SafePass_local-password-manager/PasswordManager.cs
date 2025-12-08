namespace SafePass_local_password_manager;

public class PasswordEntryManager
{
    private readonly PasswordRepo _passwordRepo;
    private readonly EncryptService _encryptService;
    
    public PasswordEntryManager(string masterPassword)
    {
        _encryptService = new EncryptService(masterPassword);
        _passwordRepo = new PasswordRepo();
        _passwordRepo.SetEncryptService(_encryptService);
    }
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
    public bool VerifyKey()
    {
        return _passwordRepo.VerifyMasterKey(_encryptService);
    }
}