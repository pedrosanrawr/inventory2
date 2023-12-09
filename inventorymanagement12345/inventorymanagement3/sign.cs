using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace inventorymanagement3
{
    public partial class sign : Form
    {
        private string username;
        private string email;
        private string password;
        private string reenterpassword;

        private User currentUser;

        public sign()
        {
            InitializeComponent();
            textBox3.PasswordChar = '•';
            textBox4.PasswordChar = '•';
            this.StartPosition = FormStartPosition.CenterScreen;
            currentUser = new User();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            username = textBox1.Text;
            email = textBox2.Text;
            password = textBox3.Text;
            reenterpassword = textBox4.Text;

            // Perform validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all the fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBox3.Text != textBox4.Text)
            {
                MessageBox.Show("Passwords do not match. Please re-enter your password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentUser.UserIdentifier = email;

            SaveToFile(username, email, password);
            MessageBox.Show("Account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            currentUser = new User(username);
            currentUser.IsAuthenticated = true;

            inventory inventory = new inventory(currentUser);
            inventory.Show();
            this.Hide();
        }

        private void SaveToFile(string username, string email, string password)
        {
            // Specify the file path (adjust as needed)
            string filePath = "user_data.json";

            // Create a User object with the provided information
            User user = new User
            {
                Username = username,
                Email = email,
                Password = password,
                UserIdentifier = currentUser.UserIdentifier
            };

            try
            {
                // Serialize the User object to JSON
                string userDataJson = JsonConvert.SerializeObject(user, Formatting.Indented);

                // Append the user information to the file
                File.AppendAllText(filePath, userDataJson + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            login login = new login();
            login.Show();
            this.Hide();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
