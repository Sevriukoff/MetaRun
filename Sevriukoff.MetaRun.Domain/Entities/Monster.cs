using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Entities;

public class Monster
{
    public uint UnityNetId { get;}
    public string Name { get; }
    public bool IsElite { get; }
    public bool IsBoss { get; }
    public TeamType Team { get; }

    public Monster( uint unityNetId, string name, bool isElite, bool isBoss, TeamType team)
    {
        UnityNetId = unityNetId;
        Name = name;
        IsElite = isElite;
        IsBoss = isBoss;
        Team = team;
    }

    public static bool operator ==(Monster first, Monster second)
    {
        return first != null &&
               second != null &&
               first.UnityNetId == second.UnityNetId;
    }

    public static bool operator !=(Monster first, Monster second)
    {
        return !(first == second);
    }
}