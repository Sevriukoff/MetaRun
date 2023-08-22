namespace Sevriukoff.MetaRun.Domain.Enum;

public enum EventType
{
    CharacterDamageDeal,
    CharacterDamageTake,
    CharacterMobKill,
    CharacterHeal,
    CharacterGoldCoinCollect,
    CharacterLunarCoinCollect,
    CharacterInventoryChange,
    CharacterLevelUp,
    CharacterEquipmentUse,
    CharacterStageFinish,
    CharacterDead,
    RunStart,
    RunPause,
    RunFinish,
    MobSpawn
}