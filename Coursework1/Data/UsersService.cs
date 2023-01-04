using System.Text.Json;

namespace Coursework1.Data;

public static class UsersService
{
    public const string SeedUsername = "admin";
    public const string SeedPassword = "admin";

    //Saves the new or updated user information.
    private static void SaveAll(List<User> users)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string appUsersFilePath = Utils.GetStaffRecordFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(appUsersFilePath, json);
    }

    //Returns records of all users.
    public static List<User> GetAll()
    {
        string appUsersFilePath = Utils.GetStaffRecordFilePath();
        if (!File.Exists(appUsersFilePath))
        {
            return new List<User>();
        }

        var json = File.ReadAllText(appUsersFilePath);
        return JsonSerializer.Deserialize<List<User>>(json);
    }

    //Creates a new user.
    public static List<User> Create(Guid userId, string username, string password, Role role)
    {
        List<User> users = GetAll();
        bool usernameExists = users.Any(x => x.Username == username);

        if (usernameExists) //executes if username present in the file is entered
        {
            throw new Exception("Username already exists.");
        }

        users.Add(
            new User
            {
                Username = username,
                PasswordHash = Utils.HashSecret(password),
                Role = role,
                CreatedBy = userId
            }
        );
        SaveAll(users);
        return users;
    }

    //Checks if current application contains two admins or not
    public static bool CheckAdmin(Role role)
    {
        List<User> users = GetAll();
        var roleCount = users.Where(x => x.Role == Role.Admin).Count();

        if (roleCount == 2 && role == Role.Admin) { //only executes if the system already contains two admins and again another admin is tried to be entered
            throw new Exception("Cannot add more than 2 Admins");
        }
        return true;
    }

    //Creates a new admin if there are no users present.
    public static void SeedUsers()
    {
        var users = GetAll().FirstOrDefault(x => x.Role == Role.Admin);

        if (users == null) //executes if the users file is empty
        {
            Create(Guid.Empty, SeedUsername, SeedPassword, Role.Admin);
        }
    }

    //Returns user based on id.
    public static User GetById(Guid id)
    {
        List<User> users = GetAll();
        return users.FirstOrDefault(x => x.Id == id);
    }

    //Deletes user from file based on id.
    public static List<User> Delete(Guid id)
    {
        List<User> users = GetAll();
        User user = users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        users.Remove(user);
        SaveAll(users);

        return users;
    }

    //Validates user credentials.
    public static User Login(string username, string password)
    {
        var loginErrorMessage = "Invalid username or password.";
        List<User> users = GetAll();
        User user = users.FirstOrDefault(x => x.Username == username);

        if (user == null)
        {
            throw new Exception(loginErrorMessage);
        }

        bool passwordIsValid = Utils.VerifyHash(password, user.PasswordHash); //passing entered and already hashed password for verification

        if (!passwordIsValid)
        {
            throw new Exception(loginErrorMessage);
        }

        return user;
    }

    //Changes current user password to a new password.
    public static User ChangePassword(Guid id, string currentPassword, string newPassword)
    {
        if (currentPassword == newPassword)
        {
            throw new Exception("New password must be different from current password.");
        }

        List<User> users = GetAll();
        User user = users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        bool passwordIsValid = Utils.VerifyHash(currentPassword, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new Exception("Incorrect current password.");
        }

        user.PasswordHash = Utils.HashSecret(newPassword);
        user.HasInitialPassword = false;
        SaveAll(users);

        return user;
    }
}
