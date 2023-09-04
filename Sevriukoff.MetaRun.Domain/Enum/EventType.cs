namespace Sevriukoff.MetaRun.Domain.Enum;

public enum EventType
{
    CharacterDamageDeal,
    CharacterDamageTake,
    CharacterMobKill,
    CharacterHeal,
    CharacterMove,
    CharacterGoldCoinCollect,
    CharacterLunarCoinCollect,
    CharacterInventoryChange,
    CharacterLevelUp,
    CharacterEquipmentUse,
    CharacterStageFinish,
    CharacterDead,
    RunStart,
    RunStageCompleted,
    RunPause,
    RunFinish,
    MobSpawn
}