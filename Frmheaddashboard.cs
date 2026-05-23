using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmHeadTeacherDashboard : Form
    {
        private string username;
        private string department;
        private TabControl tabControl;

        // Course Tab Controls
        private TextBox txtCourseCode, txtCourseName, txtCreditHours;
        private ComboBox cmbYearOffered, cmbSemester;
        private DataGridView dgvCourses;

        // Teacher Tab Controls
        private TextBox txtTeacherFullName, txtTeacherUsername, txtTeacherEmail, txtTeacherPhone, txtTeacherPassword;
        private ComboBox cmbQualification;
        private DataGridView dgvTeachers;

        // Student Tab Controls (View only - no add)
        private DataGridView dgvStudents;

        // Course Assignment for Students
        private ComboBox cmbAssignStudentCourse, cmbAssignStudent;
        private ComboBox cmbAssignAcademicYear, cmbAssignStudentSemester;
        private Button btnAssignStudentCourse;
        private DataGridView dgvStudentAssignments;

        // Assignments Tab Controls (Teacher-Course)
        private ComboBox cmbAssignCourse, cmbAssignTeacher, cmbAssignYear, cmbAssignSem;
        private DataGridView dgvAssignments;

        // Schedule Tab Controls
        private ComboBox cmbScheduleCourse;
        private DataGridView dgvSchedule;

        public frmHeadTeacherDashboard(string username, string department, DataRow data)
        {
            this.username = username;
            this.department = department;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Head Teacher Dashboard - {department} Department";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(7, 20, 43);

            // Header
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(150, 100, 255);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 70;

            Label lblHeader = new Label();
            lblHeader.Text = $"HEAD OF {department.ToUpper()} DEPARTMENT";
            lblHeader.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Dock = DockStyle.Fill;
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            headerPanel.Controls.Add(lblHeader);

            Button btnLogout = new Button();
            btnLogout.Text = "🚪 Logout";
            btnLogout.Size = new Size(100, 35);
            btnLogout.Location = new Point(20, 15);
            btnLogout.BackColor = Color.FromArgb(220, 60, 60);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(btnLogout);

            // Tab Control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.ItemSize = new Size(160, 40);

            TabPage tabDashboard = new TabPage("📊 Dashboard");
            TabPage tabCourses = new TabPage("📚 Courses");
            TabPage tabTeachers = new TabPage("👨‍🏫 Teachers");
            TabPage tabStudents = new TabPage("👥 Students");
            TabPage tabStudentAssign = new TabPage("📝 Assign Course to Student");
            TabPage tabTeacherAssign = new TabPage("📝 Assign Course to Teacher");
            TabPage tabSchedule = new TabPage("📅 Schedule");

            SetupDashboardTab(tabDashboard);
            SetupCoursesTab(tabCourses);
            SetupTeachersTab(tabTeachers);
            SetupStudentsTab(tabStudents);
            SetupStudentAssignmentTab(tabStudentAssign);
            SetupTeacherAssignmentTab(tabTeacherAssign);
            SetupScheduleTab(tabSchedule);

            tabControl.TabPages.Add(tabDashboard);
            tabControl.TabPages.Add(tabCourses);
            tabControl.TabPages.Add(tabTeachers);
            tabControl.TabPages.Add(tabStudents);
            tabControl.TabPages.Add(tabStudentAssign);
            tabControl.TabPages.Add(tabTeacherAssign);
            tabControl.TabPages.Add(tabSchedule);

            this.Controls.Add(tabControl);
            this.Controls.Add(headerPanel);
        }

        private void SetupDashboardTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(30);
            panel.AutoScroll = true;

            Label lblTitle = new Label();
            lblTitle.Text = "Department Dashboard";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Size = new Size(350, 45);
            panel.Controls.Add(lblTitle);

            try
            {
                DataTable stats = DatabaseHelper.ExecuteQuery(
                    @"SELECT 
                        (SELECT COUNT(*) FROM Courses WHERE Department = @Department) as TotalCourses,
                        (SELECT COUNT(*) FROM Teachers WHERE Department = @Department AND IsActive = 1) as TotalTeachers,
                        (SELECT COUNT(*) FROM Students WHERE Department = @Department AND Status = 'Active') as TotalStudents,
                        (SELECT COUNT(*) FROM Enrollments e JOIN Courses c ON e.CourseID = c.CourseID WHERE c.Department = @Department) as TotalEnrollments",
                    new[] { new SqlParameter("@Department", department) });

                int totalCourses = 0, totalTeachers = 0, totalStudents = 0, totalEnrollments = 0;
                if (stats.Rows.Count > 0)
                {
                    totalCourses = Convert.ToInt32(stats.Rows[0]["TotalCourses"]);
                    totalTeachers = Convert.ToInt32(stats.Rows[0]["TotalTeachers"]);
                    totalStudents = Convert.ToInt32(stats.Rows[0]["TotalStudents"]);
                    totalEnrollments = Convert.ToInt32(stats.Rows[0]["TotalEnrollments"]);
                }

                int y = 70;
                int cardWidth = 260;
                int spacing = 30;

                CreateStatCard(panel, "📚", totalCourses.ToString(), "Total Courses", Color.FromArgb(0, 150, 255), 0, y, cardWidth, 110);
                CreateStatCard(panel, "👨‍🏫", totalTeachers.ToString(), "Active Teachers", Color.FromArgb(0, 200, 100), cardWidth + spacing, y, cardWidth, 110);
                CreateStatCard(panel, "👥", totalStudents.ToString(), "Enrolled Students", Color.FromArgb(255, 150, 0), (cardWidth + spacing) * 2, y, cardWidth, 110);
                CreateStatCard(panel, "📝", totalEnrollments.ToString(), "Total Enrollments", Color.FromArgb(150, 100, 255), (cardWidth + spacing) * 3, y, cardWidth, 110);
            }
            catch { }

            tab.Controls.Add(panel);
        }

        private void CreateStatCard(Panel parent, string icon, string value, string label, Color color, int x, int y, int width, int height)
        {
            Panel card = new Panel();
            card.Size = new Size(width, height);
            card.Location = new Point(x, y);
            card.BackColor = Color.FromArgb(25, 40, 70);

            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 28);
            lblIcon.ForeColor = color;
            lblIcon.Location = new Point(20, 15);
            lblIcon.Size = new Size(55, 55);

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblValue.ForeColor = Color.White;
            lblValue.Location = new Point(85, 15);
            lblValue.Size = new Size(100, 55);

            Label lblLabel = new Label();
            lblLabel.Text = label;
            lblLabel.Font = new Font("Segoe UI", 10);
            lblLabel.ForeColor = Color.FromArgb(180, 190, 210);
            lblLabel.Location = new Point(20, 75);
            lblLabel.Size = new Size(220, 30);

            card.Controls.Add(lblIcon);
            card.Controls.Add(lblValue);
            card.Controls.Add(lblLabel);
            parent.Controls.Add(card);
        }

        private void SetupCoursesTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;

            GroupBox grpAdd = new GroupBox();
            grpAdd.Text = "Add New Course";
            grpAdd.ForeColor = Color.White;
            grpAdd.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            grpAdd.Size = new Size(800, 230);
            grpAdd.Location = new Point(0, 0);
            grpAdd.BackColor = Color.FromArgb(20, 35, 65);

            int y = 35;
            int spacing = 40;

            AddFieldToGroup(grpAdd, "Course Code:", ref txtCourseCode, 20, y, 120, 200); y += spacing;
            AddFieldToGroup(grpAdd, "Course Name:", ref txtCourseName, 20, y, 120, 300); y += spacing;
            AddFieldToGroup(grpAdd, "Credit Hours:", ref txtCreditHours, 20, y, 120, 100);
            AddComboToGroup(grpAdd, "Year Offered:", ref cmbYearOffered, new[] { "1", "2", "3", "4" }, 180, y, 100, 100);
            AddComboToGroup(grpAdd, "Semester:", ref cmbSemester, new[] { "1", "2" }, 350, y, 100, 100);
            y += spacing + 15;

            Button btnAddCourse = new Button();
            btnAddCourse.Text = "➕ ADD COURSE";
            btnAddCourse.Size = new Size(160, 40);
            btnAddCourse.Location = new Point(320, y);
            btnAddCourse.BackColor = Color.FromArgb(0, 200, 100);
            btnAddCourse.ForeColor = Color.White;
            btnAddCourse.FlatStyle = FlatStyle.Flat;
            btnAddCourse.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnAddCourse.Click += (s, e) => AddCourse();
            grpAdd.Controls.Add(btnAddCourse);

            panel.Controls.Add(grpAdd);

            Label lblList = new Label();
            lblList.Text = "Department Courses";
            lblList.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblList.ForeColor = Color.White;
            lblList.Location = new Point(0, 250);
            lblList.Size = new Size(300, 35);
            panel.Controls.Add(lblList);

            dgvCourses = new DataGridView();
            dgvCourses.Location = new Point(0, 290);
            dgvCourses.Size = new Size(1100, 380);
            dgvCourses.BackgroundColor = Color.White;
            dgvCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCourses.ReadOnly = true;
            panel.Controls.Add(dgvCourses);
            LoadCourses();

            tab.Controls.Add(panel);
        }

        private void SetupTeachersTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;

            GroupBox grpAdd = new GroupBox();
            grpAdd.Text = "Add New Teacher";
            grpAdd.ForeColor = Color.White;
            grpAdd.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            grpAdd.Size = new Size(800, 320);
            grpAdd.Location = new Point(0, 0);
            grpAdd.BackColor = Color.FromArgb(20, 35, 65);

            int y = 35;
            int spacing = 40;

            AddFieldToGroup(grpAdd, "Full Name:", ref txtTeacherFullName, 20, y, 120, 250); y += spacing;
            AddFieldToGroup(grpAdd, "Username:", ref txtTeacherUsername, 20, y, 120, 250); y += spacing;
            AddFieldToGroup(grpAdd, "Email:", ref txtTeacherEmail, 20, y, 120, 250); y += spacing;
            AddFieldToGroup(grpAdd, "Phone:", ref txtTeacherPhone, 20, y, 120, 250); y += spacing;
            AddComboToGroup(grpAdd, "Qualification:", ref cmbQualification, new[] { "PhD", "Master's Degree", "Bachelor's Degree", "Diploma" }, 20, y, 120, 200); y += spacing;
            AddPasswordToGroup(grpAdd, "Password:", ref txtTeacherPassword, 20, y, 120, 250); y += spacing + 10;

            Button btnAddTeacher = new Button();
            btnAddTeacher.Text = "➕ ADD TEACHER";
            btnAddTeacher.Size = new Size(180, 45);
            btnAddTeacher.Location = new Point(310, y);
            btnAddTeacher.BackColor = Color.FromArgb(0, 200, 100);
            btnAddTeacher.ForeColor = Color.White;
            btnAddTeacher.FlatStyle = FlatStyle.Flat;
            btnAddTeacher.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnAddTeacher.Click += (s, e) => AddTeacher();
            grpAdd.Controls.Add(btnAddTeacher);

            panel.Controls.Add(grpAdd);

            Label lblList = new Label();
            lblList.Text = "Department Teachers";
            lblList.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblList.ForeColor = Color.White;
            lblList.Location = new Point(0, 340);
            lblList.Size = new Size(300, 35);
            panel.Controls.Add(lblList);

            dgvTeachers = new DataGridView();
            dgvTeachers.Location = new Point(0, 380);
            dgvTeachers.Size = new Size(1100, 300);
            dgvTeachers.BackgroundColor = Color.White;
            dgvTeachers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTeachers.ReadOnly = true;
            panel.Controls.Add(dgvTeachers);
            LoadTeachers();

            tab.Controls.Add(panel);
        }

        private void SetupStudentsTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;

            Label lblList = new Label();
            lblList.Text = "Department Students (View Only)";
            lblList.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblList.ForeColor = Color.White;
            lblList.Location = new Point(0, 0);
            lblList.Size = new Size(350, 35);
            panel.Controls.Add(lblList);

            Label lblInfo = new Label();
            lblInfo.Text = "💡 Student registration and course assignment are managed through the Registration Portal";
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.ForeColor = Color.FromArgb(180, 190, 210);
            lblInfo.Location = new Point(0, 40);
            lblInfo.Size = new Size(600, 30);
            panel.Controls.Add(lblInfo);

            dgvStudents = new DataGridView();
            dgvStudents.Location = new Point(0, 80);
            dgvStudents.Size = new Size(1100, 550);
            dgvStudents.BackgroundColor = Color.White;
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.ReadOnly = true;
            panel.Controls.Add(dgvStudents);
            LoadStudents();

            tab.Controls.Add(panel);
        }

        private void SetupStudentAssignmentTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;

            // Assignment Section
            GroupBox grpAssign = new GroupBox();
            grpAssign.Text = "Assign Course to Student";
            grpAssign.ForeColor = Color.White;
            grpAssign.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            grpAssign.Size = new Size(700, 230);
            grpAssign.Location = new Point(0, 0);
            grpAssign.BackColor = Color.FromArgb(20, 35, 65);

            int y = 35;
            int spacing = 45;

            // Student Selection
            Label lblStudent = new Label();
            lblStudent.Text = "Select Student:";
            lblStudent.Font = new Font("Segoe UI", 10);
            lblStudent.ForeColor = Color.White;
            lblStudent.Location = new Point(20, y);
            lblStudent.Size = new Size(110, 30);
            grpAssign.Controls.Add(lblStudent);

            cmbAssignStudent = new ComboBox();
            cmbAssignStudent.Location = new Point(140, y);
            cmbAssignStudent.Size = new Size(350, 30);
            cmbAssignStudent.DropDownStyle = ComboBoxStyle.DropDownList;
            grpAssign.Controls.Add(cmbAssignStudent);
            y += spacing;

            // Course Selection
            Label lblCourse = new Label();
            lblCourse.Text = "Select Course:";
            lblCourse.Font = new Font("Segoe UI", 10);
            lblCourse.ForeColor = Color.White;
            lblCourse.Location = new Point(20, y);
            lblCourse.Size = new Size(110, 30);
            grpAssign.Controls.Add(lblCourse);

            cmbAssignStudentCourse = new ComboBox();
            cmbAssignStudentCourse.Location = new Point(140, y);
            cmbAssignStudentCourse.Size = new Size(350, 30);
            cmbAssignStudentCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            grpAssign.Controls.Add(cmbAssignStudentCourse);
            y += spacing;

            // Academic Year
            Label lblYear = new Label();
            lblYear.Text = "Academic Year:";
            lblYear.Font = new Font("Segoe UI", 10);
            lblYear.ForeColor = Color.White;
            lblYear.Location = new Point(20, y);
            lblYear.Size = new Size(110, 30);
            grpAssign.Controls.Add(lblYear);

            cmbAssignAcademicYear = new ComboBox();
            cmbAssignAcademicYear.Location = new Point(140, y);
            cmbAssignAcademicYear.Size = new Size(150, 30);
            cmbAssignAcademicYear.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssignAcademicYear.Items.AddRange(new[] { "2024/2025", "2025/2026", "2026/2027" });
            cmbAssignAcademicYear.SelectedIndex = 0;
            grpAssign.Controls.Add(cmbAssignAcademicYear);

            // Semester
            Label lblSem = new Label();
            lblSem.Text = "Semester:";
            lblSem.Font = new Font("Segoe UI", 10);
            lblSem.ForeColor = Color.White;
            lblSem.Location = new Point(310, y);
            lblSem.Size = new Size(80, 30);
            grpAssign.Controls.Add(lblSem);

            cmbAssignStudentSemester = new ComboBox();
            cmbAssignStudentSemester.Location = new Point(390, y);
            cmbAssignStudentSemester.Size = new Size(80, 30);
            cmbAssignStudentSemester.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssignStudentSemester.Items.AddRange(new[] { "1", "2" });
            cmbAssignStudentSemester.SelectedIndex = 0;
            grpAssign.Controls.Add(cmbAssignStudentSemester);
            y += spacing;

            Button btnAssign = new Button();
            btnAssign.Text = "📝 ASSIGN COURSE TO STUDENT";
            btnAssign.Size = new Size(200, 45);
            btnAssign.Location = new Point(250, y);
            btnAssign.BackColor = Color.FromArgb(150, 100, 255);
            btnAssign.ForeColor = Color.White;
            btnAssign.FlatStyle = FlatStyle.Flat;
            btnAssign.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnAssign.Click += (s, e) => AssignCourseToStudent();
            grpAssign.Controls.Add(btnAssign);

            panel.Controls.Add(grpAssign);

            // Student Assignments List
            Label lblList = new Label();
            lblList.Text = "Student Course Assignments";
            lblList.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblList.ForeColor = Color.White;
            lblList.Location = new Point(0, 250);
            lblList.Size = new Size(350, 35);
            panel.Controls.Add(lblList);

            dgvStudentAssignments = new DataGridView();
            dgvStudentAssignments.Location = new Point(0, 290);
            dgvStudentAssignments.Size = new Size(1100, 420);
            dgvStudentAssignments.BackgroundColor = Color.White;
            dgvStudentAssignments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudentAssignments.ReadOnly = true;
            panel.Controls.Add(dgvStudentAssignments);

            LoadStudentAssignments();
            LoadStudentsForAssignment();
            LoadCoursesForStudentAssignment();

            tab.Controls.Add(panel);
        }

        private void SetupTeacherAssignmentTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;

            GroupBox grpAssign = new GroupBox();
            grpAssign.Text = "Assign Course to Teacher";
            grpAssign.ForeColor = Color.White;
            grpAssign.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            grpAssign.Size = new Size(700, 230);
            grpAssign.Location = new Point(0, 0);
            grpAssign.BackColor = Color.FromArgb(20, 35, 65);

            int y = 35;
            int spacing = 45;

            // Course
            Label lblCourse = new Label();
            lblCourse.Text = "Select Course:";
            lblCourse.Font = new Font("Segoe UI", 10);
            lblCourse.ForeColor = Color.White;
            lblCourse.Location = new Point(20, y);
            lblCourse.Size = new Size(110, 30);
            grpAssign.Controls.Add(lblCourse);

            cmbAssignCourse = new ComboBox();
            cmbAssignCourse.Location = new Point(140, y);
            cmbAssignCourse.Size = new Size(300, 30);
            cmbAssignCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            grpAssign.Controls.Add(cmbAssignCourse);
            y += spacing;

            // Teacher
            Label lblTeacher = new Label();
            lblTeacher.Text = "Select Teacher:";
            lblTeacher.Font = new Font("Segoe UI", 10);
            lblTeacher.ForeColor = Color.White;
            lblTeacher.Location = new Point(20, y);
            lblTeacher.Size = new Size(110, 30);
            grpAssign.Controls.Add(lblTeacher);

            cmbAssignTeacher = new ComboBox();
            cmbAssignTeacher.Location = new Point(140, y);
            cmbAssignTeacher.Size = new Size(300, 30);
            cmbAssignTeacher.DropDownStyle = ComboBoxStyle.DropDownList;
            grpAssign.Controls.Add(cmbAssignTeacher);
            y += spacing;

            // Academic Year & Semester
            Label lblYear = new Label();
            lblYear.Text = "Academic Year:";
            lblYear.Font = new Font("Segoe UI", 10);
            lblYear.ForeColor = Color.White;
            lblYear.Location = new Point(20, y);
            lblYear.Size = new Size(110, 30);
            grpAssign.Controls.Add(lblYear);

            cmbAssignYear = new ComboBox();
            cmbAssignYear.Location = new Point(140, y);
            cmbAssignYear.Size = new Size(150, 30);
            cmbAssignYear.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssignYear.Items.AddRange(new[] { "2024/2025", "2025/2026", "2026/2027" });
            cmbAssignYear.SelectedIndex = 0;
            grpAssign.Controls.Add(cmbAssignYear);

            Label lblSem = new Label();
            lblSem.Text = "Semester:";
            lblSem.Font = new Font("Segoe UI", 10);
            lblSem.ForeColor = Color.White;
            lblSem.Location = new Point(310, y);
            lblSem.Size = new Size(80, 30);
            grpAssign.Controls.Add(lblSem);

            cmbAssignSem = new ComboBox();
            cmbAssignSem.Location = new Point(390, y);
            cmbAssignSem.Size = new Size(80, 30);
            cmbAssignSem.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssignSem.Items.AddRange(new[] { "1", "2" });
            cmbAssignSem.SelectedIndex = 0;
            grpAssign.Controls.Add(cmbAssignSem);
            y += spacing;

            Button btnAssign = new Button();
            btnAssign.Text = "📝 ASSIGN COURSE TO TEACHER";
            btnAssign.Size = new Size(200, 45);
            btnAssign.Location = new Point(250, y);
            btnAssign.BackColor = Color.FromArgb(150, 100, 255);
            btnAssign.ForeColor = Color.White;
            btnAssign.FlatStyle = FlatStyle.Flat;
            btnAssign.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnAssign.Click += (s, e) => AssignCourseToTeacher();
            grpAssign.Controls.Add(btnAssign);

            panel.Controls.Add(grpAssign);

            Label lblList = new Label();
            lblList.Text = "Teacher Course Assignments";
            lblList.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblList.ForeColor = Color.White;
            lblList.Location = new Point(0, 250);
            lblList.Size = new Size(350, 35);
            panel.Controls.Add(lblList);

            dgvAssignments = new DataGridView();
            dgvAssignments.Location = new Point(0, 290);
            dgvAssignments.Size = new Size(1100, 420);
            dgvAssignments.BackgroundColor = Color.White;
            dgvAssignments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAssignments.ReadOnly = true;
            panel.Controls.Add(dgvAssignments);

            LoadTeacherAssignments();
            LoadTeacherComboBoxes();

            tab.Controls.Add(panel);
        }

        private void SetupScheduleTab(TabPage tab)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(20);
            panel.AutoScroll = true;

            Label lblTitle = new Label();
            lblTitle.Text = "Course Schedule";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(0, 0);
            lblTitle.Size = new Size(300, 40);
            panel.Controls.Add(lblTitle);

            Label lblInfo = new Label();
            lblInfo.Text = "Select a course to view enrolled students:";
            lblInfo.Font = new Font("Segoe UI", 11);
            lblInfo.ForeColor = Color.FromArgb(180, 190, 210);
            lblInfo.Location = new Point(0, 50);
            lblInfo.Size = new Size(400, 30);
            panel.Controls.Add(lblInfo);

            cmbScheduleCourse = new ComboBox();
            cmbScheduleCourse.Location = new Point(0, 90);
            cmbScheduleCourse.Size = new Size(350, 35);
            cmbScheduleCourse.DropDownStyle = ComboBoxStyle.DropDownList;
            panel.Controls.Add(cmbScheduleCourse);

            Button btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.Location = new Point(370, 90);
            btnRefresh.BackColor = Color.FromArgb(0, 150, 255);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Click += (s, e) => LoadSchedule();
            panel.Controls.Add(btnRefresh);

            dgvSchedule = new DataGridView();
            dgvSchedule.Location = new Point(0, 140);
            dgvSchedule.Size = new Size(1100, 550);
            dgvSchedule.BackgroundColor = Color.White;
            dgvSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSchedule.ReadOnly = true;
            panel.Controls.Add(dgvSchedule);

            LoadScheduleCourses();
            tab.Controls.Add(panel);
        }

        // ==================================================
        // HELPER METHODS
        // ==================================================

        private void AddFieldToGroup(GroupBox group, string label, ref TextBox box, int x, int y, int labelWidth, int boxWidth)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 10);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(labelWidth, 30);
            group.Controls.Add(lbl);

            box = new TextBox();
            box.Location = new Point(x + labelWidth + 10, y);
            box.Size = new Size(boxWidth, 30);
            box.BackColor = Color.FromArgb(30, 50, 85);
            box.ForeColor = Color.White;
            group.Controls.Add(box);
        }

        private void AddComboToGroup(GroupBox group, string label, ref ComboBox box, string[] items, int x, int y, int labelWidth, int boxWidth)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 10);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(labelWidth, 30);
            group.Controls.Add(lbl);

            box = new ComboBox();
            box.Location = new Point(x + labelWidth + 10, y);
            box.Size = new Size(boxWidth, 30);
            box.DropDownStyle = ComboBoxStyle.DropDownList;
            box.Items.AddRange(items);
            if (items.Length > 0) box.SelectedIndex = 0;
            box.BackColor = Color.FromArgb(30, 50, 85);
            box.ForeColor = Color.White;
            group.Controls.Add(box);
        }

        private void AddPasswordToGroup(GroupBox group, string label, ref TextBox box, int x, int y, int labelWidth, int boxWidth)
        {
            Label lbl = new Label();
            lbl.Text = label;
            lbl.Font = new Font("Segoe UI", 10);
            lbl.ForeColor = Color.White;
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(labelWidth, 30);
            group.Controls.Add(lbl);

            box = new TextBox();
            box.Location = new Point(x + labelWidth + 10, y);
            box.Size = new Size(boxWidth, 30);
            box.PasswordChar = '*';
            box.BackColor = Color.FromArgb(30, 50, 85);
            box.ForeColor = Color.White;
            group.Controls.Add(box);
        }

        // ==================================================
        // DATA LOADING METHODS
        // ==================================================

        private void LoadCourses()
        {
            try
            {
                DataTable courses = DatabaseHelper.ExecuteQuery(
                    "SELECT CourseCode, CourseName, CreditHours, YearOffered, Semester FROM Courses WHERE Department = @Department ORDER BY CourseCode",
                    new[] { new SqlParameter("@Department", department) });
                dgvCourses.DataSource = courses;
            }
            catch { }
        }

        private void LoadTeachers()
        {
            try
            {
                DataTable teachers = DatabaseHelper.ExecuteQuery(
                    "SELECT FullName, Username, Email, Qualification, CASE WHEN IsActive=1 THEN 'Active' ELSE 'Inactive' END as Status FROM Teachers WHERE Department = @Department ORDER BY FullName",
                    new[] { new SqlParameter("@Department", department) });
                dgvTeachers.DataSource = teachers;
            }
            catch { }
        }

        private void LoadStudents()
        {
            try
            {
                DataTable students = DatabaseHelper.ExecuteQuery(
                    "SELECT StudentID, FirstName, FatherName, Email, YearOfStudy, Status FROM Students WHERE Department = @Department ORDER BY YearOfStudy, FirstName",
                    new[] { new SqlParameter("@Department", department) });
                dgvStudents.DataSource = students;
            }
            catch { }
        }

        private void LoadStudentsForAssignment()
        {
            try
            {
                DataTable students = DatabaseHelper.ExecuteQuery(
                    "SELECT StudentID, FirstName + ' ' + FatherName as FullName FROM Students WHERE Department = @Department AND Status = 'Active' ORDER BY FirstName",
                    new[] { new SqlParameter("@Department", department) });
                cmbAssignStudent.DisplayMember = "FullName";
                cmbAssignStudent.ValueMember = "StudentID";
                cmbAssignStudent.DataSource = students;
            }
            catch { }
        }

        private void LoadCoursesForStudentAssignment()
        {
            try
            {
                DataTable courses = DatabaseHelper.ExecuteQuery(
                    "SELECT CourseID, CourseCode, CourseName FROM Courses WHERE Department = @Department ORDER BY CourseCode",
                    new[] { new SqlParameter("@Department", department) });
                cmbAssignStudentCourse.DisplayMember = "CourseCode";
                cmbAssignStudentCourse.ValueMember = "CourseID";
                cmbAssignStudentCourse.DataSource = courses;
            }
            catch { }
        }

        private void LoadStudentAssignments()
        {
            try
            {
                DataTable assignments = DatabaseHelper.ExecuteQuery(
                    @"SELECT s.FirstName + ' ' + s.FatherName as Student, c.CourseCode, c.CourseName, e.AcademicYear, e.Semester, e.EnrolledOn
                      FROM Enrollments e
                      JOIN Students s ON e.StudentID = s.StudentID
                      JOIN Courses c ON e.CourseID = c.CourseID
                      WHERE s.Department = @Department
                      ORDER BY e.EnrolledOn DESC",
                    new[] { new SqlParameter("@Department", department) });
                dgvStudentAssignments.DataSource = assignments;
            }
            catch { }
        }

        private void LoadTeacherComboBoxes()
        {
            try
            {
                DataTable courses = DatabaseHelper.ExecuteQuery(
                    "SELECT CourseID, CourseCode FROM Courses WHERE Department = @Department ORDER BY CourseCode",
                    new[] { new SqlParameter("@Department", department) });
                cmbAssignCourse.DisplayMember = "CourseCode";
                cmbAssignCourse.ValueMember = "CourseID";
                cmbAssignCourse.DataSource = courses;

                DataTable teachers = DatabaseHelper.ExecuteQuery(
                    "SELECT TeacherID, FullName FROM Teachers WHERE Department = @Department AND IsActive = 1 ORDER BY FullName",
                    new[] { new SqlParameter("@Department", department) });
                cmbAssignTeacher.DisplayMember = "FullName";
                cmbAssignTeacher.ValueMember = "TeacherID";
                cmbAssignTeacher.DataSource = teachers;
            }
            catch { }
        }

        private void LoadTeacherAssignments()
        {
            try
            {
                DataTable assignments = DatabaseHelper.ExecuteQuery(
                    @"SELECT t.FullName as Teacher, c.CourseCode, c.CourseName, tca.AcademicYear, tca.Semester
                      FROM TeacherCourseAssignment tca
                      JOIN Teachers t ON tca.TeacherID = t.TeacherID
                      JOIN Courses c ON tca.CourseID = c.CourseID
                      WHERE c.Department = @Department
                      ORDER BY t.FullName",
                    new[] { new SqlParameter("@Department", department) });
                dgvAssignments.DataSource = assignments;
            }
            catch { }
        }

        private void LoadScheduleCourses()
        {
            try
            {
                DataTable courses = DatabaseHelper.ExecuteQuery(
                    "SELECT CourseID, CourseCode FROM Courses WHERE Department = @Department ORDER BY CourseCode",
                    new[] { new SqlParameter("@Department", department) });
                cmbScheduleCourse.DisplayMember = "CourseCode";
                cmbScheduleCourse.ValueMember = "CourseID";
                cmbScheduleCourse.DataSource = courses;
            }
            catch { }
        }

        private void LoadSchedule()
        {
            if (cmbScheduleCourse.SelectedValue == null) return;
            try
            {
                DataTable schedule = DatabaseHelper.ExecuteQuery(
                    @"SELECT s.StudentID, s.FirstName, s.FatherName, e.EnrolledOn
                      FROM Enrollments e
                      JOIN Students s ON e.StudentID = s.StudentID
                      WHERE e.CourseID = @CourseID",
                    new[] { new SqlParameter("@CourseID", cmbScheduleCourse.SelectedValue) });
                dgvSchedule.DataSource = schedule;
            }
            catch { }
        }

        // ==================================================
        // BUSINESS METHODS
        // ==================================================

        private void AddCourse()
        {
            if (string.IsNullOrEmpty(txtCourseCode.Text)) { MessageBox.Show("Enter course code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(txtCourseName.Text)) { MessageBox.Show("Enter course name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                int result = DatabaseHelper.ExecuteNonQuery(
                    @"INSERT INTO Courses (CourseCode, CourseName, CreditHours, Department, YearOffered, Semester)
                      VALUES (@Code, @Name, @Credits, @Dept, @Year, @Sem)",
                    new[] {
                        new SqlParameter("@Code", txtCourseCode.Text.ToUpper()),
                        new SqlParameter("@Name", txtCourseName.Text),
                        new SqlParameter("@Credits", Convert.ToInt32(txtCreditHours.Text)),
                        new SqlParameter("@Dept", department),
                        new SqlParameter("@Year", Convert.ToInt32(cmbYearOffered.SelectedItem)),
                        new SqlParameter("@Sem", Convert.ToInt32(cmbSemester.SelectedItem))
                    });
                if (result > 0)
                {
                    MessageBox.Show($"Course '{txtCourseCode.Text}' added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtCourseCode.Clear(); txtCourseName.Clear(); txtCreditHours.Clear();
                    LoadCourses();
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void AddTeacher()
        {
            if (string.IsNullOrEmpty(txtTeacherFullName.Text)) { MessageBox.Show("Enter full name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(txtTeacherUsername.Text)) { MessageBox.Show("Enter username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                string hash = DatabaseHelper.HashPassword(string.IsNullOrEmpty(txtTeacherPassword.Text) ? "password123" : txtTeacherPassword.Text);
                int result = DatabaseHelper.ExecuteNonQuery(
                    @"INSERT INTO Teachers (FullName, Username, PasswordHash, Email, PhoneNumber, Department, Qualification, IsActive, RegisteredAt)
                      VALUES (@Name, @User, @Hash, @Email, @Phone, @Dept, @Qual, 1, GETDATE())",
                    new[] {
                        new SqlParameter("@Name", txtTeacherFullName.Text),
                        new SqlParameter("@User", txtTeacherUsername.Text),
                        new SqlParameter("@Hash", hash),
                        new SqlParameter("@Email", txtTeacherEmail.Text ?? ""),
                        new SqlParameter("@Phone", txtTeacherPhone.Text ?? ""),
                        new SqlParameter("@Dept", department),
                        new SqlParameter("@Qual", cmbQualification.SelectedItem.ToString())
                    });
                if (result > 0)
                {
                    MessageBox.Show($"Teacher '{txtTeacherFullName.Text}' added!\nUsername: {txtTeacherUsername.Text}\nPassword: password123", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearTeacherForm();
                    LoadTeachers();
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ClearTeacherForm()
        {
            txtTeacherFullName.Clear(); txtTeacherUsername.Clear(); txtTeacherEmail.Clear(); txtTeacherPhone.Clear(); txtTeacherPassword.Clear();
        }

        private void AssignCourseToStudent()
        {
            if (cmbAssignStudent?.SelectedValue == null) { MessageBox.Show("Select a student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbAssignStudentCourse?.SelectedValue == null) { MessageBox.Show("Select a course.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string studentId = cmbAssignStudent.SelectedValue.ToString();
            int courseId = Convert.ToInt32(cmbAssignStudentCourse.SelectedValue);
            string academicYear = cmbAssignAcademicYear.SelectedItem.ToString();
            int semester = Convert.ToInt32(cmbAssignStudentSemester.SelectedItem);

            try
            {
                // Check if already enrolled
                DataTable check = DatabaseHelper.ExecuteQuery(
                    "SELECT COUNT(*) as Count FROM Enrollments WHERE StudentID = @StudentID AND CourseID = @CourseID AND AcademicYear = @Year AND Semester = @Sem",
                    new[] {
                        new SqlParameter("@StudentID", studentId),
                        new SqlParameter("@CourseID", courseId),
                        new SqlParameter("@Year", academicYear),
                        new SqlParameter("@Sem", semester)
                    });
                int count = Convert.ToInt32(check.Rows[0]["Count"]);

                if (count > 0)
                {
                    MessageBox.Show("Student is already enrolled in this course for this semester.", "Duplicate Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int result = DatabaseHelper.ExecuteNonQuery(
                    @"INSERT INTO Enrollments (StudentID, CourseID, AcademicYear, Semester, EnrolledOn)
                      VALUES (@StudentID, @CourseID, @Year, @Sem, GETDATE())",
                    new[] {
                        new SqlParameter("@StudentID", studentId),
                        new SqlParameter("@CourseID", courseId),
                        new SqlParameter("@Year", academicYear),
                        new SqlParameter("@Sem", semester)
                    });

                if (result > 0)
                {
                    MessageBox.Show($"Course assigned to student successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStudentAssignments();
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void AssignCourseToTeacher()
        {
            if (cmbAssignCourse.SelectedValue == null) { MessageBox.Show("Select a course.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbAssignTeacher.SelectedValue == null) { MessageBox.Show("Select a teacher.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                int result = DatabaseHelper.ExecuteNonQuery(
                    @"INSERT INTO TeacherCourseAssignment (TeacherID, CourseID, AcademicYear, Semester, AssignedAt)
                      VALUES (@TeacherID, @CourseID, @Year, @Sem, GETDATE())",
                    new[] {
                        new SqlParameter("@TeacherID", cmbAssignTeacher.SelectedValue),
                        new SqlParameter("@CourseID", cmbAssignCourse.SelectedValue),
                        new SqlParameter("@Year", cmbAssignYear.SelectedItem.ToString()),
                        new SqlParameter("@Sem", Convert.ToInt32(cmbAssignSem.SelectedItem))
                    });
                if (result > 0)
                {
                    MessageBox.Show("Course assigned to teacher successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadTeacherAssignments();
                }
            }
            catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
