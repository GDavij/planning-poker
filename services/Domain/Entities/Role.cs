namespace Domain.Entities;

public class Role
{
    public byte RoleId { get; init; }
    public string Name { get; init; }
    public string? Abbreviation { get; init; }
    
    public ICollection<Participant> Participants { get; init; } = [];

    private Role()
    { }
    
    public Role(byte roleId, string name, string? abbreviation)
    {
        RoleId = roleId;
        Name = name;
        Abbreviation = abbreviation;
    }
}