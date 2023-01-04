using System.Security.Cryptography;

namespace Coursework1.Data;

public static class Utils
{
    private const char _segmentDelimiter = ':';

    //Converts plain user password to hashed form.
    public static string HashSecret(string input)
    {
        var saltSize = 16;
        var iterations = 100_000;
        var keySize = 32;
        HashAlgorithmName algorithm = HashAlgorithmName.SHA256;
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

        return string.Join(
            _segmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            iterations,
            algorithm
        );
    }

    //Verifies if the plain password equals the hashed password.
    public static bool VerifyHash(string input, string hashString)
    {
        string[] segments = hashString.Split(_segmentDelimiter);
        byte[] hash = Convert.FromHexString(segments[0]);
        byte[] salt = Convert.FromHexString(segments[1]);
        int iterations = int.Parse(segments[2]);
        HashAlgorithmName algorithm = new(segments[3]);
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
            input,
            salt,
            iterations,
            algorithm,
            hash.Length
        );

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }

    //Checks the working day and hour of the bike servicing center.
    public static bool CheckInventoryOpeningTime()
    {
        var CurrentTime = DateTime.Now; //Stores current date and time of the day
        var StartTime = Convert.ToDateTime("09:00:00");
        var EndTime = Convert.ToDateTime("16:00:00");
        string Today = DateTime.Now.DayOfWeek.ToString(); //Stores current day of the week


        if (Today != "Saturday" && Today != "Sunday") //checks if current day is saturday or not
        {
            if (CurrentTime >= StartTime && CurrentTime <= EndTime) //checks current time with start and end time
            {
                return true;
            }
        }
        return false;
    }

    //Returns the main app directory path.
    public static string GetAppDirectoryPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Bike_Services"
        );
    }

    //Returns the staff record file path.
    public static string GetStaffRecordFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "users.json");
    }

    //Returns the inventory record file path.
    public static string GetInventoryRecordFilePath()//userId
    {
        return Path.Combine(GetAppDirectoryPath(), "_inventory.json");
    }

    //Returns the requested item record file path.
    public static string GetItemRequestFilePath()//userId
    {
        return Path.Combine(GetAppDirectoryPath(), "_item_request_demo1.json");
    }
}
