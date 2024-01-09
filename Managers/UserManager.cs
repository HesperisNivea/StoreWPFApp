using System.IO;
using System.Text.Json;
using StoreWPFApp.DataModels.Products;
using StoreWPFApp.Enums;


namespace StoreWPFApp.Managerrs;

public static class UserManager
{
    private static readonly IEnumerable<User>? _users = new List<User>();

    private static User _currentUser;
    private static string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Users"); 
    public static IEnumerable<User>? Users => _users;

    public static User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            CurrentUserChanged?.Invoke();
        }
    }

    public static event Action WrongPasswordLogInTryDenied;
    public static event Action NotExistnigUsernameLogIn; // change name
    public static event Action UsernameAlreadyTaken;
    public static event Action CurrentUserChanged;

    // Skicka detta efter att användarlistan ändrats eller lästs in
    public static event Action UserListChanged;

    // IsAdminLoggedIn and IsCustomerLoggedIn are used to detect what kind of user is logged in and adjust things in mainwindow 

    public static bool IsAdminLoggedIn => CurrentUser.Type is UserTypes.Admin;

    public static bool IsCustomerLoggedIn => CurrentUser.Type is UserTypes.Customer;


    public static void ChangeCurrentUser(string name, string password, UserTypes type)
    {
        //I made a log in method and ChangeCurrentUser seems unnecessary, but i put two rows of kod here, so its not empty :)
        CurrentUser = (User)Users.First(savedUser => savedUser.Name == name);
        UserManager.CurrentUser = CurrentUser;
    }

    public static void LogIn(string name, string password) //new
    {
        if (!Users.ToList().Exists(newUser => newUser.Name == name))
        {
            NotExistnigUsernameLogIn.Invoke();
            return;
        }

        if (!Users.FirstOrDefault(newUser => newUser.Name == name).Authenticate(password))
        {
            WrongPasswordLogInTryDenied.Invoke();
            return;
        }
        User user = (User)Users.First(savedUser => savedUser.Name == name);

        ChangeCurrentUser(user.Name,user.Password,user.Type);

        //CurrentUser = (User)Users.First(savedUser => savedUser.Name == name);
        //UserManager.CurrentUser = CurrentUser;
    }
    public static void LogOut()
    {
        CurrentUser = null; 
    }
    public static void UpdateCartContentOfCurrentCustomerInFile() 
    {
        User currentUserUpdated = UserManager.CurrentUser;
        if (Users is List<User> users)
        {
            users.Remove(UserManager.CurrentUser);
            users.Add(currentUserUpdated);
        }
        SaveUsersToFile();
    }
    public static void SaveNewCustomer(string name, string password, UserTypes type)
    {
        
        if (Users.ToList().Exists(newUser => newUser.Name == name))
        {
            UsernameAlreadyTaken.Invoke();
            return;
        }

        if (Users is List<User> users)
        {
            if (type == UserTypes.Admin)
            {
                users.Add(new Admin(name, password));
            }
            else if (type == UserTypes.Customer)
            {
                users.Add(new Customer(name, password));
            }

        }

        SaveUsersToFile();
    }
    public static async Task SaveUsersToFile()
    {
        //if name exist send a label 
        if (Users is List<User> users)
        {
            string json = JsonSerializer.Serialize(_users);
            
                File.WriteAllTextAsync(Path.Combine(_path, "UsersList.json"), json);
        }

        UserListChanged.Invoke();
    }

    public static async Task LoadUsersFromFile() 
    {
        Directory.CreateDirectory(_path);
        var usersSavedFromFile = File.ReadAllText(Path.Combine(_path, "UsersList.json"));
        var deserialisedUsers = new List<User>();
        using (var jsonDoc = JsonDocument.Parse(usersSavedFromFile))
        {
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
                {
                    User user;
                    List<Product> deserialisedProduct = new List<Product>();
                    switch (jsonElement.GetProperty("Type").GetByte())
                    {
                        case 1:
                            foreach (var jsonProductElement in jsonElement.GetProperty("Cart").EnumerateArray())
                            {
                                Product product;
                                switch (jsonProductElement.GetProperty("Category").GetByte())
                                {
                                    case 0:
                                        product = jsonProductElement.Deserialize<Fruit>();
                                        deserialisedProduct.Add(product);
                                        break;
                                    case 1:
                                        product = jsonProductElement.Deserialize<Vegetable>();
                                        deserialisedProduct.Add(product);
                                        break;
                                    case 2:
                                        product = jsonProductElement.Deserialize<Bakery>();
                                        deserialisedProduct.Add(product);
                                        break;

                                }
                            }
                            user = jsonElement.Deserialize<Customer>();
                            foreach (var product in deserialisedProduct)
                            {
                                user.Cart.Add(product);
                            }
                            deserialisedUsers.Add(user);
                            break;
                        case 0:
                            user = jsonElement.Deserialize<Admin>();
                            deserialisedUsers.Add(user);
                            break;

                    }
                }
            }

        }
        
        if (Users is List<User> users)
        {
            foreach (var user in deserialisedUsers)
            {
                users.Add(user);
            }
        }

        UserListChanged.Invoke();


    }
}