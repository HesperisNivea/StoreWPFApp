using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using StoreWPFApp.DataModels.Products;
using StoreWPFApp.Enums;

public abstract class User 
{
    public string Name { get; }

    public string Password { get; } 

    public abstract UserTypes Type { get; }

    public List<Product> Cart { get; } = new List<Product>();

    protected User(string name, string password)
    {
        Name = name;
        Password = password;
        
    }

    public bool Authenticate(string password)
    {
        return Password.Equals(password);
    }
    
    
}