namespace SafePass_local_password_manager;

public partial class LoginForm : Form
{
    private TextBox txtMasterKey;
    private Button btnLogin;

    public string MasterKey { get; private set; }
    
    public LoginForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "SafePass - Вход";
        this.Size = new Size(400, 200);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        Label lblTitle = new Label
        {
            Text = "SafePass - Менеджер паролей",
            Font = new Font("Arial", 14, FontStyle.Bold),
            Location = new Point(50, 20),
            Size = new Size(300, 30)
        };

        Label lblPassword = new Label
        {
            Text = "Мастер-пароль:",
            Location = new Point(30, 70),
            Size = new Size(120, 20)
        };
        
        txtMasterKey = new TextBox
        {
            Location = new Point(150, 70),
            Size = new Size(200, 25),
            PasswordChar = '*',
            Font = new Font("Arial", 10)
        };
        
        btnLogin = new Button
        {
            Text = "Войти",
            Location = new Point(150, 110),
            Size = new Size(100, 35),
            BackColor = Color.FromArgb(0, 100, 200),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        
        btnLogin.Click += BtnLogin_Click;
        
        
        this.Controls.Add(lblTitle);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtMasterKey);
        this.Controls.Add(btnLogin);

        this.AcceptButton = btnLogin;

        
    }
    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtMasterKey.Text))
        {
            MessageBox.Show("Введите мастер-пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MasterKey = txtMasterKey.Text;
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}