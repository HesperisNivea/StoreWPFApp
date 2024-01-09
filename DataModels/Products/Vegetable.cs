using StoreWPFApp.Enums;

namespace StoreWPFApp.DataModels.Products;

public class Vegetable : Product
{
    public Vegetable(string name, double price, string imageSource) : base(name, price, imageSource)
    {

    }

    public override ProductCategory Category { get; } = ProductCategory.Vegetable;
}