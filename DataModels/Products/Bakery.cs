using StoreWPFApp.DataModels.Products;
using StoreWPFApp.Enums;

public class Bakery : Product
{
    public Bakery(string name, double price, string imageSource) : base(name, price, imageSource)
    {
    }

    public override ProductCategory Category { get; } = ProductCategory.Bakery;
}