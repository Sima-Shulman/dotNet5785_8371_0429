using BlApi;
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



        public int? UserId
        {
            get { return (int)GetValue(UserIdProperty); }
            set { SetValue(UserIdProperty, value); }
        }

        public static readonly DependencyProperty UserIdProperty =
            DependencyProperty.Register("UserId", typeof(int), typeof(LoginWindow), new PropertyMetadata(null));

        /// <summary>
        /// Handles the Click event of the loginBtn control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserId is null || !UserId.ToString()!.All(char.IsDigit) || string.IsNullOrEmpty(Password.ToString()))
            {
                MessageBox.Show("Please enter a valid numeric User ID and Password.");
                return;
            }
            try
            {
                var userRole = s_bl.Volunteer.Login(UserId.Value, Password);
                if (userRole == BO.Enums.Role.Volunteer)
                {
                    new MainVolunteer(UserId.Value).Show();
                }
                else if (userRole == BO.Enums.Role.Manager)
                {
                    var result = MessageBox.Show(
                     "Choose an option:\n\nClick 'Yes' for Manager\nClick 'No' for Volunteer",
                     "Custom Selection",
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Question
                );
                    if (result == MessageBoxResult.Yes)
                    {
                        if (App.Current.Properties["IsManagerLoggedIn"] is true)
                        {
                            MessageBox.Show("A Manager is already logged in to the system.");
                            return;
                        }
                        App.Current.Properties["IsManagerLoggedIn"] = true;
                        new Manager.ManagerWindow(UserId.Value).Show();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        new MainVolunteer(UserId.Value).Show();
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
