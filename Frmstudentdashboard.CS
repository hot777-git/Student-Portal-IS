using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmStudentDashboard : Form
    {
        private string studentId;
        private string studentName;
        private DataRow? studentData;

        private Panel mainContentPanel = null!;
        private FlowLayoutPanel sidebarPanel = null!;
        private Panel headerPanel = null!;

        public frmStudentDashboard(string studentId, string studentName, DataRow data)
        {
            this.studentId = studentId;
            this.studentName = studentName;
            this.studentData = data;

            InitializeComponent();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = $"Student Dashboard - {studentName}";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 700);
            this.BackColor = Color.FromArgb(7, 20, 43);
            this.StartPosition = FormStartPosition.CenterScreen;

            // HEADER
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.BackColor = Color.FromArgb(0, 140, 255);

            Label lblWelcome = new Label();
            lblWelcome.Text = $"Welcome, {studentName}";
            lblWelcome.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblWelcome.ForeColor = Color.White;
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(30, 20);

            Label lblStudentId = new Label();
            lblStudentId.Text = $"Student ID: {studentId}";
            lblStudentId.Font = new Font("Segoe UI", 12);
            lblStudentId.ForeColor = Color.WhiteSmoke;
            lblStudentId.AutoSize = true;
            lblStudentId.Location = new Point(32, 60);

            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblStudentId);

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

            // BUTTONS - No Register Courses button
            Button btnDashboard = CreateSidebarButton("📊 Dashboard");
            Button btnCourses = CreateSidebarButton("📚 My Courses");
            Button btnGrades = CreateSidebarButton("📈 My Grades");
            Button btnSchedule = CreateSidebarButton("📅 Schedule");
            Button btnPayment = CreateSidebarButton("💰 Payment History");
            Button btnProfile = CreateSidebarButton("👤 Profile");
            Button btnLogout = CreateSidebarButton("🚪 Logout");

            btnDashboard.Click += (s, e) => ShowDashboard();
            btnCourses.Click += (s, e) => ShowMyCourses();
            btnGrades.Click += (s, e) => ShowGrades();
            btnSchedule.Click += (s, e) => ShowSchedule();
            btnPayment.Click += (s, e) => ShowPaymentHistory();
            btnProfile.Click += (s, e) => ShowProfile();
            btnLogout.Click += (s, e) => this.Close();

            sidebarPanel.Controls.Add(btnDashboard);
            sidebarPanel.Controls.Add(btnCourses);
            sidebarPanel.Controls.Add(btnGrades);
            sidebarPanel.Controls.Add(btnSchedule);
            sidebarPanel.Controls.Add(btnPayment);
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

            int enrolledCount = 0;
            try
            {
                DataTable countTable = DatabaseHelper.ExecuteQuery(
                    "SELECT COUNT(*) as Count FROM Enrollments WHERE StudentID = @StudentID",
                    new[] { new SqlParameter("@StudentID", studentId) });
                if (countTable.Rows.Count > 0)
                    enrolledCount = Convert.ToInt32(countTable.Rows[0]["Count"]);
            }
            catch { }

            Panel card = new Panel();
            card.Location = new Point(20, 110);
            card.Size = new Size(550, 200);
            card.BackColor = Color.FromArgb(20, 35, 65);

            Label lbl = new Label();
            lbl.Text = $"Welcome back, {studentName}\n\n" +
                      $"Student ID: {studentId}\n" +
                      $"Status: Active\n" +
                      $"Enrolled Courses: {enrolledCount}\n" +
                      $"Semester: 1st Semester 2024/2025";
            lbl.Font = new Font("Segoe UI", 12);
            lbl.ForeColor = Color.WhiteSmoke;
            lbl.Location = new Point(20, 20);
            lbl.Size = new Size(500, 150);

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
                    @"SELECT c.CourseCode, c.CourseName, c.CreditHours, c.Department, 
                             e.AcademicYear, e.Semester
                      FROM Enrollments e
                      JOIN Courses c ON e.CourseID = c.CourseID
                      WHERE e.StudentID = @StudentID
                      ORDER BY c.CourseCode",
                    new[] { new SqlParameter("@StudentID", studentId) });

                if (courses.Rows.Count == 0)
                {
                    Label lbl = new Label();
                    lbl.Text = "📭 No enrolled courses found.\n\nPlease contact the Registrar for course registration.";
                    lbl.Font = new Font("Segoe UI", 12);
                    lbl.ForeColor = Color.WhiteSmoke;
                    lbl.Location = new Point(20, 110);
                    lbl.AutoSize = true;
                    mainContentPanel.Controls.Add(lbl);
                }
                else
                {
                    Label lblCount = new Label();
                    lblCount.Text = $"Total Enrolled Courses: {courses.Rows.Count}";
                    lblCount.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    lblCount.ForeColor = Color.FromArgb(0, 200, 100);
                    lblCount.Location = new Point(20, 95);
                    lblCount.AutoSize = true;
                    mainContentPanel.Controls.Add(lblCount);

                    DataGridView dgv = CreateGrid(courses, 130);
                    mainContentPanel.Controls.Add(dgv);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowGrades()
        {
            ClearContent();
            AddSectionTitle("My Grades");
            AddSeparator();

            try
            {
                DataTable grades = DatabaseHelper.ExecuteQuery(
                    @"SELECT c.CourseCode, c.CourseName, c.CreditHours,
                             g.Midterm, g.Final, g.Assignment, g.Quiz,
                             g.Total, g.LetterGrade, g.GradePoint
                      FROM Enrollments e
                      JOIN Courses c ON e.CourseID = c.CourseID
                      LEFT JOIN Grades g ON e.EnrollmentID = g.EnrollmentID
                      WHERE e.StudentID = @StudentID
                      ORDER BY c.CourseCode",
                    new[] { new SqlParameter("@StudentID", studentId) });

                if (grades.Rows.Count == 0)
                {
                    Label lbl = new Label();
                    lbl.Text = "📊 No grades available yet.\n\nGrades will appear once your instructors publish them.";
                    lbl.Font = new Font("Segoe UI", 12);
                    lbl.ForeColor = Color.WhiteSmoke;
                    lbl.Location = new Point(20, 110);
                    lbl.AutoSize = true;
                    mainContentPanel.Controls.Add(lbl);
                }
                else
                {
                    DataGridView dgv = CreateGrid(grades, 110);
                    mainContentPanel.Controls.Add(dgv);

                    decimal totalPoints = 0;
                    int totalCredits = 0;
                    foreach (DataRow row in grades.Rows)
                    {
                        decimal gradePoint = row["GradePoint"] != DBNull.Value ? Convert.ToDecimal(row["GradePoint"]) : 0;
                        int credits = row["CreditHours"] != DBNull.Value ? Convert.ToInt32(row["CreditHours"]) : 0;
                        totalPoints += gradePoint * credits;
                        totalCredits += credits;
                    }
                    
                    decimal gpa = totalCredits > 0 ? totalPoints / totalCredits : 0;
                    
                    Label lblGPA = new Label();
                    lblGPA.Text = $"Current GPA: {gpa:F2}";
                    lblGPA.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                    lblGPA.ForeColor = Color.FromArgb(0, 200, 100);
                    lblGPA.Location = new Point(20, 540);
                    lblGPA.Size = new Size(300, 30);
                    mainContentPanel.Controls.Add(lblGPA);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading grades: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowSchedule()
        {
            ClearContent();
            AddSectionTitle("Weekly Schedule");
            AddSeparator();

            try
            {
                DataTable enrolledCourses = DatabaseHelper.ExecuteQuery(
                    @"SELECT c.CourseCode, c.CourseName
                      FROM Enrollments e
                      JOIN Courses c ON e.CourseID = c.CourseID
                      WHERE e.StudentID = @StudentID",
                    new[] { new SqlParameter("@StudentID", studentId) });

                string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
                int y = 110;
                int courseIndex = 0;

                foreach (string day in days)
                {
                    Panel card = new Panel();
                    card.Location = new Point(20, y);
                    card.Size = new Size(700, 55);
                    card.BackColor = Color.FromArgb(20, 35, 65);

                    Label lblDay = new Label();
                    lblDay.Text = day;
                    lblDay.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    lblDay.ForeColor = Color.FromArgb(0, 200, 100);
                    lblDay.Location = new Point(20, 15);
                    lblDay.Size = new Size(120, 25);

                    Label lblCourse = new Label();
                    
                    if (enrolledCourses.Rows.Count > courseIndex)
                    {
                        lblCourse.Text = $"📚 {enrolledCourses.Rows[courseIndex]["CourseCode"]} - {enrolledCourses.Rows[courseIndex]["CourseName"]}";
                        courseIndex++;
                    }
                    else
                    {
                        lblCourse.Text = "No class scheduled";
                    }
                    
                    lblCourse.Font = new Font("Segoe UI", 11);
                    lblCourse.ForeColor = Color.WhiteSmoke;
                    lblCourse.Location = new Point(160, 15);
                    lblCourse.AutoSize = true;

                    card.Controls.Add(lblDay);
                    card.Controls.Add(lblCourse);
                    mainContentPanel.Controls.Add(card);
                    y += 70;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowPaymentHistory()
        {
            ClearContent();
            AddSectionTitle("Payment History");
            AddSeparator();

            Label lbl = new Label();
            lbl.Text = "💰 No outstanding payments found.\n\nAll fees are currently paid.";
            lbl.Font = new Font("Segoe UI", 12);
            lbl.ForeColor = Color.FromArgb(0, 200, 100);
            lbl.Location = new Point(20, 110);
            lbl.AutoSize = true;
            mainContentPanel.Controls.Add(lbl);
        }

        private void ShowProfile()
        {
            ClearContent();
            AddSectionTitle("My Profile");
            AddSeparator();

            int enrolledCount = 0;
            try
            {
                DataTable countTable = DatabaseHelper.ExecuteQuery(
                    "SELECT COUNT(*) as Count FROM Enrollments WHERE StudentID = @StudentID",
                    new[] { new SqlParameter("@StudentID", studentId) });
                if (countTable.Rows.Count > 0)
                    enrolledCount = Convert.ToInt32(countTable.Rows[0]["Count"]);
            }
            catch { }

            Panel card = new Panel();
            card.Location = new Point(20, 110);
            card.Size = new Size(700, 350);
            card.BackColor = Color.FromArgb(20, 35, 65);

            string[] labels = { "Student ID:", "Full Name:", "Email:", "Department:", "Year:", "Status:", "Enrolled Courses:" };
            string[] values = {
                studentId,
                studentName,
                studentData?["Email"]?.ToString() ?? "N/A",
                studentData?["Department"]?.ToString() ?? "Not assigned",
                "Year 3",
                "Active",
                enrolledCount.ToString()
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
