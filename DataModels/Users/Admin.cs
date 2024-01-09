using StoreWPFApp.Enums;


public class Admin : User
{
    public override UserTypes Type { get; } = UserTypes.Admin;
    public Admin(string name, string password) : base(name, password)
    {
    }

}