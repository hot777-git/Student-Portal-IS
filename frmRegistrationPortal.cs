using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmRegistrationPortal : Form
    {
        private TabControl tabControl;
        private Label lblRegistrarInfo;
        
        // Student Registration controls
        private TextBox txtFullName, txtUsername, txtPassword, txtConfirmPassword, txtEmail, txtPhone;
        private ComboBox cmbGender, cmbDepartment;
        private DateTimePicker dtpDateOfBirth;
        
        // Head Teacher controls
        private TextBox txtHeadFullName, txtHeadUsername, txtHeadPassword, txtHeadConfirmPassword;
        private ComboBox cmbHeadDepartment, cmbHeadTeacher;
        private RadioButton rbNewHead, rbExistingTeacher;
        private DataGridView dgvHeadList;
        
        // User Management controls
        private ComboBox cmbUserType;
        private DataGridView dgvUsers;
        
        // Password Reset controls
        private ComboBox cmbResetUserType, cmbResetUser;
        private TextBox txtNewPassword, txtConfirmNewPassword;
        
        // Audit Log controls
        private TextBox txtAuditFilter;
        private DataGridView dgvAudit;

        public frmRegistrationPortal()
        {
            InitializeComponent();
            LoadAllData();
        }

        private void InitializeComponent()
        {
            this.Text = "Registration Portal - Hawassa University";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 40, 60);
            this.MinimumSize = new Size(1000, 650);

            // Header
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(220, 60, 60);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 90;

            Label lblIcon = new Label();
            lblIcon.Text = "📋";
            lblIcon.Font = new Font("Segoe UI", 28);
            lblIcon.ForeColor = Color.White;
            lblIcon.Location = new Point(20, 20);
            lblIcon.Size = new Size(50, 50);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            Label lblHeader = new Label();
            lblHeader.Text = "REGISTRATION PORTAL";
            lblHeader.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Location = new Point(80, 25);
            lblHeader.Size = new Size(350, 40);
            lblHeader.TextAlign = ContentAlignment.MiddleLeft;

            lblRegistrarInfo = new Label();
            lblRegistrarInfo.Text = $"Logged in as: {frmRegistrarLogin.CurrentRegistrar}";
            lblRegistrarInfo.Font = new Font("Segoe UI", 9);
            lblRegistrarInfo.ForeColor = Color.FromArgb(255, 200, 200);
            lblRegistrarInfo.Location = new Point(80, 60);
            lblRegistrarInfo.Size = new Size(400, 25);

            Button btnLogout = new Button();
            btnLogout.Text = "🚪 Logout";
            btnLogout.Size = new Size(100, 35);
            btnLogout.Location = new Point(1050, 25);
            btnLogout.BackColor = Color.FromArgb(150, 40, 40);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.Click += (s, e) => { frmRegistrarLogin.Logout(); this.Close(); };

            headerPanel.Controls.Add(lblIcon);
            headerPanel.Controls.Add(lblHeader);
            headerPanel.Controls.Add(lblRegistrarInfo);
            headerPanel.Controls.Add(btnLogout);

            // Tab Control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 11);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.ItemSize = new Size(160, 40);
            tabControl.BackColor = Color.FromArgb(30, 40, 60);

            // Create tabs - NO Generate ID tab
            TabPage tabStudentReg = new TabPage("📝 Register Student");
            TabPage tabHeadTeacher = new TabPage("👨‍💼 Assign Head");
            TabPage tabUserMgmt = new TabPage("👥 User Mgmt");
            TabPage tabPasswordReset = new TabPage("🔑 Reset Password");
            TabPage tabAuditLog = new TabPage("📜 Audit Log");

            // Setup tabs
            SetupStudentRegTab(tabStudentReg);
            SetupHeadTeacherTab(tabHeadTeacher);
            SetupUserManagementTab(tabUserMgmt);
            SetupPasswordResetTab(tabPasswordReset);
            SetupAuditLogTab(tabAuditLog);

            tabControl.TabPages.Add(tabStudentReg);
            tabControl.TabPages.Add(tabHeadTeacher);
            tabControl.TabPages.Add(tabUserMgmt);
            tabControl.TabPages.Add(tabPasswordReset);
            tabControl.TabPages.Add(tabAuditLog);

            // Close Button
            Button btnClose = new Button();
            btnClose.Text = "CLOSE";
            btnClose.Size = new Size(120, 40);
            btnClose.Location = new Point(540, 720);
            btnClose.BackColor = Color.FromArgb(220, 60, 60);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(tabControl);
            this.Controls.Add(headerPanel);
            this.Controls.Add(btnClose);
        }

        private void SetupStudentRegTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(30);
            panel.AutoScroll = true;
            panel.BackColor = Color.FromArgb(30, 40, 60);

            int y = 20;
            int spacing = 50;

            AddField(panel, "Full Name:", ref txtFullName, ref y, spacing);
            AddField(panel, "Username:", ref txtUsername, ref y, spacing);
            AddField(panel, "Email:", ref txtEmail, ref y, spacing);
            AddField(panel, "Phone:", ref txtPhone, ref y, spacing);
            AddPasswordField(panel, "Password:", ref txtPassword, ref y, spacing);
            AddPasswordField(panel, "Confirm:", ref txtConfirmPassword, ref y, spacing);
            AddCombo(panel, "Gender:", ref cmbGender, new[] { "Male", "Female", "Other" }, ref y, spacing);
            AddDatePicker(panel, "Date of Birth:", ref dtpDateOfBirth, ref y, spacing);
            AddCombo(panel, "Department:", ref cmbDepartment, new[] { "Computer Science", "Software Engineering", "Information Technology", "Electrical Engineering", "Civil Engineering", "Business Management" }, ref y, spacing);

            y += 20;
            
            Button btnRegister = new Button();
            btnRegister.Text = "REGISTER STUDENT";
            btnRegister.Size = new Size(200, 45);
            btnRegister.Location = new Point(140, y);
            btnRegister.BackColor = Color.FromArgb(220, 60, 60);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRegister.Click += (s, e) => RegisterStudent();
            panel.Controls.Add(btnRegister);

            tab.Controls.Add(panel);
        }

        private void SetupHeadTeacherTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;
            panel.BackColor = Color.FromArgb(30, 40, 60);

            int y = 20;

            rbNewHead = new RadioButton();
            rbNewHead.Text = "Create New Head Teacher";
            rbNewHead.Font = new Font("Segoe UI", 10);
            rbNewHead.ForeColor = Color.White;
            rbNewHead.Location = new Point(20, y);
            rbNewHead.Size = new Size(200, 30);
            rbNewHead.Checked = true;
            panel.Controls.Add(rbNewHead);

            rbExistingTeacher = new RadioButton();
            rbExistingTeacher.Text = "Assign Existing Teacher";
            rbExistingTeacher.Font = new Font("Segoe UI", 10);
            rbExistingTeacher.ForeColor = Color.White;
            rbExistingTeacher.Location = new Point(240, y);
            rbExistingTeacher.Size = new Size(200, 30);
            panel.Controls.Add(rbExistingTeacher);
            y += 50;

            // New Head Panel
            Panel newHeadPanel = new Panel();
            newHeadPanel.Location = new Point(0, y);
            newHeadPanel.Size = new Size(600, 280);
            newHeadPanel.BackColor = Color.FromArgb(20, 35, 65);

            int ny = 20;
            
            Label lblFullName = new Label();
            lblFullName.Text = "Full Name:";
            lblFullName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblFullName.ForeColor = Color.White;
            lblFullName.Location = new Point(20, ny);
            lblFullName.Size = new Size(120, 35);
            newHeadPanel.Controls.Add(lblFullName);

            txtHeadFullName = new TextBox();
            txtHeadFullName.Name = "txtHeadFullName";
            txtHeadFullName.Location = new Point(150, ny);
            txtHeadFullName.Size = new Size(300, 35);
            txtHeadFullName.BackColor = Color.FromArgb(30, 50, 85);
            txtHeadFullName.ForeColor = Color.White;
            newHeadPanel.Controls.Add(txtHeadFullName);
            ny += 50;

            AddPanelField(newHeadPanel, "Username:", ref txtHeadUsername, ref ny, 50);
            AddPanelPassword(newHeadPanel, "Password:", ref txtHeadPassword, ref ny, 50);
            AddPanelPassword(newHeadPanel, "Confirm:", ref txtHeadConfirmPassword, ref ny, 50);
            
            Label lblDept = new Label();
            lblDept.Text = "Department:";
            lblDept.Font = new Font("Segoe UI", 11);
            lblDept.ForeColor = Color.White;
            lblDept.Location = new Point(20, ny);
            lblDept.Size = new Size(120, 35);
            newHeadPanel.Controls.Add(lblDept);

            cmbHeadDepartment = new ComboBox();
            cmbHeadDepartment.Location = new Point(150, ny);
            cmbHeadDepartment.Size = new Size(300, 35);
            cmbHeadDepartment.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHeadDepartment.Items.AddRange(new[] { "Computer Science", "Software Engineering", "Information Technology", "Electrical Engineering", "Civil Engineering", "Business Management" });
            cmbHeadDepartment.SelectedIndex = 0;
            newHeadPanel.Controls.Add(cmbHeadDepartment);

            panel.Controls.Add(newHeadPanel);

            // Existing Teacher Panel
            Panel existingPanel = new Panel();
            existingPanel.Location = new Point(0, y);
            existingPanel.Size = new Size(600, 150);
            existingPanel.BackColor = Color.FromArgb(20, 35, 65);
            existingPanel.Visible = false;

            Label lblTeacher = new Label();
            lblTeacher.Text = "Select Teacher:";
            lblTeacher.Font = new Font("Segoe UI", 11);
            lblTeacher.ForeColor = Color.White;
            lblTeacher.Location = new Point(20, 20);
            lblTeacher.Size = new Size(120, 35);
            existingPanel.Controls.Add(lblTeacher);

            cmbHeadTeacher = new ComboBox();
            cmbHeadTeacher.Location = new Point(150, 20);
            cmbHeadTeacher.Size = new Size(300, 35);
            cmbHeadTeacher.DropDownStyle = ComboBoxStyle.DropDownList;
            existingPanel.Controls.Add(cmbHeadTeacher);

            Label lblHeadDept = new Label();
            lblHeadDept.Text = "Department:";
            lblHeadDept.Font = new Font("Segoe UI", 11);
            lblHeadDept.ForeColor = Color.White;
            lblHeadDept.Location = new Point(20, 70);
            lblHeadDept.Size = new Size(120, 35);
            existingPanel.Controls.Add(lblHeadDept);

            ComboBox cmbExistingDept = new ComboBox();
            cmbExistingDept.Location = new Point(150, 70);
            cmbExistingDept.Size = new Size(300, 35);
            cmbExistingDept.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbExistingDept.Items.AddRange(new[] { "Computer Science", "Software Engineering", "Information Technology", "Electrical Engineering", "Civil Engineering", "Business Management" });
            cmbExistingDept.SelectedIndex = 0;
            existingPanel.Controls.Add(cmbExistingDept);

            panel.Controls.Add(existingPanel);

            rbNewHead.CheckedChanged += (s, e) => { newHeadPanel.Visible = true; existingPanel.Visible = false; };
            rbExistingTeacher.CheckedChanged += (s, e) => { newHeadPanel.Visible = false; existingPanel.Visible = true; LoadTeachers(); };

            y += 290;

            Button btnAssign = new Button();
            btnAssign.Text = "ASSIGN HEAD TEACHER";
            btnAssign.Size = new Size(200, 45);
            btnAssign.Location = new Point(0, y);
            btnAssign.BackColor = Color.FromArgb(220, 60, 60);
            btnAssign.ForeColor = Color.White;
            btnAssign.FlatStyle = FlatStyle.Flat;
            btnAssign.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnAssign.Click += (s, e) => AssignHeadTeacher();
            panel.Controls.Add(btnAssign);

            y += 60;

            Label lblHeadList = new Label();
            lblHeadList.Text = "Assigned Head Teachers";
            lblHeadList.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblHeadList.ForeColor = Color.White;
            lblHeadList.Location = new Point(0, y);
            lblHeadList.Size = new Size(300, 35);
            panel.Controls.Add(lblHeadList);

            dgvHeadList = new DataGridView();
            dgvHeadList.Location = new Point(0, y + 40);
            dgvHeadList.Size = new Size(950, 300);
            dgvHeadList.BackgroundColor = Color.White;
            dgvHeadList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHeadList.ReadOnly = true;
            panel.Controls.Add(dgvHeadList);

            tab.Controls.Add(panel);
        }

        private void SetupUserManagementTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.BackColor = Color.FromArgb(30, 40, 60);

            Panel filterPanel = new Panel();
            filterPanel.Size = new Size(950, 50);
            filterPanel.Location = new Point(0, 0);
            filterPanel.BackColor = Color.FromArgb(20, 35, 65);

            Label lblFilter = new Label();
            lblFilter.Text = "Show:";
            lblFilter.Font = new Font("Segoe UI", 11);
            lblFilter.ForeColor = Color.White;
            lblFilter.Location = new Point(20, 10);
            lblFilter.Size = new Size(60, 30);
            filterPanel.Controls.Add(lblFilter);

            cmbUserType = new ComboBox();
            cmbUserType.Location = new Point(90, 8);
            cmbUserType.Size = new Size(200, 35);
            cmbUserType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUserType.Items.AddRange(new[] { "All Users", "Students", "Teachers", "Head Teachers" });
            cmbUserType.SelectedIndex = 0;
            cmbUserType.SelectedIndexChanged += (s, e) => LoadUserList();
            filterPanel.Controls.Add(cmbUserType);

            panel.Controls.Add(filterPanel);

            dgvUsers = new DataGridView();
            dgvUsers.Location = new Point(0, 60);
            dgvUsers.Size = new Size(950, 450);
            dgvUsers.BackgroundColor = Color.White;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.ReadOnly = true;
            panel.Controls.Add(dgvUsers);

            Button btnActivate = new Button();
            btnActivate.Text = "✓ Activate User";
            btnActivate.Size = new Size(130, 40);
            btnActivate.Location = new Point(0, 530);
            btnActivate.BackColor = Color.FromArgb(0, 200, 100);
            btnActivate.ForeColor = Color.White;
            btnActivate.FlatStyle = FlatStyle.Flat;
            btnActivate.Click += (s, e) => ActivateUser();
            panel.Controls.Add(btnActivate);

            Button btnDeactivate = new Button();
            btnDeactivate.Text = "✗ Deactivate User";
            btnDeactivate.Size = new Size(140, 40);
            btnDeactivate.Location = new Point(140, 530);
            btnDeactivate.BackColor = Color.FromArgb(220, 60, 60);
            btnDeactivate.ForeColor = Color.White;
            btnDeactivate.FlatStyle = FlatStyle.Flat;
            btnDeactivate.Click += (s, e) => DeactivateUser();
            panel.Controls.Add(btnDeactivate);

            Button btnDelete = new Button();
            btnDelete.Text = "🗑️ Delete User";
            btnDelete.Size = new Size(130, 40);
            btnDelete.Location = new Point(290, 530);
            btnDelete.BackColor = Color.FromArgb(255, 100, 0);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Click += (s, e) => DeleteUser();
            panel.Controls.Add(btnDelete);

            Button btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Location = new Point(430, 530);
            btnRefresh.BackColor = Color.FromArgb(0, 150, 255);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Click += (s, e) => LoadUserList();
            panel.Controls.Add(btnRefresh);

            tab.Controls.Add(panel);
        }

        private void SetupPasswordResetTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(30);
            panel.BackColor = Color.FromArgb(30, 40, 60);

            int y = 20;
            int spacing = 55;

            AddCombo(panel, "User Type:", ref cmbResetUserType, new[] { "Student", "Teacher", "Head Teacher" }, ref y, spacing);
            cmbResetUserType.SelectedIndexChanged += (s, e) => LoadUsersForReset();

            Label lblUser = new Label();
            lblUser.Text = "Select User:";
            lblUser.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblUser.ForeColor = Color.White;
            lblUser.Location = new Point(0, y);
            lblUser.Size = new Size(120, 35);
            panel.Controls.Add(lblUser);

            cmbResetUser = new ComboBox();
            cmbResetUser.Location = new Point(130, y);
            cmbResetUser.Size = new Size(350, 35);
            cmbResetUser.DropDownStyle = ComboBoxStyle.DropDownList;
            panel.Controls.Add(cmbResetUser);
            y += spacing;

            AddPasswordField(panel, "New Password:", ref txtNewPassword, ref y, spacing);
            AddPasswordField(panel, "Confirm:", ref txtConfirmNewPassword, ref y, spacing);

            y += 20;
            Button btnReset = new Button();
            btnReset.Text = "RESET PASSWORD";
            btnReset.Size = new Size(200, 45);
            btnReset.Location = new Point(130, y);
            btnReset.BackColor = Color.FromArgb(220, 60, 60);
            btnReset.ForeColor = Color.White;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnReset.Click += (s, e) => ResetPassword();
            panel.Controls.Add(btnReset);

            LoadUsersForReset();
            tab.Controls.Add(panel);
        }

        private void SetupAuditLogTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.BackColor = Color.FromArgb(30, 40, 60);

            Panel filterPanel = new Panel();
            filterPanel.Size = new Size(950, 50);
            filterPanel.Location = new Point(0, 0);
            filterPanel.BackColor = Color.FromArgb(20, 35, 65);

            Label lblFilter = new Label();
            lblFilter.Text = "Filter by User:";
            lblFilter.Font = new Font("Segoe UI", 11);
            lblFilter.ForeColor = Color.White;
            lblFilter.Location = new Point(20, 10);
            lblFilter.Size = new Size(100, 30);
            filterPanel.Controls.Add(lblFilter);

            txtAuditFilter = new TextBox();
            txtAuditFilter.Location = new Point(130, 8);
            txtAuditFilter.Size = new Size(200, 35);
            txtAuditFilter.BackColor = Color.FromArgb(30, 50, 85);
            txtAuditFilter.ForeColor = Color.White;
            filterPanel.Controls.Add(txtAuditFilter);

            Button btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.Location = new Point(350, 8);
            btnRefresh.BackColor = Color.FromArgb(0, 150, 255);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Click += (s, e) => LoadAuditLog();
            filterPanel.Controls.Add(btnRefresh);

            panel.Controls.Add(filterPanel);

            dgvAudit = new DataGridView();
            dgvAudit.Location = new Point(0, 60);
            dgvAudit.Size = new Size(950, 550);
            dgvAudit.BackgroundColor = Color.White;
            dgvAudit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAudit.ReadOnly = true;
            panel.Controls.Add(dgvAudit);

            LoadAuditLog();
            tab.Controls.Add(panel);
        }

        // ==================================================
        // HELPER METHODS
        // ==================================================

        private void AddField(Panel panel, string label, ref TextBox box, ref int y, int spacing)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(0, y);
            lbl.Size = new Size(120, 35);
            panel.Controls.Add(lbl);

            box = new TextBox();
            box.Location = new Point(130, y);
            box.Size = new Size(300, 35);
            box.Font = new Font("Segoe UI", 11);
            box.BackColor = Color.FromArgb(30, 50, 85);
            box.ForeColor = Color.White;
            panel.Controls.Add(box);

            y += spacing;
        }

        private void AddPasswordField(Panel panel, string label, ref TextBox box, ref int y, int spacing)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(0, y);
            lbl.Size = new Size(120, 35);
            panel.Controls.Add(lbl);

            box = new TextBox();
            box.Location = new Point(130, y);
            box.Size = new Size(300, 35);
            box.Font = new Font("Segoe UI", 11);
            box.PasswordChar = '*';
            box.BackColor = Color.FromArgb(30, 50, 85);
            box.ForeColor = Color.White;
            panel.Controls.Add(box);

            y += spacing;
        }

        private void AddCombo(Panel panel, string label, ref ComboBox box, string[] items, ref int y, int spacing)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(0, y);
            lbl.Size = new Size(120, 35);
            panel.Controls.Add(lbl);

            box = new ComboBox();
            box.Location = new Point(130, y);
            box.Size = new Size(300, 35);
            box.DropDownStyle = ComboBoxStyle.DropDownList;
            box.Items.AddRange(items);
            box.SelectedIndex = 0;
            panel.Controls.Add(box);

            y += spacing;
        }

        private void AddDatePicker(Panel panel, string label, ref DateTimePicker picker, ref int y, int spacing)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(0, y);
            lbl.Size = new Size(120, 35);
            panel.Controls.Add(lbl);

            picker = new DateTimePicker();
            picker.Location = new Point(130, y);
            picker.Size = new Size(300, 35);
            picker.MaxDate = DateTime.Today.AddYears(-16);
            picker.Value = DateTime.Today.AddYears(-18);
            panel.Controls.Add(picker);

            y += spacing;
        }

        private void AddPanelField(Panel panel, string label, ref TextBox box, ref int y, int spacing)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(20, y);
            lbl.Size = new Size(120, 35);
            panel.Controls.Add(lbl);

            box = new TextBox();
            box.Location = new Point(150, y);
            box.Size = new Size(300, 35);
            box.Font = new Font("Segoe UI", 10);
            box.BackColor = Color.FromArgb(30, 50, 85);
            box.ForeColor = Color.White;
            panel.Controls.Add(box);

            y += spacing;
        }

        private void AddPanelPassword(Panel panel, string label, ref TextBox box, ref int y, int spacing)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(20, y);
            lbl.Size = new Size(120, 35);
            panel.Controls.Add(lbl);

            box = new TextBox();
            box.Location = new Point(150, y);
            box.Size = new Size(300, 35);
            box.Font = new Font("Segoe UI", 10);
            box.PasswordChar = '*';
            box.BackColor = Color.FromArgb(30, 50, 85);
            box.ForeColor = Color.White;
            panel.Controls.Add(box);

            y += spacing;
        }

        // ==================================================
        // DATA LOADING METHODS
        // ==================================================

        private void LoadAllData()
        {
            LoadHeadTeacherList();
            LoadUserList();
            LoadAuditLog();
        }

        private void LoadHeadTeacherList()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT Username, Department, AssignedBy, CreatedAt FROM HeadCredentials ORDER BY Department", null);
                dgvHeadList.DataSource = dt;
            }
            catch { }
        }

        private void LoadUserList()
        {
            string userType = cmbUserType?.SelectedItem?.ToString() ?? "All Users";
            string query = "";
            if (userType == "Students")
                query = "SELECT 'Student' as Type, Username, StudentID as ID, FirstName, Status FROM Students";
            else if (userType == "Teachers")
                query = "SELECT 'Teacher' as Type, Username, TeacherID as ID, FullName, CASE WHEN IsActive=1 THEN 'Active' ELSE 'Inactive' END as Status FROM Teachers";
            else if (userType == "Head Teachers")
                query = "SELECT 'Head Teacher' as Type, Username, CredID as ID, Department, 'Active' as Status FROM HeadCredentials";
            else
                query = @"SELECT 'Student' as Type, Username, StudentID as ID, FirstName, Status FROM Students
                          UNION ALL
                          SELECT 'Teacher' as Type, Username, CAST(TeacherID as VARCHAR) as ID, FullName, CASE WHEN IsActive=1 THEN 'Active' ELSE 'Inactive' END as Status FROM Teachers
                          UNION ALL
                          SELECT 'Head Teacher' as Type, Username, CAST(CredID as VARCHAR) as ID, Department, 'Active' as Status FROM HeadCredentials";
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(query, null);
                dgvUsers.DataSource = dt;
            }
            catch { }
        }

        private void LoadAuditLog()
        {
            try
            {
                string filter = txtAuditFilter?.Text.Trim() ?? "";
                string query = "SELECT LogID, ActionType, TableName, RecordID, LEFT(NewValue, 100) as Details, PerformedBy, PerformedAt FROM AuditLog";
                if (!string.IsNullOrEmpty(filter)) query += " WHERE PerformedBy LIKE @Filter";
                query += " ORDER BY PerformedAt DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(query, string.IsNullOrEmpty(filter) ? null : new[] { new SqlParameter("@Filter", $"%{filter}%") });
                dgvAudit.DataSource = dt;
            }
            catch { }
        }

        private void LoadUsersForReset()
        {
            try
            {
                string userType = cmbResetUserType?.SelectedItem?.ToString() ?? "Student";
                DataTable dt = new DataTable();
                
                if (userType == "Student")
                {
                    dt = DatabaseHelper.ExecuteQuery("SELECT Username, FirstName + ' ' + FatherName as DisplayName FROM Students WHERE Status = 'Active'", null);
                    cmbResetUser.DisplayMember = "DisplayName";
                    cmbResetUser.ValueMember = "Username";
                }
                else if (userType == "Teacher")
                {
                    dt = DatabaseHelper.ExecuteQuery("SELECT Username, FullName as DisplayName FROM Teachers WHERE IsActive = 1", null);
                    cmbResetUser.DisplayMember = "DisplayName";
                    cmbResetUser.ValueMember = "Username";
                }
                else if (userType == "Head Teacher")
                {
                    dt = DatabaseHelper.ExecuteQuery("SELECT Username, Username as DisplayName FROM HeadCredentials", null);
                    cmbResetUser.DisplayMember = "DisplayName";
                    cmbResetUser.ValueMember = "Username";
                }
                
                cmbResetUser.DataSource = dt;
                if (dt.Rows.Count > 0) cmbResetUser.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadTeachers()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT TeacherID, FullName FROM Teachers WHERE IsActive = 1", null);
                cmbHeadTeacher.DisplayMember = "FullName";
                cmbHeadTeacher.ValueMember = "TeacherID";
                cmbHeadTeacher.DataSource = dt;
            }
            catch { }
        }

        // ==================================================
        // BUSINESS METHODS
        // ==================================================

        private void RegisterStudent()
        {
            if (string.IsNullOrEmpty(txtFullName?.Text)) { MessageBox.Show("Please enter full name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtFullName.Focus(); return; }
            if (string.IsNullOrEmpty(txtUsername?.Text)) { MessageBox.Show("Please enter username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtUsername.Focus(); return; }
            
            string email = txtEmail?.Text.Trim() ?? "";
            if (string.IsNullOrEmpty(email)) { MessageBox.Show("Please enter email.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtEmail.Focus(); return; }
            if (!DatabaseHelper.IsValidEmail(email)) { MessageBox.Show("Invalid email format.\n\nExample: username@gmail.com", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtEmail.Focus(); return; }
            
            if (string.IsNullOrEmpty(txtPassword?.Text)) { MessageBox.Show("Please enter password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPassword.Focus(); return; }
            if (txtPassword.Text != txtConfirmPassword.Text) { MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtPassword.Clear(); txtConfirmPassword.Clear(); txtPassword.Focus(); return; }

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) as Count FROM Students", null);
                int count = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["Count"]) : 0;
                int year = DateTime.Now.Year % 100;
                string studentId = $"HwU/{(count + 1):D4}/{year:D2}";
                string passwordHash = DatabaseHelper.HashPassword(txtPassword.Text);
                string[] nameParts = txtFullName.Text.Split(' ');
                string firstName = nameParts.Length > 0 ? nameParts[0] : txtFullName.Text;
                string fatherName = nameParts.Length > 1 ? nameParts[1] : "";

                int result = DatabaseHelper.ExecuteNonQuery(
                    @"INSERT INTO Students (StudentID, FirstName, FatherName, GrandFatherName, Gender, DateOfBirth, Email, PhoneNumber, Department, YearOfStudy, Semester, AcademicYear, Username, PasswordHash, Status, EnrollmentDate)
                      VALUES (@StudentID, @FirstName, @FatherName, @GrandFatherName, @Gender, @DOB, @Email, @Phone, @Dept, 1, 1, @Year, @Username, @Hash, 'Active', GETDATE())",
                    new[] {
                        new SqlParameter("@StudentID", studentId),
                        new SqlParameter("@FirstName", firstName),
                        new SqlParameter("@FatherName", fatherName),
                        new SqlParameter("@GrandFatherName", fatherName),
                        new SqlParameter("@Gender", cmbGender.SelectedItem?.ToString()),
                        new SqlParameter("@DOB", dtpDateOfBirth.Value),
                        new SqlParameter("@Email", email),
                        new SqlParameter("@Phone", txtPhone.Text ?? ""),
                        new SqlParameter("@Dept", cmbDepartment.SelectedItem?.ToString()),
                        new SqlParameter("@Year", DateTime.Now.Year + "/" + (DateTime.Now.Year + 1)),
                        new SqlParameter("@Username", txtUsername.Text),
                        new SqlParameter("@Hash", passwordHash)
                    });
                if (result > 0)
                {
                    MessageBox.Show($"✅ Student registered successfully!\n\nStudent ID: {studentId}\nUsername: {txtUsername.Text}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearStudentForm();
                    LoadUserList();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("UNIQUE"))
                    MessageBox.Show("Username already exists. Please choose a different username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ClearStudentForm()
        {
            txtFullName?.Clear(); txtUsername?.Clear();
            txtPassword?.Clear(); txtConfirmPassword?.Clear(); txtEmail?.Clear(); txtPhone?.Clear();
        }

        private void AssignHeadTeacher()
        {
            try
            {
                if (rbNewHead.Checked)
                {
                    if (txtHeadFullName == null || string.IsNullOrEmpty(txtHeadFullName.Text)) 
                    { 
                        MessageBox.Show("Please enter full name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                        return; 
                    }
                    if (string.IsNullOrEmpty(txtHeadUsername?.Text)) { MessageBox.Show("Please enter username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (string.IsNullOrEmpty(txtHeadPassword?.Text)) { MessageBox.Show("Please enter password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (txtHeadPassword.Text != txtHeadConfirmPassword.Text) { MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    string hash = DatabaseHelper.HashPassword(txtHeadPassword.Text);
                    int result = DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO HeadCredentials (Username, PasswordHash, Department, AssignedBy, CreatedAt)
                          VALUES (@Username, @Hash, @Dept, @By, GETDATE())",
                        new[] {
                            new SqlParameter("@Username", txtHeadUsername.Text),
                            new SqlParameter("@Hash", hash),
                            new SqlParameter("@Dept", cmbHeadDepartment.SelectedItem?.ToString()),
                            new SqlParameter("@By", frmRegistrarLogin.CurrentRegistrar)
                        });
                    if (result > 0) { MessageBox.Show($"Head Teacher '{txtHeadUsername.Text}' assigned!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadHeadTeacherList(); }
                }
                else
                {
                    if (cmbHeadTeacher?.SelectedValue == null) { MessageBox.Show("Please select a teacher.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    string hash = DatabaseHelper.HashPassword("head123");
                    string username = cmbHeadTeacher.Text.Replace(" ", "").ToLower();
                    int result = DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO HeadCredentials (Username, PasswordHash, Department, AssignedBy, CreatedAt)
                          VALUES (@Username, @Hash, @Dept, @By, GETDATE())",
                        new[] {
                            new SqlParameter("@Username", username),
                            new SqlParameter("@Hash", hash),
                            new SqlParameter("@Dept", cmbHeadDepartment.SelectedItem?.ToString()),
                            new SqlParameter("@By", frmRegistrarLogin.CurrentRegistrar)
                        });
                    if (result > 0) { MessageBox.Show($"Teacher assigned as Head!\nUsername: {username}\nPassword: head123", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadHeadTeacherList(); }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ActivateUser()
        {
            if (dgvUsers?.SelectedRows.Count == 0) { MessageBox.Show("Select a user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string type = dgvUsers.SelectedRows[0].Cells["Type"].Value.ToString();
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            try
            {
                int result = type == "Student" ? DatabaseHelper.ExecuteNonQuery("UPDATE Students SET Status = 'Active' WHERE Username = @U", new[] { new SqlParameter("@U", username) })
                                               : DatabaseHelper.ExecuteNonQuery("UPDATE Teachers SET IsActive = 1 WHERE Username = @U", new[] { new SqlParameter("@U", username) });
                if (result >= 0) { MessageBox.Show($"User '{username}' activated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadUserList(); }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DeactivateUser()
        {
            if (dgvUsers?.SelectedRows.Count == 0) { MessageBox.Show("Select a user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string type = dgvUsers.SelectedRows[0].Cells["Type"].Value.ToString();
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            if (MessageBox.Show($"Deactivate '{username}'?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                int result = type == "Student" ? DatabaseHelper.ExecuteNonQuery("UPDATE Students SET Status = 'Inactive' WHERE Username = @U", new[] { new SqlParameter("@U", username) })
                                               : DatabaseHelper.ExecuteNonQuery("UPDATE Teachers SET IsActive = 0 WHERE Username = @U", new[] { new SqlParameter("@U", username) });
                if (result >= 0) { MessageBox.Show($"User '{username}' deactivated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadUserList(); }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DeleteUser()
        {
            if (dgvUsers?.SelectedRows.Count == 0) { MessageBox.Show("Select a user to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string type = dgvUsers.SelectedRows[0].Cells["Type"].Value.ToString();
            string username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();
            string id = dgvUsers.SelectedRows[0].Cells["ID"].Value.ToString();
            
            if (MessageBox.Show($"Delete '{username}'? This cannot be undone!", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            
            try
            {
                int result = 0;
                if (type == "Student")
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM Grades WHERE EnrollmentID IN (SELECT EnrollmentID FROM Enrollments WHERE StudentID = @ID)", new[] { new SqlParameter("@ID", id) });
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM Enrollments WHERE StudentID = @ID", new[] { new SqlParameter("@ID", id) });
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM PaymentHistory WHERE StudentID = @ID", new[] { new SqlParameter("@ID", id) });
                    result = DatabaseHelper.ExecuteNonQuery("DELETE FROM Students WHERE StudentID = @ID", new[] { new SqlParameter("@ID", id) });
                }
                else if (type == "Teacher")
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM TeacherCourseAssignment WHERE TeacherID = @ID", new[] { new SqlParameter("@ID", id) });
                    result = DatabaseHelper.ExecuteNonQuery("DELETE FROM Teachers WHERE TeacherID = @ID", new[] { new SqlParameter("@ID", id) });
                }
                else if (type == "Head Teacher")
                {
                    result = DatabaseHelper.ExecuteNonQuery("DELETE FROM HeadCredentials WHERE Username = @U", new[] { new SqlParameter("@U", username) });
                }
                if (result > 0) { MessageBox.Show($"Successfully deleted {type} '{username}'.", "Delete Successful", MessageBoxButtons.OK, MessageBoxIcon.Information); LoadUserList(); LoadHeadTeacherList(); }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ResetPassword()
        {
            if (cmbResetUser?.SelectedValue == null) { MessageBox.Show("Select a user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(txtNewPassword?.Text)) { MessageBox.Show("Enter new password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtNewPassword.Text != txtConfirmNewPassword.Text) { MessageBox.Show("Passwords don't match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string username = cmbResetUser.SelectedValue.ToString();
            string userType = cmbResetUserType.SelectedItem?.ToString() ?? "Student";
            string hash = DatabaseHelper.HashPassword(txtNewPassword.Text);
            
            try
            {
                int result = 0;
                if (userType == "Student")
                    result = DatabaseHelper.ExecuteNonQuery("UPDATE Students SET PasswordHash = @H WHERE Username = @U", new[] { new SqlParameter("@H", hash), new SqlParameter("@U", username) });
                else if (userType == "Teacher")
                    result = DatabaseHelper.ExecuteNonQuery("UPDATE Teachers SET PasswordHash = @H WHERE Username = @U", new[] { new SqlParameter("@H", hash), new SqlParameter("@U", username) });
                else if (userType == "Head Teacher")
                    result = DatabaseHelper.ExecuteNonQuery("UPDATE HeadCredentials SET PasswordHash = @H WHERE Username = @U", new[] { new SqlParameter("@H", hash), new SqlParameter("@U", username) });
                
                if (result >= 0)
                {
                    MessageBox.Show($"Password reset for '{username}'!\nNew password: {txtNewPassword.Text}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNewPassword.Clear(); txtConfirmNewPassword.Clear();
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
