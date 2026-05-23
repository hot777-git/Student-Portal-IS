using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmRulesAndRegulations : Form
    {
        public frmRulesAndRegulations()
        {
            InitializeComponent();
            LoadRules();
        }

        private void InitializeComponent()
        {
            this.Text = "Rules and Regulations - Hawassa University";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(7, 20, 43);
            this.MinimumSize = new Size(800, 600);

            // Header
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(0, 150, 255);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;

            Label lblHeader = new Label();
            lblHeader.Text = "📜 RULES AND REGULATIONS";
            lblHeader.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Dock = DockStyle.Fill;
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            headerPanel.Controls.Add(lblHeader);

            // Content Panel
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(20);
            contentPanel.AutoScroll = true;

            // Back Button
            Button btnBack = new Button();
            btnBack.Text = "BACK TO DASHBOARD";
            btnBack.Size = new Size(200, 45);
            btnBack.Location = new Point(20, 620);
            btnBack.BackColor = Color.FromArgb(50, 50, 70);
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);
            this.Controls.Add(btnBack);
        }

        private void LoadRules()
        {
            try
            {
                DataTable rules = DatabaseHelper.ExecuteQuery(
                    @"SELECT Category, Title, Description, CreatedAt 
                      FROM RulesAndRegulations 
                      ORDER BY Category, RuleID",
                    null);

                Panel contentPanel = this.Controls[1] as Panel;
                if (contentPanel != null)
                {
                    int y = 20;
                    string currentCategory = "";

                    foreach (DataRow row in rules.Rows)
                    {
                        string category = row["Category"].ToString();

                        if (category != currentCategory)
                        {
                            // Category Header
                            Label lblCategory = new Label();
                            lblCategory.Text = category;
                            lblCategory.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                            lblCategory.ForeColor = Color.FromArgb(0, 150, 255);
                            lblCategory.Location = new Point(20, y);
                            lblCategory.Size = new Size(900, 35);
                            contentPanel.Controls.Add(lblCategory);
                            y += 45;
                            currentCategory = category;
                        }

                        // Rule Card
                        Panel ruleCard = new Panel();
                        ruleCard.Size = new Size(920, 100);
                        ruleCard.Location = new Point(20, y);
                        ruleCard.BackColor = Color.FromArgb(20, 35, 65);
                        ruleCard.BorderStyle = BorderStyle.None;

                        Label lblTitle = new Label();
                        lblTitle.Text = row["Title"].ToString();
                        lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                        lblTitle.ForeColor = Color.White;
                        lblTitle.Location = new Point(15, 10);
                        lblTitle.Size = new Size(890, 25);

                        Label lblDesc = new Label();
                        string desc = row["Description"].ToString();
                        lblDesc.Text = desc;
                        lblDesc.Font = new Font("Segoe UI", 10);
                        lblDesc.ForeColor = Color.FromArgb(180, 190, 210);
                        lblDesc.Location = new Point(15, 40);
                        lblDesc.Size = new Size(890, 50);

                        ruleCard.Controls.Add(lblTitle);
                        ruleCard.Controls.Add(lblDesc);
                        contentPanel.Controls.Add(ruleCard);
                        y += 110;
                    }
                }
            }
            catch (Exception ex)
            {
                // Add default rules if database is empty
                Panel contentPanel = this.Controls[1] as Panel;
                if (contentPanel != null)
                {
                    AddDefaultRule(contentPanel, "Academic Integrity", "Plagiarism Policy", 
                        "All academic work must be original. Plagiarism, cheating, and academic dishonesty are serious offences. Penalties range from zero marks to expulsion depending on severity.", 20);
                    AddDefaultRule(contentPanel, "Academic Integrity", "Code of Conduct", 
                        "Students are expected to maintain respect for fellow students, staff, and university property. Any form of harassment, violence, or discrimination is grounds for suspension or expulsion.", 130);
                    AddDefaultRule(contentPanel, "Attendance", "Minimum Attendance Requirement", 
                        "Students must attend a minimum of 75% of classes for each course. Students who fail to meet this requirement will not be allowed to sit for the final examination.", 240);
                    AddDefaultRule(contentPanel, "Grading", "Grade Distribution", 
                        "Assessment is distributed as follows: Midterm Examination = 30%, Final Examination = 50%, Assignment = 10%, Quiz = 10%. Total = 100%.", 350);
                }
            }
        }

        private void AddDefaultRule(Panel panel, string category, string title, string description, int y)
        {
            Label lblCategory = new Label();
            lblCategory.Text = category;
            lblCategory.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblCategory.ForeColor = Color.FromArgb(0, 150, 255);
            lblCategory.Location = new Point(20, y);
            lblCategory.Size = new Size(900, 35);
            panel.Controls.Add(lblCategory);

            Panel ruleCard = new Panel();
            ruleCard.Size = new Size(920, 100);
            ruleCard.Location = new Point(20, y + 40);
            ruleCard.BackColor = Color.FromArgb(20, 35, 65);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(15, 10);
            lblTitle.Size = new Size(890, 25);

            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Font = new Font("Segoe UI", 10);
            lblDesc.ForeColor = Color.FromArgb(180, 190, 210);
            lblDesc.Location = new Point(15, 40);
            lblDesc.Size = new Size(890, 50);

            ruleCard.Controls.Add(lblTitle);
            ruleCard.Controls.Add(lblDesc);
            panel.Controls.Add(ruleCard);
        }
    }
}
