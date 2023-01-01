using System.Security.Cryptography;

namespace Coursework1.Data;

public static class Utils
{
    private const char _segmentDelimiter = ':';

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

    public static bool CheckOpeningTime()
    {
        var currTime = DateTime.Now;
        var StartTime = Convert.ToDateTime("09:00:00");
        var EndTime = Convert.ToDateTime("17:00:00");
        string currDay = DateTime.Now.DayOfWeek.ToString();
        

        if (currDay != "Saturday"  && currDay != "Sunday") { 
            if (currTime >= StartTime && currTime <= EndTime) { 
                return true;
            }      
        }
        return false;
    }

    public static string GetAppDirectoryPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Bike_Services"
        );
    }

    public static string GetAppUsersFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "users.json");
    }

    public static string GetinventoryFilePath()//userId
    {
        return Path.Combine(GetAppDirectoryPath(), "_inventory.json");
    }

    public static string GetItemRequestFilePath()//userId
    {
        return Path.Combine(GetAppDirectoryPath(), "_item_request_demo.json");
    }
}
