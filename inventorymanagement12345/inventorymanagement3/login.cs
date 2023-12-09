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

namespace inventorymanagement3
{
    public partial class login : Form
    {
        private User currentUser;

        public login()
        {
            InitializeComponent();
            textBox3.PasswordChar = '•';
            this.StartPosition = FormStartPosition.CenterScreen;
            currentUser = new User();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string enteredIdentifier = textBox2.Text;
            string enteredPassword = textBox3.Text;

            // Perform login validation
            if (string.IsNullOrWhiteSpace(enteredIdentifier) || string.IsNullOrWhiteSpace(enteredPassword))
            {
                MessageBox.Show("Please enter both username/email and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the user exists in the file
            if (UserExists(enteredIdentifier, enteredPassword))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Set the UserIdentifier property
                currentUser.UserIdentifier = enteredIdentifier;
                currentUser.IsAuthenticated = true;

                inventory inventory = new inventory(currentUser);
                inventory.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username/email or password. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool UserExists(string enteredIdentifier, string enteredPassword)
        {
            string filePath = "user_data.txt";

            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Check if the entered username or email and password match any user in the file
                foreach (string line in lines)
                {
                    string[] userData = line.Split(',');

                    // Check if the entered identifier matches the stored username or email, and the password is correct
                    if ((userData[0] == enteredIdentifier || userData[1] == enteredIdentifier) && userData[2] == enteredPassword)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sign sign = new sign();
            sign.Show();
            this.Hide();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
