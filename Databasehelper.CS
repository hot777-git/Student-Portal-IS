using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace HawassaUniversity
{
    public static class DatabaseHelper
    {
        private const string ConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=HawassaUniversityDB;" +
            "Integrated Security=True;TrustServerCertificate=True;Encrypt=False;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        // Email Validation Method
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            
            try
            {
                // Check for @ and . in the email
                if (!email.Contains("@") || !email.Contains("."))
                    return false;
                
                // Check if it ends with valid domain
                string[] validDomains = { ".com", ".et", ".org", ".net", ".edu", ".gov" };
                bool hasValidDomain = false;
                foreach (string domain in validDomains)
                {
                    if (email.ToLower().EndsWith(domain))
                    {
                        hasValidDomain = true;
                        break;
                    }
                }
                
                if (!hasValidDomain)
                    return false;
                
                // Check if there's something before the @
                string[] parts = email.Split('@');
                if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                    return false;
                
                // Check if the local part (before @) is valid (at least 3 characters)
                string localPart = parts[0];
                if (localPart.Length < 3)
                    return false;
                
                // Check for valid characters (letters, numbers, dots, underscores, hyphens)
                foreach (char c in localPart)
                {
                    if (!char.IsLetterOrDigit(c) && c != '.' && c != '_' && c != '-')
                        return false;
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Student Authentication
        public static DataRow? AuthenticateStudent(string username, string password)
        {
            try
            {
                var passwordHash = HashPassword(password);
                
                using var conn = GetConnection();
                conn.Open();
                
                string query = "SELECT * FROM Students WHERE Username = @Username AND PasswordHash = @PasswordHash AND Status = 'Active'";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                
                using var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                return null;
            }
            catch (Exception ex)
            {
                ShowError(ex, "Student Authentication");
                return null;
            }
        }

        // Teacher Authentication
        public static DataRow? AuthenticateTeacher(string username, string password)
        {
            try
            {
                var passwordHash = HashPassword(password);
                
                using var conn = GetConnection();
                conn.Open();
                
                string query = "SELECT * FROM Teachers WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = 1";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                
                using var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                return null;
            }
            catch (Exception ex)
            {
                ShowError(ex, "Teacher Authentication");
                return null;
            }
        }

        // Head Teacher Authentication
        public static DataRow? AuthenticateHeadTeacher(string username, string password)
        {
            try
            {
                var passwordHash = HashPassword(password);
                
                using var conn = GetConnection();
                conn.Open();
                
                string query = "SELECT * FROM HeadCredentials WHERE Username = @Username AND PasswordHash = @PasswordHash";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                
                using var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);
                
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                return null;
            }
            catch (Exception ex)
            {
                ShowError(ex, "Head Teacher Authentication");
                return null;
            }
        }

        public static void ShowError(Exception ex, string context = "")
        {
            MessageBox.Show($"Error: {ex.Message}\n\nContext: {context}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DataTable ExecuteQuery(string sql, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public static DataTable ExecuteStoredProcQuery(string procName, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(procName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public static int ExecuteNonQuery(string sql, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }
    }

    public static class Session
    {
        public static string CurrentUsername { get; set; } = "";
        public static string CurrentRole { get; set; } = "";
        public static string CurrentStudentID { get; set; } = "";
        public static string CurrentStudentName { get; set; } = "";
        public static int CurrentTeacherId { get; set; }
        public static string CurrentTeacherName { get; set; } = "";
        public static string CurrentHeadDept { get; set; } = "";

        public static void Clear()
        {
            CurrentUsername = "";
            CurrentRole = "";
            CurrentStudentID = "";
            CurrentStudentName = "";
            CurrentTeacherId = 0;
            CurrentTeacherName = "";
            CurrentHeadDept = "";
        }
    }
}
