using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using StoreWPFApp.DataModels.Products;
using StoreWPFApp.Enums;
using StoreWPFApp.Managerrs;

namespace StoreWPFApp.Views
{
    public partial class ShopView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<Product> ShopProducts { get; set; } = new(ProductManager.Products);
        public User? CurrentUser { get; set; }

        private string _currentUserName = "hej";

        public string CurrentUserName
        {
            get { return _currentUserName; }
            set
            {
                _currentUserName = value;
                OnPropertyChanged();
            }
        }

        public Product SelectedShopProduct { get; set; } = null;

        public CartProductWithAmount SelectedCartProduct { get; set; } = null;

        public ProductCategory? SelectedProductCategory { get; set; }

        public ObservableCollection<CartProductWithAmount> UserCart { get; set; } = new();

        public ShopView()
        {
            this.DataContext = this;
            InitializeComponent();
            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;
            ProductManager.ProductListChanged += ProductManagerOnShopProductListChanged;
            ProductManager.CartListChanged += ProductManager_CartListChanged;
            ProductCategoryComboBox.ItemsSource = Enum.GetValues(typeof(ProductCategory));
        }

        private void ProductManager_CartListChanged()
        {
            UserCart.Clear();
            ProductManager.GroupProductsAndSetAmount();
            foreach (var cartProductsWithAmount in ProductManager.CartProductsWithAmounts)
            {
                UserCart.Add(cartProductsWithAmount);
            }
            
            UserManager.UpdateCartContentOfCurrentCustomerInFile();
        }

        private void ProductManagerOnShopProductListChanged()
        {
            ShopProducts.Clear();
            foreach (var product in ProductManager.Products)
            {
                ShopProducts.Add(product);
            }
        }

        private void ProductManagerOnShopProductListChanged(ProductCategory productCategory) // if category is set it adjusts what user see 
        {
            ShopProducts.Clear();
            if (SelectedProductCategory == null || SelectedProductCategory == ProductCategory.All)
            {
                ProductManagerOnShopProductListChanged();
            }
            else
            {
                foreach (var product in ProductManager.Products)
                {
                    if (productCategory == product.Category)
                    {
                        ShopProducts.Add(product);
                    }
                }
            }


        }

        private void ProdList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedShopProduct = (Product)ProdList.SelectedItem;
        }

        private void CartList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductManager.GroupProductsAndSetAmount();
        }

        private void UserManager_CurrentUserChanged()
        {
            if (UserManager.CurrentUser != null)
            {
                CurrentUser = UserManager.CurrentUser;
                CurrentUserName = UserManager.CurrentUser.Name;
                ProductManager.CartProductsWithAmounts.Clear();
                ProductManager_CartListChanged(); // without this cart content won't appear after login 
            }

        }

        private void RemoveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           ProductManager.RemoveProduct(SelectedCartProduct);
           ProductManager.CartProductsWithAmounts.Clear();
           ProductManager_CartListChanged();
        }

        private void AddBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ProductManager.AddProduct(SelectedShopProduct);
            
        }

        private void LogoutBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserManager.CurrentUser = null;
        }

        private void CheckoutBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserManager.CurrentUser.Cart.Clear();
            ProductManager.CartProductsWithAmounts.Clear();
            ProductManager_CartListChanged();
        }
         private void ProductCategoryComboBox_OnSelected(object sender, RoutedEventArgs e)
        {
            ProductManagerOnShopProductListChanged((ProductCategory)SelectedProductCategory);
        }

        //  iNotifyPropertyChanged 
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        //  iNotifyPropertyChanged  : end
    }


}
