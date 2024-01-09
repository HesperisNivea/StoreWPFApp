using StoreWPFApp.Enums;

public class Customer : User
{
    public override UserTypes Type { get; } = UserTypes.Customer;

    public Customer(string name, string password) : base(name, password)
    {

    }
   
}