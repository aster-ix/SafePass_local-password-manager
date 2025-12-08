namespace SafePass_local_password_manager;

public partial class AddEditForm : Form
{
    private Button _btnSave, _btnCancel;
    private TextBox _tbService, _tbPassword, _tbUsername;
    
    public string Service { get;private set; }
    public string Password { get; private set; }
    public string Username { get; private set; }
    
    
    public AddEditForm(string service= "", string password= "", string username = "")
    {
       InitializeComponents();
       _tbService!.Text = service;
       _tbPassword!.Text = password;
       _tbUsername!.Text = username;
    }

    private void InitializeComponents()
    {
        this.Text = "Добавить/Изменить пароль";
        this.Size = new Size(400, 250);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        Label lblService = new Label { Text = "Сервис:", Location = new Point(20, 30), Size = new Size(100, 20) };
        _tbService = new TextBox { Location = new Point(130, 30), Size = new Size(230, 25) };

        Label lblUsername = new Label { Text = "Логин:", Location = new Point(20, 70), Size = new Size(100, 20) };
        _tbUsername = new TextBox { Location = new Point(130, 70), Size = new Size(230, 25) };

        Label lblPassword = new Label { Text = "Пароль:", Location = new Point(20, 110), Size = new Size(100, 20) };
        _tbPassword = new TextBox { Location = new Point(130, 110), Size = new Size(230, 25) };
        
        _btnSave = new Button
        {
            Text = "Сохранить",
            Location = new Point(130, 160),
            Size = new Size(100, 35),
            BackColor = Color.FromArgb(0, 100, 200),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        _btnSave.Click += BtnSave_Click;
        
        _btnCancel = new Button
        {
            Text = "Отмена",
            Location = new Point(260, 160),
            Size = new Size(100, 35),
            BackColor = Color.Gray,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat  
        };
        _btnCancel.Click += (_,_) =>{ this.DialogResult = DialogResult.Cancel; this.Close(); };
        
        this.Controls.AddRange(new Control[] { lblService, _tbService, lblUsername, _tbUsername, lblPassword, _tbPassword, _btnSave, _btnCancel });
    }
    
    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_tbService.Text) || string.IsNullOrWhiteSpace(_tbUsername.Text) || string.IsNullOrWhiteSpace(_tbPassword.Text))
        {
            MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Service = _tbService.Text;
        Username = _tbUsername.Text;
        Password = _tbPassword.Text;
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}