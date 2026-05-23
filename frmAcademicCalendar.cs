using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace HawassaUniversity
{
    public partial class frmAcademicCalendar : Form
    {
        public frmAcademicCalendar()
        {
            InitializeComponent();
            LoadAcademicCalendar();
        }

        private void InitializeComponent()
        {
            this.Text = "Academic Calendar - Hawassa University";
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
            lblHeader.Text = "📅 ACADEMIC CALENDAR";
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

            // Calendar View
            MonthCalendar calendar = new MonthCalendar();
            calendar.Location = new Point(20, 20);
            calendar.Size = new Size(300, 200);
            calendar.MaxSelectionCount = 1;
            calendar.DateSelected += (s, e) => LoadEventsForDate(calendar.SelectionStart);
            contentPanel.Controls.Add(calendar);

            // Events Panel
            Panel eventsPanel = new Panel();
            eventsPanel.Location = new Point(350, 20);
            eventsPanel.Size = new Size(580, 600);
            eventsPanel.BackColor = Color.FromArgb(20, 35, 65);
            eventsPanel.AutoScroll = true;

            Label lblEventsTitle = new Label();
            lblEventsTitle.Text = "Upcoming Events";
            lblEventsTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblEventsTitle.ForeColor = Color.White;
            lblEventsTitle.Location = new Point(15, 15);
            lblEventsTitle.Size = new Size(200, 35);
            eventsPanel.Controls.Add(lblEventsTitle);

            contentPanel.Controls.Add(eventsPanel);

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
            contentPanel.Controls.Add(btnBack);

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);

            // Store events panel reference
            this.Tag = eventsPanel;
        }

        private void LoadAcademicCalendar()
        {
            try
            {
                DataTable events = DatabaseHelper.ExecuteQuery(
                    @"SELECT EventTitle, EventDate, EndDate, Description, AcademicYear, EventType 
                      FROM AcademicCalendar 
                      WHERE EventDate >= @StartDate 
                      ORDER BY EventDate",
                    new[] { new SqlParameter("@StartDate", DateTime.Today.AddMonths(-1)) });

                LoadEventsForDate(DateTime.Today);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading calendar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEventsForDate(DateTime date)
        {
            try
            {
                DataTable events = DatabaseHelper.ExecuteQuery(
                    @"SELECT EventTitle, EventDate, EndDate, Description, AcademicYear, EventType 
                      FROM AcademicCalendar 
                      WHERE CAST(EventDate AS DATE) = @EventDate
                      ORDER BY EventDate",
                    new[] { new SqlParameter("@EventDate", date.Date) });

                Panel eventsPanel = this.Tag as Panel;
                if (eventsPanel != null)
                {
                    // Clear existing controls except title
                    for (int i = eventsPanel.Controls.Count - 1; i >= 0; i--)
                    {
                        if (eventsPanel.Controls[i] != eventsPanel.Controls[0])
                            eventsPanel.Controls.RemoveAt(i);
                    }

                    if (events.Rows.Count == 0)
                    {
                        Label lblNoEvents = new Label();
                        lblNoEvents.Text = "No events scheduled for this date.";
                        lblNoEvents.Font = new Font("Segoe UI", 12);
                        lblNoEvents.ForeColor = Color.Gray;
                        lblNoEvents.Location = new Point(15, 60);
                        lblNoEvents.Size = new Size(300, 30);
                        eventsPanel.Controls.Add(lblNoEvents);
                    }
                    else
                    {
                        int y = 60;
                        foreach (DataRow row in events.Rows)
                        {
                            Panel eventCard = new Panel();
                            eventCard.Size = new Size(540, 100);
                            eventCard.Location = new Point(15, y);
                            eventCard.BackColor = Color.FromArgb(30, 50, 85);
                            eventCard.BorderStyle = BorderStyle.None;

                            // Event Type color bar
                            Panel typeBar = new Panel();
                            typeBar.Size = new Size(5, 100);
                            typeBar.Location = new Point(0, 0);
                            string eventType = row["EventType"].ToString();
                            typeBar.BackColor = eventType switch
                            {
                                "Exam" => Color.FromArgb(220, 60, 60),
                                "Registration" => Color.FromArgb(0, 200, 100),
                                "Holiday" => Color.FromArgb(255, 150, 0),
                                "Graduation" => Color.FromArgb(150, 100, 255),
                                _ => Color.FromArgb(0, 150, 255)
                            };

                            Label lblTitle = new Label();
                            lblTitle.Text = row["EventTitle"].ToString();
                            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                            lblTitle.ForeColor = Color.White;
                            lblTitle.Location = new Point(20, 10);
                            lblTitle.Size = new Size(400, 25);

                            Label lblDate = new Label();
                            DateTime eventDate = Convert.ToDateTime(row["EventDate"]);
                            lblDate.Text = $"📅 {eventDate:MMMM dd, yyyy}";
                            lblDate.Font = new Font("Segoe UI", 10);
                            lblDate.ForeColor = Color.FromArgb(150, 160, 180);
                            lblDate.Location = new Point(20, 40);
                            lblDate.Size = new Size(200, 20);

                            Label lblYear = new Label();
                            lblYear.Text = $"📖 {row["AcademicYear"]}";
                            lblYear.Font = new Font("Segoe UI", 10);
                            lblYear.ForeColor = Color.FromArgb(150, 160, 180);
                            lblYear.Location = new Point(230, 40);
                            lblYear.Size = new Size(150, 20);

                            Label lblDesc = new Label();
                            string desc = row["Description"].ToString();
                            lblDesc.Text = desc.Length > 60 ? desc.Substring(0, 60) + "..." : desc;
                            lblDesc.Font = new Font("Segoe UI", 9);
                            lblDesc.ForeColor = Color.FromArgb(180, 190, 210);
                            lblDesc.Location = new Point(20, 65);
                            lblDesc.Size = new Size(500, 25);

                            eventCard.Controls.Add(typeBar);
                            eventCard.Controls.Add(lblTitle);
                            eventCard.Controls.Add(lblDate);
                            eventCard.Controls.Add(lblYear);
                            eventCard.Controls.Add(lblDesc);
                            eventsPanel.Controls.Add(eventCard);
                            y += 110;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading events: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
