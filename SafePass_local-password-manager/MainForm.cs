
namespace SafePass_local_password_manager;

public partial class MainForm : Form
{

    private readonly PasswordEntryManager _manager;
    private ListView _listView;
    private Button _btnAdd, _btnEdit, _btnDelete, _btnShowPassword;
    
    public MainForm(PasswordEntryManager manager)
    {
        _manager = manager;
        InitializeComponents();
        LoadPasswords();
    }

    private void InitializeComponents()
    {
        this.Text = "SafePass - Менеджер паролей";
        this.Size = new Size(800, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        _listView = new ListView
        {
            Location = new Point(20, 20),
            Size = new Size(740, 350),
            View = View.Details,
            FullRowSelect = true,
            GridLines = false,
            BorderStyle = BorderStyle.None,
            BackColor = Color.White,
            Font = new Font("Segoe UI", 10),
            HeaderStyle = ColumnHeaderStyle.Nonclickable
        };

        _listView.Columns.Add("ID", 50);
        _listView.Columns.Add("Сервис/Сайт", 200);
        _listView.Columns.Add("Логин", 200);
        _listView.Columns.Add("Дата создания", 150);
        _listView.Columns.Add("Обновлен", 150);

        _btnAdd = new Button()
        {
            Text = "[+] Добавить",
            Location = new Point(20, 400),
            Size = new Size(150, 40),
            BackColor = Color.FromArgb(0, 150, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _btnAdd.Click += BtnAdd_Click;
        
        _btnEdit = new Button
        {
            Text = "[?] Изменить",
            Location = new Point(190, 400),
            Size = new Size(150, 40),
            BackColor = Color.FromArgb(0, 100, 200),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _btnEdit.Click += BtnEdit_Click;
        
        _btnDelete = new Button
        {
            Text = "[-] Удалить",
            Location = new Point(360, 400),
            Size = new Size(150, 40),
            BackColor = Color.FromArgb(150, 20, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _btnDelete.Click += BtnDelete_Click;
        
        _btnShowPassword = new Button
        {
            Text = "[=] Показать пароль",
            Location = new Point(580, 400),
            Size = new Size(180, 40),
            BackColor = Color.FromArgb(225, 160, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        _btnShowPassword.Click += btnShowPassword_Click;
        
        
        this.Controls.Add(_listView);
        this.Controls.Add(_btnAdd);
        this.Controls.Add(_btnEdit);
        this.Controls.Add(_btnShowPassword);
        this.Controls.Add(_btnDelete);
    }

    private void LoadPasswords()
    {
        _listView.Items.Clear();
        var all = _manager.GetAllPasswords().Where(e=>e.Id>0).ToList();

        foreach (var pass in all)
        {
            var password = new ListViewItem(pass.Id.ToString());
            password.SubItems.Add(pass.Service);
            password.SubItems.Add(pass.Username);
            password.SubItems.Add(pass.Created.ToString("yyyy-MM-dd HH:mm:ss"));
            password.SubItems.Add(pass.Updated.ToString("yyyy-MM-dd HH:mm:ss"));
            password.Tag = pass.Id;
            _listView.Items.Add(password);
        }
        
        
    }
    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var addform = new AddEditForm();
        if (addform.ShowDialog() == DialogResult.OK)
        {
            _manager.AddPassword(addform.Service, addform.Username, addform.Password);
            LoadPasswords();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_listView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите запись для редактирования", "Ошибка", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int id = (int)_listView.SelectedItems[0].Tag!;
        var pass = _manager.GetAllPasswords().FirstOrDefault(p => p.Id == id);

        if (pass != null)
        {
            string decryptedPassword = _manager.GetDecryptedPassword(id) ?? "";
            var editform = new AddEditForm(pass.Service ?? "", decryptedPassword, pass.Username ?? "");
        
            if (editform.ShowDialog() == DialogResult.OK)
            {
                _manager.UpdatePassword(id, editform.Service, editform.Username, editform.Password);
                LoadPasswords();
            }
        }

    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_listView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите запись для удаления", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var warn = MessageBox.Show("Вы уверены что хотите удалить пароль?", "Проверка", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (warn == DialogResult.Yes)
        {
            int id = (int)_listView.SelectedItems[0].Tag!;
            _manager.DeletePassword(id);
            LoadPasswords();
        }
    }

    private void btnShowPassword_Click(object? sender, EventArgs e)
    {
        if (_listView.SelectedItems.Count == 0)
        {
            MessageBox.Show("Выберите запись для просмотра", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        int id = (int)_listView.SelectedItems[0].Tag!;
        string pass = _manager.GetDecryptedPassword(id)!;
        Clipboard.SetText(pass);
        MessageBox.Show($"Пароль был скопирован: {pass}", "Пароль", MessageBoxButtons.OK, MessageBoxIcon.Information);
        
    }
}