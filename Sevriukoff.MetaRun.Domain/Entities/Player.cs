namespace Sevriukoff.MetaRun.Domain;

/// <summary>
/// IEntity, Domain Entity
/// </summary>
public class Player
{
    #region Props

    public ulong Id { get; }
    public string Name { get; }
    public byte[] Image { get; set; }

    #endregion

    #region ctor's

    public Player(uint id, string name)
    {
        Id = id;
        Name = name;
    }

    #endregion
    
    #region Methods

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        
        var other = (Player)obj;
        
        return this == other;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (int) Id;
            hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            return hashCode;
        }
    }

    #endregion

    #region Operators

    public static bool operator ==(Player first, Player second)
    {
        if (ReferenceEquals(first, second))
            return true;

        if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            return false;

        return first.Id == second.Id;
    }

    public static bool operator !=(Player first, Player second)
    {
        return !(first == second);
    }

    #endregion
}

