namespace Sevriukoff.MetaRun.Domain.Events.Character;

public class CharacterMinionDamageEvent : CharacterDamageEvent
{
    public string MinionName { get; set; }
}