using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Login
{

    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }


        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(LoginWindow), new PropertyMetadata(string.Empty));



        public int UserId
        {
            get { return (int)GetValue(UserIdProperty); }
            set { SetValue(UserIdProperty, value); }
        }

        public static readonly DependencyProperty UserIdProperty =
            DependencyProperty.Register("UserId", typeof(int), typeof(LoginWindow), new PropertyMetadata(0));

        /// <summary>
        /// Handles the Click event of the loginBtn control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!UserId.ToString().All(char.IsDigit) || string.IsNullOrEmpty(Password.ToString()))
            {
                MessageBox.Show("Please enter a valid numeric User ID and Password.");
                return;
            }
            try
            {
                var userRole = s_bl.Volunteer.Login(UserId, Password);
                if (userRole == BO.Enums.Role.Volunteer)
                {
                    MessageBox.Show("Login successful!");
                    new MainVolunteer(UserId).Show();
                }
                else if (userRole == BO.Enums.Role.Manager)
                {
                    var result = MessageBox.Show(
                     "Choose an option:\n\nClick 'Yes' for Manager\nClick 'No' for Volinteer",
                     "Custom Selection",
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Question
                    );
                    if (result == MessageBoxResult.Yes)
                    {
                        MessageBox.Show("You are navigated to manager window.");
                        new Manager.ManagerWindow().Show();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        MessageBox.Show("You are navigated to volunteer window.");
                        new MainVolunteer(UserId).Show();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid User ID or Password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login: {ex.Message}");
            }

        }
    }
}
