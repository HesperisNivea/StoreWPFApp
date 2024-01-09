using StoreWPFApp.DataModels.Products;

public class CartProductWithAmount //This object has a data template in App.xaml
{
    public Product ProductRepresentation { get; set; }
    public int ProductCount { get; set; }
    public CartProductWithAmount(Product productRepresentation, int productCount)
    {
        ProductRepresentation = productRepresentation;
        ProductCount = productCount;
    }
}