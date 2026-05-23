using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmGradeManagement : Form
    {
        private int teacherId;
        private ComboBox cmbCourses;
        private DataGridView dgvGrades;
        private Label lblStatus;

        public frmGradeManagement(int teacherId)
        {
            this.teacherId = teacherId;
            InitializeComponent();
            LoadTeacherCourses();
        }

        private void InitializeComponent()
        {
            this.Text = "Grade Management - Hawassa University";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(7, 20, 43);

            // Header
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(0, 200, 100);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 70;

            Label lblHeader = new Label();
            lblHeader.Text = "✏️ ENTER STUDENT GRADES";
            lblHeader.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Dock = DockStyle.Fill;
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            headerPanel.Controls.Add(lblHeader);

            // Course Selection Panel
            Panel coursePanel = new Panel();
            coursePanel.Location = new Point(20, 90);
            coursePanel.Size = new Size(1040, 50);
            coursePanel.BackColor = Color.FromArgb(20, 35, 65);

            Label lblCourse = new Label();
            lblCourse.Text = "Select Course:";
            lblCourse.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblCourse.ForeColor = Color.White;
            lblCourse.Location = new Point(20, 12);
            lblCourse.Size = new Size(120, 30);
            coursePanel.Controls.Add(lblCourse);

            cmbCourses = new ComboBox();
            cmbCourses.Location = new Point(150, 10);
            cmbCourses.Size = new Size(400, 35);
            cmbCourses.Font = new Font("Segoe UI", 11);
            cmbCourses.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCourses.SelectedIndexChanged += (s, e) => LoadStudentsForCourse();
            coursePanel.Controls.Add(cmbCourses);

            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.Font = new Font("Segoe UI", 10);
            lblStatus.ForeColor = Color.FromArgb(0, 200, 100);
            lblStatus.Location = new Point(580, 15);
            lblStatus.Size = new Size(400, 25);
            coursePanel.Controls.Add(lblStatus);

            // Grades Grid
            dgvGrades = new DataGridView();
            dgvGrades.Location = new Point(20, 160);
            dgvGrades.Size = new Size(1040, 400);
            dgvGrades.BackgroundColor = Color.White;
            dgvGrades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGrades.AllowUserToAddRows = false;
            dgvGrades.RowHeadersVisible = false;
            dgvGrades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGrades.BorderStyle = BorderStyle.None;

            // Buttons
            Button btnSave = new Button();
            btnSave.Text = "SAVE GRADES";
            btnSave.Size = new Size(180, 45);
            btnSave.Location = new Point(400, 590);
            btnSave.BackColor = Color.FromArgb(0, 200, 100);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += (s, e) => SaveGrades();

            Button btnBack = new Button();
            btnBack.Text = "BACK TO DASHBOARD";
            btnBack.Size = new Size(180, 45);
            btnBack.Location = new Point(20, 590);
            btnBack.BackColor = Color.FromArgb(50, 50, 70);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(headerPanel);
            this.Controls.Add(coursePanel);
            this.Controls.Add(dgvGrades);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnBack);
        }

        private void LoadTeacherCourses()
        {
            try
            {
                DataTable courses = DatabaseHelper.ExecuteQuery(
                    @"SELECT DISTINCT c.CourseID, c.CourseCode, c.CourseName
                      FROM TeacherCourseAssignment tca
                      JOIN Courses c ON tca.CourseID = c.CourseID
                      WHERE tca.TeacherID = @TeacherID",
                    new[] { new SqlParameter("@TeacherID", teacherId) });

                cmbCourses.DisplayMember = "CourseCode";
                cmbCourses.ValueMember = "CourseID";
                cmbCourses.DataSource = courses;

                if (courses.Rows.Count > 0)
                {
                    lblStatus.Text = $"{courses.Rows.Count} course(s) assigned to you";
                    cmbCourses.SelectedIndex = 0;
                }
                else
                {
                    lblStatus.Text = "No courses assigned to you";
                    lblStatus.ForeColor = Color.FromArgb(255, 150, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentsForCourse()
        {
            if (cmbCourses.SelectedValue == null) return;

            int courseId = Convert.ToInt32(cmbCourses.SelectedValue);

            try
            {
                DataTable students = DatabaseHelper.ExecuteQuery(
                    @"SELECT s.StudentID, s.FirstName, s.FatherName,
                             e.EnrollmentID,
                             ISNULL(g.Midterm, 0) as Midterm,
                             ISNULL(g.Final, 0) as Final,
                             ISNULL(g.Assignment, 0) as Assignment,
                             ISNULL(g.Quiz, 0) as Quiz
                      FROM Enrollments e
                      JOIN Students s ON e.StudentID = s.StudentID
                      LEFT JOIN Grades g ON e.EnrollmentID = g.EnrollmentID
                      WHERE e.CourseID = @CourseID
                      ORDER BY s.FirstName",
                    new[] { new SqlParameter("@CourseID", courseId) });

                dgvGrades.DataSource = students;

                // Make grade columns editable
                if (!dgvGrades.Columns.Contains("MidtermInput"))
                {
                    dgvGrades.Columns["EnrollmentID"].Visible = false;
                    dgvGrades.Columns["Midterm"].Visible = false;
                    dgvGrades.Columns["Final"].Visible = false;
                    dgvGrades.Columns["Assignment"].Visible = false;
                    dgvGrades.Columns["Quiz"].Visible = false;

                    // Add editable columns
                    AddEditableColumn("MidtermInput", "Midterm (0-30)", 100);
                    AddEditableColumn("FinalInput", "Final (0-50)", 100);
                    AddEditableColumn("AssignmentInput", "Assignment (0-10)", 100);
                    AddEditableColumn("QuizInput", "Quiz (0-10)", 100);
                    AddReadOnlyColumn("TotalDisplay", "Total", 80);
                    AddReadOnlyColumn("GradeDisplay", "Letter Grade", 80);
                }

                // Load existing values
                for (int i = 0; i < dgvGrades.Rows.Count; i++)
                {
                    DataGridViewRow row = dgvGrades.Rows[i];
                    row.Cells["MidtermInput"].Value = row.Cells["Midterm"].Value;
                    row.Cells["FinalInput"].Value = row.Cells["Final"].Value;
                    row.Cells["AssignmentInput"].Value = row.Cells["Assignment"].Value;
                    row.Cells["QuizInput"].Value = row.Cells["Quiz"].Value;
                    CalculateRowTotal(i);
                }

                // Add cell value changed event
                dgvGrades.CellValueChanged += (s, e) =>
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    {
                        string colName = dgvGrades.Columns[e.ColumnIndex].Name;
                        if (colName == "MidtermInput" || colName == "FinalInput" || 
                            colName == "AssignmentInput" || colName == "QuizInput")
                        {
                            CalculateRowTotal(e.RowIndex);
                        }
                    }
                };

                lblStatus.Text = $"{students.Rows.Count} student(s) enrolled in this course";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddEditableColumn(string name, string header, int width)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = name;
            col.HeaderText = header;
            col.Width = width;
            dgvGrades.Columns.Add(col);
        }

        private void AddReadOnlyColumn(string name, string header, int width)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = name;
            col.HeaderText = header;
            col.Width = width;
            col.ReadOnly = true;
            dgvGrades.Columns.Add(col);
        }

        private void CalculateRowTotal(int rowIndex)
        {
            try
            {
                DataGridViewRow row = dgvGrades.Rows[rowIndex];
                
                decimal midterm = GetValue(row, "MidtermInput");
                decimal final = GetValue(row, "FinalInput");
                decimal assignment = GetValue(row, "AssignmentInput");
                decimal quiz = GetValue(row, "QuizInput");
                
                // Cap at maximum values
                midterm = Math.Min(midterm, 30);
                final = Math.Min(final, 50);
                assignment = Math.Min(assignment, 10);
                quiz = Math.Min(quiz, 10);
                
                decimal total = midterm + final + assignment + quiz;
                string letterGrade = GetLetterGrade(total);
                
                row.Cells["TotalDisplay"].Value = total;
                row.Cells["GradeDisplay"].Value = letterGrade;
            }
            catch { }
        }

        private decimal GetValue(DataGridViewRow row, string columnName)
        {
            if (row.Cells[columnName].Value != null && decimal.TryParse(row.Cells[columnName].Value.ToString(), out decimal val))
                return val;
            return 0;
        }

        private string GetLetterGrade(decimal total)
        {
            if (total >= 90) return "A+";
            if (total >= 85) return "A";
            if (total >= 80) return "A-";
            if (total >= 75) return "B+";
            if (total >= 70) return "B";
            if (total >= 65) return "B-";
            if (total >= 60) return "C+";
            if (total >= 50) return "C";
            if (total >= 45) return "C-";
            if (total >= 40) return "D";
            return "F";
        }

        private decimal GetGradePoint(string letterGrade)
        {
            return letterGrade switch
            {
                "A+" => 4.0m, "A" => 4.0m, "A-" => 3.75m,
                "B+" => 3.5m, "B" => 3.0m, "B-" => 2.75m,
                "C+" => 2.5m, "C" => 2.0m, "C-" => 1.75m,
                "D" => 1.0m, _ => 0.0m
            };
        }

        private void SaveGrades()
        {
            if (cmbCourses.SelectedValue == null)
            {
                MessageBox.Show("Please select a course first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int saved = 0;

            using (SqlConnection conn = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=HawassaUniversityDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;"))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < dgvGrades.Rows.Count; i++)
                        {
                            DataGridViewRow row = dgvGrades.Rows[i];
                            int enrollmentId = Convert.ToInt32(row.Cells["EnrollmentID"].Value);
                            
                            decimal midterm = GetValue(row, "MidtermInput");
                            decimal final = GetValue(row, "FinalInput");
                            decimal assignment = GetValue(row, "AssignmentInput");
                            decimal quiz = GetValue(row, "QuizInput");
                            
                            midterm = Math.Min(midterm, 30);
                            final = Math.Min(final, 50);
                            assignment = Math.Min(assignment, 10);
                            quiz = Math.Min(quiz, 10);
                            
                            decimal total = midterm + final + assignment + quiz;
                            string letter = GetLetterGrade(total);
                            decimal gradePoint = GetGradePoint(letter);
                            
                            // Check if exists
                            string checkSql = "SELECT COUNT(*) FROM Grades WHERE EnrollmentID = @EID";
                            using (SqlCommand checkCmd = new SqlCommand(checkSql, conn, trans))
                            {
                                checkCmd.Parameters.AddWithValue("@EID", enrollmentId);
                                int exists = (int)checkCmd.ExecuteScalar();
                                
                                string sql;
                                if (exists > 0)
                                {
                                    sql = @"UPDATE Grades SET 
                                            TeacherID = @TID, Midterm = @M, Final = @F, 
                                            Assignment = @A, Quiz = @Q, Total = @T,
                                            LetterGrade = @L, GradePoint = @GP, GradedOn = GETDATE()
                                            WHERE EnrollmentID = @EID";
                                }
                                else
                                {
                                    sql = @"INSERT INTO Grades (EnrollmentID, TeacherID, Midterm, Final, Assignment, Quiz, Total, LetterGrade, GradePoint, GradedOn)
                                            VALUES (@EID, @TID, @M, @F, @A, @Q, @T, @L, @GP, GETDATE())";
                                }
                                
                                using (SqlCommand cmd = new SqlCommand(sql, conn, trans))
                                {
                                    cmd.Parameters.AddWithValue("@EID", enrollmentId);
                                    cmd.Parameters.AddWithValue("@TID", teacherId);
                                    cmd.Parameters.AddWithValue("@M", midterm);
                                    cmd.Parameters.AddWithValue("@F", final);
                                    cmd.Parameters.AddWithValue("@A", assignment);
                                    cmd.Parameters.AddWithValue("@Q", quiz);
                                    cmd.Parameters.AddWithValue("@T", total);
                                    cmd.Parameters.AddWithValue("@L", letter);
                                    cmd.Parameters.AddWithValue("@GP", gradePoint);
                                    cmd.ExecuteNonQuery();
                                    saved++;
                                }
                            }
                        }
                        trans.Commit();
                        MessageBox.Show($"✅ Grades saved successfully!\n\n{saved} student records updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStudentsForCourse(); // Refresh
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
