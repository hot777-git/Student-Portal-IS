using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmTeacherDashboard : Form
    {
        private int teacherId;
        private string teacherName;
        private DataRow? teacherData;

        private Panel mainContentPanel = null!;
        private FlowLayoutPanel sidebarPanel = null!;
        private Panel headerPanel = null!;

        public frmTeacherDashboard(int teacherId, string teacherName, DataRow data)
        {
            this.teacherId = teacherId;
            this.teacherName = teacherName;
            this.teacherData = data;

            InitializeComponent();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = $"Teacher Dashboard - {teacherName}";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 700);
            this.BackColor = Color.FromArgb(7, 20, 43);
            this.StartPosition = FormStartPosition.CenterScreen;

            // HEADER
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.BackColor = Color.FromArgb(0, 200, 100);

            Label lblWelcome = new Label();
            lblWelcome.Text = $"Welcome, {teacherName}";
            lblWelcome.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(30, 20);

            Label lblTeacherId = new Label();
            lblTeacherId.Text = $"Teacher ID: {teacherId}";
            lblTeacherId.Font = new Font("Segoe UI", 12);
            lblTeacherId.ForeColor = Color.WhiteSmoke;
            lblTeacherId.AutoSize = true;
            lblTeacherId.Location = new Point(32, 60);

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblTeacherId);

            // SIDEBAR
            sidebarPanel = new FlowLayoutPanel();
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.Width = 250;
            sidebarPanel.FlowDirection = FlowDirection.TopDown;
            sidebarPanel.WrapContents = false;
            sidebarPanel.AutoScroll = true;
            sidebarPanel.Padding = new Padding(0, 20, 0, 20);
            sidebarPanel.BackColor = Color.FromArgb(20, 40, 75);

            // MAIN CONTENT PANEL
            mainContentPanel = new Panel();
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.BackColor = Color.FromArgb(7, 20, 43);
            mainContentPanel.AutoScroll = true;
            mainContentPanel.Padding = new Padding(20);

            // BUTTONS
            Button btnDashboard = CreateSidebarButton("📊 Dashboard");
            Button btnCourses = CreateSidebarButton("📚 My Courses");
            Button btnGradeEntry = CreateSidebarButton("✏️ Enter Grades");
            Button btnStudents = CreateSidebarButton("👥 My Students");
            Button btnSchedule = CreateSidebarButton("📅 Schedule");
            Button btnProfile = CreateSidebarButton("👤 Profile");
            Button btnLogout = CreateSidebarButton("🚪 Logout");

            btnDashboard.Click += (s, e) => ShowDashboard();
            btnCourses.Click += (s, e) => ShowMyCourses();
            btnGradeEntry.Click += (s, e) => ShowGradeEntry();
            btnStudents.Click += (s, e) => ShowMyStudents();
            btnSchedule.Click += (s, e) => ShowSchedule();
            btnProfile.Click += (s, e) => ShowProfile();
            btnLogout.Click += (s, e) => this.Close();

            sidebarPanel.Controls.Add(btnDashboard);
            sidebarPanel.Controls.Add(btnCourses);
            sidebarPanel.Controls.Add(btnGradeEntry);
            sidebarPanel.Controls.Add(btnStudents);
            sidebarPanel.Controls.Add(btnSchedule);
            sidebarPanel.Controls.Add(btnProfile);
            sidebarPanel.Controls.Add(btnLogout);

            this.Controls.Add(mainContentPanel);
            this.Controls.Add(sidebarPanel);
            this.Controls.Add(headerPanel);
        }

        private Button CreateSidebarButton(string text)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Width = 230;
            btn.Height = 55;
            btn.Margin = new Padding(10, 5, 10, 5);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(30, 50, 90);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => { btn.BackColor = Color.FromArgb(0, 120, 215); };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.FromArgb(30, 50, 90); };
            return btn;
        }

        private void ClearContent()
        {
            mainContentPanel.Controls.Clear();
        }

        private void AddSectionTitle(string title)
        {
            Label lbl = new Label();
            lbl.Text = title;
            lbl.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lbl.ForeColor = Color.White;
            lbl.AutoSize = true;
            lbl.Location = new Point(20, 20);
            mainContentPanel.Controls.Add(lbl);
        }

        private void AddSeparator()
        {
            Panel line = new Panel();
            line.BackColor = Color.FromArgb(50, 70, 110);
            line.Location = new Point(20, 70);
            line.Width = 1000;
            line.Height = 2;
            mainContentPanel.Controls.Add(line);
        }

        private DataGridView CreateGrid(DataTable table, int yPosition = 110)
        {
            DataGridView dgv = new DataGridView();
            dgv.DataSource = table;
            dgv.Location = new Point(20, yPosition);
            dgv.Width = mainContentPanel.ClientSize.Width - 60;
            dgv.Height = 400;
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgv.BackgroundColor = Color.White;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BorderStyle = BorderStyle.None;
            return dgv;
        }

        private void ShowDashboard()
        {
            ClearContent();
            AddSectionTitle("Dashboard");
            AddSeparator();

            Panel card = new Panel();
            card.Location = new Point(20, 110);
            card.Size = new Size(550, 220);
            card.BackColor = Color.FromArgb(20, 35, 65);

            Label lbl = new Label();
            lbl.Text = $"Welcome back, {teacherName}\n\n" +
                      $"Teacher ID: {teacherId}\n" +
                      $"Status: Active\n" +
                      $"Department: {teacherData?["Department"]?.ToString() ?? "Not assigned"}\n" +
                      $"Semester: 1st Semester 2024/2025";
            lbl.Font = new Font("Segoe UI", 12);
            lbl.ForeColor = Color.WhiteSmoke;
            lbl.Location = new Point(20, 20);
            lbl.Size = new Size(500, 160);

            card.Controls.Add(lbl);
            mainContentPanel.Controls.Add(card);
        }

        private void ShowMyCourses()
        {
            ClearContent();
            AddSectionTitle("My Courses");
            AddSeparator();

            try
            {
                DataTable courses = DatabaseHelper.ExecuteQuery(
                    @"SELECT c.CourseCode, c.CourseName, c.CreditHours, tca.AcademicYear, tca.Semester
                      FROM TeacherCourseAssignment tca
                      JOIN Courses c ON tca.CourseID = c.CourseID
                      WHERE tca.TeacherID = @TeacherID",
                    new[] { new SqlParameter("@TeacherID", teacherId) });

                if (courses.Rows.Count == 0)
                {
                    Label lbl = new Label();
                    lbl.Text = "No courses assigned yet.";
                    lbl.Font = new Font("Segoe UI", 12);
                    lbl.ForeColor = Color.WhiteSmoke;
                    lbl.Location = new Point(20, 110);
                    lbl.AutoSize = true;
                    mainContentPanel.Controls.Add(lbl);
                }
                else
                {
                    DataGridView dgv = CreateGrid(courses, 110);
                    mainContentPanel.Controls.Add(dgv);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

                private void ShowGradeEntry()
        {
            var gradeForm = new frmGradeManagement(teacherId);
            gradeForm.ShowDialog();
        }

        private void ShowMyStudents()
        {
            ClearContent();
            AddSectionTitle("My Students");
            AddSeparator();

            try
            {
                DataTable students = DatabaseHelper.ExecuteQuery(
                    @"SELECT DISTINCT s.StudentID, s.FirstName, s.FatherName, s.Email, c.CourseCode
                      FROM Enrollments e
                      JOIN Students s ON e.StudentID = s.StudentID
                      JOIN Courses c ON e.CourseID = c.CourseID
                      JOIN TeacherCourseAssignment tca ON tca.CourseID = c.CourseID
                      WHERE tca.TeacherID = @TeacherID",
                    new[] { new SqlParameter("@TeacherID", teacherId) });

                if (students.Rows.Count == 0)
                {
                    Label lbl = new Label();
                    lbl.Text = "No students assigned to your courses.";
                    lbl.Font = new Font("Segoe UI", 12);
                    lbl.ForeColor = Color.WhiteSmoke;
                    lbl.Location = new Point(20, 110);
                    lbl.AutoSize = true;
                    mainContentPanel.Controls.Add(lbl);
                }
                else
                {
                    DataGridView dgv = CreateGrid(students, 110);
                    mainContentPanel.Controls.Add(dgv);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowSchedule()
        {
            ClearContent();
            AddSectionTitle("Weekly Schedule");
            AddSeparator();

            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            int y = 110;

            foreach (string day in days)
            {
                Panel card = new Panel();
                card.Location = new Point(20, y);
                card.Size = new Size(900, 60);
                card.BackColor = Color.FromArgb(20, 35, 65);

                Label lblDay = new Label();
                lblDay.Text = day;
                lblDay.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblDay.ForeColor = Color.White;
                lblDay.Location = new Point(20, 18);
                lblDay.AutoSize = true;

                Label lblInfo = new Label();
                lblInfo.Text = "No classes scheduled";
                lblInfo.Font = new Font("Segoe UI", 10);
                lblInfo.ForeColor = Color.Gainsboro;
                lblInfo.Location = new Point(200, 20);
                lblInfo.AutoSize = true;

                card.Controls.Add(lblDay);
                card.Controls.Add(lblInfo);
                mainContentPanel.Controls.Add(card);
                y += 75;
            }
        }

        private void ShowProfile()
        {
            ClearContent();
            AddSectionTitle("My Profile");
            AddSeparator();

            Panel card = new Panel();
            card.Location = new Point(20, 110);
            card.Size = new Size(700, 350);
            card.BackColor = Color.FromArgb(20, 35, 65);

            string[] labels = { "Teacher ID:", "Full Name:", "Email:", "Department:", "Qualification:", "Status:" };
            string[] values = {
                teacherId.ToString(),
                teacherName,
                teacherData?["Email"]?.ToString() ?? "N/A",
                teacherData?["Department"]?.ToString() ?? "Not assigned",
                teacherData?["Qualification"]?.ToString() ?? "N/A",
                "Active"
            };

            int y = 30;
            for (int i = 0; i < labels.Length; i++)
            {
                Label lbl1 = new Label();
                lbl1.Text = labels[i];
                lbl1.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lbl1.ForeColor = Color.White;
                lbl1.Location = new Point(30, y);
                lbl1.Size = new Size(150, 30);

                Label lbl2 = new Label();
                lbl2.Text = values[i];
                lbl2.Font = new Font("Segoe UI", 11);
                lbl2.ForeColor = Color.Gainsboro;
                lbl2.Location = new Point(200, y);
                lbl2.Size = new Size(450, 30);

                card.Controls.Add(lbl1);
                card.Controls.Add(lbl2);
                y += 50;
            }

            mainContentPanel.Controls.Add(card);
        }

        private void LoadDashboard()
        {
            ShowDashboard();
        }
    }
}





