\
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace finalProject
{
    public partial class Form2 : Form
    {
        private static readonly Regex EmailPattern = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex PhonePattern = new Regex(
            @"^[0-9+()\-\s]{5,20}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public Form2()
        {
            InitializeComponent();
        }

        public Form? form1Reference { get; set; }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                MessageBox.Show(
                    "Clear the selected registration before adding a new student.",
                    "Registration Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateStudentInput(out string validationMessage))
            {
                ShowValidationMessage(validationMessage);
                return;
            }

            const string query = @"
INSERT INTO dbo.Registration
    (firstName, lastName, dateOfBirth, gender, address, email, mobilePhone,
     homePhone, parentName, nic, contactNo)
VALUES
    (@firstName, @lastName, @dateOfBirth, @gender, @address, @email, @mobilePhone,
     @homePhone, @parentName, @nic, @contactNo);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            try
            {
                using SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                AddStudentParameters(command);
                connection.Open();

                int registrationNumber = Convert.ToInt32(command.ExecuteScalar());
                MessageBox.Show(
                    $"Record {registrationNumber} added successfully.",
                    "Register Student",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadRegistrationNumbers(registrationNumber);
            }
            catch (SqlException)
            {
                ShowDatabaseError();
            }
        }

        private void combobox1_Click(object sender, EventArgs e)
        {
            LoadRegistrationNumbers(GetSelectedRegistrationNumber());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int? registrationNumber = GetSelectedRegistrationNumber();
            if (registrationNumber == null)
            {
                return;
            }

            try
            {
                using SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString);
                using SqlCommand command = new SqlCommand(
                    "SELECT * FROM dbo.Registration WHERE regNo = @regNo",
                    connection);

                command.Parameters.Add("@regNo", SqlDbType.Int).Value = registrationNumber.Value;
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    return;
                }

                firstNameTextbox.Text = reader["firstName"].ToString();
                lastNameTextbox.Text = reader["lastName"].ToString();
                dateTimePicker.Value = Convert.ToDateTime(reader["dateOfBirth"]);
                string gender = reader["gender"].ToString() ?? string.Empty;
                maleRadioBtn.Checked = gender.Equals("Male", StringComparison.OrdinalIgnoreCase);
                femaleRadioBtn.Checked = gender.Equals("Female", StringComparison.OrdinalIgnoreCase);
                addressTextbox.Text = reader["address"].ToString();
                emailTextbox.Text = reader["email"].ToString();
                mobilePhoneTextbox.Text = reader["mobilePhone"].ToString();
                homePhoneTextbox.Text = reader["homePhone"].ToString();
                parentNameTextbox.Text = reader["parentName"].ToString();
                nicTextbox.Text = reader["nic"].ToString();
                contactNoTextbox.Text = reader["contactNo"].ToString();
            }
            catch (SqlException)
            {
                ShowDatabaseError();
            }
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            int? registrationNumber = GetSelectedRegistrationNumber();
            if (registrationNumber == null)
            {
                ShowValidationMessage("Select a registration number to update.");
                return;
            }

            if (!ValidateStudentInput(out string validationMessage))
            {
                ShowValidationMessage(validationMessage);
                return;
            }

            const string query = @"
UPDATE dbo.Registration
SET firstName = @firstName,
    lastName = @lastName,
    dateOfBirth = @dateOfBirth,
    gender = @gender,
    address = @address,
    email = @email,
    mobilePhone = @mobilePhone,
    homePhone = @homePhone,
    parentName = @parentName,
    nic = @nic,
    contactNo = @contactNo
WHERE regNo = @regNo;";

            try
            {
                using SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@regNo", SqlDbType.Int).Value = registrationNumber.Value;
                AddStudentParameters(command);
                connection.Open();

                int changedRows = command.ExecuteNonQuery();
                MessageBox.Show(
                    changedRows > 0 ? "Record updated successfully." : "No matching record was found.",
                    "Update Student",
                    MessageBoxButtons.OK,
                    changedRows > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (SqlException)
            {
                ShowDatabaseError();
            }
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            int? registrationNumber = GetSelectedRegistrationNumber();
            if (registrationNumber == null)
            {
                ShowValidationMessage("Select a registration number to delete.");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Delete registration {registrationNumber}?",
                "Delete Student",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString);
                using SqlCommand command = new SqlCommand(
                    "DELETE FROM dbo.Registration WHERE regNo = @regNo",
                    connection);

                command.Parameters.Add("@regNo", SqlDbType.Int).Value = registrationNumber.Value;
                connection.Open();
                int changedRows = command.ExecuteNonQuery();

                MessageBox.Show(
                    changedRows > 0 ? "Record deleted successfully." : "No matching record was found.",
                    "Delete Student",
                    MessageBoxButtons.OK,
                    changedRows > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                if (changedRows > 0)
                {
                    ClearForm();
                    LoadRegistrationNumbers();
                }
            }
            catch (SqlException)
            {
                ShowDatabaseError();
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void exitLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        private void logoutLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            form1Reference?.Show();
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (form1Reference != null && !form1Reference.Visible)
            {
                form1Reference.Close();
            }
        }

        private void LoadRegistrationNumbers(int? selectedRegistrationNumber = null)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString);
                using SqlCommand command = new SqlCommand(
                    "SELECT regNo FROM dbo.Registration ORDER BY regNo",
                    connection);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();

                comboBox1.BeginUpdate();
                comboBox1.Items.Clear();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetInt32(0));
                }
                comboBox1.EndUpdate();

                if (selectedRegistrationNumber != null)
                {
                    comboBox1.SelectedItem = selectedRegistrationNumber.Value;
                }
            }
            catch (SqlException)
            {
                ShowDatabaseError();
            }
        }

        private int? GetSelectedRegistrationNumber()
        {
            if (comboBox1.SelectedItem == null)
            {
                return null;
            }

            return int.TryParse(comboBox1.SelectedItem.ToString(), out int registrationNumber)
                ? registrationNumber
                : null;
        }

        private void AddStudentParameters(SqlCommand command)
        {
            command.Parameters.Add("@firstName", SqlDbType.NVarChar, 50).Value = firstNameTextbox.Text.Trim();
            command.Parameters.Add("@lastName", SqlDbType.NVarChar, 50).Value = lastNameTextbox.Text.Trim();
            command.Parameters.Add("@dateOfBirth", SqlDbType.Date).Value = dateTimePicker.Value.Date;
            command.Parameters.Add("@gender", SqlDbType.NVarChar, 10).Value = maleRadioBtn.Checked ? "Male" : "Female";
            command.Parameters.Add("@address", SqlDbType.NVarChar, 250).Value = addressTextbox.Text.Trim();
            command.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = emailTextbox.Text.Trim();
            command.Parameters.Add("@mobilePhone", SqlDbType.NVarChar, 20).Value = mobilePhoneTextbox.Text.Trim();
            command.Parameters.Add("@homePhone", SqlDbType.NVarChar, 20).Value = homePhoneTextbox.Text.Trim();
            command.Parameters.Add("@parentName", SqlDbType.NVarChar, 100).Value = parentNameTextbox.Text.Trim();
            command.Parameters.Add("@nic", SqlDbType.NVarChar, 50).Value = nicTextbox.Text.Trim();
            command.Parameters.Add("@contactNo", SqlDbType.NVarChar, 20).Value = contactNoTextbox.Text.Trim();
        }

        private bool ValidateStudentInput(out string message)
        {
            if (string.IsNullOrWhiteSpace(firstNameTextbox.Text)
                || string.IsNullOrWhiteSpace(lastNameTextbox.Text)
                || string.IsNullOrWhiteSpace(addressTextbox.Text)
                || string.IsNullOrWhiteSpace(emailTextbox.Text)
                || string.IsNullOrWhiteSpace(parentNameTextbox.Text)
                || string.IsNullOrWhiteSpace(nicTextbox.Text)
                || string.IsNullOrWhiteSpace(contactNoTextbox.Text))
            {
                message = "Complete all required student, contact, and parent fields.";
                return false;
            }

            if (!maleRadioBtn.Checked && !femaleRadioBtn.Checked)
            {
                message = "Select a gender.";
                return false;
            }

            if (dateTimePicker.Value.Date > DateTime.Today)
            {
                message = "Date of birth cannot be in the future.";
                return false;
            }

            if (!EmailPattern.IsMatch(emailTextbox.Text.Trim()))
            {
                message = "Enter a valid email address.";
                return false;
            }

            foreach (string phone in new[]
                     {
                         mobilePhoneTextbox.Text.Trim(),
                         homePhoneTextbox.Text.Trim(),
                         contactNoTextbox.Text.Trim()
                     })
            {
                if (!string.IsNullOrWhiteSpace(phone) && !PhonePattern.IsMatch(phone))
                {
                    message = "Phone numbers may contain digits, spaces, +, -, and parentheses.";
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        private void ClearForm()
        {
            comboBox1.SelectedIndex = -1;
            firstNameTextbox.Clear();
            lastNameTextbox.Clear();
            dateTimePicker.Value = DateTime.Today;
            maleRadioBtn.Checked = false;
            femaleRadioBtn.Checked = false;
            addressTextbox.Clear();
            emailTextbox.Clear();
            mobilePhoneTextbox.Clear();
            homePhoneTextbox.Clear();
            parentNameTextbox.Clear();
            nicTextbox.Clear();
            contactNoTextbox.Clear();
            ActiveControl = firstNameTextbox;
        }

        private static void ShowValidationMessage(string message)
        {
            MessageBox.Show(
                message,
                "Check Student Details",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private static void ShowDatabaseError()
        {
            MessageBox.Show(
                "The Student database could not be reached or the requested operation failed. Verify the database setup and connection settings.",
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
