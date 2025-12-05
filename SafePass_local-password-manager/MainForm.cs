namespace SafePass_local_password_manager;

public partial class MainForm : Form
{

    private PasswordEntryManager _manager;
    private ListView listView;
    private Button btnAdd, btnEdit, btnDelete, btnShowPassword;
    
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

        listView = new ListView
        {
            Location = new Point(20, 20),
            Size = new Size(740, 350),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
        };

        listView.Columns.Add("ID", 50);
        listView.Columns.Add("Сервис/Сайт", 200);
        listView.Columns.Add("Логин", 200);
        listView.Columns.Add("Дата создания", 150);

        btnAdd = new Button()
        {
            Text = "[+] Добавить",
            Location = new Point(20, 400),
            Size = new Size(150, 40),
            BackColor = Color.FromArgb(0, 150, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        // btnAdd += BtnAdd_Click;
        
        btnEdit = new Button
        {
            Text = "[?] Изменить",
            Location = new Point(190, 400),
            Size = new Size(150, 40),
            BackColor = Color.FromArgb(0, 100, 200),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        // btnEdit += BtnEdit_Click;
        
        btnDelete = new Button
        {
            Text = "[-] Удалить",
            Location = new Point(360, 400),
            Size = new Size(150, 40),
            BackColor = Color.FromArgb(150, 20, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        // btnDelete += BtnDelete_Click;
        
        btnShowPassword = new Button
        {
            Text = "[=] Показать пароль",
            Location = new Point(580, 400),
            Size = new Size(180, 40),
            BackColor = Color.FromArgb(225, 160, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Arial", 10, FontStyle.Bold)
        };
        // btnShowPassword+= btnShowPassword_Click;
        
        
        this.Controls.Add(listView);
        this.Controls.Add(btnAdd);
        this.Controls.Add(btnEdit);
        this.Controls.Add(btnShowPassword);
        this.Controls.Add(btnDelete);
    }

    private void LoadPasswords()
    {
        listView.Items.Clear();
        var all = _manager.GetAllPasswords();

        foreach (var pass in all)
        {
            var password = new ListViewItem(pass.Id.ToString());
            password.SubItems.Add(pass.Service);
            password.SubItems.Add(pass.Username);
            password.SubItems.Add(pass.Created.ToString("yyyy-MM-dd HH:mm:ss"));
            password.Tag = pass.Id;
            listView.Items.Add(password);
        }
        
        
    }

    /*private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var addform = new AddForm();
        if (addform.ShowDialog() == DialogResult.OK)
        {
            // _manager.AddPassword();
        }
    }*/
}