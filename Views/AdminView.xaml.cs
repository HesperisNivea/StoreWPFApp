using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using StoreWPFApp.DataModels.Products;
using StoreWPFApp.Enums;
using StoreWPFApp.Managerrs;
using static System.Net.Mime.MediaTypeNames;

namespace StoreWPFApp.Views
{
    public partial class AdminView : UserControl, INotifyPropertyChanged
    {
        string SelectedImagePath { get; set; }
        
        public ObservableCollection<Product> ProductsCollection { get; set; } = new (ProductManager.Products);

        public Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }
        public ProductCategory? ProductsCategory { get; set; }
        public ProductCategory? SelectedProductCategory { get; set; }

        public AdminView()
        {
            InitializeComponent();
            this.DataContext = this;
            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;
            ProductManager.ProductListChanged += ProductManagerOnProductListChanged;
            // i am getting rid of ProductCategory.All from ComboBox Selection,
            // because user cannot create a product with this category 
            AdminProductCategoryComboBox.ItemsSource = Enum.GetValues(typeof(ProductCategory));
            List<ProductCategory> productCategories = new List<ProductCategory>();
            foreach (var value in Enum.GetValues(typeof(ProductCategory)))
            {
                if (value.ToString() == "All")
                {

                }
                else
                {
                    productCategories.Add((ProductCategory)value);
                }
            }
            ProductCategoryComboBox.ItemsSource = productCategories;
        }

        // Each time a list ProductManager.Products is updated and saved to file.
        // It invokes a ProductListChanged and renews an existing ProductsCollection list 
        private void ProductManagerOnProductListChanged()
        {
            ProductsCollection.Clear();
            foreach (var product in ProductManager.Products)
            {
                ProductsCollection.Add(product);
            }
        }

        private void ProductManagerOnProductListChanged(ProductCategory productCategory) // Adjust items visible in ProductListbox after category in combobox
        {
            ProductsCollection.Clear();
            if (ProductsCategory == null || ProductsCategory == ProductCategory.All)
            {
                ProductManagerOnProductListChanged();
            }
            else
            {
                foreach (var product in ProductManager.Products)
                {
                    if (productCategory == product.Category)
                    {
                        ProductsCollection.Add(product);
                    }
                }
            }
        }

        private void UserManager_CurrentUserChanged()
        {
            
        }

        private void ProdList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProduct = (Product)ProdList.SelectedItem;
            
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e) // In method a reset Image.Source so user needs to add a new image each time he wants to change/add image to product 
        {
            if (SelectedProduct != null)
            {
                if (SelectedProductCategory == null)
                {
                    SelectedProductCategory = SelectedProduct.Category;
                }
                else
                {
                    ProductCategory alteredCategory = (ProductCategory)SelectedProductCategory;
                }

                
                if (SelectedImagePath == null) // gives possibility to choose your oun image or get one already existing
                {
                    ProductManager.UpdateProductByAdmin(SelectedProduct, NameTextBox.Text, Double.Parse(PriceTextBox.Text), SelectedProduct.ImageSource, (ProductCategory)SelectedProductCategory);
                }
                else
                {
                   ProductManager.UpdateProductByAdmin(SelectedProduct, NameTextBox.Text, Double.Parse(PriceTextBox.Text),SelectedImagePath,(ProductCategory)SelectedProductCategory); 
                }
                if (ProductsCategory == null) // if its null you get the error 
                {
                    ProductsCategory = ProductCategory.All;
                }
                ProductManagerOnProductListChanged((ProductCategory)ProductsCategory);
                SelectedImagePath = null; 
                ProdImage.Source = null;
                return;
            }

            if (SelectedProductCategory == null || NameTextBox.Text == "" || PriceTextBox.Text == "")
            {
                MessageBox.Show("Check if you set correctly: Category, Name and Price !!");
                return;
            }

            double result;
            if (!Double.TryParse((string)PriceTextBox.Text, out result ))
            {
                MessageBox.Show("Wrong price format!!");
                return;
            }
            if (SelectedProduct == null)
            { 
                ProductCategory newProduct = (ProductCategory)SelectedProductCategory;
                ProductManager.SaveNewProduct(NameTextBox.Text, Double.Parse(PriceTextBox.Text), newProduct, SelectedImagePath);
                if (ProductsCategory == null)
                {
                    ProductsCategory = ProductCategory.All;
                }
                ProductManagerOnProductListChanged((ProductCategory)ProductsCategory);
                SelectedImagePath = null;
                ProdImage.Source = null;
            }
        }

        private void RemoveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedProduct is null)
            {
                return;
            }
            ProductManager.SelectedProductRemovedByAdmin(SelectedProduct);
         }


        private void LogoutBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserManager.LogOut();
        }

        private void AddImageBtn_OnClick(object sender, RoutedEventArgs e)
        {
           
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;  
            bool? result = openFileDialog.ShowDialog();

            
            if (result == true)
            {
                SelectedImagePath = null;
                SelectedImagePath = @openFileDialog.FileName; // makes it possible to add image to a product
                

                BitmapImage bitmapImage = new BitmapImage(new Uri(SelectedImagePath));
                ProdImage.Source = bitmapImage;
            }
        }
        // INotifyPropertyChanged : start 
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

        // INotifyPropertyChanged : end
        private void AdminProductCategoryComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductManagerOnProductListChanged((ProductCategory)ProductsCategory);
        }
    }
}
