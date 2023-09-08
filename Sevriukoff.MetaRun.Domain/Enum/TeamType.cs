namespace Sevriukoff.MetaRun.Domain.Enum;

public enum TeamType: sbyte
{
    None = -1, // 0xFF
    Neutral = 0,
    Player = 1,
    Monster = 2,
    Lunar = 3,
    Void = 4,
    Count = 5,
}