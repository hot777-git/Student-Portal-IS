using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HawassaUniversity
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(7, 20, 43),
                Color.FromArgb(13, 27, 55),
                90))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Hawassa University – Student Information System";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(7, 20, 43);
            this.MinimumSize = new Size(900, 650);

            // Header Panel
            Panel headerPanel = new Panel();
            headerPanel.BackColor = Color.FromArgb(11, 26, 53);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 140;

            Label lblUniversity = new Label();
            lblUniversity.Text = "HAWASSA UNIVERSITY";
            lblUniversity.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblUniversity.ForeColor = Color.White;
            lblUniversity.Location = new Point(30, 35);
            lblUniversity.Size = new Size(500, 55);

            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Student Information System | Hawassa, Sidama Region, Ethiopia";
            lblSubtitle.Font = new Font("Segoe UI", 11);
            lblSubtitle.ForeColor = Color.FromArgb(180, 190, 210);
            lblSubtitle.Location = new Point(30, 85);
            lblSubtitle.Size = new Size(600, 30);

            headerPanel.Controls.Add(lblUniversity);
            headerPanel.Controls.Add(lblSubtitle);

            // Section Title
            Label lblSelectPortal = new Label();
            lblSelectPortal.Text = "Select your portal to continue:";
            lblSelectPortal.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblSelectPortal.ForeColor = Color.White;
            lblSelectPortal.Location = new Point(30, 160);
            lblSelectPortal.Size = new Size(450, 40);

            // Calculate positions for 2x2 grid
            int cardWidth = 480;
            int cardHeight = 200;
            int spacing = 40;
            int startX = 50;
            int startY = 220;

            // Student Portal Card (Blue)
            Panel studentCard = CreateModernCard(
                "Student Portal",
                "Login or register as a student",
                "Enter →",
                Color.FromArgb(0, 150, 255),
                "👨‍🎓",
                startX, startY, cardWidth, cardHeight);
            
            // Teacher Portal Card (Green)
            Panel teacherCard = CreateModernCard(
                "Teacher Portal",
                "Login or register as a teacher",
                "Enter →",
                Color.FromArgb(0, 200, 100),
                "👩‍🏫",
                startX + cardWidth + spacing, startY, cardWidth, cardHeight);
            
            // Head Teacher Portal Card (Purple)
            Panel headCard = CreateModernCard(
                "Head Teacher Portal",
                "Login with head credentials (provided by Registrar)",
                "Enter →",
                Color.FromArgb(150, 100, 255),
                "👨‍💼",
                startX, startY + cardHeight + spacing, cardWidth, cardHeight);
            
            // Registration Portal Card (Red)
            Panel registrarCard = CreateModernCard(
                "Registration Portal",
                "Registrar: manage students, heads, generate IDs",
                "Enter →",
                Color.FromArgb(220, 60, 60),
                "📋",
                startX + cardWidth + spacing, startY + cardHeight + spacing, cardWidth, cardHeight);
            
            // Find buttons inside cards and attach click events
            foreach (Control ctrl in studentCard.Controls)
            {
                if (ctrl is Button btn && btn.Text == "Enter →")
                    btn.Click += (s, e) => OpenStudentLogin();
            }
            studentCard.Click += (s, e) => OpenStudentLogin();

            foreach (Control ctrl in teacherCard.Controls)
            {
                if (ctrl is Button btn && btn.Text == "Enter →")
                    btn.Click += (s, e) => OpenTeacherLogin();
            }
            teacherCard.Click += (s, e) => OpenTeacherLogin();

            foreach (Control ctrl in headCard.Controls)
            {
                if (ctrl is Button btn && btn.Text == "Enter →")
                    btn.Click += (s, e) => OpenHeadTeacherLogin();
            }
            headCard.Click += (s, e) => OpenHeadTeacherLogin();

            foreach (Control ctrl in registrarCard.Controls)
            {
                if (ctrl is Button btn && btn.Text == "Enter →")
                    btn.Click += (s, e) => OpenRegistrationPortal();
            }
            registrarCard.Click += (s, e) => OpenRegistrationPortal();

            // Footer
            Label lblFooter = new Label();
            lblFooter.Text = "© 2024 Hawassa University | IT Department | it@hwu.edu.et";
            lblFooter.Font = new Font("Segoe UI", 9);
            lblFooter.ForeColor = Color.FromArgb(100, 120, 150);
            lblFooter.Dock = DockStyle.Bottom;
            lblFooter.Height = 40;
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(headerPanel);
            this.Controls.Add(lblSelectPortal);
            this.Controls.Add(studentCard);
            this.Controls.Add(teacherCard);
            this.Controls.Add(headCard);
            this.Controls.Add(registrarCard);
            this.Controls.Add(lblFooter);
        }

        private Panel CreateModernCard(string title, string description, string buttonText,
            Color accentColor, string icon, int x, int y, int width, int height)
        {
            Panel card = new Panel();
            card.Size = new Size(width, height);
            card.Location = new Point(x, y);
            card.BackColor = Color.FromArgb(20, 35, 65);
            card.Cursor = Cursors.Hand;

            // Rounded corners
            card.Paint += (s, e) =>
            {
                GraphicsPath path = GetRoundedRectangle(card.ClientRectangle, 15);
                card.Region = new Region(path);
            };

            // Top accent bar
            Panel accentBar = new Panel();
            accentBar.Size = new Size(width, 5);
            accentBar.Location = new Point(0, 0);
            accentBar.BackColor = accentColor;

            // Icon
            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 40);
            lblIcon.ForeColor = accentColor;
            lblIcon.Location = new Point(20, 25);
            lblIcon.Size = new Size(70, 60);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(105, 30);
            lblTitle.Size = new Size(250, 35);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Description
            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Font = new Font("Segoe UI", 11);
            lblDesc.ForeColor = Color.FromArgb(180, 190, 210);
            lblDesc.Location = new Point(20, 95);
            lblDesc.Size = new Size(380, 50);
            lblDesc.TextAlign = ContentAlignment.TopLeft;

            // Button
            Button btn = new Button();
            btn.Text = buttonText;
            btn.Size = new Size(120, 40);
            btn.Location = new Point(width - 140, height - 55);
            btn.BackColor = accentColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;

            // Hover effects
            card.MouseEnter += (s, e) => { card.BackColor = Color.FromArgb(30, 50, 85); };
            card.MouseLeave += (s, e) => { card.BackColor = Color.FromArgb(20, 35, 65); };
            btn.MouseEnter += (s, e) => { btn.BackColor = DarkenColor(accentColor); };
            btn.MouseLeave += (s, e) => { btn.BackColor = accentColor; };

            card.Controls.Add(accentBar);
            card.Controls.Add(lblIcon);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDesc);
            card.Controls.Add(btn);

            return card;
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Color DarkenColor(Color color)
        {
            return Color.FromArgb(
                Math.Max(0, color.R - 40),
                Math.Max(0, color.G - 40),
                Math.Max(0, color.B - 40));
        }

        private void OpenStudentLogin()
        {
            var loginForm = new frmStudentLogin();
            loginForm.ShowDialog();
        }

        private void OpenTeacherLogin()
        {
            var loginForm = new frmTeacherLogin();
            loginForm.ShowDialog();
        }

        private void OpenHeadTeacherLogin()
        {
            // This will be fixed separately - for now show message
            new frmHeadTeacherLogin().ShowDialog();
        }

                private void OpenRegistrationPortal()
        {
            // Require registrar login first
            using (var loginForm = new frmRegistrarLogin())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    var regPortal = new frmRegistrationPortal();
                    regPortal.ShowDialog();
                }
            }
        }
    }
}



