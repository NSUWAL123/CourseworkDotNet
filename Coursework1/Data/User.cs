namespace Coursework1.Data;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid(); //Uniquely identifies each user.
    public string Username { get; set; } //Username of the user.
    public string PasswordHash { get; set; } //A hashed password of the user.
    public Role Role { get; set; } //Identifies if the user's role is Admin or Staff.
    public bool HasInitialPassword { get; set; } = true; //Identifies whether the user has changed the default password.
    public DateTime CreatedAt { get; set; } = DateTime.Now; //Stores Date and Time of the user creation.
    public Guid CreatedBy { get; set; } //Stores Admin who has created the user.
}
