using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmTeacherLogin : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;

        public frmTeacherLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Teacher Login - Hawassa University";
            this.Size = new Size(550, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(7, 20, 43);

            // Header
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(0, 200, 100);
            headerPanel.Size = new Size(550, 90);
            headerPanel.Location = new Point(0, 0);

            Label lblHeader = new Label();
            lblHeader.Text = "TEACHER LOGIN";
            lblHeader.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Size = new Size(350, 40);
            lblHeader.Location = new Point(100, 25);
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;

            headerPanel.Controls.Add(lblHeader);

            // Username
            Label lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblUsername.ForeColor = Color.White;
            lblUsername.Size = new Size(100, 35);
            lblUsername.Location = new Point(70, 130);
            lblUsername.TextAlign = ContentAlignment.MiddleRight;

            txtUsername = new TextBox();
            txtUsername.Size = new Size(250, 35);
            txtUsername.Location = new Point(180, 132);
            txtUsername.Font = new Font("Segoe UI", 12);
            txtUsername.BackColor = Color.FromArgb(30, 50, 85);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            // Password
            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPassword.ForeColor = Color.White;
            lblPassword.Size = new Size(100, 35);
            lblPassword.Location = new Point(70, 185);
            lblPassword.TextAlign = ContentAlignment.MiddleRight;

            txtPassword = new TextBox();
            txtPassword.Size = new Size(250, 35);
            txtPassword.Location = new Point(180, 187);
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
            chkShowPassword.Size = new Size(130, 30);
            chkShowPassword.Location = new Point(180, 230);
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
            };

            // Login Button
            Button btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Size = new Size(120, 45);
            btnLogin.Location = new Point(180, 275);
            btnLogin.BackColor = Color.FromArgb(0, 200, 100);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += (s, e) => PerformLogin();

            // Back Button
            Button btnBack = new Button();
            btnBack.Text = "BACK";
            btnBack.Size = new Size(120, 45);
            btnBack.Location = new Point(310, 275);
            btnBack.BackColor = Color.FromArgb(50, 50, 70);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();

            // Info Label (NO REGISTRATION LINK)
            Label lblInfo = new Label();
            lblInfo.Text = "Contact the Registrar's Office for account creation";
            lblInfo.Font = new Font("Segoe UI", 9);
            lblInfo.ForeColor = Color.FromArgb(150, 150, 180);
            lblInfo.Location = new Point(130, 350);
            lblInfo.Size = new Size(350, 25);
            lblInfo.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(headerPanel);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(chkShowPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnBack);
            this.Controls.Add(lblInfo);
        }

        private async void PerformLogin()
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

            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;

            try
            {
                string hashedPassword = DatabaseHelper.HashPassword(password);
                
                using (SqlConnection conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=HawassaUniversityDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;"))
                {
                    conn.Open();
                    
                    string query = "SELECT * FROM Teachers WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int teacherId = Convert.ToInt32(reader["TeacherID"]);
                                string teacherName = reader["FullName"].ToString();
                                
                                MessageBox.Show($"Welcome {teacherName}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                
                                this.Hide();
                                var dashboard = new frmTeacherDashboard(teacherId, teacherName, null);
                                dashboard.ShowDialog();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Username or Password!\n\nContact Registrar for assistance.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtPassword.Clear();
                                txtPassword.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.Enabled = true;
            }
        }
    }
}
