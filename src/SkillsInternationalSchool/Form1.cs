\
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace finalProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            usernameTextbox.Clear();
            passwordTextbox.Clear();
            ActiveControl = usernameTextbox;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username = usernameTextbox.Text.Trim();
            string password = passwordTextbox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                ShowInvalidLogin();
                return;
            }

            try
            {
                using SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString);
                using SqlCommand command = new SqlCommand(
                    "SELECT [password] FROM dbo.Logins WHERE username = @username",
                    connection);

                command.Parameters.Add("@username", SqlDbType.NVarChar, 50).Value = username;
                connection.Open();

                object? storedPassword = command.ExecuteScalar();
                bool authenticated = storedPassword != null
                    && string.Equals(storedPassword.ToString(), password, StringComparison.Ordinal);

                if (!authenticated)
                {
                    ShowInvalidLogin();
                    return;
                }

                passwordTextbox.Clear();
                Form2 registrationForm = new Form2
                {
                    form1Reference = this
                };

                Hide();
                registrationForm.Show();
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "The Student database could not be reached. Run database/setup.sql and verify the connection settings.",
                    "Database Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void ShowInvalidLogin()
        {
            MessageBox.Show(
                "Invalid login credentials. Check the username and password and try again.",
                "Invalid Login Details",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
