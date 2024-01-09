using System;
using System.Windows;
using System.Windows.Controls;
using StoreWPFApp.Enums;
using StoreWPFApp.Managerrs;

namespace StoreWPFApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;
            UserManager.UserListChanged += UserManager_UserListChanged;
            UserManager.UsernameAlreadyTaken += () => RegistrationErrorLabel.Visibility = Visibility.Visible;
            UserManager.NotExistnigUsernameLogIn += () =>  LogInUsernameErrorLabel.Visibility = Visibility.Visible;
            UserManager.WrongPasswordLogInTryDenied += () => LogInPasswordErrorLabel.Visibility = Visibility.Visible;
            // not sure that this needs to be hear it can act as indicator to apdate screen ? or list that we are using  
        }
        private void RegisterPwd_OnPasswordChanged(object sender, RoutedEventArgs e) // fix!!
        {
            RegistrationPasswordErrorLabel.Visibility = Visibility.Hidden;
        }
        private void LoginName_OnTextChanged(object sender, TextChangedEventArgs e) 
        {
            LogInUsernameErrorLabel.Visibility = Visibility.Hidden;
        }
        private void RegisterName_OnTextChanged(object sender, TextChangedEventArgs e) // if user starts writing something into textBox all warnings that showed up earlier disappear 
        {
            RegistrationErrorLabel.Visibility = Visibility.Hidden;
        }
        private void LoginPwd_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            LogInPasswordErrorLabel.Visibility = Visibility.Hidden;
        }
        private void UserManager_CurrentUserChanged()
        {
          LoginName.Text = string.Empty;
          LoginPwd.Password = string.Empty;
        }

        private void UserManager_UserListChanged() //new method not sure if necessary 
        {
            RegisterName.Text = string.Empty;
            RegisterPwd.Password = string.Empty;
        }

        private void LoginBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {  
            UserManager.LogIn(LoginName.Text,LoginPwd.Password); 
        }

        private void RegisterAdminBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (RegisterPwd.Password == string.Empty)
            {
                RegistrationPasswordErrorLabel.Visibility = Visibility.Visible;
                return;
            }
            UserManager.SaveNewCustomer(RegisterName.Text, RegisterPwd.Password, UserTypes.Admin);
           
        }

        private void RegisterCustomerBtn_OnClickmerBtn_Click(object sender, RoutedEventArgs e)
        {
            UserManager.SaveNewCustomer(RegisterName.Text, RegisterPwd.Password, UserTypes.Customer);
          
        }


       
    }
}
