using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;
using StoreWPFApp.DataModels.Products;
using StoreWPFApp.Enums;
using Path = System.IO.Path;

namespace StoreWPFApp.Managerrs;

public static class ProductManager
{
    private static readonly IEnumerable<Product>? _products = new List<Product>();
    public static IEnumerable<Product>? Products => _products; 

    private static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Products");

    public static List<CartProductWithAmount> CartProductsWithAmounts { get; set; } = new List<CartProductWithAmount>(); 
    // Skicka detta efter att produktlistan ändrats eller lästs in
    public static event Action ProductListChanged;
    public static event Action CartListChanged;
    public static void AddProduct(Product product)
    {
        UserManager.CurrentUser.Cart.Add(product);
        CartListChanged.Invoke();
    }

    public static Product UpdateProductByAdmin(Product selectedProduct, string name, double price, string imageSource, ProductCategory category)
    {
        if (selectedProduct == null) 
        {return selectedProduct;}
        
        Product alterProduct; 

        if (category == ProductCategory.Bakery)
        {
            alterProduct = new Bakery(name, price, imageSource);
        }
        else if (category == ProductCategory.Fruit)
        {
            alterProduct = new Fruit(name, price, imageSource);
        }
        else // (category == ProductCategory.Vegetable)
        {
            alterProduct = new Vegetable(name, price, imageSource);
        }


        if (Products is List<Product> products)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Name == selectedProduct.Name)
                {
                    products[i] = alterProduct;
                }
            }

        }

        SaveProductsToFile();
        return alterProduct;
    }
    public static void RemoveProduct(CartProductWithAmount groupProduct) // this method removes product from cart,
    {
        if (groupProduct == null) return;
        UserManager.CurrentUser.Cart.Remove(groupProduct.ProductRepresentation); // it looks after selected product
                                                                                 // in actual current user cart  
        CartListChanged.Invoke();
    }

    public static void GroupProductsAndSetAmount()
    {
        if (UserManager.CurrentUser == null)
        {
            return;
        }

        // It updates image if it was added after product was bought and exists in customer cart
        for (int i = 0; i < UserManager.CurrentUser.Cart.Count; i++) 
        {
            if (Products.Any(cp => cp.Name == UserManager.CurrentUser.Cart[i].Name))
            {
                UserManager.CurrentUser.Cart[i] =
                    Products.First(cp => cp.Name == UserManager.CurrentUser.Cart[i].Name);
            }
        
        }
        // when objects in cart are no longer existing (cannot be bought) it removes them 
        List<Product> productsToRemove = new List<Product>();

        foreach(var cartProduct in UserManager.CurrentUser.Cart) 
        {
            if (!Products.Any(cp => cp.Name == cartProduct.Name))
            {
                productsToRemove.Add(cartProduct);    
            }
        }
 
        foreach (var product in productsToRemove)
        {
            UserManager.CurrentUser.Cart.Remove(product);
        }

        // this method checks if an item with a given name exists in cart list if not adds it if yes count how many
        foreach (var product in UserManager.CurrentUser.Cart)
        {
            if (!CartProductsWithAmounts.Exists((vProduct) => vProduct.ProductRepresentation.Name == product.Name))
            {
                CartProductsWithAmounts.Add(new(product, UserManager.CurrentUser.Cart.Count(prod => prod.Name == product.Name)));
            }
            else
            {
                foreach (var cartProduct in CartProductsWithAmounts)
                {
                    if (cartProduct.ProductRepresentation.Name == product.Name)
                    {
                        cartProduct.ProductCount = UserManager.CurrentUser.Cart.Count(prod => prod.Name == product.Name);
                    }
                }
            }
        }
    }
    public static void SaveNewProduct(string name, double price, ProductCategory category, string imageSource)
    {
        if (Products.ToList().Exists(newProduct => newProduct.Name == name))
        {
            return;
        }

        if (Products is List<Product> product)
        {
            if (category == ProductCategory.Bakery)
            {
                product.Add(new Bakery(name, price, imageSource));
            }
            else if (category == ProductCategory.Fruit)
            {
                product.Add(new Fruit(name, price, imageSource));
            }
            else if (category == ProductCategory.Vegetable)
            {
                product.Add(new Vegetable(name, price, imageSource));
            }
        }

        SaveProductsToFile();
    }

    public static void SelectedProductRemovedByAdmin(Product selectedProduct)
    {
        if (Products is List<Product> product)
        {
            if (product.Contains(selectedProduct))
            {
                product.Remove(selectedProduct);
            }
        }

        SaveProductsToFile();
    }
    public static async Task SaveProductsToFile()
    {

        if (Products is List<Product> product)
        {
            string json = JsonSerializer.Serialize(_products);
            File.WriteAllTextAsync(Path.Combine(_path, "ProductList.json"), json);
        }
        ProductListChanged.Invoke();
    }

    public static async Task LoadProductsFromFile()
    {
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
        }
        var productSavedFromFile = File.ReadAllText(Path.Combine(_path, "ProductList.json"));
        var deserialisedProduct = new List<Product>();
        using (var jsonDoc = JsonDocument.Parse(productSavedFromFile))
        {
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
                {
                    Product product;
                    switch (jsonElement.GetProperty("Category").GetByte())
                    {
                        case 0:
                            product = jsonElement.Deserialize<Fruit>();
                            deserialisedProduct.Add(product);
                            break;
                        case 1:
                            product = jsonElement.Deserialize<Vegetable>();
                            deserialisedProduct.Add(product);
                            break;
                        case 2:
                            product = jsonElement.Deserialize<Bakery>();
                            deserialisedProduct.Add(product);
                            break;

                    }
                }
            }

        }

        if (Products is List<Product> products)
        {
            foreach (var product in deserialisedProduct)
            {
                products.Add(product);
            }
        }

        ProductListChanged.Invoke();

    }
}
