using System.Runtime.Serialization;

namespace Domain.Entities;

public class Account
{
    public long AccountId { get; init; }
    public string? FirebaseUserId { get; private set; }
    public string? PhotoUrl { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public bool Deleted { get; private set; }
    public bool IsAdmin { get; private set; }
    
    [IgnoreDataMember]
    public string? Password { get; private set; }
    
    private Account()
    { }
    
    public Account(string email, string name)
    {
        Email = email;
        Name = name;
    }

    public void UseFirebaseIdentity(string firebaseUserId)
    {
        FirebaseUserId = firebaseUserId;
    }
    
    public void TakePhoto(string photoUrl)
    {
        PhotoUrl = photoUrl;
    }

    public void SecureWithPassword(string password)
    {
        Password = password;
    }
    
    public ICollection<Match> Matches { get; init; } = [];
    public ICollection<Participant> Participants { get; init; } = [];
}