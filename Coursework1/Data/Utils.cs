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

    public static bool CheckInventoryOpeningTime()
    {
        var CurrentTime = DateTime.Now;
        var StartTime = Convert.ToDateTime("09:00:00");
        var EndTime = Convert.ToDateTime("17:00:00");
        string Today = DateTime.Now.DayOfWeek.ToString();


        if (Today != "Saturday" && Today != "Sunday")
        {
            if (CurrentTime >= StartTime && CurrentTime <= EndTime)
            {
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

    public static string GetStaffRecordFilePath()
    {
        return Path.Combine(GetAppDirectoryPath(), "users.json");
    }

    public static string GetInventoryRecordFilePath()//userId
    {
        return Path.Combine(GetAppDirectoryPath(), "_inventory.json");
    }

    public static string GetItemRequestFilePath()//userId
    {
        return Path.Combine(GetAppDirectoryPath(), "_item_request_demo1.json");
    }

    public static string ItemsTakenOutRecordFilePath()//userId
    {
        return Path.Combine(GetAppDirectoryPath(), "_item_taken_out_demo.json");
    }
}
