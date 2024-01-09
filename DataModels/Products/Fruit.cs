using StoreWPFApp.Enums;

namespace StoreWPFApp.DataModels.Products;

public class Fruit : Product
{
    public Fruit(string name, double price, string imageSource) : base(name, price, imageSource)
    {
    }
    public override ProductCategory Category { get; } = ProductCategory.Fruit;
}