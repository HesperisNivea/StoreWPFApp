using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using StoreWPFApp.Enums;

namespace StoreWPFApp.DataModels.Products;

public abstract class Product : INotifyPropertyChanged //This object has a data template in App.xaml
{
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    private double _price;
    public double Price
    {
        get
        {
            return _price;
        }
        set
        {
            _price = value;
            OnPropertyChanged();
        }
    }
    public abstract ProductCategory Category { get; }

    public string _imageSource;
    public string ImageSource
    {
        get
        {
            return _imageSource;
        }
        set
        {
            _imageSource = value;
        }
    }
    protected Product(string name, double price, string imageSource)
    {
        Name = name;
        Price = price;
        ImageSource = imageSource;
    }

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

}