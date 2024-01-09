using StoreWPFApp.Managerrs;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;
            UserManager.LoadUsersFromFile();
            ProductManager.LoadProductsFromFile();

            //default setting for MainView
            ShopTab.Visibility = Visibility.Collapsed;
            AdminTab.Visibility = Visibility.Collapsed;
            LoginTab.Visibility = Visibility.Visible;
            LoginTab.IsSelected = true;
        }

        private void UserManager_CurrentUserChanged()
        {
            // if UserManager.CurrentUser is Admin/Customer show adequate windows 
            if (UserManager.CurrentUser == null)
            {
                ShopTab.Visibility = Visibility.Collapsed;
                AdminTab.Visibility = Visibility.Collapsed;
                LoginTab.Visibility = Visibility.Visible;
                LoginTab.IsSelected = true;
            }
            else if (UserManager.IsAdminLoggedIn)
            {
                AdminTab.Visibility = Visibility.Visible;
                ShopTab.Visibility = Visibility.Collapsed;
                LoginTab.Visibility = Visibility.Collapsed;
                AdminTab.IsSelected = true;
            }
            else if (UserManager.IsCustomerLoggedIn)
            {
                ShopTab.Visibility = Visibility.Visible;
                AdminTab.Visibility = Visibility.Collapsed;
                LoginTab.Visibility = Visibility.Collapsed;
                ShopTab.IsSelected = true;
            }



        }
    }
}