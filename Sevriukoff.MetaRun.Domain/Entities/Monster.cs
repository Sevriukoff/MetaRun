namespace Sevriukoff.MetaRun.Domain;

public class Monster
{
    public string Name { get; }
    public bool IsElite { get; }
    public bool IsBoss { get; }

    public Monster(string name, bool isElite, bool isBoss)
    {
        IsElite = isElite;
        IsBoss = isBoss;
    }
}

public class Character
{
    
}