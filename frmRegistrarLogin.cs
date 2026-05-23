using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmRegistrarLogin : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private Button btnLogin;
        private Button btnCancel;

        public static string CurrentRegistrar { get; private set; } = "";
        public static string CurrentRegistrarRole { get; private set; } = "";
        public static int CurrentRegistrarId { get; private set; } = 0;

        public frmRegistrarLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Registrar Login - Hawassa University";
            this.Size = new Size(500, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(7, 20, 43);

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(220, 60, 60);
            headerPanel.Size = new Size(500, 90);
            headerPanel.Location = new Point(0, 0);

            Label lblIcon = new Label();
            lblIcon.Text = "🔐";
            lblIcon.Font = new Font("Segoe UI", 32);
            lblIcon.ForeColor = Color.White;
            lblIcon.Location = new Point(25, 20);
            lblIcon.Size = new Size(60, 55);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            Label lblTitle = new Label();
            lblTitle.Text = "REGISTRAR LOGIN";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(95, 25);
            lblTitle.Size = new Size(280, 45);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            headerPanel.Controls.Add(lblIcon);
            headerPanel.Controls.Add(lblTitle);

            // Content Panel
            Panel contentPanel = new Panel();
            contentPanel.Location = new Point(0, 90);
            contentPanel.Size = new Size(500, 330);
            contentPanel.Padding = new Padding(40, 50, 40, 30);

            // Username
            Label lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblUsername.ForeColor = Color.White;
            lblUsername.Location = new Point(0, 20);
            lblUsername.Size = new Size(110, 35);
            lblUsername.TextAlign = ContentAlignment.MiddleRight;

            txtUsername = new TextBox();
            txtUsername.Location = new Point(120, 22);
            txtUsername.Size = new Size(250, 35);
            txtUsername.Font = new Font("Segoe UI", 12);
            txtUsername.BackColor = Color.FromArgb(30, 50, 85);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            // Password
            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPassword.ForeColor = Color.White;
            lblPassword.Location = new Point(0, 75);
            lblPassword.Size = new Size(110, 35);
            lblPassword.TextAlign = ContentAlignment.MiddleRight;

            txtPassword = new TextBox();
            txtPassword.Location = new Point(120, 77);
            txtPassword.Size = new Size(250, 35);
            txtPassword.Font = new Font("Segoe UI", 12);
            txtPassword.PasswordChar = '*';
            txtPassword.BackColor = Color.FromArgb(30, 50, 85);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;

            // Show Password
            chkShowPassword = new CheckBox();
            chkShowPassword.Text = "Show Password";
            chkShowPassword.Font = new Font("Segoe UI", 10);
            chkShowPassword.ForeColor = Color.White;
            chkShowPassword.Location = new Point(120, 120);
            chkShowPassword.Size = new Size(130, 30);
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
            };

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Size = new Size(120, 45);
            btnLogin.Location = new Point(120, 170);
            btnLogin.BackColor = Color.FromArgb(220, 60, 60);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += (s, e) => PerformLogin();

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Size = new Size(120, 45);
            btnCancel.Location = new Point(250, 170);
            btnCancel.BackColor = Color.FromArgb(50, 50, 70);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            contentPanel.Controls.Add(lblUsername);
            contentPanel.Controls.Add(txtUsername);
            contentPanel.Controls.Add(lblPassword);
            contentPanel.Controls.Add(txtPassword);
            contentPanel.Controls.Add(chkShowPassword);
            contentPanel.Controls.Add(btnLogin);
            contentPanel.Controls.Add(btnCancel);

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);
        }

        private void PerformLogin()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter your Username", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your Password", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                string hashedPassword = DatabaseHelper.HashPassword(password);
                
                using (SqlConnection conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=HawassaUniversityDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;"))
                {
                    conn.Open();
                    
                    string query = "SELECT RegistrarID, FullName, Username, Role, IsActive FROM Registrars WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CurrentRegistrar = reader["FullName"].ToString();
                                CurrentRegistrarRole = reader["Role"].ToString();
                                CurrentRegistrarId = Convert.ToInt32(reader["RegistrarID"]);
                                
                                reader.Close();
                                
                                // Update last login
                                string updateQuery = "UPDATE Registrars SET LastLogin = GETDATE() WHERE RegistrarID = @ID";
                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@ID", CurrentRegistrarId);
                                    updateCmd.ExecuteNonQuery();
                                }
                                
                                MessageBox.Show($"Welcome {CurrentRegistrar}!\nRole: {CurrentRegistrarRole}", 
                                    "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Username or Password!", 
                                    "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtPassword.Clear();
                                txtPassword.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Logout()
        {
            CurrentRegistrar = "";
            CurrentRegistrarRole = "";
            CurrentRegistrarId = 0;
        }
    }
}
