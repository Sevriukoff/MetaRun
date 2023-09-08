using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Entities;

public class Monster
{
    public string Name { get; }
    public bool IsElite { get; }
    public bool IsBoss { get; }
    public TeamType Team { get; set; }

    public Monster(string name, bool isElite, bool isBoss, TeamType team)
    {
        IsElite = isElite;
        IsBoss = isBoss;
        Team = team;
    }
}